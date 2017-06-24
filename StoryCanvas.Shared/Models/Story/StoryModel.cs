using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using StoryCanvas.Shared.Models.Entities;
using StoryCanvas.Shared.Models.EntitySet;
using StoryCanvas.Shared.ViewTools.Resources;
using StoryCanvas.Shared.Models.EntityRelate;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using StoryCanvas.Shared.Messages.Picker;
using StoryCanvas.Shared.ViewTools;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.IO.Compression;
using Sockets.Plugin;
using Sockets.Plugin.Abstractions;
using StoryCanvas.Shared.Messages.Network;
using StoryCanvas.Shared.Types;
using static StoryCanvas.Shared.ViewTools.ControlModels.TreeListViewControlModel;
using StoryCanvas.Shared.Messages.Common;
using StoryCanvas.Shared.Common;
using StoryCanvas.Shared.Messages.Page;
using StoryCanvas.Shared.Models.IO;
using StoryCanvas.Shared.Models.Network;
using System.Collections.ObjectModel;
using StoryCanvas.Shared.Models.Editor;

#if XAMARIN_FORMS
using StoryCanvas.Messages.IO;
#endif

namespace StoryCanvas.Shared.Models.Story
{
	public delegate void CurrentStoryChangedEventHandler(StoryModel story, StoryModel oldStory);
	public delegate void LoadStreamCompleteEventHandler();
	public delegate void NewStoryCreatedEventHandler(object sender, EventArgs e);
	public delegate void AutoSaveRequestedEventHandler(object sender, EventArgs e);

	[DataContract]
	[KnownType(typeof(GroupEntity))]
	[KnownType(typeof(WordEntity))]
	public class StoryModel : INotifyPropertyChanged
	{
		/// <summary>
		/// 現在アクティブになっているストーリーモデルが変更された時に発行されるイベント
		/// </summary>
		public static event CurrentStoryChangedEventHandler CurrentStoryChanged = delegate { };

		/// <summary>
		/// ストリームからStoryModelのロードが完了した時に発行されるイベント
		/// </summary>
		public event LoadStreamCompleteEventHandler LoadStreamCompleted = delegate { };

		public StoryModel()
		{
			this.Initialize(default(StreamingContext));
			this.Initialize2(default(StreamingContext));
		}

		[OnDeserializing]
		private void Initialize(StreamingContext context)
		{
			this._personSceneRelation = new EntityRelationModel<PersonSceneEntityRelate, PersonEntity, SceneEntity>();
			this._groupPersonRelation = new EntityRelationModel<GroupPersonEntityRelate, GroupEntity, PersonEntity>();
			this._placeSceneRelation = new EntityRelationModel<PlaceSceneEntityRelate, PlaceEntity, SceneEntity>();
			this._sceneChapterRelation = new EntityRelationModel<SceneChapterEntityRelate, SceneEntity, ChapterEntity>();
			this._sexPersonRelation = new EntityRelationModel<SexPersonEntityRelate, SexEntity, PersonEntity>();
			this._personParameterRelation = new EntityRelationModel<PersonParameterEntityRelate, PersonEntity, ParameterEntity>();
			this._wordPersonRelation = new EntityRelationModel<WordPersonEntityRelate, WordEntity, PersonEntity>();
			this._wordSceneRelation = new EntityRelationModel<WordSceneEntityRelate, WordEntity, SceneEntity>();

			this.LoadStreamCompleted += this.GiveEntitySetToRelates;
		}

		[OnDeserialized]
		private void Initialize2(StreamingContext context)
		{
			// 1.1で追加＝1.0で作成したデータでは_groupsがnullである＝読み込み時に例外
			if (this._groups == null)
			{
				this._groups = new EntityTreeModel<GroupEntity>(new GroupEntity());
			}

			// 1.3で追加
			if (this._sexes == null)
			{
				this._sexes = new EntityListModel<SexEntity>();
			}
			if (this._sexes.Count == 0)
			{
				this.Sexes.Add(SexEntity.NewMale());
				this.Sexes.Add(SexEntity.NewFemale());
				foreach (var sex in this.Sexes)
				{
					sex.StoryModel = this;
				}
			}
			if (this._memoes == null)
			{
				this._memoes = new EntityListModel<MemoEntity>();
			}

			// 1.5で追加
			if (this._parameters == null)
			{
				this._parameters = new EntityListModel<ParameterEntity>();
			}

			// 2.1で追加
			if (this._words == null)
			{
				this._words = new EntityTreeModel<WordEntity>(new WordEntity());
			}

			this._people.EntityRemoving += (entity) => this.RemoveRelate(entity);
			this._places.EntityRemoving += (entity) => this.RemoveRelate(entity);
			this._scenes.EntityRemoving += (entity) => this.RemoveRelate(entity);
			this._chapters.EntityRemoving += (entity) => this.RemoveRelate(entity);
			this._groups.EntityRemoving += (entity) => this.RemoveRelate(entity);
			this._sexes.EntityRemoving += (entity) => this.RemoveRelate(entity);
			this._parameters.EntityRemoving += (entity) => this.RemoveRelate(entity);
			this._words.EntityRemoving += (entity) => this.RemoveRelate(entity);

			// 人物と性別は一対一　一対多はない
			this._sexPersonRelation.RelationAdded += (sender, e) =>
			{
				var personSexes = this._sexPersonRelation.FindRelated(e.Entity2).ToList();
				if (personSexes.Count > 1)
				{
					var removeSexes = from item in personSexes
									  where item.Entity1.Id != e.Entity1.Id
									  select item.Entity1;
					foreach (var sex in removeSexes)
					{
						this._sexPersonRelation.RemoveRelate(sex, e.Entity2);
					}
				}
			};
		}

		public void SetTestData()
		{
			if (StringResourceResolver.Resolve("Language") == "ja")
			{
				this.SetTestData_Japanese();
			}
			else
			{
				this.SetTestData_English();
			}
		}

