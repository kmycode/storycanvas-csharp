using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using StoryCanvas.Shared.Common;

namespace StoryCanvas.Shared.Models.IO
{
    [Obsolete]
    public abstract class StorageWorkspaceBase
	{
		private bool _isSlotsReadOnly;
		public bool IsSlotsReadOnly
		{
			get
			{
				return this._isSlotsReadOnly;
			}
			set
			{
				foreach (var slot in this.Slots)
				{
					slot.IsReadOnly = value;
				}
				this._isSlotsReadOnly = value;
			}
		}

		/// <summary>
		/// スロットの配列
		/// </summary>
		public List<StorageSlotBase> Slots { get; } = new List<StorageSlotBase>();

		/// <summary>
		/// スロットの数
		/// </summary>
		private int SlotNum;

		/// <summary>
		/// インデックスファイルの中のデータ
		/// </summary>
		protected StorageWorkspace.SlotIndexData IndexData;

		/// <summary>
		/// ワークスペース番号
		/// </summary>
		protected int WorkspaceNumber;

		/// <summary>
		/// ワークスペース名
		/// </summary>
		private string _workspaceName;
		public string WorkspaceName
		{
			get
			{
				return this._workspaceName;
			}
			protected set
			{
				this._workspaceName = value;
			}
		}
		public override string ToString()
		{
			return this.WorkspaceName;
		}

		protected StorageWorkspaceBase(int workspaceNumber, bool isReadOnly = false)
		{
			this.WorkspaceNumber = workspaceNumber;
			this.IsSlotsReadOnly = isReadOnly;
			this._workspaceName = StringResourceResolver.Resolve("Workspace") + this.WorkspaceNumber;
		}

		protected void Initialize()
		{
			this.CreateWorkspaceFolder();

			// インデクスファイルを開く
			var serialization = new SerializationModel();
			using (var stream = this.OpenIndexFile())
			{
				try
				{
					this.IndexData = serialization.Deserialize<StorageWorkspace.SlotIndexData>(stream);
				}
				catch
				{
					this.IndexData = new StorageWorkspace.SlotIndexData();
				}
			}

			// スロットを列挙
			this.SlotNum = 10;
			for (int i = 0; i < this.SlotNum; i++)
			{
				// インデクスファイルに該当するものがないか探す
				string comment = "";
				string name = "";
				foreach (var info in this.IndexData.DataInfoes)
				{
					if (info.SlotNumber == i + 1)
					{
						name = info.SlotName;
						comment = info.SlotComment;
						break;
					}
				}

				var slot = this.NewStorageSlot(i + 1, name, comment, this.IsSlotsReadOnly);
				slot.SlotInfoChanged += () => this.UpdateIndex();
				this.Slots.Add(slot);
			}
		}

		/// <summary>
		/// インデクスファイルを更新
		/// </summary>
		public void UpdateIndex()
		{
			this.IndexData = new StorageWorkspace.SlotIndexData();
			int i = 1;
			foreach (var slot in this.Slots)
			{
				if (slot.IsExists)
				{
					var info = new StorageWorkspace.SlotIndexDataInfo();
					info.SlotName = slot.SlotName;
					info.SlotNumber = i;
					info.SlotComment = slot.SlotComment;
					this.IndexData.DataInfoes.Add(info);
				}
				i++;
			}

			this.UpdateIndexFile();
		}

		/// <summary>
		/// ワークスペースのフォルダを作成
		/// </summary>
		protected abstract void CreateWorkspaceFolder();

		/// <summary>
		/// インデクスファイルを開く
		/// </summary>
		/// <returns>インデクスファイルを読み込むストリーム</returns>
		protected abstract Stream OpenIndexFile();

		/// <summary>
		/// 新しいスロットを取得
		/// </summary>
		/// <param name="slotNum"></param>
		/// <param name="name"></param>
		/// <param name="comment"></param>
		/// <returns></returns>
		protected abstract StorageSlotBase NewStorageSlot(int slotNum, string name, string comment, bool isReadOnly);

		/// <summary>
		/// インデクスファイルを更新
		/// </summary>
		protected abstract void UpdateIndexFile();
	}
}
