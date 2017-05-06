using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Sockets.Plugin;
using Sockets.Plugin.Abstractions;
using StoryCanvas.Shared.Common;
using StoryCanvas.Shared.Messages.Common;
using StoryCanvas.Shared.Messages.Network;
using StoryCanvas.Shared.Models.IO;
using StoryCanvas.Shared.Models.Story;
using StoryCanvas.Shared.Types;
using StoryCanvas.Shared.ViewTools;

namespace StoryCanvas.Shared.Models.Network
{
    public class NetworkStoryModel : INotifyPropertyChanged
	{
		/// <summary>
		/// モデルを正常に受信した時に発行されるイベント
		/// </summary>
		/// <param name="model">受信成功したモデル</param>
		public delegate void ModelReceivedEventHandler(StoryModel model);
		public event ModelReceivedEventHandler ModelReceived;

		private SerializationModel Serialization = new SerializationModel();

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged([CallerMemberName] string name = null)
		{
			if (PropertyChanged == null) return;

			PropertyChanged(this, new PropertyChangedEventArgs(name));
		}

		#endregion

		private NetworkSendStatus _networkSendStatus;
		public NetworkSendStatus NetworkSendStatus
		{
			get
			{
				return this._networkSendStatus;
			}
			set
			{
				this._networkSendStatus = value;
				this.OnPropertyChanged();
			}
		}

		private NetworkReceiveStatus _networkReceiveStatus;
		public NetworkReceiveStatus NetworkReceiveStatus
		{
			get
			{
				return this._networkReceiveStatus;
			}
			set
			{
				this._networkReceiveStatus = value;
				this.OnPropertyChanged();
			}
		}

		private string _storyTitle;
		public string StoryTitle
		{
			get
			{
				return this._storyTitle;
			}
			set
			{
				this._storyTitle = (value != null && value == "") ? StringResourceResolver.Resolve("TitleEmpty") : value;
				this.OnPropertyChanged();
			}
		}

		private CommsInterface _interface;
		public CommsInterface Interface
		{
			get
			{
				return this._interface;
			}
			set
			{
				this._interface = value;
				this.OnPropertyChanged();
			}
		}

		public Collection<CommsInterface> AllInterfaces
		{
			get
			{
				var result = new Collection<CommsInterface>();
				Func<Task<List<CommsInterface>>> cmd = async () =>
				{
					return await CommsInterface.GetAllInterfacesAsync();
				};
				var interfacesTask = Task.Run(cmd);
				interfacesTask.Wait();
				foreach (var i in interfacesTask.Result)
					if (i.IsUsable && !i.IsLoopback)
						result.Add(i);
				return result;
			}
		}

		public Collection<CommsInterface> LocalInterfaces
		{
			get
			{
				var result = new Collection<CommsInterface>();
				foreach (var i in this.AllInterfaces)
					if (i.IpAddress.StartsWith("192.168"))
						result.Add(i);
				return result;
			}
		}

		public Collection<CommsInterface> Interfaces
		{
			get
			{
				return this.IsAllInterfaces ? this.AllInterfaces : this.LocalInterfaces;
			}
		}

		private bool _isAllInterfaces;
		public bool IsAllInterfaces
		{
			get
			{
				return this._isAllInterfaces;
			}
			set
			{
				this._isAllInterfaces = value;
				this.OnPropertyChanged();
				this.OnPropertyChanged("Interfaces");
			}
		}

		/*
		 * 1. Receiver が UDP を立ち上げ
		 * 2. Receiver が TCP を立ち上げ
		 * 3. Sender が TCP を立ち上げ
		 * 4. Sender が Receiver の UDP に参加して、メタデータを送信
		 * 5. Receiver が Sender からのメタデータを UDP で受信
		 * 6. Receiver が Sender の TCP に折り返しメタデータを送信
		 * 7. Sender が Receiver からの折り返しメタデータを TCP で受信
		 * 8. Sender が Receiver の TCP にモデルデータを送信
		 * 9. Receiver が Sender からのモデルデータを TCP で受信、受信完了
		 */

		private UdpSocketMulticastClient MulticastClient;
		private TcpSocketClient TcpClient;
		private TcpSocketListener TcpListener;

