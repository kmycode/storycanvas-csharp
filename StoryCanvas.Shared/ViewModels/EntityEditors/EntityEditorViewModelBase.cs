using StoryCanvas.Shared.Models.Editor;
using StoryCanvas.Shared.Models.Entities;
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

        public T Entity
        {
            get => this.Editor.Entity;
            set => this.Editor.Entity = value;
        }

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
}