		private void SetTestData_English()
		{
#if DEBUG
			this.People.Add(new PersonEntity
			{
				FirstName = "James",
				LastName = "Brown",
				IsWesternerName = true,
				BirthDay = Date.StoryCalendar.AnnoDomini.Date(2003, 2, 13),
				Color = new ColorResource { R = 0x80, G = 0x00, B = 0x00 },
			});
			this.People.Add(new PersonEntity
			{
				FirstName = "Harry",
				LastName = "Watson",
				IsWesternerName = true,
				Color = new ColorResource { R = 0x00, G = 0x00, B = 0xff },
			});
			this.People.Add(new PersonEntity
			{
				FirstName = "Peck",
				LastName = "Johnson",
				IsWesternerName = true,
				Color = new ColorResource { R = 0xc0, G = 0xc0, B = 0x00 },
			});
			this.PersonPersonRelates.Add(new PersonPersonEntityRelate(this.People[0], this.People[1]));
			this.PersonPersonRelates.Add(new PersonPersonEntityRelate(this.People[0], this.People[2]));
			this.SexPersonRelates.Add(new SexPersonEntityRelate(this.Sexes[0], this.People[0]));

			this.Scenes.Add(new SceneEntity
			{
				Name = "On Beach",
				StartDateTime = new Date.StoryDateTime
				{
					Date = Date.StoryCalendar.AnnoDomini.Date(2016, 8, 16),
					Time = Date.StoryCalendar.AnnoDomini.Time(17, 0, 0),
				},
				EndDateTime = new Date.StoryDateTime
				{
					Date = Date.StoryCalendar.AnnoDomini.Date(2016, 8, 16),
					Time = Date.StoryCalendar.AnnoDomini.Time(17, 20, 0),
				},
				Color = new ColorResource { R = 0x00, G = 0x00, B = 0xff },
			});
			this.PersonSceneRelates.Add(new PersonSceneEntityRelate(this.People[0], this.Scenes[0]));
			this.PersonSceneRelates[0].Note = "Sleep on beach";
			this.PersonSceneRelates.Add(new PersonSceneEntityRelate(this.People[1], this.Scenes[0]));
			this.Scenes.Add(new SceneEntity
			{
				Name = "Car drive",
				StartDateTime = new Date.StoryDateTime
				{
					Date = Date.StoryCalendar.AnnoDomini.Date(2016, 6, 16),
					Time = Date.StoryCalendar.AnnoDomini.Time(15, 30, 0),
				},
				EndDateTime = new Date.StoryDateTime
				{
					Date = Date.StoryCalendar.AnnoDomini.Date(2016, 6, 16),
					Time = Date.StoryCalendar.AnnoDomini.Time(16, 0, 0),
				},
			});
			this.PersonSceneRelates.Add(new PersonSceneEntityRelate(this.People[0], this.Scenes[1]));
			this.Scenes.Add(new SceneEntity
			{
				Name = "Rival appears",
				StartDateTime = new Date.StoryDateTime
				{
					Date = Date.StoryCalendar.AnnoDomini.Date(2016, 6, 16),
					Time = Date.StoryCalendar.AnnoDomini.Time(17, 0, 0),
				},
				EndDateTime = new Date.StoryDateTime
				{
					Date = Date.StoryCalendar.AnnoDomini.Date(2016, 6, 16),
					Time = Date.StoryCalendar.AnnoDomini.Time(17, 20, 0),
				},
				Color = new ColorResource { R = 0xff, G = 0x00, B = 0x00 },
			});
			this.PersonSceneRelates.Add(new PersonSceneEntityRelate(this.People[0], this.Scenes[2]));
			this.PersonSceneRelates.Add(new PersonSceneEntityRelate(this.People[2], this.Scenes[2]));
			this.Scenes.Add(new SceneEntity
			{
				Name = "Dinner",
				StartDateTime = new Date.StoryDateTime
				{
					Date = Date.StoryCalendar.AnnoDomini.Date(2016, 6, 16),
					Time = Date.StoryCalendar.AnnoDomini.Time(19, 20, 0),
				},
				EndDateTime = new Date.StoryDateTime
				{
					Date = Date.StoryCalendar.AnnoDomini.Date(2016, 6, 16),
					Time = Date.StoryCalendar.AnnoDomini.Time(20, 0, 0),
				},
			});
			this.PersonSceneRelates.Add(new PersonSceneEntityRelate(this.People[1], this.Scenes[3]));

			this.StoryConfig.Title = "Sample Story";
			this.StoryConfig.Comment = "This is a sample plot.";
#endif
		}

