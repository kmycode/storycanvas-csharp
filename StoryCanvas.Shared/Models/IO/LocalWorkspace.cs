using PCLStorage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace StoryCanvas.Shared.Models.IO
{
    /// <summary>
    /// ローカルワークスペースのクラス
    /// </summary>
    class LocalWorkspace : IWorkspace, INotifyPropertyChanged
    {
        private bool isInitialized;
        private IFolder folder;

        public ObservableCollection<ISlot> Slots { get; } = new ObservableCollection<ISlot>();

        public ISlot SelectedSlot
        {
            get => this._selectedSlot;
            set
            {
                if (this._selectedSlot != value)
                {
                    this._selectedSlot = value;
                    this.OnPropertyChanged();
                }
            }
        }
        private ISlot _selectedSlot;

        public int Id { get; set; }

        public string Name
        {
            get => this._name;
            set
            {
                if (this._name != value)
                {
                    this._name = value;
                    this.OnPropertyChanged();
                }
            }
        }
        private string _name;

        public async Task LoadAsync()
        {
            if (this.isInitialized) return;
            this.isInitialized = true;
            
            // ディレクトリの作成
            this.folder = await FileSystem.Current.LocalStorage.CreateFolderAsync("workspace" + this.Id, CreationCollisionOption.OpenIfExists);

            // インデクスファイルを開く
            var indexFile = new LocalFile(this.folder, "index.dat");
            StorageWorkspace.SlotIndexData index;
            try
            {
                index = await indexFile.LoadAsync<StorageWorkspace.SlotIndexData>();
            }
            catch
            {
                index = new StorageWorkspace.SlotIndexData();
            }

            // スロットの作成
            for (var i = 0; i < 10; i++)
            {
                var slot = new LocalSlot(this.folder)
                {
                    Id = i + 1,
                };
                slot.File.Created += async (sender, e) =>
                {
                    await this.UpdateIndexAsync();
                };
                slot.File.Modified += async (sender, e) =>
                {
                    await this.UpdateIndexAsync();
                };
                slot.File.Deleted += async (sender, e) =>
                {
                    await this.UpdateIndexAsync();
                };
                this.Slots.Add(slot);
            }

            // スロットのデータの追加
            foreach (var info in index.DataInfoes)
            {
                var slot = this.Slots[info.SlotNumber - 1];
                slot.Comment = info.SlotComment;
                slot.Name = info.SlotName;
                slot.LastModified = info.LastModified;
                slot.IsExists = true;
            }

            this.SelectedSlot = this.Slots[0];
        }

        private async Task UpdateIndexAsync()
        {
            var index = new StorageWorkspace.SlotIndexData();
            foreach (var slot in this.Slots.Where(s => s.IsExists))
            {
                index.DataInfoes.Add(new StorageWorkspace.SlotIndexDataInfo
                {
                    SlotNumber = slot.Id,
                    SlotName = slot.Name,
                    SlotComment = slot.Comment,
                    LastModified = slot.LastModified,
                });
            }

            var indexFile = new LocalFile(this.folder, "index.dat");
            await indexFile.SaveAsync(index);
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
