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

    [Obsolete]
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
		}

		public override string ToString()
		{
			return StringResourceResolver.Resolve("StorageLocal");
		}
	}
}
