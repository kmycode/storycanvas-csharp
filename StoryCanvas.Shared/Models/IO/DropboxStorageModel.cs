using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DropNetRT;
using DropNetRT.Models;
using StoryCanvas.Shared.Utils;
using StoryCanvas.Shared.Common;

namespace StoryCanvas.Shared.Models.IO
{
	public class DropboxStorageModel : StorageModelBase
	{
		private static DropboxStorageModel _default;
		public static DropboxStorageModel Default
		{
			get
			{
				if (_default == null)
				{
					_default = new DropboxStorageModel();
				}
				return _default;
			}
		}

		private DropNetClient DropboxClient;

		public bool DropboxExecute(Func<DropNetClient, Task> execute)
		{
			bool isError = true;
			Func<Task> cmd = async () =>
			{
				try
				{
					await execute(this.DropboxClient);
					isError = false;
				}
				catch (Exception e)
				{
					this.AutoSaveNetworkError();
				}
			};
			Task.Run(cmd).Wait();
			return isError;
		}

		private DropboxStorageModel() : base(4)
		{
		}

		public override void Login()
		{
			this.DropboxClient = new DropNetClient(DropboxLicense.ApiKey, DropboxLicense.AppSecret);
			UserLogin reqToken = null;

			Func<Task> cmdReq = async () =>
			{
				reqToken = await this.DropboxClient.GetRequestToken();
			};
			Task.Run(cmdReq).Wait();

			// 認証ページをブラウザで開く
			var browser = AutofacUtil.AuthBrowserProvider.OpenBrowser();
			browser.GoToUrl(this.DropboxClient.BuildAuthorizeUrl(reqToken));

			browser.UrlChanged += (sender, e) =>
			{
				// 認証済
				if (e.Url.EndsWith("authorize_submit"))
				{
					this.IsLogining = true;

					// ブラウザを閉じる
					browser.Close();

					// ログイン処理
					Func<Task> cmd = async () =>
					{
						this.DropboxClient.UserLogin = await this.DropboxClient.GetAccessToken();
						this.DropboxClient.UseSandbox = true;
						this.HasLogined = true;
						this._isInitializing = false;
					};
					Task.Run(cmd);
				}
			};
		}

		protected override StorageWorkspaceBase NewStorageWorkspace(int number, bool isReadOnly)
		{
			return new DropboxStorageWorkspace(this, number, isReadOnly);
		}

		protected override void SaveReallySlot(Func<StorageSlotBase> slotGetter, bool isAutoSave)
		{
			Func<Task> cmd = async () =>
			{
				using (Stream stream = await this.DummySlot.GetLocalFile().OpenAsync(PCLStorage.FileAccess.Read))
				{
					var bytes = new byte[stream.Length];
					stream.Read(bytes, 0, (int)stream.Length);
					if (!isAutoSave || slotGetter().CheckDataMD5(bytes))
					{
						slotGetter().WriteStream().Write(bytes, 0, bytes.Length);
						slotGetter().SlotName = this.DummySlot.IsEmptySlotName ? "" : this.DummySlot.SlotName;
						slotGetter().SlotComment = this.DummySlot.SlotComment;
					}
				}
			};
			Task.Run(cmd).Wait();
		}

		private bool _isInitializing = true;
		private Dictionary<string, Metadata> _metaCache = new Dictionary<string, Metadata>();
		private object _metaCacheLock = new object();
		public Metadata GetMetaData(string path = null)
		{
			if (this._isInitializing && this._metaCache.ContainsKey(path))
			{
				return this._metaCache[path];
			}

			Metadata data = null;
			this.DropboxExecute(async (client) =>
			{
				data = await this.DropboxClient.GetMetaData(path);
			});

			if (this._isInitializing)
			{
				lock (this._metaCacheLock)
				{
					if (!this._metaCache.ContainsKey(path))
						this._metaCache.Add(path, data);
				}
			}

			return data;
		}

		public override string ToString()
		{
			return "Dropbox";
		}
	}
}
