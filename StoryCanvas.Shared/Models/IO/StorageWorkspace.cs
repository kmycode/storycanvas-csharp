using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using PCLStorage;
using StoryCanvas.Shared.Common;

namespace StoryCanvas.Shared.Models.IO
{
    public class StorageWorkspace : StorageWorkspaceBase
	{
        /// <summary>
        /// フォルダ
        /// </summary>
        [Obsolete]
        protected IFolder Folder;

        [Obsolete]
        public StorageWorkspace(int workspaceNumber, bool isReadOnly = false)
			: base(workspaceNumber, isReadOnly)
		{
			this.Initialize();
		}

        [Obsolete]
        protected override void CreateWorkspaceFolder()
		{
			Func<Task> cmd = async () =>
			{
#if WPF
				var localFolder = await FileSystem.Current.GetFolderFromPathAsync(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
				var rootFolder = await localFolder.CreateFolderAsync("StoryCanvas", CreationCollisionOption.OpenIfExists);
#elif WINDOWS_UWP
				var rootFolder = FileSystem.Current.LocalStorage;
#elif XAMARIN_FORMS
				var rootFolder = FileSystem.Current.LocalStorage;
#endif

				// ワークスペースのフォルダを作成
				var folderName = "workspace" + this.WorkspaceNumber;
				this.Folder = await rootFolder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);
			};

			Task.Run(cmd).Wait();
		}

        [Obsolete]
        protected override System.IO.Stream OpenIndexFile()
		{
			System.IO.Stream stream = null;

			Func<Task> cmd = async () =>
			{
				var indexFile = await this.Folder.CreateFileAsync("index.dat", CreationCollisionOption.OpenIfExists);
				stream = await indexFile.OpenAsync(FileAccess.Read);
			};

			Task.Run(cmd).Wait();

			return stream;
		}

        [Obsolete]
        protected override StorageSlotBase NewStorageSlot(int slotNum, string name, string comment, bool isReadOnly)
		{
			return new StorageSlot(this.Folder, slotNum, name, comment, isReadOnly);
		}

        [Obsolete]
        protected override void UpdateIndexFile()
		{
			Func<Task> cmd = async () =>
			{
				var indexFile = await this.Folder.CreateFileAsync("index.dat", CreationCollisionOption.OpenIfExists);
				var serialization = new SerializationModel();
				using (System.IO.Stream stream = await indexFile.OpenAsync(FileAccess.ReadAndWrite))
				{
					serialization.Serialize(stream, this.IndexData);
				}
			};
			Task.Run(cmd).Wait();
		}

		/// <summary>
		/// インデックスファイルの中身（全体）
		/// </summary>
		[DataContract]
		public struct SlotIndexData
		{
			[DataMember]
			private List<SlotIndexDataInfo> _dataInfoes;
			public List<SlotIndexDataInfo> DataInfoes
			{
				get
				{
					if (this._dataInfoes == null)
					{
						this._dataInfoes = new List<SlotIndexDataInfo>();
					}

					return this._dataInfoes;
				}
			}
		}

		/// <summary>
		/// インデックスファイルの中身（スロットごと）
		/// </summary>
		[DataContract]
		public struct SlotIndexDataInfo
		{
			[DataMember]
			public int SlotNumber { get; set; }

			[DataMember]
			public string SlotName { get; set; }

			[DataMember]
			public string SlotComment { get; set; }

            [DataMember]
            public DateTime LastModified { get; set; }
		}
	}
}
