using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OneDriveRestAPI;
using StoryCanvas.Shared.Models.IO;

namespace Libraries.StoryCanvas.Native.Models.IO
{
	class OneDriveStorageSlot : StorageSlotBase
	{
		private OneDriveStorageWorkspace Workspace;
		private bool isWriting;

		public OneDriveStorageSlot(OneDriveStorageWorkspace workspace, int slotNumber, string name, string comment, bool isReadOnly = false) : base(slotNumber, name, comment, isReadOnly)
		{
			this.Workspace = workspace;
			this.CheckExists();
			this.SlotComment = comment;
			this.SlotInfoChanging = false;
		}

		public override void CheckExists()
		{
			if (this.isWriting)
			{
				// 保存中は必ずtrueを返す
				this.IsExists = true;
			}
			else
			{
				this.IsExists = this.Workspace.GetFile(this.FileName, this.Workspace.GetWorkspaceFolder().Id, false) != null;
			}
		}

		public override Stream OpenStream()
		{
			Stream stream = new MemoryStream();
			var file = this.Workspace.GetFile(this.FileName, this.Workspace.GetWorkspaceFolder().Id, false);
			this.Workspace.Storage.OneDriveExecute(async c => stream = await c.DownloadAsync(file.Id));
			return stream;
		}

		public override Stream WriteStream()
		{
			this.isWriting = true;
			var stream = new RelayStream(new MemoryStream());
			stream.Flushed += (sender, e) =>
			{
				var bytes = new byte[e.Stream.Length];
				e.Stream.Seek(0, SeekOrigin.Begin);
				e.Stream.Read(bytes, 0, bytes.Length);
				if (!this.Workspace.Storage.OneDriveExecute(async c => await c.UploadAsync(this.Workspace.GetWorkspaceFolder().Id, new MemoryStream(bytes), this.FileName, OneDriveRestAPI.Model.OverwriteOption.Overwrite)))
				{
					this.LastSaveDateTime = DateTime.Now;
				}
				this.isWriting = false;
			};
			return stream;
		}

		protected override void Delete_Private()
		{
			var file = this.Workspace.GetFile(this.FileName, this.Workspace.GetWorkspaceFolder().Id, false);
			if (file != null)
			{
				this.Workspace.Storage.OneDriveExecute(async c => await c.DeleteAsync(file.Id));
				this.Workspace.Storage.DeleteFileCache(file.Name, file.Parent_Id);
			}
		}

		protected override DateTime CheckLastSaveDateTime()
		{
			DateTime time = default(DateTime);
			this.Workspace.Storage.OneDriveExecute(async (c) =>
			{
				var file = this.Workspace.GetFile(this.FileName, this.Workspace.GetWorkspaceFolder().Id, false);
				var f = await c.GetFileAsync(file.Id);
				time = Convert.ToDateTime(f.Updated_Time).ToLocalTime();
			});
			return time;
		}
	}
}
