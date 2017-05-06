using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DropNetRT.Exceptions;

namespace StoryCanvas.Shared.Models.IO
{
	public class DropboxStorageWorkspace : StorageWorkspaceBase
	{
		public DropboxStorageModel Storage;

		public int WorkspaceNum
		{
			get
			{
				return base.WorkspaceNumber;
			}
		}

		public DropboxStorageWorkspace(DropboxStorageModel storage, int workspaceNumber, bool isReadOnly = false) : base(workspaceNumber, isReadOnly)
		{
			this.Storage = storage;
			this.Initialize();
		}

		protected override void CreateWorkspaceFolder()
		{
			this.Storage.DropboxExecute(async (client) =>
			{
				await client.CreateFolder("/workspace" + this.WorkspaceNumber);
			});
		}

		protected override StorageSlotBase NewStorageSlot(int slotNum, string name, string comment, bool isReadOnly)
		{
			return new DropboxStorageSlot(this, slotNum, name, comment, isReadOnly);
		}

		protected override Stream OpenIndexFile()
		{
			Stream stream = null;

			this.Storage.DropboxExecute(async (client) =>
			{
				stream = await client.GetFileStream("/workspace" + this.WorkspaceNumber + "/index.dat");
			});

			return stream ?? new MemoryStream();
		}

		protected override void UpdateIndexFile()
		{
			using (RelayStream stream = new RelayStream(new MemoryStream(), false))
			{
				stream.Flushed += (sender, e) =>
				{
					this.Storage.DropboxExecute(async (client) =>
					{
						e.Stream.Seek(0, SeekOrigin.Begin);
						var bytes = new byte[e.Stream.Length];
						e.Stream.Read(bytes, 0, bytes.Length);
						await client.Upload("/workspace" + this.WorkspaceNumber + "/", "index.dat", bytes);
					});
				};

				var serialization = new SerializationModel();
				serialization.Serialize(stream, this.IndexData);
				stream.Finish();
			}
		}
	}
}
