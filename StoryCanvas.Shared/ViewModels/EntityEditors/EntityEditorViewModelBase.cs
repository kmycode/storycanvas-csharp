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
        /// 現在選択されている要素
        /// </summary>
        public T SelectedEntity
        {
            get => this.Editor.SelectedEntity;
        }

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
            get => this._selectedRelation;
            set
            {
                if (this._selectedRelation != value)
                {
                    this._selectedRelation = value;
                    this.OnPropertyChanged();
                }
            }
        }
        private EntityRelateBase<T, T> _selectedRelation;

        /// <summary>
        /// 関連付けが選択されているか
        /// </summary>
        public bool IsRelationSelected
        {
            get => this._isRelationSelected;
            private set
            {
                if (this._isRelationSelected != value)
                {
                    this._isRelationSelected = value;
                    this.OnPropertyChanged();
                }
            }
        }
        private bool _isRelationSelected;

        protected EntityEditorWithEachRelationViewModelBase(EntityEditorModelBase<T> model) : base(model)
        {
            this.Editor.PropertyChanged += (sender, e) =>
            {
                this.IsRelationSelected = this.SelectedRelation != null;
            };
        }
    }
}