		private void SetTestData_Japanese()
		{
#if DEBUG
			this.People.Add(new PersonEntity
			{
				LastName = "斎藤",
				FirstName = "夏海",
				BirthDay = Date.StoryCalendar.AnnoDomini.Date(2003, 2, 13),
				Color = new ColorResource { R = 0xff, G = 0x00, B = 0xde },
			});
			this.People.Add(new PersonEntity
			{
				LastName = "植村",
				FirstName = "篤",
				Color = new ColorResource { R = 0x00, G = 0xa0, B = 0x00 },
			});
			this.People.Add(new PersonEntity
			{
				LastName = "中坂",
				FirstName = "義則",
				Color = new ColorResource { R = 0x80, G = 0x40, B = 0x20 },
			});
			this.PersonPersonRelates.Add(new PersonPersonEntityRelate(this.People[0], this.People[1])
            {
                Note = "友達",
                OtherNote = "好き",
            });
			this.PersonPersonRelates.Add(new PersonPersonEntityRelate(this.People[0], this.People[2])
            {
                Note = "敵",
            });
			this.SexPersonRelates.Add(new SexPersonEntityRelate(this.Sexes[1], this.People[0]));

			this.Scenes.Add(new SceneEntity
			{
				Name = "図書館での出会い",
				StartDateTime = new Date.StoryDateTime
				{
					Date = Date.StoryCalendar.AnnoDomini.Date(2016, 6, 16),
					Time = Date.StoryCalendar.AnnoDomini.Time(16, 0, 0),
				},
				EndDateTime = new Date.StoryDateTime
				{
					Date = Date.StoryCalendar.AnnoDomini.Date(2016, 6, 16),
					Time = Date.StoryCalendar.AnnoDomini.Time(16, 50, 0),
				},
				Color = new ColorResource { R = 0x00, G = 0x00, B = 0xff },
			});
			this.PersonSceneRelates.Add(new PersonSceneEntityRelate(this.People[0], this.Scenes[0]));
			this.PersonSceneRelates[0].Note = "本を落とす";
			this.PersonSceneRelates.Add(new PersonSceneEntityRelate(this.People[1], this.Scenes[0]));
			this.PersonSceneRelates[1].Note = "本を拾ってあげる";
			this.Scenes.Add(new SceneEntity
			{
				Name = "斎藤の下校",
				StartDateTime = new Date.StoryDateTime
				{
					Date = Date.StoryCalendar.AnnoDomini.Date(2016, 6, 16),
					Time = Date.StoryCalendar.AnnoDomini.Time(15, 30, 0),
				},
				EndDateTime = new Date.StoryDateTime
				{
					Date = Date.StoryCalendar.AnnoDomini.Date(2016, 6, 16),
					Time = Date.StoryCalendar.AnnoDomini.Time(16, 0, 0),
				},
			});
			this.PersonSceneRelates.Add(new PersonSceneEntityRelate(this.People[0], this.Scenes[1]));
			this.Scenes.Add(new SceneEntity
			{
				Name = "ライバル登場",
				StartDateTime = new Date.StoryDateTime
				{
					Date = Date.StoryCalendar.AnnoDomini.Date(2016, 6, 16),
					Time = Date.StoryCalendar.AnnoDomini.Time(17, 0, 0),
				},
				EndDateTime = new Date.StoryDateTime
				{
					Date = Date.StoryCalendar.AnnoDomini.Date(2016, 6, 16),
					Time = Date.StoryCalendar.AnnoDomini.Time(17, 20, 0),
				},
				Color = new ColorResource { R = 0xff, G = 0x00, B = 0x00 },
			});
			this.PersonSceneRelates.Add(new PersonSceneEntityRelate(this.People[0], this.Scenes[2]));
			this.PersonSceneRelates.Add(new PersonSceneEntityRelate(this.People[2], this.Scenes[2]));
			this.Scenes.Add(new SceneEntity
			{
				Name = "植村の回想",
				StartDateTime = new Date.StoryDateTime
				{
					Date = Date.StoryCalendar.AnnoDomini.Date(2016, 6, 16),
					Time = Date.StoryCalendar.AnnoDomini.Time(19, 20, 0),
				},
				EndDateTime = new Date.StoryDateTime
				{
					Date = Date.StoryCalendar.AnnoDomini.Date(2016, 6, 16),
					Time = Date.StoryCalendar.AnnoDomini.Time(20, 0, 0),
				},
				Color = new ColorResource { R = 0x00, G = 0x80, B = 0x00 },
			});
			this.PersonSceneRelates.Add(new PersonSceneEntityRelate(this.People[1], this.Scenes[3]));
            this.Groups.Add(new GroupEntity
            {
                Name = "学生",
                Children =
                {
                    new GroupEntity
                    {
                        Name = "１年生",
                    },
                    new GroupEntity
                    {
                        Name = "２年生",
                    },
                    new GroupEntity
                    {
                        Name = "３年生",
                    },
                },
            });
            this.Groups.Add(new GroupEntity
            {
                Name = "先生",
            });

            // 人物のマップ
            this._personEditorModel = new PersonEditorModel(this);
            var map = this._personEditorModel.MapGroup.SelectedMap;
            map.Elements.Add(new Editor.Map.MapEntityElement<PersonEntity>
            {
                Entity = this.People[0],
                X = 30,
                Y = 30,
            });
            map.Elements.Add(new Editor.Map.MapEntityElement<PersonEntity>
            {
                Entity = this.People[1],
                X = 160,
                Y = 30,
            });
            map.Elements.Add(new Editor.Map.MapEntityElement<PersonEntity>
            {
                Entity = this.People[2],
                X = 130,
                Y = 210,
            });
            map.Elements.RemoveAt(2);

            this.StoryConfig.Title = "サンプル小説";
			this.StoryConfig.Comment = "これはサンプルです。";
#endif
		}

		private static StoryModel _current;
		public static StoryModel Current
		{
			get
			{
				return _current;
			}
			set
			{
				StoryModel old = _current;
				_current = value;
				CurrentStoryChanged(value, old);

				// オートセーブのイベントを回す
				value?.SetNextAutoSaveEvent();
			}
		}

		[DataMember]
		private long EntityCount
		{
			get
			{
				return Entity.EntityCount;
			}
			set
			{
				Entity.EntityCount = value;
			}
		}

		[DataMember]
		private long PersonParameterSelectCellCount
		{
			get
			{
				return ParameterEntity.SelectCell.LastId;
			}
			set
			{
				ParameterEntity.SelectCell.LastId = value;
			}
		}