		/// <summary>
		/// 送信するオブジェクト
		/// </summary>
		private StoryModel SendModel;

		/// <summary>
		/// ネットワークからStoryModelの情報を受信する準備をする
		/// </summary>
		public void ReadyReceiveNetwork()
		{
			this.StoryTitle = null;

			try
			{
				// 1. Receiver が UDP を立ち上げ
				this.NetworkReceiveStatus = NetworkReceiveStatus.MakingUdp;
				this.ReadyReceiveUDP(12930);        // 5.  6.

				// 2. Receiver が TCP を立ち上げ
				this.NetworkReceiveStatus = NetworkReceiveStatus.MakingTcp;
				this.ReadyReceiveTCP(12930);
			}
			catch
			{
				this.NetworkReceiveStatus = NetworkReceiveStatus.ErrorAndAbort;
				this.CleaningUpReceiveNetwork_Private();
			}

			// 送信者から信号が送られてくるのを待つ（5.へ）
			this.NetworkReceiveStatus = NetworkReceiveStatus.WaitingForSenderSignal;

			// WPFでモーダル表示になるとき、一時的に制御が止まるので
			Messenger.Default.Send(null, new OpenNetworkReceiveMessage());
		}

		private Func<Task> SendLoop;

		private bool IsCancelSendLoop;

		/// <summary>
		/// ネットワークへStoryModelの情報を送信する準備をする
		/// </summary>
		public void ReadySendNetwork(StoryModel sendObject)
		{
			this.SendModel = sendObject;

			this.StoryTitle = sendObject.StoryConfig.Title;
			this.ReceivedRMetaData = null;

			// WPFで画面表示（MarsApp.Metroのダイアログは非同期なので大丈夫）
			Messenger.Default.Send(null, new OpenNetworkSendMessage());

			this.IsCancelSendLoop = false;
			this.SendLoop = async () =>
			{
				for (int i = 0; i < 5; i++)
				{
					if (this.IsCancelSendLoop)
					{
						throw new OperationCanceledException();
					}

					try
					{
						// 3. Sender が TCP を立ち上げ
						this.NetworkSendStatus = NetworkSendStatus.MakingTcp;
						this.ReadyReceiveTCP_Sender(12930); // 7.  8.

						// 4. Sender が Receiver の UDP に参加して、メタデータを送信
						this.NetworkSendStatus = NetworkSendStatus.SendingMetaData;
						this.SendUdpNetwork(12930, new StoryModelMetaData(sendObject));

						// 受信側からの応答を待つ（7.へ）
						this.NetworkSendStatus = NetworkSendStatus.WaitingForReceiverSignal;
					}
					catch
					{
						this.NetworkSendStatus = NetworkSendStatus.ErrorAndAbort;
						this.CleaningUpReceiveNetwork_Private();
						break;
					}

					int count = 0;
					while (this.ReceivedRMetaData == null && count++ < 300)
					{
						await Task.Delay(10);
						if (this.IsCancelSendLoop)
						{
							throw new OperationCanceledException();
						}
					}
					if (this.ReceivedRMetaData != null)
					{
						break;
					}

					this.CleaningUpReceiveNetwork_Private();
				}

				if (this.ReceivedRMetaData == null)
				{
					this.NetworkSendStatus = NetworkSendStatus.NoSignalsAndAbort;
				}
			};
			Task.Run(this.SendLoop);
		}

		private void SendUdpNetwork(int port, object model)
		{
			Func<Task> cmd = async () =>
			{
				string address = "239.192.0.1";         // マルチキャストアドレス

				using (var receiver = new UdpSocketMulticastClient())
				{
					receiver.TTL = 5;

					receiver.MessageReceived += (sender, e) => { };

					// マルチキャストのグループに参加する
					await receiver.JoinMulticastGroupAsync(address, port, this.Interface);

					using (MemoryStream stream = new MemoryStream())
					{
						this.Serialization.Serialize(stream, model);

						// 情報を送信
						await receiver.SendMulticastAsync(stream.ToArray());
					}

					await receiver.DisconnectAsync();
				}
			};
			Task.Run(cmd).Wait();
		}

