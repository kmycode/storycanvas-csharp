using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using StoryCanvas.Shared.Models.Date;
using StoryCanvas.Shared.Models.Entities;
using StoryCanvas.Shared.Models.EntitySet;
using StoryCanvas.Shared.Models.Story;
using StoryCanvas.Shared.ViewTools;
using static StoryCanvas.Shared.Models.Story.TimelineModel;

namespace StoryCanvas.Shared.ViewModels.Editors
{
    public class TimelineViewModel : ViewModelBase
    {
		private static TimelineViewModel _default;
		public static TimelineViewModel Default
		{
			get
			{
				return _default = _default ?? new TimelineViewModel();
			}
		}

		private StoryModel StoryModel = StoryModel.Current;
		private TimelineModel Model = TimelineModel.Default;

		private TimelineViewModel()
		{
			this.StoreModel(this.Model);
		}

		/// <summary>
		/// タイムラインの内容が変更になった時発行されるイベント
		/// </summary>
		public event EventHandler TimelineChanged
		{
			add
			{
				this.Model.TimelineChanged += value;
			}
			remove
			{
				this.Model.TimelineChanged -= value;
			}
		}

		/// <summary>
		/// 画面をリロード
		/// </summary>
		private RelayCommand _reloadCommand;
		public RelayCommand ReloadCommand
		{
			get
			{
				return this._reloadCommand = this._reloadCommand ?? new RelayCommand((obj) =>
				{
					this.Model.Reload();
				});
			}
		}

		/// <summary>
		/// 現在選択中のシーンを編集
		/// </summary>
		private RelayCommand _editSelectedSceneCommand;
		public RelayCommand EditSelectedSceneCommand
		{
			get
			{
				return this._editSelectedSceneCommand = this._editSelectedSceneCommand ?? new RelayCommand((obj) =>
				{
					this.Model.EditSelectedScene();
				});
			}
		}

		/// <summary>
		/// すべての人物を表示
		/// </summary>
		private RelayCommand _showAllPeopleCommand;
		public RelayCommand ShowAllPeopleCommand
		{
			get
			{
				return this._showAllPeopleCommand = this._showAllPeopleCommand ?? new RelayCommand((obj) =>
				{
					this.Model.ShowAllPeople();
				});
			}
		}

		/// <summary>
		/// すべての人物を消去
		/// </summary>
		private RelayCommand _clearAllPeopleCommand;
		public RelayCommand ClearAllPeopleCommand
		{
			get
			{
				return this._clearAllPeopleCommand = this._clearAllPeopleCommand ?? new RelayCommand((obj) =>
				{
					this.Model.ClearAllPeople();
				});
			}
		}

		/// <summary>
		/// 標準で表示する日付を探す
		/// </summary>
		private RelayCommand _chooseDefaultDisplayDayCommand;
		public RelayCommand ChooseDefaultDisplayDayCommand
		{
			get
			{
				return this._chooseDefaultDisplayDayCommand = this._chooseDefaultDisplayDayCommand ?? new RelayCommand((obj) =>
				{
					this.Model.ChooseDefaultDisplayDay();
				});
			}
		}

		/// <summary>
		/// 翌日
		/// </summary>
		private RelayCommand _nextDayCommand;
		public RelayCommand NextDayCommand
		{
			get
			{
				return this._nextDayCommand = this._nextDayCommand ?? new RelayCommand((obj) =>
				{
					this.Model.NextDay();
				});
			}
		}

		/// <summary>
		/// 前日
		/// </summary>
		private RelayCommand _prevDayCommand;
		public RelayCommand PrevDayCommand
		{
			get
			{
				return this._prevDayCommand = this._prevDayCommand ?? new RelayCommand((obj) =>
				{
					this.Model.PrevDay();
				});
			}
		}

		/// <summary>
		/// 翌月
		/// </summary>
		private RelayCommand _nextMonthCommand;
		public RelayCommand NextMonthCommand
		{
			get
			{
				return this._nextMonthCommand = this._nextMonthCommand ?? new RelayCommand((obj) =>
				{
					this.Model.NextMonth();
				});
			}
		}

		/// <summary>
		/// 前月
		/// </summary>
		private RelayCommand _prevMonthCommand;
		public RelayCommand PrevMonthCommand
		{
			get
			{
				return this._prevMonthCommand = this._prevMonthCommand ?? new RelayCommand((obj) =>
				{
					this.Model.PrevMonth();
				});
			}
		}

		/// <summary>
		/// 翌年
		/// </summary>
		private RelayCommand _nextYearCommand;
		public RelayCommand NextYearCommand
		{
			get
			{
				return this._nextYearCommand = this._nextYearCommand ?? new RelayCommand((obj) =>
				{
					this.Model.NextYear();
				});
			}
		}

		/// <summary>
		/// 前年
		/// </summary>
		private RelayCommand _prevYearCommand;
		public RelayCommand PrevYearCommand
		{
			get
			{
				return this._prevYearCommand = this._prevYearCommand ?? new RelayCommand((obj) =>
				{
					this.Model.PrevYear();
				});
			}
		}

		/// <summary>
		/// 人物一覧
		/// </summary>
		public EntityListModel<PersonEntity> People
		{
			get
			{
				return this.StoryModel.People;
			}
		}

		/// <summary>
		/// タイムラインに表示する登場人物アイテム
		/// </summary>
		public ObservableCollection<PersonItem> PersonItems
		{
			get
			{
				return this.Model.PersonItems;
			}
		}

		/// <summary>
		/// 全ての登場人物アイテム（タイムラインに表示しないもの含む）
		/// </summary>
		public Collection<PersonItem> PermanencePersonItems
		{
			get
			{
				return this.Model.PermanencePersonItems;
			}
		}

		/// <summary>
		/// タイムラインに表示するシーンアイテム
		/// </summary>
		public Dictionary<long, ObservableCollection<SceneItem>> SceneItems
		{
			get
			{
				return this.Model.SceneItems;
			}
		}

		/// <summary>
		/// メインで使う暦
		/// </summary>
		public StoryCalendar MainCalendar
		{
			get
			{
				return this.StoryModel.StoryConfig.MainCalendar;
			}
		}

		/// <summary>
		/// 現在選択中のシーンアイテム
		/// </summary>
		public SceneItem SelectedSceneItem
		{
			get
			{
				return this.Model.SelectedSceneItem;
			}
			set
			{
				this.Model.SelectedSceneItem = value;
			}
		}

		/// <summary>
		/// 表示開始日
		/// </summary>
		public StoryDateTime DisplayStartDay
		{
			get
			{
				return this.Model.DisplayStartDay;
			}
			set
			{
				this.Model.DisplayStartDay = value;
			}
		}

		/// <summary>
		/// 表示終了日
		/// </summary>
		public StoryDateTime DisplayEndDay
		{
			get
			{
				return this.Model.DisplayEndDay;
			}
			set
			{
				this.Model.DisplayEndDay = value;
			}
		}
    }
}
