using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using StoryCanvas.Shared.Messages.EditEntity;
using StoryCanvas.Shared.Messages.Picker;
using StoryCanvas.Shared.Models.Date;
using StoryCanvas.Shared.Models.Entities;
using StoryCanvas.Shared.Models.EntitySet;
using StoryCanvas.Shared.Models.EntityRelate;
using StoryCanvas.Shared.Models.Story;
using StoryCanvas.Shared.ViewTools;
using static StoryCanvas.Shared.ViewTools.ControlModels.TreeListViewControlModel;
using StoryCanvas.Shared.Types;
using StoryCanvas.Shared.Messages.Page;
using StoryCanvas.Shared.Common;
using StoryCanvas.Shared.Messages.Common;

namespace StoryCanvas.Shared.ViewModels.Entities
{
	public class PersonEntityViewModel : EntityViewModelBase<PersonEntity>
	{
		public PersonEntityViewModel()
		{
			base.EntityChanged += (e, old) =>
			{
				try
				{
					GC.Collect();
				}
				catch { }

				this.SelectedPersonRelation = null;
				this.SelectedGroupRelation = null;
				this.OnPropertyChanged("RelatedPeople");
				this.OnPropertyChanged("NotRelatedPeople");
				this.OnPropertyChanged("RelatedGroups");
				this.OnPropertyChanged("NotRelatedGroups");
				this.OnPropertyChanged("NotRelatedGroupTreeItems");
				this.OnPropertyChanged("RelatedParameters");
				this.OnPropertyChanged("NotRelatedParameters");
				this.OnPropertyChanged("RelatedWords");
				this.OnPropertyChanged("NotRelatedWords");
				this.OnPropertyChanged("NotRelatedWordTreeItems");
				this.OnPropertyChanged("FirstName");
				this.OnPropertyChanged("LastName");
				this.OnPropertyChanged("IsWesternerName");
				this.OnPropertyChanged("BirthDay");
				this.OnPropertyChanged("DeathDay");
				this.OnPropertyChanged("Sex");
				this.ReloadGroupParameters();
			};

			StoryEditorModel.Default.MainModeChanged += (o, n) =>
			{
				if (n == Types.MainMode.EditPerson)
				{
					if (!this.IsDummy)
					{
						this.Entity = this.Entity;
#if XAMARIN_FORMS
						// XFのObjectPickerでは、NameをBindingできない
						((EntityListModel<SexEntity>)this.Sexes).OnCollectionChanged();
#endif
					}
				}
			};
		}

		protected override PersonEntity CreateDummyEntity()
		{
			return new PersonEntity();
		}

		/// <summary>
		/// 名前
		/// </summary>
		public string FirstName
		{
			get
			{
				return this.Entity.FirstName;
			}
			set
			{
				this.Entity.FirstName = value;
			}
		}

		/// <summary>
		/// 苗字
		/// </summary>
		public string LastName
		{
			get
			{
				return this.Entity.LastName;
			}
			set
			{
				this.Entity.LastName = value;
			}
		}

		/// <summary>
		/// 名前が欧米式（FirstNameを先にする）であるか
		/// </summary>
		public bool IsWesternerName
		{
			get
			{
				return this.Entity.IsWesternerName;
			}
			set
			{
				this.Entity.IsWesternerName = value;
			}
		}

		/// <summary>
		/// 誕生日
		/// </summary>
		public StoryDateTime BirthDay
		{
			get
			{
				return this.Entity.BirthDay;
			}
			set
			{
				this.Entity.BirthDay = value;
			}
		}

		/// <summary>
		/// 誕生日選択画面の表示
		/// </summary>
		private RelayCommand _birthDayPickerCommand;
		public RelayCommand BirthDayPickerCommand
		{
			get
			{
				return this._birthDayPickerCommand = this._birthDayPickerCommand ?? new RelayCommand((obj) =>
				{
					Messenger.Default.Send(this, new DatePickerMessage(StoryCalendar.AnnoDomini, this.BirthDay, (date) =>
					{
						this.BirthDay = date;
					}));
				});
			}
		}

		/// <summary>
		/// 死亡日
		/// </summary>
		public StoryDateTime DeathDay
		{
			get
			{
				return this.Entity.DeathDay;
			}
			set
			{
				this.Entity.DeathDay = value;
			}
		}

