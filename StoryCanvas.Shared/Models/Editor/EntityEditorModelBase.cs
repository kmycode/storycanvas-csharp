﻿using StoryCanvas.Shared.Models.Common;
using StoryCanvas.Shared.Models.Editor.Map;
using StoryCanvas.Shared.Models.Entities;
using StoryCanvas.Shared.Models.EntityRelate;
using StoryCanvas.Shared.Models.EntitySet;
using StoryCanvas.Shared.Models.Story;
using StoryCanvas.Shared.View.Paint.Editor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
            set
            {
                if (this._selectedEntity?.Id != value?.Id)
                {
                    var old = this._selectedEntity;
                    this._selectedEntity = value;
                    this.OnPropertyChanged();
                    this.EntitySelectionChanged?.Invoke(this, new ValueChangedEventArgs<T>
                    {
                        OldValue = old,
                        NewValue = value,
                    });
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

        public virtual void CopyTo(EntityEditorModelBase<T> to)
        {
        }
        
        /// <summary>
        /// エンティティの選択が変更された時に発行
        /// </summary>
        public ValueChangedEventHandler<T> EntitySelectionChanged;

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
        private SimpleEntityMapGroup<T> _mapGroup;
        public SimpleEntityMapGroup<T> MapGroup
        {
            get
            {
                if (this._mapGroup == null)
                {
                    this._mapGroup = new SimpleEntityMapGroup<T>();
                }
                return this._mapGroup;
            }
        }

        /// <summary>
        /// キャンバス
        /// </summary>
        public EntityEditorCanvasWithSimpleMapBase<T> Canvas { get; }

        public IEnumerable<T> Entities { get; }

        protected EntityEditorWithSingleCanvasModelBase(StoryModel story, EntityEditorCanvasWithSimpleMapBase<T> canvas, IEnumerable<T> entities) : base(story)
        {
            this.Canvas = canvas;
            this.Entities = entities;

            this.MapGroup.SelectedMapChanged += (sender, e) =>
            {
                this.UpdateMapSelection();
            };

            this.Canvas.SelectedEntityChanged += (sender, e) =>
            {
                this.SelectedEntity = e.NewValue;
            };

            this.Canvas.DrawUpdating += (sender, e) =>
            {
                // 現在削除された要素を追跡
                var removedEntities = this.Canvas.Map.Elements.Select(el => el.Entity).Except(entities).ToArray();
                foreach (var entity in removedEntities)
                {
                    var element = this.Canvas.Map.Elements.SingleOrDefault(el => el.Entity == entity);
                    if (element != null)
                    {
                        this.Canvas.Map.Elements.Remove(element);
                    }
                }
            };
        }

        protected EntityEditorWithSingleCanvasModelBase(EntityEditorWithSingleCanvasModelBase<T> other, EntityEditorCanvasWithSimpleMapBase<T> canvas) : this(other.Story, canvas, other.Entities)
        {
            this._mapGroup = other._mapGroup;
            this.UpdateMapSelection();
        }

        private void UpdateMapSelection()
        {
            this.Canvas.Map = this.MapGroup.SelectedMap;
            this.Canvas.RequestRedraw(true);
        }

        public override void CopyTo(EntityEditorModelBase<T> to)
        {
            base.CopyTo(to);

            if (to is EntityEditorWithSingleCanvasModelBase<T> too)
            {
                too.MapGroup.Maps.Clear();
                too.MapGroup.Maps.AddRange(this.MapGroup.Maps);
            }
        }

        public void LoadEntities(EntitySetModel<T> entities)
        {
            this.MapGroup.LoadEntities(entities);
        }
    }

    public interface IRelationSelectable<E1, E2>
        where E1 : Entity where E2 : Entity
    {
        EntityRelateBase<E1, E2> SelectedRelation { get; set; }
    }

    [DataContract]
    public abstract class EntityEditorWithEachRelationModelBase<T> : EntityEditorWithSingleCanvasModelBase<T>, IRelationSelectable<T, T>
        where T : Entity
    {
        /// <summary>
        /// 現在選択中の関連付け
        /// </summary>
        public EntityRelateBase<T, T> SelectedRelation
        {
            get => this._selectedRelation;
            set
            {
                if (this._selectedRelation?.Id != value?.Id)
                {
                    var old = this._selectedRelation;
                    this._selectedRelation = value;
                    this.OnPropertyChanged();
                    this.RelationSelectionChanged?.Invoke(this, new ValueChangedEventArgs<EntityRelateBase<T, T>>
                    {
                        OldValue = old,
                        NewValue = value,
                    });
                }
            }
        }
        private EntityRelateBase<T, T> _selectedRelation;

        protected EntityEditorWithEachRelationModelBase(StoryModel story, EntityEditorCanvasWithSimpleMapBase<T> canvas, IEnumerable<T> entities) : base(story, canvas, entities)
        {
            this.Initialize();
        }

        protected EntityEditorWithEachRelationModelBase(EntityEditorWithEachRelationModelBase<T> other, EntityEditorCanvasWithSimpleMapBase<T> canvas) : base(other, canvas)
        {
            this.Initialize();
        }

        private void Initialize()
        {
            this.Canvas.SelectedRelationChanged += (sender, e) =>
            {
                this.SelectedRelation = e.NewValue;
            };
        }

        /// <summary>
        /// 関連付けの選択が変更された時に発行
        /// </summary>
        public event ValueChangedEventHandler<EntityRelateBase<T, T>> RelationSelectionChanged;
    }
}
