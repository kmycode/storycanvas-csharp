using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using PCLStorage;
using StoryCanvas.Shared.Common;
using StoryCanvas.Shared.Messages.Common;
using StoryCanvas.Shared.Messages.IO;
using StoryCanvas.Shared.Models.Story;
using StoryCanvas.Shared.ViewTools;

namespace StoryCanvas.Shared.Models.IO
{
	public enum AutoSaveStatus
	{
		Enabled,
		DisabledBecauseNoSlotSelected,		// 保存されていないので無効
		DisabledBecauseOfStoryConfig,		// ストーリー設定でオートセーブが切られてるので無効
		DisabledBecauseWaitingUserAction,	// ユーザのアクションを待機中のため無効（競合確認メッセージなど）
		NetworkError,						// ネットワークエラー（一定時間経過後に再試行する）
	}

    public class StorageModel : StorageModelBase
	{
		// シングルトン
		public static readonly StorageModel Default = new StorageModel();
		private StorageModel() : base(4)
		{
			//this.HasLogined = true;
#if XAMARIN_FORMS
			this.CheckOldSlots();			// 1.1以前に対応
#endif
		}

		/// <summary>
		/// Android、iOSには旧仕様（v1.1以前）のスロットがあるので、もしあれば新しいスロットへ移動する
		/// </summary>
		protected override void CheckOldSlots()
		{
			Func<Task> cmd = async () =>
			{
				var folder = FileSystem.Current.LocalStorage;
				var newFolder = await FileSystem.Current.LocalStorage.CreateFolderAsync("workspace1", CreationCollisionOption.OpenIfExists);
				for (int i = 1; i <= 4; i++)
				{
					var fileName = "slot" + i + ".dat";
					if (await folder.CheckExistsAsync(fileName) == ExistenceCheckResult.FileExists)
					{
						var file = await folder.GetFileAsync(fileName);
						await file.MoveAsync(newFolder.Path + "/" + fileName);
					}
				}
			};
			Task.Run(cmd).Wait();
		}

		protected override StorageWorkspaceBase NewStorageWorkspace(int number, bool isReadOnly)
		{
			return new StorageWorkspace(number, isReadOnly);
		}

		protected override void SaveReallySlot(Func<StorageSlotBase> slotGetter, bool isAutoSave)
		{
			Func<Task> cmd = async () =>
			{
				// 保存する前のハッシュチェックは行わない

				await this.DummySlot.GetLocalFile().MoveAsync(((StorageSlot)slotGetter()).GetLocalFile().Path);
				slotGetter().SlotName = this.DummySlot.IsEmptySlotName ? "" : this.DummySlot.SlotName;
				slotGetter().SlotComment = this.DummySlot.SlotComment;

				// Moveしちゃったので次回保存の時にダミーファイルがない！ってなるので、
				// ダミーファイルを新しく作りなおす
				this.CreateDummySlot();
			};
			Task.Run(cmd).Wait();
		}

		public override void Login()
		{
			this.HasLogined = true;
			//throw new NotImplementedException();

			/*
			Func<Task> cmd = async () =>
			{
				var client = new DropNetRT.DropNetClient("6m0xdh0cknuc9jr", "tkv5uv2m51nbzjl");
				var reqToken = await client.GetRequestToken();
				Messenger.Default.Send(this, new Messages.Network.OpenUrlMessage(client.BuildAuthorizeUrl(reqToken)));

				Task.Delay(5000).Wait();
				client.UserLogin = await client.GetAccessToken();
				using (var s = new System.IO.MemoryStream(Encoding.UTF8.GetBytes("こんにちは")))
					await client.Upload("/", "test.txt", s);
			};
			Task.Run(cmd).Wait();
			*/
			/*
			Action cmd = () =>
			{
				var options = new OneDriveRestAPI.Options
				{
					ClientId = "ba74befd-5c70-49ad-8c87-caf7accc342f",
					//ClientSecret = "49Jucm3FOJkrD6y5z1guhJr",
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
				});

				//Messenger.Default.Send(this, new Messages.Network.OpenUrlMessage(authRequestUrl));
				var browser = StoryCanvas.Shared.Utils.AutofacUtil.AuthBrowserProvider.OpenBrowser();
				browser.GoToUrl(authRequestUrl);
				browser.UrlChanged += async (sender, e) =>
				{
					var parameters = Utils.HttpUtil.GetUrlParameters(e.Url);
					if (parameters.ContainsKey("code"))
					{
						var token = await client.GetAccessTokenAsync(parameters["code"]);
						var options2 = new OneDriveRestAPI.Options
						{
							ClientId = "ba74befd-5c70-49ad-8c87-caf7accc342f",
							ClientSecret = "49Jucm3FOJkrD6y5z1guhJr",
							CallbackUrl = "https://login.live.com/oauth20_desktop.srf",
							AccessToken = token.Access_Token,
							RefreshToken = token.Refresh_Token,
						};
						var client2 = new OneDriveRestAPI.Client(options2);
						var rootFolder = await client2.GetFolderAsync();
						var contents = await client2.GetContentsAsync(rootFolder.Id);
						bool existFlag = false;
						OneDriveRestAPI.Model.File folder = null;
						foreach (var c in contents)
						{
							if (c.Name == "StoryCanvasCloud")
							{
								existFlag = true;
								folder = c;
							}
						}
						if (!existFlag)
						{
							folder = await client2.CreateFolderAsync(rootFolder.Id, "StoryCanvasCloud", "StoryCanvas datas");
						}
						await client2.UploadAsync(folder.Id, new System.IO.MemoryStream(Encoding.UTF8.GetBytes("こんにちは")), "test.txt", OneDriveRestAPI.Model.OverwriteOption.Overwrite);

						System.Windows.MessageBox.Show("完了");
					}
				};
			};
			cmd.Invoke();
			*/
		}

		public override string ToString()
		{
			return StringResourceResolver.Resolve("StorageLocal");
		}
	}
}
