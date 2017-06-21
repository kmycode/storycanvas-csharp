using StoryCanvas.Shared.Models.Editor;
using StoryCanvas.Shared.Models.Entities;
using StoryCanvas.Shared.Models.EntityRelate;
using StoryCanvas.Shared.ViewTools;
using System;
using System.Collections.Generic;
using System.Text;

namespace StoryCanvas.Shared.ViewModels.EntityEditors
{
    /// <summary>
    /// 要素のエディタのビューモデル基底クラス
    /// </summary>
    public class EntityEditorViewModelBase<T> : ViewModelBase
        where T : Entity
    {
        public EntityEditorModelBase<T> Editor { get; }

        /// <summary>
        /// 選択オンリー（編集はできない）
        /// </summary>
        public bool IsSelectOnlyMode
        {
            get => this._isSelectOnlyMode;
            set
            {
                if (this._isSelectOnlyMode != value)
                {
                    this._isSelectOnlyMode = value;
                    this.OnPropertyChanged();
                }
            }
        }
        private bool _isSelectOnlyMode;
        
        /// <summary>
        /// 現在選択されている要素
        /// </summary>
        public T SelectedEntity
        {
            get => this.Editor.SelectedEntity;
            set => this.Editor.SelectedEntity = value;
        }

        /// <summary>
        /// 現在要素が選択されているか
        /// </summary>
        public bool IsEntitySelected => this.Editor.IsEntitySelected;

        protected EntityEditorViewModelBase(EntityEditorModelBase<T> model)
        {
            this.Editor = model;
            this.StoreModel(this.Editor);
        }
    }

    public class EntityEditorWithEachRelationViewModelBase<T> : EntityEditorViewModelBase<T>
        where T : Entity
    {
        /// <summary>
        /// 現在選択されている関連付け
        /// </summary>
        public EntityRelateBase<T, T> SelectedRelation
        {
            get => (this.Editor as IRelationSelectable<T, T>)?.SelectedRelation;
        }

        protected EntityEditorWithEachRelationViewModelBase(EntityEditorModelBase<T> model) : base(model)
        {
        }
    }
}
