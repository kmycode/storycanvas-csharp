using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoryCanvas.Shared.Models.EntitySet;
using StoryCanvas.Shared.Models.Entities;
using StoryCanvas.Shared.Models.Story;
using StoryCanvas.Shared.ViewTools;
using System.Collections.Specialized;
using StoryCanvas.Shared.Types;
using System.Collections.ObjectModel;
using StoryCanvas.Shared.Messages.EditEntity;
using StoryCanvas.Shared.ViewTools.ControlModels;
using StoryCanvas.Shared.Messages.Network;
using StoryCanvas.Shared.Messages.IO;
using StoryCanvas.Shared.Models.IO;
using StoryCanvas.Shared.Models;
using StoryCanvas.Shared.ViewTools.BehaviorHelper;
using StoryCanvas.Shared.Messages.Page;

namespace StoryCanvas.Shared.ViewModels
{
	public class StoryViewModel : ViewModelBase
	{
		// シングルトン
        [Obsolete]
		public static readonly StoryViewModel Default = new StoryViewModel();
		public StoryViewModel()
		{
			this.StoreModel(this.EditorModel);

            //StoryModel.CurrentStoryChanged += (story, oldStory) => {
            //	this.OnPropertyChanged("People");
            //	this.OnPropertyChanged("Groups");
            //	this.OnPropertyChanged("Places");
            //	this.OnPropertyChanged("Scenes");
            //	this.OnPropertyChanged("Chapters");
            //	this.OnPropertyChanged("Sexes");
            //	this.OnPropertyChanged("Parameters");
            //	this.OnPropertyChanged("Memoes");
            //	this.OnPropertyChanged("Words");
            //	story.LoadStreamCompleted += () =>
            //	{
            //	};
            //};

            //this.EditorModel.PersonSelection.SelectionChanged +=
            //	(n, _) => Messenger.Default.Send(this, new EditPersonEntityPrimaryMessage(n));
            //this.EditorModel.GroupSelection.SelectionChanged +=
            //	(n, _) => Messenger.Default.Send(this, new EditGroupEntityPrimaryMessage(n));
            //this.EditorModel.PlaceSelection.SelectionChanged +=
            //	(n, _) => Messenger.Default.Send(this, new EditPlaceEntityPrimaryMessage(n));
            //this.EditorModel.ChapterSelection.SelectionChanged +=
            //	(n,_) => Messenger.Default.Send(this, new EditChapterEntityPrimaryMessage(n));
            //this.EditorModel.SceneSelection.SelectionChanged +=
            //	(n, _) => Messenger.Default.Send(this, new EditSceneEntityPrimaryMessage(n));
            //this.EditorModel.SexSelection.SelectionChanged +=
            //	(n, _) => Messenger.Default.Send(this, new EditSexEntityPrimaryMessage(n));
            //this.EditorModel.ParameterSelection.SelectionChanged +=
            //	(n, _) => Messenger.Default.Send(this, new EditParameterEntityPrimaryMessage(n));
            //this.EditorModel.MemoSelection.SelectionChanged +=
            //	(n, _) => Messenger.Default.Send(this, new EditMemoEntityPrimaryMessage(n));
            //this.EditorModel.WordSelection.SelectionChanged +=
            //	(n, _) => Messenger.Default.Send(this, new EditWordEntityPrimaryMessage(n));

            // 検索
            //this.PersonSearch.EntitySearchRequested += (sender, e) =>
            // {
            //	 this.StoryModel.Search(this.StoryModel.People, e.Query);
            //	 this.OnPropertyChanged("PersonSearchResult");
            // };
            //this.GroupSearch.EntitySearchRequested +=
            //	(sender, e) => this.StoryModel.SearchTree(this.StoryModel.Groups, e.Query);
            //this.PlaceSearch.EntitySearchRequested +=
            //	(sender, e) => this.StoryModel.SearchTree(this.StoryModel.Places, e.Query);
            //this.SceneSearch.EntitySearchRequested +=
            //	(sender, e) => this.StoryModel.Search(this.StoryModel.Scenes, e.Query);
            //this.ChapterSearch.EntitySearchRequested +=
            //	(sender, e) => this.StoryModel.Search(this.StoryModel.Chapters, e.Query);
            //this.SexSearch.EntitySearchRequested +=
            //	(sender, e) => this.StoryModel.Search(this.StoryModel.Sexes, e.Query);
            //this.ParameterSearch.EntitySearchRequested +=
            //	(sender, e) => this.StoryModel.Search(this.StoryModel.Parameters, e.Query);
            //this.MemoSearch.EntitySearchRequested +=
            //	(sender, e) => this.StoryModel.Search(this.StoryModel.Memoes, e.Query);
            //this.WordSearch.EntitySearchRequested +=
            //	(sender, e) => this.StoryModel.Search(this.StoryModel.Words, e.Query);

            //StoryModel.CurrentStoryChanged += (newStory, oldStory) =>
            //{
            //	this.PersonSet = new EntityListEditorModel<PersonEntity, EditPersonEntityPrimaryMessage>("Person", StoryModel.Current.People, (entity) => new EditPersonEntityPrimaryMessage(entity));
            //	this.GroupSet = new EntityTreeEditorModel<GroupEntity, EditGroupEntityPrimaryMessage>("Group", StoryModel.Current.Groups, (entity) => new EditGroupEntityPrimaryMessage(entity));
            //	this.PlaceSet = new EntityTreeEditorModel<PlaceEntity, EditPlaceEntityPrimaryMessage>("Place", StoryModel.Current.Places, (entity) => new EditPlaceEntityPrimaryMessage(entity));
            //	this.SceneSet = new EntityListEditorModel<SceneEntity, EditSceneEntityPrimaryMessage>("Scene", StoryModel.Current.Scenes, (entity) => new EditSceneEntityPrimaryMessage(entity));
            //	this.ChapterSet = new EntityListEditorModel<ChapterEntity, EditChapterEntityPrimaryMessage>("Chapter", StoryModel.Current.Chapters, (entity) => new EditChapterEntityPrimaryMessage(entity));
            //	this.SexSet = new EntityListEditorModel<SexEntity, EditSexEntityPrimaryMessage>("Sex", StoryModel.Current.Sexes, (entity) => new EditSexEntityPrimaryMessage(entity));
            //	this.ParameterSet = new EntityListEditorModel<ParameterEntity, EditParameterEntityPrimaryMessage>("Parameter", StoryModel.Current.Parameters, (entity) => new EditParameterEntityPrimaryMessage(entity));
            //	this.MemoSet = new EntityListEditorModel<MemoEntity, EditMemoEntityPrimaryMessage>("Memo", StoryModel.Current.Memoes, (entity) => new EditMemoEntityPrimaryMessage(entity));
            //	this.WordSet = new EntityTreeEditorModel<WordEntity, EditWordEntityPrimaryMessage>("Word", StoryModel.Current.Words, (entity) => new EditWordEntityPrimaryMessage(entity));

            //	this.StoreModel(this.PersonSet);
            //	this.StoreModel(this.GroupSet);
            //	this.StoreModel(this.PlaceSet);
            //	this.StoreModel(this.SceneSet);
            //	this.StoreModel(this.ChapterSet);
            //	this.StoreModel(this.SexSet);
            //	this.StoreModel(this.ParameterSet);
            //	this.StoreModel(this.MemoSet);
            //	this.StoreModel(this.WordSet);

            //	this.StoreModel(this._beforeStorage);
            //	this.StoreModel(newStory.StoryConfig);

            //	newStory.LoadStreamCompleted += () =>
            //	{
            //		this.StoreModel(StoryModel.Current.StoryConfig);
            //		this.OnPropertyChanged("Title");
            //	};

            //	StorageModelBase.LastUseStorageChanged += (sender, e) =>
            //	{
            //		this.SpendModel(this._beforeStorage);
            //		this._beforeStorage = StorageModelBase.LastUseStorage;
            //		this.StoreModel(this._beforeStorage);
            //		this.OnPropertyChanged("AutoSaveStatus");
            //	};
            //};

            StoryModel.CurrentStoryChanged += (newStory, oldStory) =>
            {
                this.OnPropertyChanged();
            };
		}