		private void SendTcpNetwork(string toAddress, int port, object model)
		{
			Func<Task> cmd = async () =>
			{
				var r = new Random();

				this.TcpClient = new TcpSocketClient();
				await this.TcpClient.ConnectAsync(toAddress, port);

				using (MemoryStream stream = new MemoryStream())
				{
					this.Serialization.Serialize(this.TcpClient.WriteStream, model);
					await this.TcpClient.WriteStream.FlushAsync();
					// await Task.Delay(TimeSpan.FromMilliseconds(500));
				}
			};
			Task.Run(cmd).Wait();
		}

		private void ReadyReceiveUDP(int port)
		{
			Func<Task> cmd = async () =>
			{
				string address = "239.192.0.1";         // マルチキャストアドレス

				this.MulticastClient = new UdpSocketMulticastClient();
				this.MulticastClient.TTL = 5;

				// 5. Receiver が Sender からのメタデータを UDP で受信
				this.MulticastClient.MessageReceived += this.ReceiveByUDP;

				// マルチキャストのグループに参加する
				await this.MulticastClient.JoinMulticastGroupAsync(address, port, this.Interface);
			};
			Task.Run(cmd).Wait();
		}

		private void ReadyReceiveTCP(int port)
		{
			this.TcpListener = new TcpSocketListener();

			// when we get connections, read byte-by-byte from the socket's read stream
			this.TcpListener.ConnectionReceived += this.ReceiveByTCP;

			Func<Task> cmd = async () =>
			{
				await this.TcpListener.StartListeningAsync(port, this.Interface);
			};
			Task.Run(cmd).Wait();
		}

		private void ReadyReceiveTCP_Sender(int port)
		{
			this.TcpListener = new TcpSocketListener();

			// 7. Sender が Receiver からの折り返しメタデータを TCP で受信
			this.TcpListener.ConnectionReceived += this.ReceiveByTCP_Sender;

			Func<Task> cmd = async () =>
			{
				await this.TcpListener.StartListeningAsync(port, this.Interface);
			};
			Task.Run(cmd).Wait();
		}

		/// <summary>
		/// ネットワークから受信したストーリーモデル
		/// </summary>
		private StoryModel ReceivedModel = default(StoryModel);
		private StoryModelMetaData ReceivedMetaData = default(StoryModelMetaData);
		private StoryModelReceivedMetaData ReceivedRMetaData = default(StoryModelReceivedMetaData);

		/// <summary>
		/// ネットワークから情報を受信して、データを読み込む
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ReceiveByUDP(object sender, UdpSocketMessageReceivedEventArgs e)
		{
			// 5. Receiver が Sender からのメタデータを UDP で受信
			this.NetworkReceiveStatus = NetworkReceiveStatus.ReceivingMetaData;

			byte[] data = e.ByteData;
			using (MemoryStream stream = new MemoryStream(data))
			{
				StoryModelMetaData model = this.Serialization.Deserialize<StoryModelMetaData>(stream);

				if (model != null)
				{
					this.ReceivedMetaData = model;

					// バージョン番号を取得
					var receivedVersion = new ApplicationVersionResolver(model.ApplicationVersion);
					var myVersion = new ApplicationVersionResolver(StringResourceResolver.Resolve("ApplicationVersion"));

					// TCPを使った受信形式への変更は1.2以降
					if (receivedVersion >= new ApplicationVersionResolver("1.2.0"))
					{
						this.CleaningUpReceiveUDPNetwork();     // UIスレッドではないのでUIスレッド用のメソッドは呼ばない

						// 相手から受け取った情報を保存
						this.StoryTitle = model.StoryTitle;

						// 6. Receiver が Sender の TCP に折り返しメタデータを送信
						this.SendTcpNetwork(e.RemoteAddress, int.Parse(e.RemotePort), new StoryModelReceivedMetaData(model));
						this.NetworkReceiveStatus = NetworkReceiveStatus.WaitingForSenderData;
					}
				}
				else
				{
					// 欲しかったのはコレジャナイ（StoryModelMetaDataではない）ので、引き続き待機する
					this.NetworkReceiveStatus = NetworkReceiveStatus.WaitingForSenderSignal;
				}
			}
		}

