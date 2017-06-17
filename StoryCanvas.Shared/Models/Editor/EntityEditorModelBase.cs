using StoryCanvas.Shared.Models.Entities;
using StoryCanvas.Shared.Models.Story;
using StoryCanvas.Shared.View.Paint.Editor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace StoryCanvas.Shared.Models.Editor
{
    /// <summary>
    /// エンティティのエディタで、値などを保持するモデル
    /// </summary>
    public abstract class EntityEditorModelBase<T> : INotifyPropertyChanged
        where T : Entity
    {
        /// <summary>
        /// 現在編集中のエンティティ
        /// </summary>
        public T Entity
        {
            get => this._entity;
            set
            {
                if (this._entity?.Id != value?.Id)
                {
                    this._entity = value;
                    this.OnPropertyChanged();
                }
            }
        }
        private T _entity;

        /// <summary>
        /// 現在選択中のエンティティ
        /// </summary>
        public T SelectedEntity
        {
            get => this._selectedEntity;
            private set
            {
                if (this._selectedEntity?.Id != value?.Id)
                {
                    this._selectedEntity = value;
                    this.OnPropertyChanged();
                }
            }
        }
        private T _selectedEntity;

        /// <summary>
        /// もととなるストーリー
        /// </summary>
        public StoryModel Story { get; }

        public EntityEditorModelBase(StoryModel story)
        {
            this.Story = story;
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
