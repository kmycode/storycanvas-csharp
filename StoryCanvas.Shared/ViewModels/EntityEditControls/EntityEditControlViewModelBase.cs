using StoryCanvas.Shared.Models.Entities;
using StoryCanvas.Shared.ViewTools;
using System;
using System.Collections.Generic;
using System.Text;

namespace StoryCanvas.Shared.ViewModels.EntityEditControls
{
    /// <summary>
    /// エンティティを編集する部分に適用するビューモデルの基底クラス
    /// </summary>
    public class EntityEditControlViewModelBase<T> : ViewModelBase
        where T : Entity
    {
        /// <summary>
        /// 編集対象のエンティティ
        /// </summary>
        public T Entity
        {
            get => this._entity;
            set
            {
                if (this._entity != value)
                {
                    this._entity = value;
                    this.OnPropertyChanged();
                }
            }
        }
        private T _entity;
    }
}
