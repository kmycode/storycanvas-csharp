using System;
using System.Collections.Generic;
using System.Text;
using StoryCanvas.Shared.Common;
using StoryCanvas.Shared.Messages.Common;
using StoryCanvas.Shared.Messages.EditEntity;
using StoryCanvas.Shared.Models.Entities;
using StoryCanvas.Shared.Models.EntityRelate;
using StoryCanvas.Shared.Models.EntitySet;
using StoryCanvas.Shared.Models.Story;
using StoryCanvas.Shared.ViewTools;

namespace StoryCanvas.Shared.ViewModels.Entities
{
	public class WordEntityViewModel : EntityViewModelBase<WordEntity>
	{
		protected override WordEntity CreateDummyEntity()
		{
			return new WordEntity();
		}

		public WordEntityViewModel()
		{
			base.EntityChanged += (e, old) =>
			{
				this.OnPropertyChanged("RelatedPeople");
				this.OnPropertyChanged("NotRelatedPeople");
				this.OnPropertyChanged("RelatedScenes");
				this.OnPropertyChanged("NotRelatedScenes");
			};

			StoryEditorModel.Default.MainModeChanged += (o, n) =>
			{
				if (n == Types.MainMode.EditWord)
				{
					if (!this.IsDummy)
					{
						this.Entity = this.Entity;
					}
				}
			};
		}

		#region 人物との関連付け

		/// <summary>
		/// 関連付けるために選択された人物
		/// </summary>
		private EntitySelectionModel<PersonEntity> _forRelatePersonSelection = new EntitySelectionModel<PersonEntity>();
		public PersonEntity SelectedPersonForRelate
		{
			get
			{
				return this._forRelatePersonSelection.Selected;
			}
			set
			{
				this._forRelatePersonSelection.Selected = value;
			}
		}

		/// <summary>
		/// 選択された人物との関連付け
		/// </summary>
		private EntitySelectionModel<WordPersonEntityRelate> _relatedPersonSelection = new EntitySelectionModel<WordPersonEntityRelate>();
		public WordPersonEntityRelate SelectedPersonRelation
		{
			get
			{
				return this._relatedPersonSelection.Selected;
			}
			set
			{
				this._relatedPersonSelection.Selected = value;
			}
		}

		/// <summary>
		/// 人物全員のリスト
		/// </summary>
		public ICollection<PersonEntity> People
		{
			get
			{
				return this.Entity.StoryModel.People;
			}
		}

		/// <summary>
		/// 関連付けられた人物のリスト
		/// </summary>
		public IEnumerable<WordPersonEntityRelate> RelatedPeople
		{
			get
			{
				return this.Entity.RelatedPeople;
			}
		}

		/// <summary>
		/// 関連付けられていない人物のリスト
		/// </summary>
		public IEnumerable<PersonEntity> NotRelatedPeople
		{
			get
			{
				return this.Entity.NotRelatedPeople;
			}
		}

		/// <summary>
		/// 人物との関連付けを追加
		/// </summary>
		private RelayCommand _addPersonRelationCommand;
		public RelayCommand AddPersonRelationCommand
		{
			get
			{
				return this._addPersonRelationCommand = this._addPersonRelationCommand ?? new RelayCommand((obj) =>
				{
					if (this.SelectedPersonForRelate != null)
					{
						this.Entity.StoryModel.WordPersonRelation.AddRelate(this.Entity, this.SelectedPersonForRelate);
						this.OnPropertyChanged("RelatedPeople");
						this.OnPropertyChanged("NotRelatedPeople");
					}
					else
					{
						Messenger.Default.Send(this, new AlertMessage(StringResourceResolver.Resolve("SelectPersonMessage")));
					}
				});
			}
		}

		/// <summary>
		/// 人物との関連付けを削除
		/// </summary>
		private RelayCommand _removePersonRelationCommand;
		public RelayCommand RemovePersonRelationCommand
		{
			get
			{
				return this._removePersonRelationCommand = this._removePersonRelationCommand ?? new RelayCommand((obj) =>
				{
					if (this.SelectedPersonRelation != null)
					{
						this.Entity.StoryModel.WordPersonRelation.RemoveRelate(this.Entity, this.SelectedPersonRelation.Entity2);
						this.OnPropertyChanged("RelatedPeople");
						this.OnPropertyChanged("NotRelatedPeople");
					}
					else
					{
						Messenger.Default.Send(this, new AlertMessage(StringResourceResolver.Resolve("SelectRelationMessage")));
					}
				});
			}
		}

		/// <summary>
		/// 人物との関連付けを編集
		/// </summary>
		private RelayCommand _editPersonRelationCommand;
		public RelayCommand EditPersonRelationCommand
		{
			get
			{
				return this._editPersonRelationCommand = this._editPersonRelationCommand ?? new RelayCommand((obj) =>
				{
					Messenger.Default.Send(this, new EntityRelationEditorMessage(
						new EntityRelationEditorModel
						{
							Direction = EntityRelationEditorModel.RelationDirection.Entity2,
							AddCommand = this.AddPersonRelationCommand,
							RemoveCommand = this.RemovePersonRelationCommand,
							RelatedEntityItemsGetter = () => this.RelatedPeople,
							NotRelatedEntitiesGetter = () => this.NotRelatedPeople,
							RelatedEntityItemSelection = new EntitySelectionModelWrapper<IRelationEntity, WordPersonEntityRelate>(this._relatedPersonSelection),
							EntityForRelateSelection = new EntitySelectionModelWrapper<Entity, PersonEntity>(this._forRelatePersonSelection),
						}));
				});
			}
		}

		#endregion

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
		private EntitySelectionModel<WordSceneEntityRelate> _relatedSceneSelection = new EntitySelectionModel<WordSceneEntityRelate>();
		public WordSceneEntityRelate SelectedSceneRelation
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
		/// シーン全員のリスト
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
		public IEnumerable<WordSceneEntityRelate> RelatedScenes
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
					if (this.SelectedSceneForRelate != null)
					{
						this.Entity.StoryModel.WordSceneRelation.AddRelate(this.Entity, this.SelectedSceneForRelate);
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
						this.Entity.StoryModel.WordSceneRelation.RemoveRelate(this.Entity, this.SelectedSceneRelation.Entity2);
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
							Direction = EntityRelationEditorModel.RelationDirection.Entity2,
							AddCommand = this.AddSceneRelationCommand,
							RemoveCommand = this.RemoveSceneRelationCommand,
							RelatedEntityItemsGetter = () => this.RelatedScenes,
							NotRelatedEntitiesGetter = () => this.NotRelatedScenes,
							RelatedEntityItemSelection = new EntitySelectionModelWrapper<IRelationEntity, WordSceneEntityRelate>(this._relatedSceneSelection),
							EntityForRelateSelection = new EntitySelectionModelWrapper<Entity, SceneEntity>(this._forRelateSceneSelection),
						}));
				});
			}
		}

		#endregion
	}
}
