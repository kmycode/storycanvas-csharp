using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryCanvas.Shared.Models.IO
{
	public class DropboxStorageSlot : StorageSlotBase
	{
		private DropboxStorageWorkspace Workspace;
		private bool isWriting;

		public DropboxStorageSlot(DropboxStorageWorkspace workspace, int slotNumber, string name, string comment, bool isReadOnly = false) : base(slotNumber, name, comment, isReadOnly)
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
				this.IsExists = this.Workspace.Storage.GetMetaData("/workspace" + this.Workspace.WorkspaceNum).Contents.Any(c => c.Name == this.FileName);
			}
		}

		public override Stream OpenStream()
		{
			Stream stream = null;

			this.Workspace.Storage.DropboxExecute(async (client) =>
			{
				stream = await client.GetFileStream("/workspace" + this.Workspace.WorkspaceNum + "/" + this.FileName);
			});

			return stream;
		}

		public override Stream WriteStream()
		{
			this.isWriting = true;
			var stream = new RelayStream(new MemoryStream());

			stream.Flushed += (sender, e) =>
			{
				var isError = this.Workspace.Storage.DropboxExecute(async (client) =>
				{
					e.Stream.Seek(0, SeekOrigin.Begin);
					await client.Upload("/workspace" + this.Workspace.WorkspaceNum + "/", this.FileName, e.Stream);
				});
				this.isWriting = false;
				if (!isError)
				{
					this.LastSaveDateTime = DateTime.Now;
				}
			};

			return stream;
		}

		protected override void Delete_Private()
		{
			this.Workspace.Storage.DropboxExecute(async (client) =>
			{
				await client.Delete("/workspace" + this.Workspace.WorkspaceNum + "/" + this.FileName);
			});
		}

		protected override DateTime CheckLastSaveDateTime()
		{
			DateTime time = default(DateTime);
			this.Workspace.Storage.DropboxExecute(async (c) =>
			{
				var meta = await c.GetMetaData("/workspace" + this.Workspace.WorkspaceNum + "/" + this.FileName);
				time = meta.ModifiedDate.ToLocalTime();
			});
			return time;
		}
	}
}
