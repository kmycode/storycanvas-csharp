using StoryCanvas.Shared.Models.Story;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StoryCanvas.Shared.Models.IO
{
    /// <summary>
    /// スロットへの保存と、現在のスロットの管理を行うサービス。オートセーブもサポート
    /// </summary>
    /// <typeparam name="T">保存するオブジェクトの型</typeparam>
    class SlotSaveService<T>
    {
        /// <summary>
        /// 上書き保存可能か
        /// </summary>
        public bool CanOverWriteSave => this.Slot != null;

        /// <summary>
        /// 保存先のスロット
        /// </summary>
        public ISlot Slot { get; private set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="isDependsCurrentStorage">現在のストレージに依存するか（ストレージ変更時、オートセーブ先を消去するか）</param>
        public SlotSaveService(bool isDependsCurrentStorage = true)
        {
            if (isDependsCurrentStorage)
            {
                StorageUtil.CurrentStorageChanged += (sender, e) =>
                {
                    this.Slot = null;
                };
            }
        }

        public async Task SaveAsync(T obj, ISlot slot)
        {
            this.Slot = slot;
            await this.SaveAsync(obj);
        }

        public async Task SaveAsync(T obj)
        {
            this.Slot.LastModified = DateTime.Now;
            await this.Slot.File.SaveAsync(obj);
        }

        public async Task AutoSaveAsync(T obj)
        {
            if (this.Slot != null)
            {
                await this.SaveAsync(obj);
            }
        }

        public async Task<T> LoadAsync(ISlot slot)
        {
            this.Slot = slot;
            return await this.LoadAsync();
        }

        public async Task<T> LoadAsync()
        {
            return await this.Slot.File.LoadAsync<T>();
        }
    }
}