		private StorageModelBase _beforeStorage = StorageModelBase.LastUseStorage;
		private StoryModel StoryModel => StoryModel.Current;
		private readonly StoryEditorModel EditorModel = StoryEditorModel.Default;

        public StoryModel Story => this.StoryModel;

		#region 検索

		/// <summary>
		/// 登場人物の検索
		/// </summary>
		public EntitySearchModel PersonSearch { get; private set; } = new EntitySearchModel();
		public IEnumerable<PersonEntity> PersonSearchResult
		{
			get
			{
				return from item in this.StoryModel.People
					   where item.IsSearchHit
					   select item;
			}
		}

		public EntitySearchModel GroupSearch { get; private set; } = new EntitySearchModel();
		public EntitySearchModel PlaceSearch { get; private set; } = new EntitySearchModel();
		public EntitySearchModel SceneSearch { get; private set; } = new EntitySearchModel();
		public EntitySearchModel ChapterSearch { get; private set; } = new EntitySearchModel();
		public EntitySearchModel SexSearch { get; private set; } = new EntitySearchModel();
		public EntitySearchModel ParameterSearch { get; private set; } = new EntitySearchModel();
		public EntitySearchModel MemoSearch { get; private set; } = new EntitySearchModel();
		public EntitySearchModel WordSearch { get; private set; } = new EntitySearchModel();

