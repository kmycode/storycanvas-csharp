using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using StoryCanvas.Shared.Common;
using StoryCanvas.Shared.Messages.Common;
using StoryCanvas.Shared.Messages.IO;
using StoryCanvas.Shared.Messages.Page;
using StoryCanvas.Shared.Models.IO;
using StoryCanvas.Shared.Models.Story;
using StoryCanvas.Shared.Utils;
using StoryCanvas.Shared.ViewTools;
using StoryCanvas.Shared.ViewTools.BehaviorHelpers;

namespace StoryCanvas.Shared.ViewModels
{
    public class StorageViewModel : ViewModelBase
	{
        public ObservableCollection<IStorage> Storages { get; } = new ObservableCollection<IStorage>
        {
            StorageUtil.LocalStorage,
        };

        public IStorage Storage
        {
            get => StorageUtil.CurrentStorage;
            set => StorageUtil.CurrentStorage = value;
        }

        public string SlotComment
        {
            get => StoryModel.Current.StoryConfig.Comment;
            set => StoryModel.Current.StoryConfig.Comment = value;
        }

        public ErrorMessageHelper ErrorHelper => StoryModel.Current.ErrorHelper;

		public StorageViewModel()
		{
            StorageUtil.CurrentStorageChanged += (sender, e) =>
            {
                this.OnPropertyChanged(nameof(Storage));
            };

            StorageUtil.InitializeStorages();
		}

		/// <summary>
		/// スロットを選択してメッセージを発行、その後の処理を行う
		/// </summary>
		public RelayCommand<ISlot> SaveCommand
		{
			get
			{
				return this._saveCommand = this._saveCommand ?? new RelayCommand<ISlot>((slot) =>
				{
                    StoryModel.Current.Save(slot);
				});
			}
        }
        private RelayCommand<ISlot> _saveCommand;

        /// <summary>
        /// 最後に保存したスロットに対して保存
        /// </summary>
		public RelayCommand SaveLastSlotCommand
		{
			get
			{
				return this._saveLastSlotCommand = this._saveLastSlotCommand ?? new RelayCommand((obj) =>
				{
				});
			}
		}
        private RelayCommand _saveLastSlotCommand;

        /// <summary>
        /// スロットを選択してメッセージを発行、その後の処理を行う
        /// </summary>
        public RelayCommand<ISlot> LoadCommand
		{
			get
			{
				return this._loadCommand = this._loadCommand ?? new RelayCommand<ISlot>((slot) =>
                {
                    StoryModel.Current.Load(slot);
                    this.OnPropertyChanged(nameof(SlotComment));
                });
			}
		}
        private RelayCommand<ISlot> _loadCommand;

        /// <summary>
        /// スロットを削除
        /// </summary>
		public RelayCommand<ISlot> DeleteCommand
		{
			get
			{
				return this._deleteCommand = this._deleteCommand ?? new RelayCommand<ISlot>((slot) =>
				{
				});
			}
		}
        private RelayCommand<ISlot> _deleteCommand;

        /// <summary>
        /// ストーリーを新規作成
        /// </summary>
        private RelayCommand _createNewCommand;
		public RelayCommand CreateNewCommand
		{
			get
			{
				return this._createNewCommand = this._createNewCommand ?? new RelayCommand((obj) =>
				{
					StoryModel.Current.CreateNew();
				});
			}
		}

		/// <summary>
		/// クラウドなどへのログイン
		/// </summary>
		public RelayCommand LoginCommand
		{
			get
			{
				return this._loginCommand = this._loginCommand ?? new RelayCommand(async (obj) =>
				{
                    await this.Storage.StartLoginAsync();
				});
			}
		}
        private RelayCommand _loginCommand;
    }
}