		/// <summary>
		/// 死亡日選択画面の表示
		/// </summary>
		private RelayCommand _deathDayPickerCommand;
		public RelayCommand DeathDayPickerCommand
		{
			get
			{
				return this._deathDayPickerCommand = this._deathDayPickerCommand ?? new RelayCommand((obj) =>
				{
					Messenger.Default.Send(this, new DatePickerMessage(StoryCalendar.AnnoDomini, this.DeathDay, (date) =>
					{
						this.DeathDay = date;
					}));
				});
			}
		}

		#region 性別との関連付け

		/// <summary>
		/// 性別の一覧
		/// </summary>
		public INotifyCollectionChanged Sexes
		{
			get
			{
				return this.Entity.StoryModel?.Sexes;
			}
		}

		/// <summary>
		/// 人物の性別
		/// </summary>
		public SexEntity Sex
		{
			get
			{
				return this.Entity.Sex;
			}
			set
			{
				this.Entity.Sex = value;
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
		private EntitySelectionModel<PersonPersonEntityRelate> _relatedPersonSelection = new EntitySelectionModel<PersonPersonEntityRelate>();
		public PersonPersonEntityRelate SelectedPersonRelation
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
		public IEnumerable<PersonPersonEntityRelate> RelatedPeople
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
						this.Entity.StoryModel.AddRelate(this.SelectedPersonForRelate, this.Entity);
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
						this.Entity.StoryModel.RemoveRelate(this.SelectedPersonRelation.Entity1, this.SelectedPersonRelation.Entity2);
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
							Direction = EntityRelationEditorModel.RelationDirection.NotFocused,
							AddCommand = this.AddPersonRelationCommand,
							RemoveCommand = this.RemovePersonRelationCommand,
							RelatedEntityItemsGetter = () => this.RelatedPeople,
							NotRelatedEntitiesGetter = () => this.NotRelatedPeople,
							RelatedEntityItemSelection = new EntitySelectionModelWrapper<IRelationEntity, PersonPersonEntityRelate>(this._relatedPersonSelection),
							EntityForRelateSelection = new EntitySelectionModelWrapper<Entity, PersonEntity>(this._forRelatePersonSelection),
						}));
				});
			}
		}

		#endregion

		#region 集団との関連付け

		/// <summary>
		/// 関連付けるために選択された集団
		/// </summary>
		private EntitySelectionModel<GroupEntity> _forRelateGroupSelection = new EntitySelectionModel<GroupEntity>();
		public TreeEntityListItem SelectedGroupForRelate
		{
			set
			{
				this._forRelateGroupSelection.Selected = value?.Entity as GroupEntity;
			}
		}

		// TODO: Xamarin Forms専用プロパティ
		public TreeEntity SelectedGroupEntityForRelate
		{
			set
			{
				this._forRelateGroupSelection.Selected = (GroupEntity)value;
			}
		}

		/// <summary>
		/// 選択された集団との関連付け
		/// </summary>
		private EntitySelectionModel<GroupPersonEntityRelate> _relatedGroupSelection = new EntitySelectionModel<GroupPersonEntityRelate>();
		public GroupPersonEntityRelate SelectedGroupRelation
		{
			get
			{
				return this._relatedGroupSelection.Selected;
			}
			set
			{
				this._relatedGroupSelection.Selected = value;
			}
		}

		/// <summary>
		/// 集団全部のリスト
		/// </summary>
		public ICollection<TreeEntityListItem> Groups
		{
			get
			{
				return this.Entity.StoryModel.Groups.List;
			}
		}

		/// <summary>
		/// 関連付けられた集団のリスト
		/// </summary>
		public IEnumerable<GroupPersonEntityRelate> RelatedGroups
		{
			get
			{
				return this.Entity.RelatedGroups;
			}
		}

		/// <summary>
		/// 関連付けられていない集団のリスト
		/// </summary>
		public IEnumerable<GroupEntity> NotRelatedGroups
		{
			get
			{
				return this.Entity.NotRelatedGroups;
			}
		}

		/// <summary>
		/// 関連付けられていない集団のリスト（ツリーアイテム用）
		/// </summary>
		public IEnumerable<TreeEntityListItem> NotRelatedGroupTreeItems
		{
			get
			{
				return this.Entity.NotRelatedGroupTreeItems;
			}
		}

		/// <summary>
		/// 集団との関連付けを追加
		/// </summary>
		private RelayCommand _addGroupRelationCommand;
		public RelayCommand AddGroupRelationCommand
		{
			get
			{
				return this._addGroupRelationCommand = this._addGroupRelationCommand ?? new RelayCommand((obj) =>
				{
					if (this._forRelateGroupSelection.Selected != null)
					{
						this.Entity.StoryModel.GroupPersonRelation.AddRelate(this._forRelateGroupSelection.Selected, this.Entity);
						this.OnPropertyChanged("RelatedGroups");
						this.OnPropertyChanged("NotRelatedGroups");
						this.OnPropertyChanged("NotRelatedGroupTreeItems");
					}
					else
					{
						Messenger.Default.Send(this, new AlertMessage(StringResourceResolver.Resolve("SelectGroupMessage")));
					}
				});
			}
		}

		/// <summary>
		/// 集団との関連付けを削除
		/// </summary>
		private RelayCommand _removeGroupRelationCommand;
		public RelayCommand RemoveGroupRelationCommand
		{
			get
			{
				return this._removeGroupRelationCommand = this._removeGroupRelationCommand ?? new RelayCommand((obj) =>
				{
					if (this.SelectedGroupRelation != null)
					{
						this.Entity.StoryModel.GroupPersonRelation.RemoveRelate(this.SelectedGroupRelation.Entity1, this.Entity);
						this.OnPropertyChanged("RelatedGroups");
						this.OnPropertyChanged("NotRelatedGroups");
						this.OnPropertyChanged("NotRelatedGroupTreeItems");
					}
					else
					{
						Messenger.Default.Send(this, new AlertMessage(StringResourceResolver.Resolve("SelectRelationMessage")));
					}
				});
			}
		}

		/// <summary>
		/// 集団との関連付けを編集
		/// </summary>
		private RelayCommand _editGroupRelationCommand;
		public RelayCommand EditGroupRelationCommand
		{
			get
			{
				return this._editGroupRelationCommand = this._editGroupRelationCommand ?? new RelayCommand((obj) =>
				{
					Messenger.Default.Send(this, new EntityRelationEditorMessage(
						new EntityRelationEditorModel
						{
							Direction = EntityRelationEditorModel.RelationDirection.Entity1,
							AddCommand = this.AddGroupRelationCommand,
							RemoveCommand = this.RemoveGroupRelationCommand,
							RelatedEntityItemsGetter = () => this.RelatedGroups,
							NotRelatedEntitiesGetter = () => this.NotRelatedGroups,
							RelatedEntityItemSelection = new EntitySelectionModelWrapper<IRelationEntity, GroupPersonEntityRelate>(this._relatedGroupSelection),
							EntityForRelateSelection = new EntitySelectionModelWrapper<Entity, GroupEntity>(this._forRelateGroupSelection),
						}));
				});
			}
		}

		#endregion

		#region パラメータとの関連付け

		/// <summary>
		/// 関連付けるために選択されたパラメータ
		/// </summary>
		private EntitySelectionModel<ParameterEntity> _forRelateParameterSelection = new EntitySelectionModel<ParameterEntity>();
		public ParameterEntity SelectedParameterForRelate
		{
			get
			{
				return this._forRelateParameterSelection.Selected;
			}
			set
			{
				this._forRelateParameterSelection.Selected = value;
				this.OnPropertyChanged();
			}
		}

		/// <summary>
		/// 選択されたパラメータとの関連付け
		/// </summary>
		private EntitySelectionModel<PersonParameterEntityRelate> _relatedParameterSelection = new EntitySelectionModel<PersonParameterEntityRelate>();
		public PersonParameterEntityRelate SelectedParameterRelation
		{
			get
			{
				return this._relatedParameterSelection.Selected;
			}
			set
			{
				this._relatedParameterSelection.Selected = value;
			}
		}

		/// <summary>
		/// パラメータ全部のリスト
		/// </summary>
		public ICollection<ParameterEntity> Parameters
		{
			get
			{
				return this.Entity.StoryModel.Parameters;
			}
		}

		/// <summary>
		/// 関連付けられたパラメータのリスト
		/// </summary>
		public IEnumerable<PersonParameterEntityRelate> RelatedParameters
		{
			get
			{
				return this.Entity.RelatedParameters;
			}
		}

		/// <summary>
		/// 関連付けられていないパラメータのリスト
		/// </summary>
		public IEnumerable<ParameterEntity> NotRelatedParameters
		{
			get
			{
				return this.Entity.NotRelatedParameters;
			}
		}

		/// <summary>
		/// パラメータとの関連付けを追加
		/// </summary>
		private RelayCommand _addParameterRelationCommand;
		public RelayCommand AddParameterRelationCommand
		{
			get
			{
				return this._addParameterRelationCommand = this._addParameterRelationCommand ?? new RelayCommand((obj) =>
				{
					if (this._forRelateParameterSelection.Selected != null)
					{
						this.Entity.StoryModel.PersonParameterRelation.AddRelate(this.Entity, this.SelectedParameterForRelate);
						this.OnPropertyChanged("RelatedParameters");
						this.OnPropertyChanged("NotRelatedParameters");
						this.SelectedParameterForRelate = null;
						this.SelectedParameterRelation = null;
					}
					else
					{
						Messenger.Default.Send(this, new AlertMessage(StringResourceResolver.Resolve("SelectParameterMessage")));
					}
				});
			}
		}

		/// <summary>
		/// パラメータとの関連付けを削除
		/// </summary>
		private RelayCommand _removeParameterRelationCommand;
		public RelayCommand RemoveParameterRelationCommand
		{
			get
			{
				return this._removeParameterRelationCommand = this._removeParameterRelationCommand ?? new RelayCommand((obj) =>
				{
					if (this.SelectedParameterRelation != null)
					{
						this.Entity.StoryModel.PersonParameterRelation.RemoveRelate(this.Entity, this.SelectedParameterRelation.Entity2);
						this.OnPropertyChanged("RelatedParameters");
						this.OnPropertyChanged("NotRelatedParameters");
						this.SelectedParameterForRelate = null;
						this.SelectedParameterRelation = null;
					}
					else
					{
						Messenger.Default.Send(this, new AlertMessage(StringResourceResolver.Resolve("SelectRelationMessage")));
					}
				});
			}
		}

		/// <summary>
		/// パラメータとの関連付けを編集
		/// </summary>
		private RelayCommand _editParameterRelationCommand;
		public RelayCommand EditParameterRelationCommand
		{
			get
			{
				return this._editParameterRelationCommand = this._editParameterRelationCommand ?? new RelayCommand((obj) =>
				{
					if (this.SelectedParameterRelation != null)
					{
						Messenger.Default.Send(this, new EditEntityRelationMessage(this.SelectedParameterRelation));
					}
				});
			}
		}

		private RelayCommand _goParameterEditorCommand;
		public RelayCommand GoParameterEditorCommand
		{
			get
			{
				return this._goParameterEditorCommand = this._goParameterEditorCommand ?? new RelayCommand((obj) =>
				{
					Messenger.Default.Send(this, new ParameterEditorPageCloseEventMessage(() =>
					{
						this.OnPropertyChanged("RelatedParameters");
						this.OnPropertyChanged("NotRelatedParameters");
					}));
					StoryEditorModel.Default.MainMode = Types.MainMode.EditParameter;
				});
			}
		}

		#endregion

		#region 用語との関連付け

		/// <summary>
		/// 関連付けるために選択された用語
		/// </summary>
		private EntitySelectionModel<WordEntity> _forRelateWordSelection = new EntitySelectionModel<WordEntity>();
		public TreeEntityListItem SelectedWordForRelate
		{
			set
			{
				this._forRelateWordSelection.Selected = value?.Entity as WordEntity;
			}
		}

		// TODO: Xamarin Forms専用プロパティ
		public TreeEntity SelectedWordEntityForRelate
		{
			set
			{
				this._forRelateWordSelection.Selected = (WordEntity)value;
			}
		}

		/// <summary>
		/// 選択された用語との関連付け
		/// </summary>
		private EntitySelectionModel<WordPersonEntityRelate> _relatedWordSelection = new EntitySelectionModel<WordPersonEntityRelate>();
		public WordPersonEntityRelate SelectedWordRelation
		{
			get
			{
				return this._relatedWordSelection.Selected;
			}
			set
			{
				this._relatedWordSelection.Selected = value;
			}
		}

		/// <summary>
		/// 用語全部のリスト
		/// </summary>
		public ICollection<TreeEntityListItem> Words
		{
			get
			{
				return this.Entity.StoryModel.Words.List;
			}
		}

		/// <summary>
		/// 関連付けられた用語のリスト
		/// </summary>
		public IEnumerable<WordPersonEntityRelate> RelatedWords
		{
			get
			{
				return this.Entity.RelatedWords;
			}
		}

		/// <summary>
		/// 関連付けられていない用語のリスト
		/// </summary>
		public IEnumerable<WordEntity> NotRelatedWords
		{
			get
			{
				return this.Entity.NotRelatedWords;
			}
		}

		/// <summary>
		/// 関連付けられていない用語のリスト（ツリーアイテム用）
		/// </summary>
		public IEnumerable<TreeEntityListItem> NotRelatedWordTreeItems
		{
			get
			{
				return this.Entity.NotRelatedWordTreeItems;
			}
		}

		/// <summary>
		/// 用語との関連付けを追加
		/// </summary>
		private RelayCommand _addWordRelationCommand;
		public RelayCommand AddWordRelationCommand
		{
			get
			{
				return this._addWordRelationCommand = this._addWordRelationCommand ?? new RelayCommand((obj) =>
				{
					if (this._forRelateWordSelection.Selected != null)
					{
						this.Entity.StoryModel.WordPersonRelation.AddRelate(this._forRelateWordSelection.Selected, this.Entity);
						this.OnPropertyChanged("RelatedWords");
						this.OnPropertyChanged("NotRelatedWords");
						this.OnPropertyChanged("NotRelatedWordTreeItems");
					}
					else
					{
						Messenger.Default.Send(this, new AlertMessage(StringResourceResolver.Resolve("SelectWordMessage")));
					}
				});
			}
		}

		/// <summary>
		/// 用語との関連付けを削除
		/// </summary>
		private RelayCommand _removeWordRelationCommand;
		public RelayCommand RemoveWordRelationCommand
		{
			get
			{
				return this._removeWordRelationCommand = this._removeWordRelationCommand ?? new RelayCommand((obj) =>
				{
					if (this.SelectedWordRelation != null)
					{
						this.Entity.StoryModel.WordPersonRelation.RemoveRelate(this.SelectedWordRelation.Entity1, this.Entity);
						this.OnPropertyChanged("RelatedWords");
						this.OnPropertyChanged("NotRelatedWords");
						this.OnPropertyChanged("NotRelatedWordTreeItems");
					}
					else
					{
						Messenger.Default.Send(this, new AlertMessage(StringResourceResolver.Resolve("SelectRelationMessage")));
					}
				});
			}
		}

		/// <summary>
		/// 用語との関連付けを編集
		/// </summary>
		private RelayCommand _editWordRelationCommand;
		public RelayCommand EditWordRelationCommand
		{
			get
			{
				return this._editWordRelationCommand = this._editWordRelationCommand ?? new RelayCommand((obj) =>
				{
					Messenger.Default.Send(this, new EntityRelationEditorMessage(
						new EntityRelationEditorModel
						{
							Direction = EntityRelationEditorModel.RelationDirection.Entity1,
							AddCommand = this.AddWordRelationCommand,
							RemoveCommand = this.RemoveWordRelationCommand,
							RelatedEntityItemsGetter = () => this.RelatedWords,
							NotRelatedEntitiesGetter = () => this.NotRelatedWords,
							RelatedEntityItemSelection = new EntitySelectionModelWrapper<IRelationEntity, WordPersonEntityRelate>(this._relatedWordSelection),
							EntityForRelateSelection = new EntitySelectionModelWrapper<Entity, WordEntity>(this._forRelateWordSelection),
						}));
				});
			}
		}

		#endregion

		#region パラメータ編集モードの切替

		/// <summary>
		/// パラメータ編集モードであるか
		/// </summary>
		private bool _isParameterMode = false;
		public bool IsParameterMode
		{
			get
			{
				return this._isParameterMode;
			}
			set
			{
				this._isParameterMode = value;
				this.OnPropertyChanged();
			}
		}

		/// <summary>
		/// 編集モードを切り替える
		/// </summary>
		private RelayCommand _editModeChangeCommand;
		public RelayCommand EditModeChangeCommand
		{
			get
			{
				return this._editModeChangeCommand = this._editModeChangeCommand ?? new RelayCommand((obj) =>
				{
					this.IsParameterMode ^= true;
					if (this.IsParameterMode)
					{
						this.ReloadGroupParameters();
					}
				});
			}
		}

		private void ReloadGroupParameters()
		{
			foreach (var group in this.RelatedGroups)
			{
				group.ReloadParameters();
			}
		}

		#endregion
	}
}
