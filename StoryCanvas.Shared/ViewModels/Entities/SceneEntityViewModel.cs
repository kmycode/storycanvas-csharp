using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoryCanvas.Shared.Models.Entities;
using StoryCanvas.Shared.Messages.StorylineDesigner;
using StoryCanvas.Shared.Messages.EditEntity;
using System.Collections.ObjectModel;
using StoryCanvas.Shared.Models.EntitySet;
using StoryCanvas.Shared.ViewTools;
using StoryCanvas.Shared.Models.EntityRelate;
using static StoryCanvas.Shared.ViewTools.ControlModels.TreeListViewControlModel;
using StoryCanvas.Shared.Models.Date;
using StoryCanvas.Shared.Messages.Picker;
using StoryCanvas.Shared.Models.Story;
using System.ComponentModel;
using StoryCanvas.Shared.Common;
using StoryCanvas.Shared.Messages.Common;

namespace StoryCanvas.Shared.ViewModels.Entities
{
	public class SceneEntityViewModel : EntityViewModelBase<SceneEntity>
	{
		protected override SceneEntity CreateDummyEntity()
		{
			return new SceneEntity();
		}

		public SceneEntityViewModel()
		{
			this.EntityChanged += (e, old) =>
			{
				this.SelectedPersonRelation = null;
				this.SelectedPlaceRelation = null;
				this.SelectedChapterRelation = null;
				this.OnPropertyChanged("RelatedPeople");
				this.OnPropertyChanged("NotRelatedPeople");
				this.OnPropertyChanged("RelatedPlaces");
				this.OnPropertyChanged("NotRelatedPlaces");
				this.OnPropertyChanged("NotRelatedPlaceTreeItems");
				this.OnPropertyChanged("RelatedChapters");
				this.OnPropertyChanged("NotRelatedChapters");
				this.OnPropertyChanged("RelatedWords");
				this.OnPropertyChanged("NotRelatedWords");
				this.OnPropertyChanged("NotRelatedWordTreeItems");
				this.OnPropertyChanged("StartDateTime");
				this.OnPropertyChanged("EndDateTime");
				this.OnPropertyChanged("Text");

				if (old != null)
				{
					old.PropertyChanged -= this.ScenePropertyChanged;
				}
				if (e != null)
				{
					e.PropertyChanged += this.ScenePropertyChanged;
				}
			};

			StoryEditorModel.Default.MainModeChanged += (o, n) =>
			{
				if (n == Types.MainMode.EditScene)
				{
					if (!this.IsDummy)
					{
						this.Entity = this.Entity;
					}
				}
			};
		}

		private void ScenePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			// 人物の年齢の表示を更新
			if (e.PropertyName == "StartDateTime")
			{
				this.OnPropertyChanged("RelatedPeople");
			}
		}

		/// <summary>
		/// 開始時刻
		/// </summary>
		public StoryDateTime StartDateTime
		{
			get
			{
				return this.Entity.StartDateTime;
			}
			set
			{
				this.Entity.StartDateTime = value;
			}
		}

		/// <summary>
		/// 開始時刻選択画面の表示
		/// </summary>
		private RelayCommand _startDateTimePickerCommand;
		public RelayCommand StartDateTimePickerCommand
		{
			get
			{
				return this._startDateTimePickerCommand = this._startDateTimePickerCommand ?? new RelayCommand((obj) =>
				{
					Messenger.Default.Send(this, new DateTimePickerMessage(StoryCalendar.AnnoDomini, this.StartDateTime, (date) =>
					{
						this.StartDateTime = date;
					}));
				});
			}
		}

		/// <summary>
		/// 終了時刻
		/// </summary>
		public StoryDateTime EndDateTime
		{
			get
			{
				return this.Entity.EndDateTime;
			}
			set
			{
				this.Entity.EndDateTime = value;
			}
		}

		/// <summary>
		/// 終了時刻選択画面の表示
		/// </summary>
		private RelayCommand _endDateTimePickerCommand;
		public RelayCommand EndDateTimePickerCommand
		{
			get
			{
				return this._endDateTimePickerCommand = this._endDateTimePickerCommand ?? new RelayCommand((obj) =>
				{
					Messenger.Default.Send(this, new DateTimePickerMessage(StoryCalendar.AnnoDomini, this.EndDateTime, (date) =>
					{
						this.EndDateTime = date;
					}));
				});
			}
		}

