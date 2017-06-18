using StoryCanvas.Shared.Models.Editor.Map;
using StoryCanvas.Shared.Models.Entities;
using StoryCanvas.Shared.Models.EntityRelate;
using StoryCanvas.Shared.Models.Story;
using StoryCanvas.Shared.View.Paint.Editor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;

namespace StoryCanvas.Shared.Models.Editor
{
    /// <summary>
    /// エンティティのエディタで、値などを保持するモデル
    /// </summary>
    [DataContract]
    public abstract class EntityEditorModelBase<T> : INotifyPropertyChanged
        where T : Entity
    {
        /// <summary>
        /// 現在選択中のエンティティ
        /// </summary>
        public T SelectedEntity
        {
            get => this._selectedEntity;
            protected set
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
        /// 要素が選択されているか
        /// </summary>
        public bool IsEntitySelected
        {
            get => this._isEntitySelected;
            private set
            {
                if (this._isEntitySelected != value)
                {
                    this._isEntitySelected = value;
                    this.OnPropertyChanged();
                }
            }
        }
        private bool _isEntitySelected;

        /// <summary>
        /// もととなるストーリー
        /// </summary>
        public StoryModel Story { get; }

        public EntityEditorModelBase(StoryModel story)
        {
            this.Story = story;

            this.PropertyChanged += (sender, e) =>
            {
                this.IsEntitySelected = this.SelectedEntity != null;
            };
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    [DataContract]
    public abstract class EntityEditorWithSingleCanvasModelBase<T> : EntityEditorModelBase<T>
        where T : Entity
    {
        /// <summary>
        /// マップデータ
        /// </summary>
        [DataMember]
        private SimpleEntityMapGroup<T> _mapGroup = new SimpleEntityMapGroup<T>();
        public SimpleEntityMapGroup<T> MapGroup => this._mapGroup;

        /// <summary>
        /// キャンバス
        /// </summary>
        public EntityEditorCanvasWithSimpleMapBase<T> Canvas { get; }

        protected EntityEditorWithSingleCanvasModelBase(StoryModel story, EntityEditorCanvasWithSimpleMapBase<T> canvas) : base(story)
        {
            this.Canvas = canvas;

            this.MapGroup.SelectedMapChanged += (sender, e) =>
            {
                this.Canvas.Map = this.MapGroup.SelectedMap;
                this.Canvas.RequestRedraw(true);
            };
        }
    }

    [DataContract]
    public abstract class EntityEditorWithEachRelationModelBase<T> : EntityEditorWithSingleCanvasModelBase<T>
        where T : Entity
    {
        /// <summary>
        /// 現在選択中の関連付け
        /// </summary>
        public EntityRelateBase<T, T> SelectedRelation
        {
            get => this._selectedRelation;
            protected set
            {
                if (this._selectedRelation?.Id != value?.Id)
                {
                    this._selectedRelation = value;
                    this.OnPropertyChanged();
                }
            }
        }
        private EntityRelateBase<T, T> _selectedRelation;

        protected EntityEditorWithEachRelationModelBase(StoryModel story, EntityEditorCanvasWithSimpleMapBase<T> canvas) : base(story, canvas)
        {
        }
    }
}
