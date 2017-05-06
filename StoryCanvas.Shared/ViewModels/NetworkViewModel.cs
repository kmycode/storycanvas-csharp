using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Sockets.Plugin;
using StoryCanvas.Shared.Common;
using StoryCanvas.Shared.Messages.Common;
using StoryCanvas.Shared.Messages.Network;
using StoryCanvas.Shared.Models.Story;
using StoryCanvas.Shared.Types;
using StoryCanvas.Shared.ViewTools;

namespace StoryCanvas.Shared.ViewModels
{
    public class NetworkViewModel : ViewModelBase
    {
		private StoryModel StoryModel = StoryModel.Current;

		public NetworkViewModel()
		{
			this.StoreModel(this.StoryModel);
			this.OnPropertyChanged("");
		}

		/// <summary>
		/// ネットワークの送信状態
		/// </summary>
		public NetworkSendStatus NetworkSendStatus
		{
			get
			{
				return this.StoryModel.NetworkSendStatus;
			}
		}

		/// <summary>
		/// ネットワークの受信状態
		/// </summary>
		public NetworkReceiveStatus NetworkReceiveStatus
		{
			get
			{
				return this.StoryModel.NetworkReceiveStatus;
			}
		}

		/// <summary>
		/// ネットワークインターフェース
		/// </summary>
		public Collection<CommsInterface> Interfaces
		{
			get
			{
				return this.StoryModel.Interfaces;
			}
		}

		/// <summary>
		/// 今回の接続で利用するインターフェース
		/// </summary>
		public CommsInterface Interface
		{
			get
			{
				return this.StoryModel.Interface;
			}
			set
			{
				this.StoryModel.Interface = value;
			}
		}

		/// <summary>
		/// 全てのインターフェースを選択肢として画面に表示するか？
		/// </summary>
		public bool IsAllInterfaces
		{
			get
			{
				return this.StoryModel.IsAllInterfaces;
			}
			set
			{
				this.StoryModel.IsAllInterfaces = value;
			}
		}

		/// <summary>
		/// ストーリーのタイトル
		/// </summary>
		public string StoryTitle
		{
			get
			{
				return this.StoryModel.Network.StoryTitle;
			}
		}

		/// <summary>
		/// ストーリーモデルをネットワークへ送信
		/// </summary>
		private RelayCommand _sendModelCommand;
		public RelayCommand SendModelCommand
		{
			get
			{
				return this._sendModelCommand = this._sendModelCommand ?? new RelayCommand((obj) =>
				{
					try
					{
						throw new NotImplementedException();
						//this.StoryModel.Network.ReadySendNetwork(this.StoryModel);
					}
					catch (System.AggregateException)
					{
						Messenger.Default.Send(this, new LightAlertMessage(StringResourceResolver.Resolve("UndefinedErrorMessage")));
					}
				});
			}
		}

		/// <summary>
		/// ネットワークを選択する画面を表示
		/// </summary>
		private RelayCommand _chooseNetworkCommand;
		public RelayCommand ChooseNetworkCommand
		{
			get
			{
				return this._chooseNetworkCommand = this._chooseNetworkCommand ?? new RelayCommand((obj) =>
				{
					Messenger.Default.Send(this, new OpenNetworkChooseMessage());
				});
			}
		}

		/// <summary>
		/// ネットワークからストーリーモデルを受信する準備を開始
		/// </summary>
		private RelayCommand _readyReceiveModelCommand;
		public RelayCommand ReadyReceiveModelCommand
		{
			get
			{
				return this._readyReceiveModelCommand = this._readyReceiveModelCommand ?? new RelayCommand((obj) =>
				{
					this.StoryModel.Network.ReadyReceiveNetwork();
				});
			}
		}

		/// <summary>
		/// ネットワークからストーリーモデルを受信する準備を終了
		/// </summary>
		private RelayCommand _cleaningUpReceiveModelCommand;
		public RelayCommand CleaningUpReceiveModelCommand
		{
			get
			{
				return this._cleaningUpReceiveModelCommand = this._cleaningUpReceiveModelCommand ?? new RelayCommand((obj) =>
				{
					this.StoryModel.Network.CleaningUpNetwork();
#if WPF
					StoryEditorModel.Default.MainMode = MainMode.EditPerson;
#endif
				});
			}
		}

		/// <summary>
		/// ネットワークからストーリーモデルを受信する準備を開始
		/// </summary>
		private RelayCommand _readySendModelCommand;
		public RelayCommand ReadySendModelCommand
		{
			get
			{
				return this._readySendModelCommand = this._readySendModelCommand ?? new RelayCommand((obj) =>
				{
					this.StoryModel.Network.ReadySendNetwork(this.StoryModel);
				});
			}
		}

		/// <summary>
		/// ネットワークからストーリーモデルを受信する準備を終了
		/// </summary>
		private RelayCommand _cleaningUpSendModelCommand;
		public RelayCommand CleaningUpSendModelCommand
		{
			get
			{
				return this._cleaningUpSendModelCommand = this._cleaningUpSendModelCommand ?? new RelayCommand((obj) =>
				{
					this.StoryModel.Network.CleaningUpNetwork();
				});
			}
		}

		/// <summary>
		/// ネットワークのトラブルシューティングページ表示
		/// </summary>
		private RelayCommand _goNetworkTroubleShootingCommand;
		public RelayCommand GoNetworkTroubleShootingCommand
		{
			get
			{
				return this._goNetworkTroubleShootingCommand = this._goNetworkTroubleShootingCommand ?? new RelayCommand((obj) =>
				{
					Messenger.Default.Send(this, new OpenUrlMessage("https://storycanvas.kmycode.net/" + StringResourceResolver.Resolve("Language") + "/documents/sp-troubleshooting-network.html"));
				});
			}
		}
	}
}
