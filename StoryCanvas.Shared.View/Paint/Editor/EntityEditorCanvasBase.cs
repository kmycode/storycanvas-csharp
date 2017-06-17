using StoryCanvas.Shared.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace StoryCanvas.Shared.View.Paint.Editor
{
    /// <summary>
    /// エンティティエディタのキャンバスの基底クラス
    /// </summary>
    public abstract class EntityEditorCanvasBase<T> : CanvasBase
        where T : Entity
    {
        /// <summary>
        /// 選択中のエンティティ
        /// </summary>
        public T SelectedEntity
        {
            get => this._selectedEntity;
            protected set
            {
                if (this._selectedEntity?.Id != value?.Id)
                {
                    this._selectedEntity = value;
                    this.SelectedEntityChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }
        private T _selectedEntity;

        /// <summary>
        /// 選択中のエンティティが変更された時に呼び出し
        /// </summary>
        public event EventHandler SelectedEntityChanged;
    }
}