		private void ReceiveByTCP(object sender, TcpSocketListenerConnectEventArgs e)
		{
			this.NetworkReceiveStatus = NetworkReceiveStatus.Receiving;

			var stream = e.SocketClient.ReadStream;
			StoryModel model = this.Serialization.Deserialize<StoryModel>(stream);

			// byte[] data = e.ByteData;
			// using (MemoryStream stream = new MemoryStream(data))

			if (model != null)
			{
				this.ReceivedModel = model;
				this.CleaningUpReceiveNetwork_Private();
				this.NetworkReceiveStatus = NetworkReceiveStatus.Received;
			}
			else
			{
				this.CleaningUpReceiveNetwork_Private();
				this.ReadyReceiveUDP(12930);
				this.NetworkReceiveStatus = NetworkReceiveStatus.WaitingForSenderData;
			}
		}

		private void ReceiveByTCP_Sender(object sender, TcpSocketListenerConnectEventArgs e)
		{
			// 7. Sender が Receiver からの折り返しメタデータを TCP で受信
			this.NetworkSendStatus = NetworkSendStatus.ReceivingReceivedMetaData;

			string remoteAddress = e.SocketClient.RemoteAddress;
			int remotePort = e.SocketClient.RemotePort;

			var stream = e.SocketClient.ReadStream;
			StoryModelReceivedMetaData model = this.Serialization.Deserialize<StoryModelReceivedMetaData>(stream);

			// byte[] data = e.ByteData;
			// using (MemoryStream stream = new MemoryStream(data))

			if (model != null)
			{
				this.ReceivedRMetaData = model;
				this.NetworkReceiveStatus = NetworkReceiveStatus.Received;

				// 8. Sender が Receiver の TCP にモデルデータを送信
				this.NetworkSendStatus = NetworkSendStatus.Sending;
				this.SendTcpNetwork(remoteAddress, 12930, this.SendModel);
				this.NetworkSendStatus = NetworkSendStatus.Sent;
			}
			else
			{
				// 欲しかったのはコレジャナイ（StoryModelReceivedMetaDataではない）ので、引き続き待機
				this.NetworkSendStatus = NetworkSendStatus.WaitingForReceiverSignal;
			}
		}

		/// <summary>
		/// ネットワークからStoryModelの情報を受信する準備をやめる
		/// （※これはUIスレッドから呼び出されることが保証される）
		/// </summary>
		public void CleaningUpNetwork()
		{
			// 送信のループが継続中なら途中で中止する
			this.IsCancelSendLoop = true;

			this.CleaningUpReceiveNetwork_Private();

			// ストーリーモデルの切り替え処理はUIスレッドでやらないとデリゲートまわりでエラーになる
			if (this.ReceivedModel != null)
			{
				this.NetworkReceiveStatus = NetworkReceiveStatus.Deserializing;
				this.ModelReceived(this.ReceivedModel);
				this.ReceivedModel = default(StoryModel);

				// 上書き保存できないようにする（最後に保存されたスロットの情報をリセット）
				StorageModel.Default.ResetLastSlot();
			}
		}

		private void CleaningUpReceiveNetwork_Private()
		{
			this.CleaningUpReceiveTCPNetwork();
			this.CleaningUpReceiveUDPNetwork();
		}

		/// <summary>
		/// ネットワークからStoryModelの情報を受信する準備をやめる
		/// （UIスレッド以外から呼び出してもよい）
		/// </summary>
		private void CleaningUpReceiveUDPNetwork()
		{
			if (this.MulticastClient != null)
			{
				Func<Task> cmd = async () => await this.MulticastClient.DisconnectAsync();
				Task.Run(cmd).Wait();
				this.MulticastClient.Dispose();
				this.MulticastClient = null;
			}
		}

		private void CleaningUpReceiveTCPNetwork()
		{
			if (this.TcpClient != null)
			{
				Func<Task> cmd = async () => await this.TcpClient.DisconnectAsync();
				Task.Run(cmd).Wait();
				this.TcpClient.Dispose();
				this.TcpClient = null;
			}
			if (this.TcpListener != null)
			{
				Func<Task> cmd = async () => await this.TcpListener.StopListeningAsync();
				Task.Run(cmd).Wait();
				this.TcpListener.Dispose();
				this.TcpListener = null;
			}
		}
	}
}