		/// <summary>
		/// 脚本
		/// </summary>
		public string Text
		{
			get
			{
				return this.Entity.Text;
			}
			set
			{
				this.Entity.Text = value;
			}
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
		private EntitySelectionModel<PersonSceneEntityRelate> _relatedPersonSelection = new EntitySelectionModel<PersonSceneEntityRelate>();
		public PersonSceneEntityRelate SelectedPersonRelation
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
		public IEnumerable<PersonSceneEntityRelate> RelatedPeople
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
						this.Entity.StoryModel.PersonSceneRelation.AddRelate(this.SelectedPersonForRelate, this.Entity);
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
						this.Entity.StoryModel.PersonSceneRelation.RemoveRelate(this.SelectedPersonRelation.Entity1, this.Entity);
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
							Direction = EntityRelationEditorModel.RelationDirection.Entity1,
							AddCommand = this.AddPersonRelationCommand,
							RemoveCommand = this.RemovePersonRelationCommand,
							RelatedEntityItemsGetter = () => this.RelatedPeople,
							NotRelatedEntitiesGetter = () => this.NotRelatedPeople,
							RelatedEntityItemSelection = new EntitySelectionModelWrapper<IRelationEntity, PersonSceneEntityRelate>(this._relatedPersonSelection),
							EntityForRelateSelection = new EntitySelectionModelWrapper<Entity, PersonEntity>(this._forRelatePersonSelection),
						}));
				});
			}
		}

		#endregion

		#region 場所との関連付け

		/// <summary>
		/// 関連付けるために選択された場所
		/// </summary>
		private EntitySelectionModel<PlaceEntity> _forRelatePlaceSelection = new EntitySelectionModel<PlaceEntity>();
		public TreeEntityListItem SelectedPlaceForRelate
		{
			set
			{
				this._forRelatePlaceSelection.Selected = value?.Entity as PlaceEntity;
			}
		}

		// TODO: Xamarin Forms専用プロパティ
		public TreeEntity SelectedPlaceEntityForRelate
		{
			set
			{
				this._forRelatePlaceSelection.Selected = (PlaceEntity)value;
			}
		}

		/// <summary>
		/// 選択された場所との関連付け
		/// </summary>
		private EntitySelectionModel<PlaceSceneEntityRelate> _relatedPlaceSelection = new EntitySelectionModel<PlaceSceneEntityRelate>();
		public PlaceSceneEntityRelate SelectedPlaceRelation
		{
			get
			{
				return this._relatedPlaceSelection.Selected;
			}
			set
			{
				this._relatedPlaceSelection.Selected = value;
			}
		}

		/// <summary>
		/// 場所全部のリスト
		/// </summary>
		public ICollection<TreeEntityListItem> Places
		{
			get
			{
				return this.Entity.StoryModel.Places.List;
			}
		}

		/// <summary>
		/// 関連付けられた場所のリスト
		/// </summary>
		public IEnumerable<PlaceSceneEntityRelate> RelatedPlaces
		{
			get
			{
				return this.Entity.RelatedPlaces;
			}
		}

		/// <summary>
		/// 関連付けられていない場所のリスト
		/// </summary>
		public IEnumerable<PlaceEntity> NotRelatedPlaces
		{
			get
			{
				return this.Entity.NotRelatedPlaces;
			}
		}

		/// <summary>
		/// 関連付けられていない場所のリスト（ツリーアイテム用）
		/// </summary>
		public IEnumerable<TreeEntityListItem> NotRelatedPlaceTreeItems
		{
			get
			{
				return this.Entity.NotRelatedPlaceTreeItems;
			}
		}

		/// <summary>
		/// 場所との関連付けを追加
		/// </summary>
		private RelayCommand _addPlaceRelationCommand;
		public RelayCommand AddPlaceRelationCommand
		{
			get
			{
				return this._addPlaceRelationCommand = this._addPlaceRelationCommand ?? new RelayCommand((obj) =>
				{
					if (this._forRelatePlaceSelection.Selected != null)
					{
						this.Entity.StoryModel.PlaceSceneRelation.AddRelate(this._forRelatePlaceSelection.Selected, this.Entity);
						this.OnPropertyChanged("RelatedPlaces");
						this.OnPropertyChanged("NotRelatedPlaces");
						this.OnPropertyChanged("NotRelatedPlaceTreeItems");
					}
					else
					{
						Messenger.Default.Send(this, new AlertMessage(StringResourceResolver.Resolve("SelectPlaceMessage")));
					}
				});
			}
		}

		/// <summary>
		/// 場所との関連付けを削除
		/// </summary>
		private RelayCommand _removePlaceRelationCommand;
		public RelayCommand RemovePlaceRelationCommand
		{
			get
			{
				return this._removePlaceRelationCommand = this._removePlaceRelationCommand ?? new RelayCommand((obj) =>
				{
					if (this.SelectedPlaceRelation != null)
					{
						this.Entity.StoryModel.PlaceSceneRelation.RemoveRelate(this.SelectedPlaceRelation.Entity1, this.Entity);
						this.OnPropertyChanged("RelatedPlaces");
						this.OnPropertyChanged("NotRelatedPlaces");
						this.OnPropertyChanged("NotRelatedPlaceTreeItems");
					}
					else
					{
						Messenger.Default.Send(this, new AlertMessage(StringResourceResolver.Resolve("SelectRelationMessage")));
					}
				});
			}
		}

		/// <summary>
		/// 場所との関連付けを編集
		/// </summary>
		private RelayCommand _editPlaceRelationCommand;
		public RelayCommand EditPlaceRelationCommand
		{
			get
			{
				return this._editPlaceRelationCommand = this._editPlaceRelationCommand ?? new RelayCommand((obj) =>
				{
					Messenger.Default.Send(this, new EntityRelationEditorMessage(
						new EntityRelationEditorModel
						{
							Direction = EntityRelationEditorModel.RelationDirection.Entity1,
							AddCommand = this.AddPlaceRelationCommand,
							RemoveCommand = this.RemovePlaceRelationCommand,
							RelatedEntityItemsGetter = () => this.RelatedPlaces,
							NotRelatedEntitiesGetter = () => this.NotRelatedPlaces,
							RelatedEntityItemSelection = new EntitySelectionModelWrapper<IRelationEntity, PlaceSceneEntityRelate>(this._relatedPlaceSelection),
							EntityForRelateSelection = new EntitySelectionModelWrapper<Entity, PlaceEntity>(this._forRelatePlaceSelection),
						}));
				});
			}
		}

		#endregion

		#region 章との関連付け

		/// <summary>
		/// 関連付けるために選択された章
		/// </summary>
		private EntitySelectionModel<ChapterEntity> _forRelateChapterSelection = new EntitySelectionModel<ChapterEntity>();
		public ChapterEntity SelectedChapterForRelate
		{
			get
			{
				return this._forRelateChapterSelection.Selected;
			}
			set
			{
				this._forRelateChapterSelection.Selected = value;
			}
		}

		/// <summary>
		/// 選択された章との関連付け
		/// </summary>
		private EntitySelectionModel<SceneChapterEntityRelate> _relatedChapterSelection = new EntitySelectionModel<SceneChapterEntityRelate>();
		public SceneChapterEntityRelate SelectedChapterRelation
		{
			get
			{
				return this._relatedChapterSelection.Selected;
			}
			set
			{
				this._relatedChapterSelection.Selected = value;
			}
		}

		/// <summary>
		/// 章全部のリスト
		/// </summary>
		public ICollection<ChapterEntity> Chapters
		{
			get
			{
				return this.Entity.StoryModel.Chapters;
			}
		}

		/// <summary>
		/// 関連付けられた章のリスト
		/// </summary>
		public IEnumerable<SceneChapterEntityRelate> RelatedChapters
		{
			get
			{
				return this.Entity.RelatedChapters;
			}
		}

		/// <summary>
		/// 関連付けられていない章のリスト
		/// </summary>
		public IEnumerable<ChapterEntity> NotRelatedChapters
		{
			get
			{
				return this.Entity.NotRelatedChapters;
			}
		}

		/// <summary>
		/// 章との関連付けを追加
		/// </summary>
		private RelayCommand _addChapterRelationCommand;
		public RelayCommand AddChapterRelationCommand
		{
			get
			{
				return this._addChapterRelationCommand = this._addChapterRelationCommand ?? new RelayCommand((obj) =>
				{
					if (this._forRelateChapterSelection.Selected != null)
					{
						this.Entity.StoryModel.SceneChapterRelation.AddRelate(this.Entity, this.SelectedChapterForRelate);
						this.OnPropertyChanged("RelatedChapters");
						this.OnPropertyChanged("NotRelatedChapters");
					}
					else
					{
						Messenger.Default.Send(this, new AlertMessage(StringResourceResolver.Resolve("SelectChapterMessage")));
					}
				});
			}
		}

		/// <summary>
		/// 章との関連付けを削除
		/// </summary>
		private RelayCommand _removeChapterRelationCommand;
		public RelayCommand RemoveChapterRelationCommand
		{
			get
			{
				return this._removeChapterRelationCommand = this._removeChapterRelationCommand ?? new RelayCommand((obj) =>
				{
					if (this.SelectedChapterRelation != null)
					{
						this.Entity.StoryModel.SceneChapterRelation.RemoveRelate(this.Entity, this.SelectedChapterRelation.Entity2);
						this.OnPropertyChanged("RelatedChapters");
						this.OnPropertyChanged("NotRelatedChapters");
					}
					else
					{
						Messenger.Default.Send(this, new AlertMessage(StringResourceResolver.Resolve("SelectRelationMessage")));
					}
				});
			}
		}

		/// <summary>
		/// 章との関連付けを編集
		/// </summary>
		private RelayCommand _editChapterRelationCommand;
		public RelayCommand EditChapterRelationCommand
		{
			get
			{
				return this._editChapterRelationCommand = this._editChapterRelationCommand ?? new RelayCommand((obj) =>
				{
					Messenger.Default.Send(this, new EntityRelationEditorMessage(
						new EntityRelationEditorModel
						{
							Direction = EntityRelationEditorModel.RelationDirection.Entity2,
							AddCommand = this.AddChapterRelationCommand,
							RemoveCommand = this.RemoveChapterRelationCommand,
							RelatedEntityItemsGetter = () => this.RelatedChapters,
							NotRelatedEntitiesGetter = () => this.NotRelatedChapters,
							RelatedEntityItemSelection = new EntitySelectionModelWrapper<IRelationEntity, SceneChapterEntityRelate>(this._relatedChapterSelection),
							EntityForRelateSelection = new EntitySelectionModelWrapper<Entity, ChapterEntity>(this._forRelateChapterSelection),
						}));
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
		private EntitySelectionModel<WordSceneEntityRelate> _relatedWordSelection = new EntitySelectionModel<WordSceneEntityRelate>();
		public WordSceneEntityRelate SelectedWordRelation
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
		public IEnumerable<WordSceneEntityRelate> RelatedWords
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
						this.Entity.StoryModel.WordSceneRelation.AddRelate(this._forRelateWordSelection.Selected, this.Entity);
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
						this.Entity.StoryModel.WordSceneRelation.RemoveRelate(this.SelectedWordRelation.Entity1, this.Entity);
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
							RelatedEntityItemSelection = new EntitySelectionModelWrapper<IRelationEntity, WordSceneEntityRelate>(this._relatedWordSelection),
							EntityForRelateSelection = new EntitySelectionModelWrapper<Entity, WordEntity>(this._forRelateWordSelection),
						}));
				});
			}
		}

		#endregion

		/*
		/// <summary>
		/// ストーリーラインデザイナで、シーンを新たに追加するメッセージを受け取った時
		/// </summary>
		/// <param name="message">メッセージ</param>
		public void OnAddSceneMessage(AddSceneToDesignerMessage message)
		{
			this.Entity = message.Scene;
		}

		/*
		/// <summary>
		/// シーンを編集する時
		/// </summary>
		/// <param name="message">メッセージ</param>
		public void OnEditEntityMessage(EditSceneEntityMessage message)
		{
			this.Entity = message.Scene;
		}
		*/
	}
}
