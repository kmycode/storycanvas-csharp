using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OneDriveRestAPI;
using StoryCanvas.Shared.Models.IO;
using StoryCanvas.Shared.Utils;
using Libraries.StoryCanvas.Native.Common;

namespace Libraries.StoryCanvas.Native.Models.IO
{
	public class OneDriveStorageModel : StorageModelBase
	{
		private Client OneDriveClient;

		public OneDriveStorageModel() : base(4)
		{
		}

		public bool OneDriveExecute(Func<Client, Task> execute)
		{
			bool isError = true;
			bool isMore = true;

			while (isMore)
			{
				isMore = false;
				Func<Task> cmd = async () =>
				{
					try
					{
						await execute(this.OneDriveClient);
						isError = false;
					}
					catch (Exception e)
					{
						// 期限切れ
						if (e.Message.Contains("request_token_expired") || e.InnerException?.Message.Contains("request_token_expired") == true)
						{
							try
							{
								var options = new OneDriveRestAPI.Options
								{
									ClientId = OneDriveStorageLicense.ClientId,
									CallbackUrl = "https://login.live.com/oauth20_desktop.srf",
									RefreshToken = this.OneDriveClient.UserRefreshToken,

									AutoRefreshTokens = true,
									PrettyJson = false,
									ReadRequestsPerSecond = 2,
									WriteRequestsPerSecond = 2
								};
								var login = await new Client(options).RefreshAccessTokenAsync();
								this.OneDriveClient.UserAccessToken = login.Access_Token;
								this.OneDriveClient.UserRefreshToken = login.Refresh_Token;
								await execute(this.OneDriveClient);
								isError = false;
							}
							catch
							{
								this.AutoSaveNetworkError();
							}
						}

						else if (e is TaskCanceledException)
						{
							isMore = true;
						}

						// その他
						else
						{
							this.AutoSaveNetworkError();
						}
					}
				};

				lock(this)
					Task.Run(cmd).Wait();
			}

			return isError;
		}

		public override void Login()
		{
			var options = new OneDriveRestAPI.Options
			{
				ClientId = OneDriveStorageLicense.ClientId,
				CallbackUrl = "https://login.live.com/oauth20_desktop.srf",

				AutoRefreshTokens = true,
				PrettyJson = false,
				ReadRequestsPerSecond = 2,
				WriteRequestsPerSecond = 2
			};
			var client = new OneDriveRestAPI.Client(options);
			var authRequestUrl = client.GetAuthorizationRequestUrl(new[] {
					OneDriveRestAPI.Model.Scope.Basic,
					OneDriveRestAPI.Model.Scope.Signin,
					OneDriveRestAPI.Model.Scope.SkyDrive,
					OneDriveRestAPI.Model.Scope.SkyDriveUpdate,
					OneDriveRestAPI.Model.Scope.OfflineAccess,
				});
			
			// 認証ページをブラウザで開く
			var browser = AutofacUtil.AuthBrowserProvider.OpenBrowser();
			browser.GoToUrl(authRequestUrl);

			browser.UrlChanged += async (sender, e) =>
			{
				var parameters = HttpUtil.GetUrlParameters(e.Url);
				if (parameters.ContainsKey("code"))
				{
					// 認証ならば、トークンを取得する
					var token = await client.GetAccessTokenAsync(parameters["code"]);
					var options2 = new OneDriveRestAPI.Options
					{
						ClientId = OneDriveStorageLicense.ClientId,
						ClientSecret = OneDriveStorageLicense.ClientSecret,
						CallbackUrl = "https://login.live.com/oauth20_desktop.srf",
						AccessToken = token.Access_Token,
						RefreshToken = token.Refresh_Token,
						AutoRefreshTokens = true,
					};
					this.OneDriveClient = new OneDriveRestAPI.Client(options2);

					this.IsLogining = true;

					// ブラウザを閉じる
					browser.Close();

					// ここでログイン処理が行われる
					Action cmdLogin = () =>
					{
						try
						{
							this.HasLogined = true;
							this._isInitializing = false;
						}
						catch (Exception ex)
						{

						}
					};
					await Task.Run(cmdLogin);
				}
			};
		}