		#endregion

		#region 画面表示

		/// <summary>
		/// 現在の画面モード
		/// </summary>
		public MainMode MainMode
		{
			get
			{
				return this.EditorModel.MainMode;
			}
			set
			{
				this.EditorModel.MainMode = value;
			}
		}

		/// <summary>
		/// パラメータを渡せばそのとおりにメインモードを設定してくれるコマンド
		/// </summary>
		private RelayCommand _mainModeSelectCommand;
		public RelayCommand MainModeSelectCommand
		{
			get
			{
				return this._mainModeSelectCommand = this._mainModeSelectCommand ?? new RelayCommand((mode) =>
				{
#if XAMARIN_FORMS
					this.IsMainModeChangable = false;
#endif
                    this.MainMode = (MainMode)Enum.Parse(typeof(MainMode), mode.ToString());
				},
				(obj) =>
				{
					return this.IsMainModeChangable;
				});
			}
		}

		private bool _isMainModeChangable = true;
		public bool IsMainModeChangable
		{
			get
			{
				return this._isMainModeChangable;
			}
			set
			{
				this._isMainModeChangable = value;
				this.MainModeSelectCommand.OnCanExecuteChanged();
			}
		}

		/// <summary>
		/// 編の一覧
		/// </summary>
		public ICollection<PartEntity> Parts
		{
			get
			{
				return StoryModel.Current.Parts;
			}
		}

		/// <summary>
		/// ストーリーの題名
		/// </summary>
		public string Title
		{
			get
			{
				return this.StoryModel.StoryConfig.Title;
			}
		}

#endregion

#region 要素一般

		/// <summary>
		/// 人物
		/// </summary>
		public EntityListEditorModel<PersonEntity, EditPersonEntityPrimaryMessage> PersonSet { get; private set; }

		/// <summary>
		/// 集団
		/// </summary>
		public EntityTreeEditorModel<GroupEntity, EditGroupEntityPrimaryMessage> GroupSet { get; private set; }

		/// <summary>
		/// 場所
		/// </summary>
		public EntityTreeEditorModel<PlaceEntity, EditPlaceEntityPrimaryMessage> PlaceSet { get; private set; }

		/// <summary>
		/// シーン
		/// </summary>
		public EntityListEditorModel<SceneEntity, EditSceneEntityPrimaryMessage> SceneSet { get; private set; }

		/// <summary>
		/// 章
		/// </summary>
		public EntityListEditorModel<ChapterEntity, EditChapterEntityPrimaryMessage> ChapterSet { get; private set; }

		/// <summary>
		/// 性
		/// </summary>
		public EntityListEditorModel<SexEntity, EditSexEntityPrimaryMessage> SexSet { get; private set; }

		/// <summary>
		/// パラメータ
		/// </summary>
		public EntityListEditorModel<ParameterEntity, EditParameterEntityPrimaryMessage> ParameterSet { get; private set; }

		/// <summary>
		/// メモ
		/// </summary>
		public EntityListEditorModel<MemoEntity, EditMemoEntityPrimaryMessage> MemoSet { get; private set; }