		/// <summary>
		/// ストーリーの設定
		/// </summary>
		[DataMember]
		private StoryConfig _storyConfig = new StoryConfig();
		public StoryConfig StoryConfig
		{
			get
			{
				return this._storyConfig;
			}
			private set
			{
				this._storyConfig = value;
				this.OnPropertyChanged();
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

		#region 人物

		/// <summary>
		/// 人物一覧
		/// </summary>
		[DataMember]
		private readonly EntityListModel<PersonEntity> _people = new EntityListModel<PersonEntity>();
		public EntityListModel<PersonEntity> People
		{
			get
			{
				return this._people;
			}
		}

		#endregion

		#region 性別

		[DataMember]
		private EntityListModel<SexEntity> _sexes = new EntityListModel<SexEntity>();
		public EntityListModel<SexEntity> Sexes
		{
			get
			{
				return this._sexes;
			}
		}

		#endregion

		#region 集団

		/// <summary>
		/// 集団一覧
		/// </summary>
		[DataMember]
		private EntityTreeModel<GroupEntity> _groups = new EntityTreeModel<GroupEntity>(new GroupEntity());
		public EntityTreeModel<GroupEntity> Groups
		{
			get
			{
				return this._groups;
			}
		}

		#endregion

		#region 場所

		/// <summary>
		/// 場所一覧
		/// </summary>
		[DataMember]
		private readonly EntityTreeModel<PlaceEntity> _places = new EntityTreeModel<PlaceEntity>(new PlaceEntity());
		public EntityTreeModel<PlaceEntity> Places
		{
			get
			{
				return this._places;
			}
		}

		#endregion

		#region 編

		/// <summary>
		/// 編一覧
		/// </summary>
		[DataMember]
		private readonly EntityListModel<PartEntity> _parts = new EntityListModel<PartEntity>();
		public EntityListModel<PartEntity> Parts
		{
			get
			{
				return this._parts;
			}
		}

		#endregion

		#region シーン

		/// <summary>
		/// シーン一覧
		/// </summary>
		[DataMember]
		private readonly EntityListModel<SceneEntity> _scenes = new EntityListModel<SceneEntity>();
		public EntityListModel<SceneEntity> Scenes
		{
			get
			{
				return this._scenes;
			}
		}

		#endregion

		#region 章

		/// <summary>
		/// 章一覧
		/// </summary>
		[DataMember]
		private readonly EntityListModel<ChapterEntity> _chapters = new EntityListModel<ChapterEntity>();
		public EntityListModel<ChapterEntity> Chapters
		{
			get
			{
				return this._chapters;
			}
		}

		#endregion

		#region 用語

		/// <summary>
		/// 用語一覧
		/// </summary>
		[DataMember]
		private EntityTreeModel<WordEntity> _words = new EntityTreeModel<WordEntity>(new WordEntity());
		public EntityTreeModel<WordEntity> Words
		{
			get
			{
				return this._words;
			}
		}

		#endregion

		#region パラメータ

		/// <summary>
		/// パラメータ一覧
		/// </summary>
		[DataMember]
		private EntityListModel<ParameterEntity> _parameters = new EntityListModel<ParameterEntity>();
		public EntityListModel<ParameterEntity> Parameters
		{
			get
			{
				return this._parameters;
			}
		}

		/// <summary>
		/// 指定したパラメータをすべての人物に登録する
		/// </summary>
		/// <param name="parameter">パラメータ</param>
		public void AddParameterToAllPeople(ParameterEntity parameter)
		{
			foreach (var person in this.People)
			{
				this.PersonParameterRelation.AddRelate(person, parameter);
			}
		}

		/// <summary>
		/// 指定したパラメータのうち、コメントなどが設定されていないものを人物の登録から解除する
		/// </summary>
		/// <param name="parameter"></param>
		public void RemoveEmptyParameterToAllPeople(ParameterEntity parameter)
		{
			var removeList = new Collection<PersonParameterEntityRelate>();
			foreach (var relation in this.PersonParameterRelation.Relations)
			{
				if (relation.Entity2.Id == parameter.Id &&
					string.IsNullOrWhiteSpace(relation.StringValue))
				{
					removeList.Add(relation);
				}
			}
			foreach (var relation in removeList)
			{
				this.PersonParameterRelation.RemoveRelate(relation.Entity1, relation.Entity2);
			}
		}

		#endregion

		#region メモ

		/// <summary>
		/// メモ一覧
		/// </summary>
		[DataMember]
		private EntityListModel<MemoEntity> _memoes = new EntityListModel<MemoEntity>();
		public EntityListModel<MemoEntity> Memoes
		{
			get
			{
				return this._memoes;
			}
		}

		#endregion

		#region ストーリーライン

		/// <summary>
		/// ストーリーライン一覧
		/// </summary>
		[DataMember]
		private readonly EntityListModel<StorylineEntity> _storylines = new EntityListModel<StorylineEntity>();
		public EntityListModel<StorylineEntity> Storylines
		{
			get
			{
				return this._storylines;
			}
		}

		#endregion

		#region 関連付け（人物・シーン）

		/// <summary>
		/// 人物・シーン関連付けのリスト
		/// </summary>
		private EntityRelationModel<PersonSceneEntityRelate, PersonEntity, SceneEntity> _personSceneRelation;
		public EntityRelationModel<PersonSceneEntityRelate, PersonEntity, SceneEntity> PersonSceneRelation
		{
			get
			{
				return this._personSceneRelation;
			}
		}
		[DataMember]
		private EntityListModel<PersonSceneEntityRelate> _personSceneRelates
		{
			get
			{
				return this._personSceneRelation.Relations;
			}
			set
			{
				this._personSceneRelation.Relations = value;
			}
		}
		private EntityListModel<PersonSceneEntityRelate> PersonSceneRelates
		{
			get
			{
				return this._personSceneRelates;
			}
		}

		#endregion

		#region 関連付け（場所・シーン）

		/// <summary>
		/// 場所・シーン関連付けのリスト
		/// </summary>
		private EntityRelationModel<PlaceSceneEntityRelate, PlaceEntity, SceneEntity> _placeSceneRelation;
		public EntityRelationModel<PlaceSceneEntityRelate, PlaceEntity, SceneEntity> PlaceSceneRelation
		{
			get
			{
				return this._placeSceneRelation;
			}
		}
		[DataMember]
		private EntityListModel<PlaceSceneEntityRelate> _placeSceneRelates
		{
			get
			{
				return this._placeSceneRelation.Relations;
			}
			set
			{
				this._placeSceneRelation.Relations = value;
			}
		}
		private EntityListModel<PlaceSceneEntityRelate> PlaceSceneRelates
		{
			get
			{
				return this._placeSceneRelates;
			}
		}

		#endregion

		#region 関連付け（章・シーン）

		/// <summary>
		/// 章・シーン関連付けのリスト
		/// </summary>
		private EntityRelationModel<SceneChapterEntityRelate, SceneEntity, ChapterEntity> _sceneChapterRelation;
		public EntityRelationModel<SceneChapterEntityRelate, SceneEntity, ChapterEntity> SceneChapterRelation
		{
			get
			{
				return this._sceneChapterRelation;
			}
		}
		[DataMember]
		private EntityListModel<SceneChapterEntityRelate> _sceneChapterRelates
		{
			get
			{
				return this._sceneChapterRelation.Relations;
			}
			set
			{
				this._sceneChapterRelation.Relations = value;
			}
		}
		private EntityListModel<SceneChapterEntityRelate> SceneChapterRelates
		{
			get
			{
				return this._sceneChapterRelates;
			}
		}

		#endregion

		#region 関連付け（人物・人物）

		/// <summary>
		/// 人物・人物関連付けのリスト
		/// </summary>
		[DataMember]
		private EntityListModel<PersonPersonEntityRelate> _personPersonRelates = new EntityListModel<PersonPersonEntityRelate>();
		public EntityListModel<PersonPersonEntityRelate> PersonPersonRelates
		{
			get
			{
				return this._personPersonRelates;
			}
		}

        /// <summary>
        /// 人物・人物関連付け
        /// </summary>
        public EachFocusableEntityRelationModel<PersonEntity> PersonPersonRelation
        {
            get
            {
                return this._personPersonRelation = this._personPersonRelation ?? new EachFocusableEntityRelationModel<PersonEntity>
                {
                    Relations = this.PersonPersonRelates,
                };
            }
        }
        private EachFocusableEntityRelationModel<PersonEntity> _personPersonRelation;

		/// <summary>
		/// 人物に関連付けられている人物を取得
		/// </summary>
		/// <param name="person">人物</param>
		/// <returns>人物</returns>
        [Obsolete]
		public IEnumerable<PersonPersonEntityRelate> FindRelatedPeople(PersonEntity person)
		{
			var list = this.PersonPersonRelates.Where((obj) => obj.Entity1 == person || obj.Entity2 == person);
			foreach (var item in list)
			{
				item.FocusedEntity = person;
			}
			return list;
		}

		/// <summary>
		/// 人物に関連付けられていない人物を取得
		/// </summary>
		/// <param name="person">人物</param>
		/// <returns>関連付けられていない人物</returns>
        [Obsolete]
		public IEnumerable<PersonEntity> FindNotRelatedPeople(PersonEntity person)
		{
			var related = this.FindRelatedPeople(person);
			/*
			var list = from item in this.People
					   from relation in related.DefaultIfEmpty()
					   where (person != item) && (relation == null || ((relation.Entity1 != person || relation.Entity2 != item) && (relation.Entity2 != person || relation.Entity1 != item)))
					   select item;
					   */
			var list = new List<PersonEntity>();
			foreach (var item in this.People)
			{
				bool hit = item != person;
				if (hit)
				{
					foreach (var relation in related)
					{
						if ((relation.Entity1 == person && relation.Entity2 == item) || (relation.Entity2 == person && relation.Entity1 == item))
						{
							hit = false;
							break;
						}
					}
				}
				if (hit)
				{
					list.Add(item);
				}
			}
			return list;
		}

		/// <summary>
		/// 新しい関連付けを追加
		/// </summary>
		/// <param name="person">人物</param>
		/// <param name="scene">人物</param>
        [Obsolete]
		public void AddRelate(PersonEntity person, PersonEntity scene)
		{
			if (person != scene && !this.PersonPersonRelates.ContainsIf((item) => (item.Entity1 == person && item.Entity2 == scene) || (item.Entity2 == person && item.Entity1 == scene)))
			{
				this.PersonPersonRelates.Add(new PersonPersonEntityRelate(person, scene));
			}
		}

		/// <summary>
		/// 関連付けを削除
		/// </summary>
		/// <param name="person">人物</param>
		/// <param name="scene">人物</param>
        [Obsolete]
		public void RemoveRelate(PersonEntity person, PersonEntity scene)
		{
			this.PersonPersonRelates.RemoveIf((item) => (item.Entity1 == person && item.Entity2 == scene) || (item.Entity2 == person && item.Entity1 == scene));
		}

		#endregion

		#region 関連付け（人物・性別）

		/// <summary>
		/// 人物・性別関連付けのリスト
		/// なお、一対多（１人が複数の性別を持つこと）を禁じる処理は、StoryModelの初期化処理の中で記述
		/// 将来的に一対多を許可する予定があるので留意
		/// </summary>
		private EntityRelationModel<SexPersonEntityRelate, SexEntity, PersonEntity> _sexPersonRelation;
		public EntityRelationModel<SexPersonEntityRelate, SexEntity, PersonEntity> SexPersonRelation
		{
			get
			{
				return this._sexPersonRelation;
			}
		}
		[DataMember]
		private EntityListModel<SexPersonEntityRelate> _sexPersonRelates
		{
			get
			{
				return this._sexPersonRelation.Relations;
			}
			set
			{
				this._sexPersonRelation.Relations = value;
			}
		}
		private EntityListModel<SexPersonEntityRelate> SexPersonRelates
		{
			get
			{
				return this._sexPersonRelates;
			}
		}

		#endregion

		#region 関連付け（集団・人物）

		/// <summary>
		/// 集団・人物関連付けのリスト
		/// </summary>
		private EntityRelationModel<GroupPersonEntityRelate, GroupEntity, PersonEntity> _groupPersonRelation;
		public EntityRelationModel<GroupPersonEntityRelate, GroupEntity, PersonEntity> GroupPersonRelation
		{
			get
			{
				return this._groupPersonRelation;
			}
		}
		[DataMember]
		private EntityListModel<GroupPersonEntityRelate> _groupPersonRelates
		{
			get
			{
				return this._groupPersonRelation.Relations;
			}
			set
			{
				this._groupPersonRelation.Relations = value;
			}
		}
		public EntityListModel<GroupPersonEntityRelate> GroupPersonRelates
		{
			get
			{
				return this._groupPersonRelates;
			}
		}

		#endregion

		#region 関連付け（人物・パラメータ）

		/// <summary>
		/// 人物・パラメータ関連付けのリスト
		/// </summary>
		private EntityRelationModel<PersonParameterEntityRelate, PersonEntity, ParameterEntity> _personParameterRelation;
		public EntityRelationModel<PersonParameterEntityRelate, PersonEntity, ParameterEntity> PersonParameterRelation
		{
			get
			{
				return this._personParameterRelation;
			}
		}
		[DataMember]
		private EntityListModel<PersonParameterEntityRelate> _personParameterRelates
		{
			get
			{
				return this._personParameterRelation.Relations;
			}
			set
			{
				this._personParameterRelation.Relations = value;
			}
		}
		private EntityListModel<PersonParameterEntityRelate> PersonParameterRelates
		{
			get
			{
				return this._personParameterRelates;
			}
		}

		#endregion

		#region 関連付け（用語・人物）

		/// <summary>
		/// 用語・人物関連付けのリスト
		/// </summary>
		private EntityRelationModel<WordPersonEntityRelate, WordEntity, PersonEntity> _wordPersonRelation;
		public EntityRelationModel<WordPersonEntityRelate, WordEntity, PersonEntity> WordPersonRelation
		{
			get
			{
				return this._wordPersonRelation;
			}
		}
		[DataMember]
		private EntityListModel<WordPersonEntityRelate> _wordPersonRelates
		{
			get
			{
				return this._wordPersonRelation.Relations;
			}
			set
			{
				this._wordPersonRelation.Relations = value;
			}
		}
		public EntityListModel<WordPersonEntityRelate> WordPersonRelates
		{
			get
			{
				return this._wordPersonRelates;
			}
		}

		#endregion

		#region 関連付け（用語・シーン）

		/// <summary>
		/// 用語・シーン関連付けのリスト
		/// </summary>
		private EntityRelationModel<WordSceneEntityRelate, WordEntity, SceneEntity> _wordSceneRelation;
		public EntityRelationModel<WordSceneEntityRelate, WordEntity, SceneEntity> WordSceneRelation
		{
			get
			{
				return this._wordSceneRelation;
			}
		}
		[DataMember]
		private EntityListModel<WordSceneEntityRelate> _wordSceneRelates
		{
			get
			{
				return this._wordSceneRelation.Relations;
			}
			set
			{
				this._wordSceneRelation.Relations = value;
			}
		}
		public EntityListModel<WordSceneEntityRelate> WordSceneRelates
		{
			get
			{
				return this._wordSceneRelates;
			}
		}

		#endregion

		#region 関連付け共通

		/// <summary>
		/// 指定人物の関連付けを削除
		/// </summary>
		/// <param name="person">人物</param>
		private void RemoveRelate(PersonEntity person)
		{
			this.PersonSceneRelates.RemoveIf((item) => item.Entity1 == person);
			this.GroupPersonRelates.RemoveIf((item) => item.Entity2 == person);
			this.PersonPersonRelates.RemoveIf((item) => item.Entity1 == person || item.Entity2 == person);
			this.SexPersonRelates.RemoveIf((item) => item.Entity2 == person);
			this.PersonParameterRelates.RemoveIf((item) => item.Entity1 == person);
			this.WordPersonRelates.RemoveIf((item) => item.Entity2 == person);
		}

		/// <summary>
		/// 指定集団の関連付けを解除
		/// </summary>
		/// <param name="group">集団</param>
		private void RemoveRelate(GroupEntity group)
		{
			this.GroupPersonRelates.RemoveIf((item) => item.Entity1 == group);
		}

		/// <summary>
		/// 指定場所の関連付けを削除
		/// </summary>
		/// <param name="place">場所</param>
		private void RemoveRelate(PlaceEntity place)
		{
			this.PlaceSceneRelates.RemoveIf((item) => item.Entity1 == place);
		}

		/// <summary>
		/// 指定シーンの関連付けを削除
		/// </summary>
		/// <param name="scene">シーン</param>
		private void RemoveRelate(SceneEntity scene)
		{
			this.PersonSceneRelates.RemoveIf((item) => item.Entity2 == scene);
			this.PlaceSceneRelates.RemoveIf((item) => item.Entity2 == scene);
			this.SceneChapterRelates.RemoveIf((item) => item.Entity1 == scene);
			this.WordSceneRelates.RemoveIf((item) => item.Entity2 == scene);
		}

		/// <summary>
		/// 指定章の関連付けを削除
		/// </summary>
		/// <param name="chapter">章</param>
		private void RemoveRelate(ChapterEntity chapter)
		{
			this.SceneChapterRelates.RemoveIf((item) => item.Entity2 == chapter);
		}

		/// <summary>
		/// 指定性の関連付けを解除
		/// </summary>
		/// <param name="sex">性</param>
		private void RemoveRelate(SexEntity sex)
		{
			this.SexPersonRelates.RemoveIf((item) => item.Entity1 == sex);
		}

		/// <summary>
		/// 指定パラメータの関連付けを解除
		/// </summary>
		/// <param name="parameter">パラメータ</param>
		private void RemoveRelate(ParameterEntity parameter)
		{
			this.PersonParameterRelates.RemoveIf((item) => item.Entity2 == parameter);
		}

		/// <summary>
		/// 指定用語の関連付けを解除
		/// </summary>
		/// <param name="word">用語</param>
		private void RemoveRelate(WordEntity word)
		{
			this.WordPersonRelates.RemoveIf((item) => item.Entity1 == word);
			this.WordSceneRelates.RemoveIf((item) => item.Entity1 == word);
		}

		private void GiveEntitySetToRelates()
		{
			foreach (var item in this.PersonPersonRelates)
			{
				item.GiveEntitySet(this.People, this.People);
			}
			this.PersonPersonRelates.RemoveIf((item) => !item.IsValid);
			foreach (var item in this.PersonSceneRelates)
			{
				item.GiveEntitySet(this.People, this.Scenes);
			}
			this.PersonSceneRelates.RemoveIf((item) => !item.IsValid);
			foreach (var item in this.PlaceSceneRelates)
			{
				item.GiveEntitySet(this.Places, this.Scenes);
			}
			this.PlaceSceneRelates.RemoveIf((item) => !item.IsValid);
			foreach (var item in SceneChapterRelates)
			{
				item.GiveEntitySet(this.Scenes, this.Chapters);
			}
			this.SceneChapterRelates.RemoveIf((item) => !item.IsValid);
			foreach (var item in GroupPersonRelates)
			{
				item.GiveEntitySet(this.Groups, this.People);
			}
			this.GroupPersonRelates.RemoveIf((item) => !item.IsValid);
			foreach (var item in this.SexPersonRelates)
			{
				item.GiveEntitySet(this.Sexes, this.People);
			}
			this.SexPersonRelates.RemoveIf((item) => !item.IsValid);
			foreach (var item in this.PersonParameterRelates)
			{
				item.GiveEntitySet(this.People, this.Parameters);
			}
			this.PersonParameterRelates.RemoveIf((item) => !item.IsValid);
			foreach (var item in this.WordPersonRelates)
			{
				item.GiveEntitySet(this.Words, this.People);
			}
			this.WordPersonRelates.RemoveIf((item) => !item.IsValid);
			foreach (var item in this.WordSceneRelates)
			{
				item.GiveEntitySet(this.Words, this.Scenes);
			}
			this.WordSceneRelates.RemoveIf((item) => !item.IsValid);

			// シーンの脚本
			foreach (var scene in this.Scenes)
			{
				scene.Scenario.FindEntity(this.People, this.Places);
			}
		}

		#endregion

		#region 検索

		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="entities">検索のリスト</param>
		/// <param name="query">検索クエリ</param>
		public void Search<E>(EntitySetModel<E> entities, EntitySearchQuery query) where E : Entity
		{
			foreach (var e in entities)
			{
				e.Search(query);
			}
		}

		/// <summary>
		/// ツリーアイテムから検索
		/// </summary>
		/// <param name="tree">検索のリスト</param>
		/// <param name="query">検索クエリ</param>
		public void SearchTree<E>(EntityTreeModel<E> tree, EntitySearchQuery query) where E : TreeEntity
		{
			foreach (var e in tree.List)
			{
				e.Entity.Search(query);
			}
		}

        #endregion

        #region 編集画面用モデル

        [DataMember]
        private PersonEditorModel _personEditorModel;
        public PersonEditorModel PersonEditorModel
        {
            get
            {
                if (this._personEditorModel == null)
                {
                    this._personEditorModel = new PersonEditorModel(this);
                }
                return new PersonEditorModel(this._personEditorModel);
            }
        }

        #endregion

        #region ファイル

        private SlotSaveService<StoryModel> storySaveService = new SlotSaveService<StoryModel>();

        public void Save(ISlot slot)
        {
            try
            {
                slot.Name = this.StoryConfig.Title;
                slot.Comment = this.StoryConfig.Comment;
                this.storySaveService.SaveAsync(this, slot).ConfigureAwait(false);
            }
            catch
            {
                throw new NotImplementedException();
            }
        }

        public void Load(ISlot slot)
        {
            try
            {
                var story = Task.Run(async () => await this.storySaveService.LoadAsync(slot)).Result;

                this.ChangeStoryModel(story);
            }
            catch
            {
                throw new NotImplementedException();
            }
        }

        private SerializationModel Serialization = new SerializationModel();

		/// <summary>
		/// 排他制御用のロックオブジェクト
		/// </summary>
		private readonly static object lockObject = new object();

		/// <summary>
		/// ストーリーがこれから新規作成される時に呼び出されるイベント
		/// </summary>
		public event NewStoryCreatedEventHandler NewStoryCreating = delegate { };

		/// <summary>
		/// ストーリーが新規作成された時に呼び出されるイベント
		/// </summary>
		public event NewStoryCreatedEventHandler NewStoryCreated = delegate { };

		/// <summary>
		/// ストーリーの自動保存が要求された時に呼び出されるイベント
		/// </summary>
		public event AutoSaveRequestedEventHandler AutoSaveRequested = delegate { };

		/// <summary>
		/// 新規作成
		/// </summary>
		public void CreateNew()
		{
			lock (lockObject)
			{
				Messenger.Default.Send(this, new AlertMessage(StringResourceResolver.Resolve("IsCreateNewMessage"), AlertMessageType.YesNo, (result) =>
				{
					if (result == AlertMessageResult.Yes)
					{
						this.NewStoryCreating?.Invoke(this, new EventArgs());
						this.Reset();
						this.LoadStreamCompleted();
						this.NewStoryCreated?.Invoke(this, new EventArgs());
						Messenger.Default.Send(this, new NavigationBackToRootMessage());
					}
				}));
			}
		}

		/// <summary>
		/// ファイルを開く（本体）
		/// </summary>
		/// <param name="filePath">保存先のファイル名</param>
		private void OpenFile(string filePath)
		{
			lock (lockObject)
			{
				StoryModel model = null;
#if WPF
				using (var fileStream = new FileStream(filePath, FileMode.Open))
				{
					model = this.Serialization.Deserialize<StoryModel>(fileStream);
				}

				if (model != null)
				{
					this.ChangeStoryModel(model);
				}
#elif XAMARIN_FORMS
				Messenger.Default.Send(this, new OpenSlotPickerMessage(async (file) =>
				{
					using (var fileStream = await file.OpenAsync(PCLStorage.FileAccess.Read))
					{
						model = this.Serialization.Deserialize<StoryModel>(fileStream);

						if (model != null)
						{
							this.ChangeStoryModel(model);
						}
					}
					Messenger.Default.Send(this, new NavigationBackToRootMessage());
				}));
#endif
			}
		}
		public void LoadSlot(StorageSlotBase slot)
		{
			lock (lockObject)
			{
				StoryModel model = null;
				try
				{
					using (var fileStream = slot.OpenStream())
					{
						model = this.Serialization.Deserialize<StoryModel>(fileStream);
					}

					if (model != null)
					{
						this.ChangeStoryModel(model);
					}

					Messenger.Default.Send(this, new NavigationBackToRootMessage());
				}
				catch
				{
					Messenger.Default.Send(this, new AlertMessage(StringResourceResolver.Resolve("UndefinedErrorMessage")));
					model = null;
				}
			}
		}

		/// <summary>
		/// スロットのロードが終わったことを通知
		/// </summary>
		public event EventHandler LoadSlotCompleted;

		/// <summary>
		/// スロットのセーブが終わったことを通知
		/// </summary>
		public event EventHandler SaveSlotCompleted;

		private void ChangeStoryModel(StoryModel model)
		{
			//StoryModel.Current = model;
			//this.ResetEntities(model);
			this.CopyFrom(model);
			model.LoadStreamCompleted();
			this.LoadStreamCompleted();
		}

		/// <summary>
		/// ファイルに保存（本体）
		/// </summary>
		/// <param name="filePath">保存先のファイル名</param>
		/// <param name="isMessage">保存時にメッセージを出すか</param>
		private void SaveFile(string filePath, bool isMessage = true)
		{
			lock (lockObject)
			{
				XmlWriterSettings settings = new XmlWriterSettings();
				settings.Encoding = new UTF8Encoding(false);

#if WPF
				using (var fileStream = new FileStream(filePath, FileMode.Create))
				{
					this.Serialization.Serialize(fileStream, this);
				}
				if (isMessage)
				{
					Messenger.Default.Send(this, new LightAlertMessage(StringResourceResolver.Resolve("SaveFinishMessage")));
				}
#elif XAMARIN_FORMS
				Messenger.Default.Send(this, new SaveSlotPickerMessage(async (file) =>
				{
					using (var fileStream = await file.OpenAsync(PCLStorage.FileAccess.ReadAndWrite))
					{
						this.Serialization.Serialize(fileStream, this);
					}
					//Messenger.Default.Send(this, new LightAlertMessage(StringResourceResolver.Resolve("SaveFinishMessage")));
					if (isMessage)
					{
						Messenger.Default.Send(this, new NavigationBackToRootMessage());
					}
				}));
#endif
			}
		}

		/// <summary>
		/// スロットに保存する
		/// </summary>
		/// <param name="slot">保存先スロット</param>
		/// <param name="isMessage">保存時にメッセージを表示するか？</param>
		/// <returns></returns>
		public bool SaveSlot(StorageSlotBase slot, bool isMessage = true)
		{
			bool result = true;
			lock (lockObject)
			{
				try
				{
					Stream fileStream = slot.WriteStream();
					using (fileStream)
					{
						this.Serialization.Serialize(fileStream, this);
					}
					slot.SlotName = this.StoryConfig.Title;
					slot.SlotComment = this.StoryConfig.Comment;
					//slot.CheckExists();		// クラウドでこの行が保存より先に呼ばれることがある
					slot.IsExists = true;

					if (isMessage)
					{
						Messenger.Default.Send(this, new NavigationBackToRootMessage());
						Messenger.Default.Send(this, new LightAlertMessage(StringResourceResolver.Resolve("SaveFinishMessage")));
					}
				}
				catch
				{
					if (isMessage)
					{
						Messenger.Default.Send(this, new AlertMessage(StringResourceResolver.Resolve("UndefinedErrorMessage")));
					}
					result = false;
				}
			}
			return result;
		}

		/// <summary>
		/// 保存先のパス
		/// </summary>
		private string FilePath = null;

		/// <summary>
		/// 上書き保存
		/// </summary>
		public void SaveFile()
		{
#if WPF
			if (this.FilePath == null || this.FilePath == "")
			{
				this.SaveAsFile();
			}
			else
			{
				this.SaveFile(this.FilePath);
			}
#elif XAMARIN_FORMS
			this.SaveFile("");
#endif
		}

		/// <summary>
		/// 名前をつけて保存
		/// </summary>
		public void SaveAsFile()
		{
			var message = new SaveFilePickerMessage();
			Messenger.Default.Send(this, message);
			if (!message.IsSelected)
			{
				return;
			}
			this.FilePath = message.FileName;
			this.SaveFile();
		}

		/// <summary>
		/// ファイルを開く
		/// </summary>
		public void OpenFile()
		{
#if WPF
			var message = new OpenFilePickerMessage();
			Messenger.Default.Send(this, message);
			if (!message.IsSelected)
			{
				return;
			}
			this.FilePath = message.FileName;
			this.OpenFile(this.FilePath);
#elif XAMARIN_FORMS
			this.OpenFile("");
#endif
		}

		/// <summary>
		/// 自動保存のループを回す
		/// </summary>
		private void SetNextAutoSaveEvent()
		{
			// XFのオートセーブを元に戻す時は、StorageModelのコンストラクタも修正のこと
			Task.Run(async () =>
			{
				while (true)
				{
					await Task.Delay(60 * 1000);
					if (this.StoryConfig.IsAutoSave)
					{
						this.AutoSaveRequested(this, new EventArgs());
					}
				}
			});
		}

		/// <summary>
		/// アプリ終了時にビュー側から直接呼び出し
		/// </summary>
		public void Quit()
		{
			this.AutoSaveRequested(this, new EventArgs());
		}

		#endregion

		#region ネットワーク

		public NetworkSendStatus NetworkSendStatus
		{
			get
			{
				return this.Network.NetworkSendStatus;
			}
			set
			{
				this.Network.NetworkSendStatus = value;
			}
		}

		public NetworkReceiveStatus NetworkReceiveStatus
		{
			get
			{
				return this.Network.NetworkReceiveStatus;
			}
			set
			{
				this.Network.NetworkReceiveStatus = value;
			}
		}

		public Collection<CommsInterface> Interfaces
		{
			get
			{
				return this.Network.Interfaces;
			}
		}

		public CommsInterface Interface
		{
			get
			{
				return this.Network.Interface;
			}
			set
			{
				this.Network.Interface = value;
			}
		}

		public bool IsAllInterfaces
		{
			get
			{
				return this.Network.IsAllInterfaces;
			}
			set
			{
				this.Network.IsAllInterfaces = value;
			}
		}

		/// <summary>
		/// ネットワークを取り扱うモデル
		/// </summary>
		private NetworkStoryModel _network;
		public NetworkStoryModel Network
		{
			get
			{
				if (this._network == null)
				{
					this._network = new NetworkStoryModel();
					this._network.ModelReceived += (model) =>
					{
						this.ChangeStoryModel(model);
					};
					this._network.PropertyChanged += (sender, e) =>
					{
						this.OnPropertyChanged(e.PropertyName);
					};
				}
				return this._network;
			}
		}

		#endregion

		private void ResetEntities(StoryModel o)
		{
			foreach (var e in o.People)
			{
				e.StoryModel = o;
			}
		}

		/// <summary>
		/// ストーリーのデータを全て初期化する
		/// </summary>
		private void Reset()
		{
			this.People.RemoveAll();
			this.Groups.Root.Children.RemoveAll();
			this.Groups.CheckParentChildren();
			this.Places.Root.Children.RemoveAll();
			this.Places.CheckParentChildren();
			this.Scenes.RemoveAll();
			this.Chapters.RemoveAll();
			this.Sexes.RemoveAll();
			this.Parameters.RemoveAll();
			this.Memoes.RemoveAll();
			this.Words.Root.Children.RemoveAll();
			this.Words.CheckParentChildren();
			this.SceneChapterRelates.RemoveAll();
			this.PersonPersonRelates.RemoveAll();
			this.PersonSceneRelates.RemoveAll();
			this.PlaceSceneRelates.RemoveAll();
			this.GroupPersonRelates.RemoveAll();
			this.SexPersonRelates.RemoveAll();
			this.WordPersonRelates.RemoveAll();
			this.WordSceneRelates.RemoveAll();
			this.EntityCount = 0;
			this.PersonParameterSelectCellCount = 0;

			this.StoryConfig = new StoryConfig();
		}

		/// <summary>
		/// ストーリーモデルの内容をまることコピーする
		/// </summary>
		/// <param name="o">コピー元</param>
		private void CopyFrom(StoryModel o)
		{
			this.ResetList(this.People, o.People);
			this.ResetList(this.Groups.Root.Children, o.Groups?.Root.Children);		// v1.1追加
			this.Groups.CheckParentChildren();
			this.ResetList(this.Places.Root.Children, o.Places.Root.Children);
			this.Places.CheckParentChildren();
			this.ResetList(this.Scenes, o.Scenes);
			this.ResetList(this.Chapters, o.Chapters);
			this.ResetList(this.Sexes, o.Sexes);
			this.ResetList(this.Parameters, o.Parameters);
			this.ResetList(this.Memoes, o.Memoes);
			this.ResetList(this.Words.Root.Children, o.Words.Root.Children);
			this.Words.CheckParentChildren();
			this.ResetList(this.SceneChapterRelates, o.SceneChapterRelates);
			this.ResetList(this.PersonPersonRelates, o.PersonPersonRelates);
			this.ResetList(this.PersonSceneRelates, o.PersonSceneRelates);
			this.ResetList(this.PlaceSceneRelates, o.PlaceSceneRelates);
			this.ResetList(this.GroupPersonRelates, o.GroupPersonRelates);
			this.ResetList(this.SexPersonRelates, o.SexPersonRelates);
			this.ResetList(this.PersonParameterRelates, o.PersonParameterRelates);
			this.ResetList(this.WordPersonRelates, o.WordPersonRelates);
			this.ResetList(this.WordSceneRelates, o.WordSceneRelates);
			this.EntityCount = o.EntityCount;
			this.PersonParameterSelectCellCount = o.PersonParameterSelectCellCount;

            o._personEditorModel.CopyTo(this._personEditorModel);
            this._personEditorModel.LoadEntities(this.People);

			this.StoryConfig = o.StoryConfig;
		}

		private void ResetList<E>(EntityListModel<E> list, ICollection<E> collection) where E : IEntity
		{
			if (list != null)
			{
				list.RemoveAll();
				if (collection != null)
				{
					list.AddRange(collection);
				}
			}
		}

	}
}
