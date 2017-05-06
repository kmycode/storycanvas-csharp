using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using OneDriveRestAPI;
using StoryCanvas.Shared.Models.IO;

namespace Libraries.StoryCanvas.Native.Models.IO
{
	public class OneDriveStorageWorkspace : StorageWorkspaceBase
	{
		public OneDriveStorageModel Storage;

		public OneDriveStorageWorkspace(int workspaceNumber, bool isReadOnly, OneDriveStorageModel storage) : base(workspaceNumber, isReadOnly)
		{
			this.Storage = storage;
			this.Initialize();
		}

		protected override void CreateWorkspaceFolder()
		{
			this.GetWorkspaceFolder();
		}

		private OneDriveRestAPI.Model.File GetRootFolder()
		{
			return this.GetFolder("StoryCanvasCloud");
		}

		public OneDriveRestAPI.Model.File GetWorkspaceFolder()
		{
			var parentFolder = this.GetRootFolder();
			return this.GetFolder("workspace" + this.WorkspaceNumber, parentFolder.Id);
		}

		public OneDriveRestAPI.Model.File GetFolder(string name, string parentId = null)
		{
			return this.Storage.GetFolder(name, parentId);
		}

		public OneDriveRestAPI.Model.File GetFile(string name, string parentId = null, bool isCreate = true, bool forceSearch = false)
		{
			return this.Storage.GetFile(name, parentId, isCreate, forceSearch);
		}

		protected override StorageSlotBase NewStorageSlot(int slotNum, string name, string comment, bool isReadOnly)
		{
			return new OneDriveStorageSlot(this, slotNum, name, comment, isReadOnly);
		}

		protected override Stream OpenIndexFile()
		{
			Stream stream = null;

			var folder = this.GetWorkspaceFolder();
			this.Storage.OneDriveExecute(async c => stream = await c.DownloadAsync(this.GetFile("index.dat", folder.Id, true, true).Id));

			return stream;
		}

		protected override void UpdateIndexFile()
		{
			using (RelayStream stream = new RelayStream(new MemoryStream()))
			{
				stream.Flushed += (sender, e) =>
				{
					e.Stream.Seek(0, SeekOrigin.Begin);
					var bytes = new byte[e.Stream.Length];
					e.Stream.Read(bytes, 0, bytes.Length);
					var folder = this.GetWorkspaceFolder();
					using (var s = new MemoryStream(bytes))
						this.Storage.OneDriveExecute(async c => await c.UploadAsync(folder.Id, s, "index.dat", OneDriveRestAPI.Model.OverwriteOption.Overwrite));
				};

				var serialization = new SerializationModel();
				serialization.Serialize(stream, this.IndexData);
			}
		}
	}
}