		/// <summary>
		/// 用語
		/// </summary>
		public EntityTreeEditorModel<WordEntity, EditWordEntityPrimaryMessage> WordSet { get; private set; }

		#endregion

		#region 保存・読み込み

		/// <summary>
		/// 最後に保存したスロットに対して保存
		/// </summary>
		private RelayCommand _saveLastSlotCommand;
		public RelayCommand SaveLastSlotCommand
		{
			get
			{
				return this._saveLastSlotCommand = this._saveLastSlotCommand ?? new RelayCommand((obj) =>
				{
					Messenger.Default.Send(this, new DetermineInputingMessage());
					StorageModel.Default.SaveLastSlot();
				});
			}
		}

		/// <summary>
		/// 上書き保存
		/// </summary>
		private RelayCommand _saveFileCommand;
		public RelayCommand SaveFileCommand
		{
			get
			{
				return this._saveFileCommand = this._saveFileCommand ?? new RelayCommand((obj) =>
				{
					this.StoryModel.SaveFile();
				});
			}
		}

		/// <summary>
		/// 名前をつけて保存
		/// </summary>
		private RelayCommand _saveAsFileCommand;
		public RelayCommand SaveAsFileCommand
		{
			get
			{
				return this._saveAsFileCommand = this._saveAsFileCommand ?? new RelayCommand((obj) =>
				{
					this.StoryModel.SaveAsFile();
				});
			}
		}

		private RelayCommand _openFileCommand;
		public RelayCommand OpenFileCommand
		{
			get
			{
				return this._openFileCommand = this._openFileCommand ?? new RelayCommand((obj) =>
				{
					this.StoryModel.OpenFile();
				});
			}
		}

		private RelayCommand _openStoragePageCommand;
		public RelayCommand OpenStoragePageCommand
		{
			get
			{
				return this._openStoragePageCommand = this._openStoragePageCommand ?? new RelayCommand((obj) =>
				{
#if WPF
					this.MainMode = MainMode.StoragePage;
#endif
					Messenger.Default.Send(this, new OpenStoragePageMessage());
				});
			}
		}

#endregion

		#region 画面遷移・その他

		private RelayCommand _openNetworkPageCommand;
		public RelayCommand OpenNetworkPageCommand
		{
			get
			{
				return this._openNetworkPageCommand = this._openNetworkPageCommand ?? new RelayCommand((obj) =>
				{
#if WPF
					this.MainMode = MainMode.NetworkPage;
#endif
					Messenger.Default.Send(this, new OpenNetworkPageMessage());
				});
			}
		}

		private RelayCommand _aboutApplicationCommand;
		public RelayCommand AboutApplicationCommand
		{
			get
			{
				return this._aboutApplicationCommand = this._aboutApplicationCommand ?? new RelayCommand((obj) =>
				{
					this.MainMode = MainMode.AboutPage;
				});
			}
		}

		private RelayCommand _goApplicationHPCommand;
		public RelayCommand GoApplicationHPCommand
		{
			get
			{
				return this._goApplicationHPCommand = this._goApplicationHPCommand ?? new RelayCommand((obj) =>
				{
					Messenger.Default.Send(this, new OpenUrlMessage("https://storycanvas.kmycode.net/"));
				});
			}
		}

		private RelayCommand _goSupportForumCommand;
		public RelayCommand GoSupportForumCommand
		{
			get
			{
				return this._goSupportForumCommand = this._goSupportForumCommand ?? new RelayCommand((obj) =>
				{
					Messenger.Default.Send(this, new OpenUrlMessage("http://jbbs.shitaraba.net/otaku/17663/"));
				});
			}
		}

		/// <summary>
		/// ネットワークからストーリーモデルを受信する準備を開始
		/// </summary>
		private RelayCommand _readyReceiveModelCommand;
		public RelayCommand ReadyReceiveModelCommand
		{
			get
			{
				return this._readyReceiveModelCommand = this._readyReceiveModelCommand ?? new RelayCommand((obj) =>
				{
					this.StoryModel.Network.ReadyReceiveNetwork();
				});
			}
		}

		/// <summary>
		/// オートセーブの状態
		/// </summary>
		public AutoSaveStatus AutoSaveStatus
		{
			get
			{
				return StorageModelBase.LastUseStorage.AutoSaveStatus;
			}
		}

#endregion
	}
}
