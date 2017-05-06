using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using PCLStorage;
using StoryCanvas.Shared.Common;
using StoryCanvas.Shared.Messages.Common;
using StoryCanvas.Shared.Messages.Page;
using StoryCanvas.Shared.ViewTools;

namespace StoryCanvas.Shared.Models.IO
{
	/// <summary>
	/// セーブ・ロードに使うスロット
	/// </summary>
	public class StorageSlot : StorageSlotBase
	{
		private IFolder Folder;

		public StorageSlot(IFolder folder, int slotNumber, string name, string comment, bool isReadOnly = false)
			: base(slotNumber, name, comment, isReadOnly)
		{
			this.Folder = folder;
			this.CheckExists();
			this.SlotComment = comment;
			this.SlotInfoChanging = false;
		}

		public override void CheckExists()
		{
			Func<Task> cmd = async () =>
			{
				this.IsExists = (await this.Folder.CheckExistsAsync(this.FileName)) == ExistenceCheckResult.FileExists;
			};
			Task.Run(cmd).Wait();
		}

		public override Stream OpenStream()
		{
			Stream stream = null;
			Func<Task> cmd = async () =>
			{
				stream = await this.GetFile().OpenAsync(PCLStorage.FileAccess.Read);
			};
			Task.Run(cmd).Wait();
			return stream;
		}

		public override Stream WriteStream()
		{
			Stream stream = null;
			Func<Task> cmd = async () =>
			{
				stream = await this.GetFile().OpenAsync(PCLStorage.FileAccess.ReadAndWrite);
				this.LastSaveDateTime = DateTime.Now;
			};
			Task.Run(cmd).Wait();
			return stream;
		}

		/// <summary>
		/// ファイルオブジェクトを取得
		/// </summary>
		/// <returns>ファイルオブジェクト</returns>
		private IFile GetFile()
		{
			Func<Task<IFile>> cmd = async () =>
			{
				if (!this.IsReadOnly)
				{
					return await this.Folder.CreateFileAsync(this.FileName, CreationCollisionOption.OpenIfExists);
				}
				return null;
			};
			var result = Task.Run(cmd);
			result.Wait();
			return result.Result;
		}

		/// <summary>
		/// ファイルオブジェクトを取得
		/// （仕様変更があり、外部からは基本的にStorageSlotではなくStorageSlotBaseを使うことになった。
		/// 　GetFileをpublicのままにしてると、外部の修正漏れがコンパイル時に見逃される可能性が高いので
		/// 　あえてこれをprivateにして、新しい名前のメソッドを作っておく）
		/// </summary>
		/// <returns></returns>
		public IFile GetLocalFile()
		{
			return this.GetFile();
		}

		protected override void Delete_Private()
		{
			Func<Task> cmd = async () =>
			{
				await this.GetFile().DeleteAsync();
			};
			Task.Run(cmd).Wait();
		}

		protected override DateTime CheckLastSaveDateTime()
		{
			return this.LastSaveDateTime;
		}
	}
}
