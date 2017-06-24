using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using StoryCanvas.Shared.Models.EntitySet;
using System.Collections.ObjectModel;
using StoryCanvas.Shared.Models.EntityRelate;
using StoryCanvas.Shared.Models.Date;
using static StoryCanvas.Shared.ViewTools.ControlModels.TreeListViewControlModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using StoryCanvas.Shared.ViewTools;

namespace StoryCanvas.Shared.Models.Entities
{
	[DataContract]
	public class SceneEntity : Entity
	{
		[OnDeserialized]
		private void Initialize2(StreamingContext context)
		{
			// TextLengthの値を更新
			this.Text = this.Text;

			// ver2.2の新機能を、2.1以前のデータに対応
			if (this._scenario == null)
			{
				this._scenario = new SceneScenario();
			}
		}

		/// <summary>
		/// 親となるストーリーライン
		/// </summary>
		private WeakReference<StorylineEntity> _storyline = new WeakReference<StorylineEntity>(null);
		public StorylineEntity Storyline
		{
			get
			{
				StorylineEntity entity = null;
				this._storyline.TryGetTarget(out entity);
				return entity;
			}
			set
			{
				if (this.Storyline != value)
				{
					this.Storyline?.Scenes.Remove(this);
					if (value?.Scenes.Contains(this) != true)
					{
						value?.Scenes.Add(this);
					}
					this._storyline.SetTarget(value);
				}
			}
		}

		/// <summary>
		/// 脚本
		/// </summary>
		[DataMember]
		private string _text;
		public string Text
		{
			get
			{
				return this._text;
			}
			set
			{
				this._text = value;
				this.OnPropertyChanged();

				if (this._text != null)
				{
					this.TextLength = this._text.Length;
				}
			}
		}

		/// <summary>
		/// 脚本の文字数
		/// </summary>
		private int _textLength;
		public int TextLength
		{
			get
			{
				return this._textLength;
			}
			private set
			{
				this._textLength = value;
				this.OnPropertyChanged();
			}
		}

		/// <summary>
		/// 開始日時
		/// </summary>
		[DataMember]
		private StoryDateTime _startDateTime;
		public StoryDateTime StartDateTime
		{
			get
			{
				return this._startDateTime;
			}
			set
			{
				this._startDateTime = value;
				this.OnPropertyChanged();
			}
		}

		/// <summary>
		/// 終了日時
		/// </summary>
		[DataMember]
		private StoryDateTime _endDateTime;
		public StoryDateTime EndDateTime
		{
			get
			{
				return this._endDateTime;
			}
			set
			{
				this._endDateTime = value;
				this.OnPropertyChanged();
			}
		}

		/// <summary>
		/// シーンデザイナ位に表示するシナリオ
		/// </summary>
		[DataMember]
		private SceneScenario _scenario = new SceneScenario();
		public SceneScenario Scenario
		{
			get
			{
				return this._scenario;
			}
			set
			{
				this._scenario = value;
				this.OnPropertyChanged();
			}
		}

		/// <summary>
		/// 関連付けられた人物
		/// </summary>
		public IEnumerable<PersonSceneEntityRelate> RelatedPeople
		{
			get
			{
                return this.StoryModel.PersonSceneRelation.FindRelated(this);
			}
		}

		/// <summary>
		/// 関連付けられていない人物
		/// </summary>
		public IEnumerable<PersonEntity> NotRelatedPeople
		{
			get
			{
				return this.StoryModel.PersonSceneRelation.FindNotRelated(this, this.StoryModel.People);
			}
		}

		/// <summary>
		/// 関連付けられた場所
		/// </summary>
		public IEnumerable<PlaceSceneEntityRelate> RelatedPlaces
		{
			get
			{
                return this.StoryModel.PlaceSceneRelation.FindRelated(this);
			}
		}

		/// <summary>
		/// 関連付けられていない場所（ツリーアイテム）
		/// </summary>
		public IEnumerable<TreeEntityListItem> NotRelatedPlaceTreeItems
		{
			get
			{
				return this.StoryModel.PlaceSceneRelation.FindNotRelatedTreeItems(this, this.StoryModel.Places.List);
			}
		}

