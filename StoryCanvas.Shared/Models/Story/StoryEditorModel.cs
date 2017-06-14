using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using StoryCanvas.Shared.Messages.Page;
using StoryCanvas.Shared.Models.Entities;
using StoryCanvas.Shared.Models.EntitySet;
using StoryCanvas.Shared.Types;
using StoryCanvas.Shared.ViewTools;

namespace StoryCanvas.Shared.Models.Story
{
	/// <summary>
	/// 編集画面を操作するためのモデル
	/// </summary>
    public class StoryEditorModel : INotifyPropertyChanged
    {
		public static readonly StoryEditorModel Default = new StoryEditorModel();

		public StoryEditorModel()
		{
			this._personSelection.SelectionChanged += (e, o) => this.OnPropertyChanged("SelectedPerson");
			this._groupSelection.SelectionChanged += (e, o) => this.OnPropertyChanged("SelectedGroup");
			this._placeSelection.SelectionChanged += (e, o) => this.OnPropertyChanged("SelectedPlace");
			this._chapterSelection.SelectionChanged += (e, o) => this.OnPropertyChanged("SelectedChapter");
			this._partSelection.SelectionChanged += (e, o) => this.OnPropertyChanged("SelectedPart");
			this._storylineSelection.SelectionChanged += (e, o) => this.OnPropertyChanged("SelectedStoryline");
			this._sceneSelection.SelectionChanged += (e, o) => this.OnPropertyChanged("SelectedScene");
			this._sexSelection.SelectionChanged += (e, o) => this.OnPropertyChanged("SelectedSex");
			this._memoSelection.SelectionChanged += (e, o) => this.OnPropertyChanged("SelectedMemo");
			this._parameterSelection.SelectionChanged += (e, o) => this.OnPropertyChanged("SelectedParameter");
		}

		/// <summary>
		/// 現在選択中の人物
		/// </summary>
		private EntitySelectionModel<PersonEntity> _personSelection = new EntitySelectionModel<PersonEntity>();
		public EntitySelectionModel<PersonEntity> PersonSelection
		{
			get
			{
				return this._personSelection;
			}
		}

		/// <summary>
		/// 現在選択中の集団
		/// </summary>
		private EntitySelectionModel<GroupEntity> _groupSelection = new EntitySelectionModel<GroupEntity>();
		public EntitySelectionModel<GroupEntity> GroupSelection
		{
			get
			{
				return this._groupSelection;
			}
		}

		/// <summary>
		/// 現在選択中の場所
		/// </summary>
		private EntitySelectionModel<PlaceEntity> _placeSelection = new EntitySelectionModel<PlaceEntity>();
		public EntitySelectionModel<PlaceEntity> PlaceSelection
		{
			get
			{
				return this._placeSelection;
			}
		}

		/// <summary>
		/// 現在選択中の章
		/// </summary>
		private EntitySelectionModel<ChapterEntity> _chapterSelection = new EntitySelectionModel<ChapterEntity>();
		public EntitySelectionModel<ChapterEntity> ChapterSelection
		{
			get
			{
				return this._chapterSelection;
			}
		}

		/// <summary>
		/// 現在選択中の編
		/// </summary>
		private EntitySelectionModel<PartEntity> _partSelection = new EntitySelectionModel<PartEntity>();
		public PartEntity SelectedPart
		{
			get
			{
				return this._partSelection.Selected;
			}
			set
			{
				this._partSelection.Selected = value;
			}
		}

		/// <summary>
		/// 現在選択中の性別
		/// </summary>
		private EntitySelectionModel<SexEntity> _sexSelection = new EntitySelectionModel<SexEntity>();
		public EntitySelectionModel<SexEntity> SexSelection
		{
			get
			{
				return this._sexSelection;
			}
		}

		/// <summary>
		/// 現在選択中のパラメータ
		/// </summary>
		private EntitySelectionModel<ParameterEntity> _parameterSelection = new EntitySelectionModel<ParameterEntity>();
		public EntitySelectionModel<ParameterEntity> ParameterSelection
		{
			get
			{
				return this._parameterSelection;
			}
		}

		/// <summary>
		/// 現在選択中のメモ
		/// </summary>
		private EntitySelectionModel<MemoEntity> _memoSelection = new EntitySelectionModel<MemoEntity>();
		public EntitySelectionModel<MemoEntity> MemoSelection
		{
			get
			{
				return this._memoSelection;
			}
		}

		/// <summary>
		/// 現在選択中のストーリーライン
		/// </summary>
		private EntitySelectionModel<StorylineEntity> _storylineSelection = new EntitySelectionModel<StorylineEntity>();
		public StorylineEntity SelectedStoryline
		{
			get
			{
				return this._storylineSelection.Selected;
			}
			set
			{
				this._storylineSelection.Selected = value;
			}
		}

		/// <summary>
		/// 現在選択中のシーン
		/// </summary>
		private EntitySelectionModel<SceneEntity> _sceneSelection = new EntitySelectionModel<SceneEntity>();
		public EntitySelectionModel<SceneEntity> SceneSelection
		{
			get
			{
				return this._sceneSelection;
			}
		}

		/// <summary>
		/// 現在選択中の用語
		/// </summary>
		private EntitySelectionModel<WordEntity> _wordSelection = new EntitySelectionModel<WordEntity>();
		public EntitySelectionModel<WordEntity> WordSelection
		{
			get
			{
				return this._wordSelection;
			}
		}

		/// <summary>
		/// 画面モードが変更された時に発行されるイベント
		/// </summary>
		public delegate void MainModeChangedEventHandler(MainMode oldMode, MainMode newMode);
		public event MainModeChangedEventHandler MainModeChanged = delegate { };

		/// <summary>
		/// 現在の画面モード
		/// </summary>
		private bool IsMainModeChanging = false;
		private MainMode _mainMode = MainMode.EditPerson;
		public MainMode MainMode
		{
			get
			{
				return this._mainMode;
			}
			set
			{
				var oldMode = this._mainMode;
				this._mainMode = value;
				this.OnPropertyChanged();

				this.MainModeChanged(oldMode, value);
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