		protected override StorageWorkspaceBase NewStorageWorkspace(int number, bool isReadOnly)
		{
			return new OneDriveStorageWorkspace(number, isReadOnly, this);
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
						slotGetter().SlotComment = this.DummySlot.SlotComment;      // CheckExistsの実行も含む
					}
				}
			};
			Task.Run(cmd).Wait();
		}

		private Dictionary<FolderKey, OneDriveRestAPI.Model.File> _folderCache = new Dictionary<FolderKey, OneDriveRestAPI.Model.File>();
		private readonly object _folderCacheLock = new object();
		private void AddFolderCache(FolderKey key, OneDriveRestAPI.Model.File file)
		{
			lock (this._folderCacheLock)
			{
				if (!this._folderCache.ContainsKey(key))
				{
					this._folderCache.Add(key, file);
				}
			}
		}

		public OneDriveRestAPI.Model.File GetFolder(string name, string parentId = null)
		{
			OneDriveRestAPI.Model.File folder = null;

			// キャッシュから探す
			var key = new FolderKey(name, parentId);
			if (this._folderCache.ContainsKey(key))
			{
				folder = this._folderCache[key];
			}

			else
			{
				IEnumerable<OneDriveRestAPI.Model.File> contents = null;
				this.OneDriveExecute(async c => contents = await c.GetContentsAsync(parentId));
				bool existFlag = false;
				if (contents != null)
				{
					foreach (var c in contents)
					{
						if (c.Name == name)
						{
							existFlag = true;
							folder = c;
						}
						// 検索対象でないものもまとめてキャッシュにぶちこむ
						var tmpKey = new FolderKey(c.Name, parentId);
						if (!this._folderCache.Keys.Any(k => k.Equals(tmpKey)))
						{
							this.AddFolderCache(tmpKey, c);
						}
					}
				}
				if (!existFlag)
				{
					this.OneDriveExecute(async c => await c.CreateFolderAsync(parentId, name));
					this.AddFolderCache(key, folder);
				}
			}

			return folder;
		}

		private bool _isInitializing = true;
		private ICollection<string> _searchedDirectoryId = new Collection<string>();
		public OneDriveRestAPI.Model.File GetFile(string name, string parentId = null, bool isCreate = true, bool forceSearch = false)
		{
			OneDriveRestAPI.Model.File file = null;

			// キャッシュから探す
			var key = new FolderKey(name, parentId);
			if (this._folderCache.ContainsKey(key))
			{
				file = this._folderCache[key];
			}

			else
			{
				if (this._isInitializing)
				{
					if ((parentId == null || forceSearch || !this._searchedDirectoryId.Contains(parentId)))
					{
						lock (this._folderCacheLock)
						{
							if (!this._searchedDirectoryId.Contains(parentId ?? ""))
								this._searchedDirectoryId.Add(parentId ?? "");
						}
					}
					else
					{
						// 初期化中にスロットを保存するなんてありえないので、前回の検索で存在しなければあっさりnull返す
						return null;
					}
				}
				
				IEnumerable<OneDriveRestAPI.Model.File> contents = null;
				this.OneDriveExecute(async c => contents = await c.GetContentsAsync(parentId));
				bool existFlag = false;
				if (contents != null)
				{
					foreach (var c in contents)
					{
						if (c.Name == name)
						{
							existFlag = true;
							file = c;
						}
						// 検索対象でないものもまとめてキャッシュにぶちこむ
						var tmpKey = new FolderKey(c.Name, parentId);
						if (!this._folderCache.Keys.Any(k => k.Equals(tmpKey)))
						{
							this.AddFolderCache(tmpKey, c);
						}
					}
				}
				if (!existFlag)
				{
					if (isCreate)
					{
						using (Stream s = new MemoryStream(new byte[] { }))
						{
							this.OneDriveExecute(async c =>
							{
								var fileInfo = await c.UploadAsync(parentId, s, name, OneDriveRestAPI.Model.OverwriteOption.Overwrite);
								file = await c.GetFileAsync(fileInfo.Id);
							});
						}
						this.AddFolderCache(key, file);
					}
				}
			}

			return file;
		}

		public void DeleteFileCache(string name, string parentId = null)
		{
			var key = new FolderKey(name, parentId ?? "");
			if (this._folderCache.ContainsKey(key))
			{
				this._folderCache.Remove(key);
			}
		}

		public override string ToString()
		{
			return "OneDrive";
		}

		private struct FolderKey
		{
			public string Name { get; }
			public string ParentId { get; }
			public FolderKey(string name, string parentId)
			{
				this.Name = name;
				this.ParentId = parentId;
			}
			public override bool Equals(object obj)
			{
				if (obj is FolderKey)
				{
					var t = (FolderKey)obj;
					return t.Name == this.Name && t.ParentId == this.ParentId;
				}
				return base.Equals(obj);
			}
			public override int GetHashCode()
			{
				return this.Name.GetHashCode() | (this.ParentId?.GetHashCode() ?? 5);
			}
		}
	}
}