		/// <summary>
		/// 関連付けられていない場所
		/// </summary>
		public IEnumerable<PlaceEntity> NotRelatedPlaces
		{
			get
			{
				return this.StoryModel.PlaceSceneRelation.FindNotRelatedTreeItemEntities(this, this.StoryModel.Places.List);
			}
		}

		/// <summary>
		/// 関連付けられた章
		/// </summary>
		public IEnumerable<SceneChapterEntityRelate> RelatedChapters
		{
			get
			{
                return this.StoryModel.SceneChapterRelation.FindRelated(this);
			}
		}

		/// <summary>
		/// 関連付けられていない章
		/// </summary>
		public IEnumerable<ChapterEntity> NotRelatedChapters
		{
			get
			{
				return this.StoryModel.SceneChapterRelation.FindNotRelated(this, this.StoryModel.Chapters);
			}
		}

		/// <summary>
		/// 関連付けられた用語
		/// </summary>
		public IEnumerable<WordSceneEntityRelate> RelatedWords
		{
			get
			{
                return this.StoryModel.WordSceneRelation.FindRelated(this);
			}
		}

		/// <summary>
		/// 関連付けられていない用語（ツリーアイテム）
		/// </summary>
		public IEnumerable<TreeEntityListItem> NotRelatedWordTreeItems
		{
			get
			{
				return this.StoryModel.WordSceneRelation.FindNotRelatedTreeItems(this, this.StoryModel.Words.List);
			}
		}

		/// <summary>
		/// 関連付けられていない用語
		/// </summary>
		public IEnumerable<WordEntity> NotRelatedWords
		{
			get
			{
				return this.StoryModel.WordSceneRelation.FindNotRelatedTreeItemEntities(this, this.StoryModel.Words.List);
			}
		}

		/// <summary>
		/// リソース名
		/// </summary>
		protected override string ResourceName
		{
			get
			{
				return "scene";
			}
		}

		/// <summary>
		/// 中身が空であるか
		/// </summary>
		public override bool IsEmpty
		{
			get
			{
				return base.IsEmpty
					&& this.StartDateTime == null
					&& this.EndDateTime == null
					&& string.IsNullOrEmpty(this.Text);
			}
		}

		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="query"></param>
		protected override bool IsWordExists(string word)
		{
			if (!base.IsWordExists(word))
			{
				bool? r = false;
				r |= this.Text?.IndexOf(word) >= 0;
				foreach (var item in this.RelatedPeople)
				{
					r |= item.Entity1.Name?.IndexOf(word) >= 0;
					r |= item.Note?.IndexOf(word) >= 0;
				}
				foreach (var item in this.RelatedPlaces)
				{
					r |= item.Entity1.Name?.IndexOf(word) >= 0;
					r |= item.Note?.IndexOf(word) >= 0;
				}
				foreach (var item in this.RelatedChapters)
				{
					r |= item.Entity2.Name?.IndexOf(word) >= 0;
					r |= item.Note?.IndexOf(word) >= 0;
				}

				return r == true;
			}
			else
			{
				return true;
			}
		}
	}

	/// <summary>
	/// シーンデザイナに表示するシナリオ
	/// </summary>
	[DataContract]
	public class SceneScenario : INotifyPropertyChanged
	{
		[DataMember]
		private ObservableCollection<SceneScenarioSerif> _serifs = new ObservableCollection<SceneScenarioSerif>();
		public IReadOnlyCollection<SceneScenarioSerif> Serifs
		{
			get
			{
				return this._serifs;
			}
		}

		private SceneScenarioSerif _selectedSerif;
		public SceneScenarioSerif SelectedSerif
		{
			get
			{
				return this._selectedSerif;
			}
			set
			{
				this._selectedSerif = value;
				this.OnPropertyChanged();
			}
		}

		public void AddSerif(Entity entity)
		{
			this._serifs.Add(new SceneScenarioSerif
			{
				Person = entity as PersonEntity,
				Place = entity as PlaceEntity,
				Text = "",
				Scenario = this,
			});
		}

		private RelayCommand _addSerifCommand;
		public RelayCommand AddSerifCommand
		{
			get
			{
				return this._addSerifCommand = this._addSerifCommand ?? new RelayCommand((obj) =>
				{
					var entity = obj as Entity;
					if (entity != null)
					{
						this.AddSerif(entity);
					}
				});
			}
		}

