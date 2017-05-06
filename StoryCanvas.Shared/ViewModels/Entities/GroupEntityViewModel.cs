using System;
using System.Collections.Generic;
using System.Text;
using StoryCanvas.Shared.Models.Entities;
using StoryCanvas.Shared.Models.EntitySet;
using StoryCanvas.Shared.Models.EntityRelate;
using StoryCanvas.Shared.Models.Story;
using StoryCanvas.Shared.ViewTools;
using StoryCanvas.Shared.Messages.EditEntity;
using StoryCanvas.Shared.Common;
using StoryCanvas.Shared.Messages.Common;

namespace StoryCanvas.Shared.ViewModels.Entities
{
	public class GroupEntityViewModel : EntityViewModelBase<GroupEntity>
	{
		protected override GroupEntity CreateDummyEntity()
		{
			return new GroupEntity();
		}

		public GroupEntityViewModel()
		{
			base.EntityChanged += (e, old) =>
			{
				this.SelectedPersonRelation = null;
				this.OnPropertyChanged("RelatedPeople");
				this.OnPropertyChanged("NotRelatedPeople");
				this.OnPropertyChanged("RelatedPersonParameters");
			};

			StoryEditorModel.Default.MainModeChanged += (o, n) =>
			{
				if (n == Types.MainMode.EditGroup)
				{
					if (!this.IsDummy)
					{
						this.Entity = this.Entity;
					}
				}
			};
		}

		#region 人物との関連付けで利用するパラメータ

		/// <summary>
		/// 人物との関連付けで利用するパラメータ
		/// </summary>
		public EntityListModel<ParameterEntity> RelatedPersonParameters => this.Entity.RelatedPersonParameters;

		/// <summary>
		/// 現在選択中のパラメータ項目
		/// </summary>
		public ParameterEntity SelectedRelatedPersonParameter { get; set; }

		/// <summary>
		/// 人物との関連付けを追加
		/// </summary>
		private RelayCommand _addRelatedPersonParameterCommand;
		public RelayCommand AddRelatedPersonParameterCommand
		{
			get
			{
				return this._addRelatedPersonParameterCommand = this._addRelatedPersonParameterCommand ?? new RelayCommand((obj) =>
				{
					this.RelatedPersonParameters.Add(new ParameterEntity());
				});
			}
		}

		/// <summary>
		/// 人物との関連付けを削除
		/// </summary>
		private RelayCommand _removeRelatedPersonParameterCommand;
		public RelayCommand RemoveRelatedPersonParameterCommand
		{
			get
			{
				return this._removeRelatedPersonParameterCommand = this._removeRelatedPersonParameterCommand ?? new RelayCommand((obj) =>
				{
					if (this.SelectedRelatedPersonParameter != null)
					{
						this.RelatedPersonParameters.Remove(this.SelectedRelatedPersonParameter);
					}
					else
					{
						Messenger.Default.Send(this, new AlertMessage(StringResourceResolver.Resolve("SelectRelationMessage")));
					}
				});
			}
		}

		#endregion

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
		private EntitySelectionModel<GroupPersonEntityRelate> _relatedPersonSelection = new EntitySelectionModel<GroupPersonEntityRelate>();
		public GroupPersonEntityRelate SelectedPersonRelation
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
		public IEnumerable<GroupPersonEntityRelate> RelatedPeople
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
						this.Entity.StoryModel.GroupPersonRelation.AddRelate(this.Entity, this.SelectedPersonForRelate);
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
						this.Entity.StoryModel.GroupPersonRelation.RemoveRelate(this.Entity, this.SelectedPersonRelation.Entity2);
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
							RelatedEntityItemSelection = new EntitySelectionModelWrapper<IRelationEntity, GroupPersonEntityRelate>(this._relatedPersonSelection),
							EntityForRelateSelection = new EntitySelectionModelWrapper<Entity, PersonEntity>(this._forRelatePersonSelection),
						}));
				});
			}
		}

		#endregion
	}
}
