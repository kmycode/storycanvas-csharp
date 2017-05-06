using System;
using System.Collections.Generic;
using System.Text;
using StoryCanvas.Shared.Messages.EditEntity;
using StoryCanvas.Shared.Models.Entities;
using StoryCanvas.Shared.Models.EntitySet;
using StoryCanvas.Shared.Models.EntityRelate;
using StoryCanvas.Shared.Models.Story;
using StoryCanvas.Shared.ViewTools;
using StoryCanvas.Shared.Common;
using StoryCanvas.Shared.Messages.Common;

namespace StoryCanvas.Shared.ViewModels.Entities
{
	public class ChapterEntityViewModel : EntityViewModelBase<ChapterEntity>
	{
		protected override ChapterEntity CreateDummyEntity()
		{
			return new ChapterEntity();
		}

		public ChapterEntityViewModel()
		{
			base.EntityChanged += (e, old) =>
			{
				this.SelectedSceneRelation = null;
				this.OnPropertyChanged("RelatedScenes");
				this.OnPropertyChanged("NotRelatedScenes");
			};

			StoryEditorModel.Default.MainModeChanged += (o, n) =>
			{
				if (n == Types.MainMode.EditChapter)
				{
					if (!this.IsDummy)
					{
						this.Entity = this.Entity;
					}
				}
			};
		}

		#region シーンとの関連付け

		/// <summary>
		/// 関連付けるために選択されたシーン
		/// </summary>
		private EntitySelectionModel<SceneEntity> _forRelateSceneSelection = new EntitySelectionModel<SceneEntity>();
		public SceneEntity SelectedSceneForRelate
		{
			get
			{
				return this._forRelateSceneSelection.Selected;
			}
			set
			{
				this._forRelateSceneSelection.Selected = value;
			}
		}

		/// <summary>
		/// 選択されたシーンとの関連付け
		/// </summary>
		private EntitySelectionModel<SceneChapterEntityRelate> _relatedSceneSelection = new EntitySelectionModel<SceneChapterEntityRelate>();
		public SceneChapterEntityRelate SelectedSceneRelation
		{
			get
			{
				return this._relatedSceneSelection.Selected;
			}
			set
			{
				this._relatedSceneSelection.Selected = value;
			}
		}

		/// <summary>
		/// シーン全部のリスト
		/// </summary>
		public ICollection<SceneEntity> Scenes
		{
			get
			{
				return this.Entity.StoryModel.Scenes;
			}
		}

		/// <summary>
		/// 関連付けられたシーンのリスト
		/// </summary>
		public IEnumerable<SceneChapterEntityRelate> RelatedScenes
		{
			get
			{
				return this.Entity.RelatedScenes;
			}
		}

		/// <summary>
		/// 関連付けられていないシーンのリスト
		/// </summary>
		public IEnumerable<SceneEntity> NotRelatedScenes
		{
			get
			{
				return this.Entity.NotRelatedScenes;
			}
		}

		/// <summary>
		/// シーンとの関連付けを追加
		/// </summary>
		private RelayCommand _addSceneRelationCommand;
		public RelayCommand AddSceneRelationCommand
		{
			get
			{
				return this._addSceneRelationCommand = this._addSceneRelationCommand ?? new RelayCommand((obj) =>
				{
					if (this._forRelateSceneSelection.Selected != null)
					{
						this.Entity.StoryModel.SceneChapterRelation.AddRelate(this.SelectedSceneForRelate, this.Entity);
						this.OnPropertyChanged("RelatedScenes");
						this.OnPropertyChanged("NotRelatedScenes");
					}
					else
					{
						Messenger.Default.Send(this, new AlertMessage(StringResourceResolver.Resolve("SelectSceneMessage")));
					}
				});
			}
		}

		/// <summary>
		/// シーンとの関連付けを削除
		/// </summary>
		private RelayCommand _removeSceneRelationCommand;
		public RelayCommand RemoveSceneRelationCommand
		{
			get
			{
				return this._removeSceneRelationCommand = this._removeSceneRelationCommand ?? new RelayCommand((obj) =>
				{
					if (this.SelectedSceneRelation != null)
					{
						this.Entity.StoryModel.SceneChapterRelation.RemoveRelate(this.SelectedSceneRelation.Entity1, this.Entity);
						this.OnPropertyChanged("RelatedScenes");
						this.OnPropertyChanged("NotRelatedScenes");
					}
					else
					{
						Messenger.Default.Send(this, new AlertMessage(StringResourceResolver.Resolve("SelectRelationMessage")));
					}
				});
			}
		}

		/// <summary>
		/// シーンとの関連付けを編集
		/// </summary>
		private RelayCommand _editSceneRelationCommand;
		public RelayCommand EditSceneRelationCommand
		{
			get
			{
				return this._editSceneRelationCommand = this._editSceneRelationCommand ?? new RelayCommand((obj) =>
				{
					Messenger.Default.Send(this, new EntityRelationEditorMessage(
						new EntityRelationEditorModel
						{
							Direction = EntityRelationEditorModel.RelationDirection.Entity1,
							AddCommand = this.AddSceneRelationCommand,
							RemoveCommand = this.RemoveSceneRelationCommand,
							RelatedEntityItemsGetter = () => this.RelatedScenes,
							NotRelatedEntitiesGetter = () => this.NotRelatedScenes,
							RelatedEntityItemSelection = new EntitySelectionModelWrapper<IRelationEntity, SceneChapterEntityRelate>(this._relatedSceneSelection),
							EntityForRelateSelection = new EntitySelectionModelWrapper<Entity, SceneEntity>(this._forRelateSceneSelection),
						}));
				});
			}
		}

		#endregion
	}
}