		public void RemoveSerif()
		{
			this._serifs.Remove(this.SelectedSerif);
		}

		public void UpSerif()
		{
			var index = this._serifs.IndexOf(this.SelectedSerif);
			if (index > 0)
			{
				this._serifs.Move(index, index - 1);
			}
		}

		public void DownSerif()
		{
			var index = this._serifs.IndexOf(this.SelectedSerif);
			if (index < this._serifs.Count - 1)
			{
				this._serifs.Move(index, index + 1);
			}
		}

		public void FindEntity(EntitySetModel<PersonEntity> people, EntitySetModel<PlaceEntity> places)
		{
			foreach (var serif in this.Serifs)
			{
				serif.FindPersonEntity(people);
				serif.FindPlaceEntity(places);
				serif.Scenario = this;
			}
		}

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged([CallerMemberName] string name = null)
		{
			if (PropertyChanged == null) return;

			PropertyChanged(this, new PropertyChangedEventArgs(name));
		}

		#endregion
	}

	/// <summary>
	/// シーンデザイナのセリフ１つ文
	/// </summary>
	[DataContract]
	public class SceneScenarioSerif : INotifyPropertyChanged
	{
		private WeakReference<SceneScenario> _scenario;
		public SceneScenario Scenario
		{
			get
			{
				if (this._scenario == null)
				{
					this._scenario = new WeakReference<SceneScenario>(null);
				}
				SceneScenario s = null;
				this._scenario.TryGetTarget(out s);
				return s;
			}
			set
			{
				if (this._scenario == null)
				{
					this._scenario = new WeakReference<SceneScenario>(null);
				}
				this._scenario.SetTarget(value);
			}
		}

		[DataMember]
		private EntityReferenceModel<PersonEntity> _person = new EntityReferenceModel<PersonEntity>();
		public PersonEntity Person
		{
			get
			{
				return this._person.Entity;
			}
			set
			{
				this._person.Entity = value;
				this.OnPropertyChanged();
			}
		}

		[DataMember]
		private EntityReferenceModel<PlaceEntity> _place = new EntityReferenceModel<PlaceEntity>();
		public PlaceEntity Place
		{
			get
			{
				return this._place.Entity;
			}
			set
			{
				this._place.Entity = value;
				this.OnPropertyChanged();
			}
		}

		public Entity AnyEntity
		{
			get
			{
				if (this.Person != null)
				{
					return this.Person;
				}
				return this.Place;
			}
		}

		[DataMember]
		private string _text;
		public string Text
		{
			get
			{
				return this._text;
			}
			set
			{
				this._text = value;
				this.OnPropertyChanged();
			}
		}

		public void FindPersonEntity(EntitySetModel<PersonEntity> entities)
		{
			this._person.FindEntity(entities);
		}

		public void FindPlaceEntity(EntitySetModel<PlaceEntity> entities)
		{
			this._place.FindEntity(entities);
		}

		private RelayCommand _removeSerifCommand;
		public RelayCommand RemoveSerifCommand
		{
			get
			{
				return this._removeSerifCommand = this._removeSerifCommand ?? new RelayCommand((obj) =>
				{
					this.Scenario.SelectedSerif = this;
					this.Scenario.RemoveSerif();
				});
			}
		}

		private RelayCommand _upSerifCommand;
		public RelayCommand UpSerifCommand
		{
			get
			{
				return this._upSerifCommand = this._upSerifCommand ?? new RelayCommand((obj) =>
				{
					this.Scenario.SelectedSerif = this;
					this.Scenario.UpSerif();
				});
			}
		}

		private RelayCommand _downSerifCommand;
		public RelayCommand DownSerifCommand
		{
			get
			{
				return this._downSerifCommand = this._downSerifCommand ?? new RelayCommand((obj) =>
				{
					this.Scenario.SelectedSerif = this;
					this.Scenario.DownSerif();
				});
			}
		}

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged([CallerMemberName] string name = null)
		{
			if (PropertyChanged == null) return;

			PropertyChanged(this, new PropertyChangedEventArgs(name));
		}

		#endregion
	}
}
