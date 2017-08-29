using StoryCanvas.Shared.Models.Editor;
using StoryCanvas.Shared.Models.Entities;
using StoryCanvas.Shared.Models.EntityRelate;
using StoryCanvas.Shared.Models.EntitySet;
using StoryCanvas.Shared.ViewTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StoryCanvas.Shared.ViewModels.EntityEditors
{
    public interface IEntityPickerViewModel<out E>
        where E : Entity
    {
        E SelectedEntity { get; }
    }

    /// <summary>
    /// 要素のエディタのビューモデル基底クラス
    /// </summary>
    public class EntityEditorViewModelBase<E> : ViewModelBase, IEntityPickerViewModel<E>
        where E : Entity, new()
    {
        public EntityEditorModelBase<E> Editor { get; }

        private EntitySetModel<E> entitySet;

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
        public E SelectedEntity
        {
            get => this.Editor.SelectedEntity;
            set => this.Editor.SelectedEntity = value;
        }

        /// <summary>
        /// 現在要素が選択されているか
        /// </summary>
        public bool IsEntitySelected => this.Editor.IsEntitySelected;

        protected EntityEditorViewModelBase(EntityEditorModelBase<E> model, EntitySetModel<E> entitySet)
        {
            this.Editor = model;
            this.entitySet = entitySet;
            this.StoreModel(this.Editor);
        }

        /// <summary>
        /// 新規エンティティを追加するコマンド
        /// </summary>
        public RelayCommand AddCommand
        {
            get
            {
                return this._addCommand = this._addCommand ?? new RelayCommand((obj) =>
                {
                    var entity = new E();
                    this.entitySet.Add(entity);
                    this.SelectedEntity = entity;
                });
            }
        }
        private RelayCommand _addCommand;

        /// <summary>
        /// 選択されたエンティティを削除するコマンド
        /// </summary>
        public RelayCommand RemoveCommand
        {
            get
            {
                return this._removeCommand = this._removeCommand ?? new RelayCommand((obj) =>
                {
                    if (this.SelectedEntity != null)
                    {
                        var list = this.entitySet.ToList();
                        var index = list.IndexOf(this.SelectedEntity);
                        var afterSelection = list.ElementAtOrDefault(index + 1) ?? list.ElementAtOrDefault(index - 1);

                        this.entitySet.Remove(this.SelectedEntity);

                        this.SelectedEntity = afterSelection;
                    }
                });
            }
        }
        private RelayCommand _removeCommand;

        /// <summary>
        /// エンティティを上に移動するコマンド
        /// </summary>
        public RelayCommand UpCommand
        {
            get
            {
                return this._upCommand = this._upCommand ?? new RelayCommand((obj) =>
                {
                    if (this.SelectedEntity != null)
                    {
                        var afterSelection = this.SelectedEntity;
                        this.entitySet.Up(this.SelectedEntity);
                        this.SelectedEntity = afterSelection;
                    }
                });
            }
        }
        private RelayCommand _upCommand;

        /// <summary>
        /// エンティティを下に移動するコマンド
        /// </summary>
        public RelayCommand DownCommand
        {
            get
            {
                return this._downCommand = this._downCommand ?? new RelayCommand((obj) =>
                {
                    if (this.SelectedEntity != null)
                    {
                        var afterSelection = this.SelectedEntity;
                        this.entitySet.Down(this.SelectedEntity);
                        this.SelectedEntity = afterSelection;
                    }
                });
            }
        }
        private RelayCommand _downCommand;
    }

    public class EntityEditorWithEachRelationViewModelBase<E> : EntityEditorViewModelBase<E>
        where E : Entity, new()
    {
        /// <summary>
        /// 現在選択されている関連付け
        /// </summary>
        public EntityRelateBase<E, E> SelectedRelation
        {
            get => (this.Editor as IRelationSelectable<E, E>)?.SelectedRelation;
        }

        protected EntityEditorWithEachRelationViewModelBase(EntityEditorModelBase<E> model, EntitySetModel<E> entitySet) : base(model, entitySet)
        {
        }
    }
}
