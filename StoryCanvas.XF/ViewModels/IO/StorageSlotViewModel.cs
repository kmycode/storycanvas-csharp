using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoryCanvas.Messages.IO;
using StoryCanvas.Models.IO;
using StoryCanvas.Shared.ViewTools;

namespace StoryCanvas.ViewModels.IO
{
	class StorageSlotViewModel : ViewModelBase
	{
		/// <summary>
		/// スロットの配列
		/// </summary>
		public List<StorageSlot> Slots { get; private set; } = new List<StorageSlot>();

		/// <summary>
		/// スロットの数
		/// </summary>
		private readonly int SlotNum;

		/// <summary>
		/// 選択されているスロット
		/// </summary>
		public StorageSlot SelectedSlot { get; set; }

		/// <summary>
		/// 初期化
		/// </summary>
		public StorageSlotViewModel(bool isReadOnly = true)
		{
			// プラットフォームによって変える　検討できればする
			this.SlotNum = 4;

			for (int i = 0; i < this.SlotNum; i++)
			{
				this.Slots.Add(new StorageSlot(i + 1, isReadOnly));
			}
		}

		/// <summary>
		/// スロットを選択してメッセージを発行
		/// </summary>
		private RelayCommand _slotSelectCommand;
		public RelayCommand SlotSelectCommand
		{
			get
			{
				return this._slotSelectCommand = this._slotSelectCommand ?? new RelayCommand((obj) =>
				{
					if (this.SelectedSlot != null)
					{
						Messenger.Default.Send(this, new SlotChoosedMessage(this.SelectedSlot.GetFile()));
					}
				});
			}
		}
	}
}
