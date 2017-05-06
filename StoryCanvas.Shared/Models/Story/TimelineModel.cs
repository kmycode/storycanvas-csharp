using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using StoryCanvas.Shared.Common;
using StoryCanvas.Shared.Messages.Common;
using StoryCanvas.Shared.Messages.EditEntity;
using StoryCanvas.Shared.Models.Date;
using StoryCanvas.Shared.Models.Entities;
using StoryCanvas.Shared.ViewTools;

namespace StoryCanvas.Shared.Models.Story
{
    public class TimelineModel : INotifyPropertyChanged
    {
		private static TimelineModel _default;
		public static TimelineModel Default
		{
			get
			{
				if (_default == null)
				{
					_default = new TimelineModel();
				}
				return _default;
			}
		}

		private StoryModel Model = StoryModel.Current;

		/// <summary>
		/// タイムラインに表示する人物のリスト
		/// </summary>
		public ObservableCollection<PersonItem> PersonItems { get; private set; } = new ObservableCollection<PersonItem>();

		/// <summary>
		/// すべての人物と、表示する・しないをまとめたリスト
		/// </summary>
		private Collection<PersonItem> _permanencePersonItems = new ObservableCollection<PersonItem>();
		public Collection<PersonItem> PermanencePersonItems
		{
			get
			{
				return this._permanencePersonItems;
			}
			private set
			{
				this._permanencePersonItems = value;
				this.OnPropertyChanged();
			}
		}

		/// <summary>
		/// タイムラインに表示するシーンと、登場人物のIDを結びつけたリスト
		/// </summary>
		public Dictionary<long, ObservableCollection<SceneItem>> SceneItems { get; private set; } = new Dictionary<long, ObservableCollection<SceneItem>>();

		private static int k = 0;

		private TimelineModel()
		{
			// ストーリーが変更された時（読み込みなど）の処理
			StoryModel.Current.LoadStreamCompleted += () =>
			{
				this.PermanencePersonItems.Clear();     // キャッシュも含まれているのでこのさいクリアする
				this.ReloadPersonItems();
				this.ChooseDefaultDisplayDay();
			};
			this.ChooseDefaultDisplayDay();
		}

		/// <summary>
		/// 画面表示時、最初に表示する日付を考える
		/// </summary>
		public void ChooseDefaultDisplayDay()
		{
			this.DisplayStartDay = this.Model.StoryConfig.TimelineDisplayStartDateTime;
			this.DisplayEndDay = this.Model.StoryConfig.TimelineDisplayEndDateTime;
			if (this.DisplayStartDay != null && this.DisplayEndDay != null)
			{
				return;
			}

			// 表示開始日付がまだ決まっていない場合、存在するシーンから日付を選ぶ
			foreach (var scene in this.Model.Scenes)
			{
				if (scene.StartDateTime != null && scene.EndDateTime != null)
				{
					if (this.DisplayStartDay != null)
					{
						// 表示開始日には、より古い日付を選択
						if (this.DisplayStartDay > scene.StartDateTime.Date)
						{
							this.DisplayStartDay = scene.StartDateTime.Date;
						}
					}
					else
					{
						this.DisplayStartDay = scene.StartDateTime.Date;
					}
				}
			}

			// 表示終了日は、表示開始日と同じ
			this.DisplayEndDay = this.DisplayStartDay;
		}

		/// <summary>
		/// データをリロードする
		/// </summary>
		public void Reload()
		{
			this.ReloadPersonItems();
			this.CheckPersonAlives();
			this.ReloadSceneItems();
			this.TimelineChanged(this, new EventArgs());
		}

		/// <summary>
		/// 人物のリストを更新する
		/// </summary>
		private void ReloadPersonItems()
		{
			this.PersonItems.Clear();
			this.SceneItems.Clear();
			var newPermanencePersonItems = new ObservableCollection<PersonItem>();      // ついでに新しい順番に従ってソートする

			// 今回のメソッド呼び出しで新たに追加された人の数
			int newPeopleCount = 0;
#if WPF
			const int newPeopleCountMax = 30;
#else
			const int newPeopleCountMax = 10;
#endif

			foreach (var person in this.Model.People)
			{
				// 可視状態がすでに登録されているか？
				PersonItem addItem = null;
				foreach (var item in this.PermanencePersonItems)
				{
					if (item.Person.Id == person.Id)
					{
						addItem = item;
						break;
					}
				}
				addItem = addItem ?? new PersonItem
				{
					Person = person,
					IsShow = (++newPeopleCount <= newPeopleCountMax),
					LineStatus = PersonItem.LineState.ShowAll,
					LineLength = 1,
				};
				newPermanencePersonItems.Add(addItem);
				if (!addItem.IsShow)
				{
					continue;
				}

				this.PersonItems.Add(addItem);
				this.SceneItems.Add(person.Id, new ObservableCollection<SceneItem>());
			}

			// もとの人物リストと新しい人物リストの内容に差異はないか？
			bool isDifference = true;
			if (this.PermanencePersonItems.Count == this.Model.People.Count)
			{
				isDifference = false;
				for (int i = 0; i < this.PermanencePersonItems.Count; i++)
				{
					if (this.PermanencePersonItems[i].Person.Id != newPermanencePersonItems[i].Person.Id)
					{
						isDifference = true;
						break;
					}
				}
			}
			if (isDifference)
			{
				/*
				this.PermanencePersonItems.Clear();
				foreach (var item in newPermanencePersonItems)
				{
					this.PermanencePersonItems.Add(item);
				}
				*/
				this.PermanencePersonItems = newPermanencePersonItems;
			}
		}

		/// <summary>
		/// それぞれの人物が、TLの表示開始日／終了日現在生存しているかを確認する
		/// </summary>
		private void CheckPersonAlives()
		{
			if (this.DisplayStartDay == null || this.DisplayEndDay == null)
			{
				return;
			}

			foreach (var item in this.PermanencePersonItems)
			{
				var person = item.Person;
				
				if (person.BirthDay != null && person.DeathDay != null)
				{
					if (person.BirthDay.Date <= this.DisplayStartDay.Date &&
						person.DeathDay >= this.DisplayEndDay.Date)
					{
						item.LineStatus = PersonItem.LineState.ShowAll;
					}
					else
					{
						item.LineStatus = PersonItem.LineState.Hide;
					}
				}
				else if (person.BirthDay != null)
				{
					if (person.BirthDay.Date <= this.DisplayStartDay.Date)
					{
						item.LineStatus = PersonItem.LineState.ShowAll;
					}
					else
					{
						item.LineStatus = PersonItem.LineState.Hide;
					}
				}
				else if (person.DeathDay != null)
				{
					if (person.DeathDay.Date >= this.DisplayEndDay.Date)
					{
						item.LineStatus = PersonItem.LineState.ShowAll;
					}
					else
					{
						item.LineStatus = PersonItem.LineState.Hide;
					}
				}
				else
				{
					item.LineStatus = PersonItem.LineState.ShowAll;
				}
			}
		}

		/// <summary>
		/// シーンのリストを更新する
		/// </summary>
		private void ReloadSceneItems()
		{
			// 最初から作り直す
			if (this.DisplayStartDay == null)
			{
				return;
			}
			foreach (var scene in this.Model.Scenes)
			{
				this.TryAddSceneItem(scene);
			}
		}

		/// <summary>
		/// 指定されたシーンに関連付けられた登場人物のリストを取得する
		/// </summary>
		/// <param name="scene">シーン</param>
		/// <returns>関連付けられた登場人物のリスト</returns>
		private ICollection<PersonItem> GetRelatedPersonItems(SceneEntity scene)
		{
			var relatedPeople = this.Model.PersonSceneRelation.FindRelated(scene);
			var result = new Collection<PersonItem>();
			foreach (var relatedPerson in relatedPeople)
			{
				foreach (var personItem in this.PersonItems)
				{
					if (personItem.Person.Id == relatedPerson.Entity1.Id)
					{
						result.Add(personItem);
						break;
					}
				}
			}
			return result;
		}

		/// <summary>
		/// 指定されたシーンが画面表示条件にあうものならば追加する
		/// </summary>
		/// <param name="scene">シーン</param>
		/// <returns></returns>
		private bool TryAddSceneItem(SceneEntity scene)
		{
			var relatedPersonItems = this.GetRelatedPersonItems(scene);
			foreach (var personItem in relatedPersonItems)
			{
				this.TryAddSceneItem(scene, this.SceneItems[personItem.Person.Id]);
			}
			return false;
		}

		/// <summary>
		/// 指定されたシーンが画面に表示するための条件にあうものならば追加する
		/// </summary>
		/// <param name="scene">追加したいシーン</param>
		private bool TryAddSceneItem(SceneEntity scene, ICollection<SceneItem> sceneItems)
		{
			if (scene.StartDateTime != null && scene.EndDateTime != null &&
				((scene.StartDateTime.Date < scene.EndDateTime.Date) || (scene.StartDateTime.Date == scene.EndDateTime.Date && scene.StartDateTime.Time <= scene.EndDateTime.Time)))
			{
				// 表示する日付の範囲内にあるか調べる
				if (this.DisplayStartDay.Date >= scene.StartDateTime.Date && this.DisplayEndDay.Date <= scene.EndDateTime.Date)
				{
					// 開始日付が表示開始日付より前のときは、0時のところから表示を開始する
					var calendar = scene.StartDateTime.Time.Calendar;
					var dStartTime = scene.StartDateTime;
					if (scene.StartDateTime.Date < this.DisplayStartDay)
					{
						dStartTime = calendar.Time(0, 0, 0);
					}

					// 終了日付が表示終了日付より後のときも同様
					var dEndTime = scene.EndDateTime;
					if (scene.EndDateTime.Date > this.DisplayEndDay)
					{
						dEndTime = calendar.Time(calendar.HourMax, calendar.MinuteMax, calendar.SecondMax);
					}

					// 表示する時刻の範囲で、重複するシーンを列挙
					var overlappingSceneItems = new Collection<SceneItem>();
					int divisionsMax = 1;
					foreach (var item in sceneItems)
					{
						if (!(item.Scene.EndDateTime.Time <= dStartTime || dEndTime <= item.Scene.StartDateTime.Time))
						{
							overlappingSceneItems.Add(item);
							if (item.Divisions > divisionsMax)
							{
								divisionsMax = item.Divisions;
							}
						}
					}
					int divisions = overlappingSceneItems.Count > divisionsMax ? overlappingSceneItems.Count : divisionsMax;
					int numerator = divisions - 1;

					// 重複するシーンが存在した場合
					if (overlappingSceneItems.Count > 0)
					{
						divisions++;

						// 分子（分割した中で何番目の位置にシーンを表示するか）
						var isSceneDisplayedArray = new bool[divisions];
						for (int i = 0; i < overlappingSceneItems.Count; i++)
						{
							var item = overlappingSceneItems[i];
							isSceneDisplayedArray[item.Numerator] = true;

							// ついでに分割数も上書きしておく
							item.Divisions = divisions;
						}
						for (int i = 0; i < divisions; i++)
						{
							if (!isSceneDisplayedArray[i])
							{
								numerator = i;
								break;
							}
						}
					}

					sceneItems.Add(new SceneItem
					{
						Scene = scene,
						Divisions = divisions,
						Numerator = numerator,
						DisplayStartTime = dStartTime,
						DisplayEndTime = dEndTime,
					});
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// 現在選択中のシーンを編集する画面を表示
		/// </summary>
		public void EditSelectedScene()
		{
			if (this.SelectedSceneItem != null)
			{
				var message = new StartEditSceneMessage(() =>
				{
					Messenger.Default.Send(this, new EditSceneEntityNewMessage(this.SelectedSceneItem.Scene));
				});
				message.EditorClosed += (sender, e) =>
				{
					this.Reload();
				};
				Messenger.Default.Send(this, message);
			}
			else
			{
				Messenger.Default.Send(this, new AlertMessage(StringResourceResolver.Resolve("SelectSceneMessage")));
			}
		}

		/// <summary>
		/// すべての人物のタイムラインを表示
		/// </summary>
		public void ShowAllPeople()
		{
			foreach (var item in this.PermanencePersonItems)
			{
				item.IsShow = true;
			}
			this.Reload();
		}

		/// <summary>
		/// すべての人物をタイムラインから除去
		/// </summary>
		public void ClearAllPeople()
		{
			foreach (var item in this.PermanencePersonItems)
			{
				item.IsShow = false;
			}
			this.Reload();
		}

		/// <summary>
		/// 翌日
		/// </summary>
		public void NextDay()
		{
			if (this.DisplayStartDay == null)
			{
				return;
			}
			this.DisplayEndDay.Date = this.DisplayStartDay.Date += this.DisplayStartDay.Date.Calendar.Date(0, 0, 1);
			this.OnPropertyChanged("DisplayStartDay");
			this.OnPropertyChanged("DisplayEndDay");
			this.Reload();
		}

		/// <summary>
		/// 前日
		/// </summary>
		public void PrevDay()
		{
			if (this.DisplayStartDay == null)
			{
				return;
			}
			this.DisplayEndDay.Date = this.DisplayStartDay.Date -= this.DisplayStartDay.Date.Calendar.Date(0, 0, 1);
			this.OnPropertyChanged("DisplayStartDay");
			this.OnPropertyChanged("DisplayEndDay");
			this.Reload();
		}

		/// <summary>
		/// 翌月
		/// </summary>
		public void NextMonth()
		{
			if (this.DisplayStartDay == null)
			{
				return;
			}
			this.DisplayEndDay.Date = this.DisplayStartDay.Date += this.DisplayStartDay.Date.Calendar.Date(0, 1, 0);
			this.OnPropertyChanged("DisplayStartDay");
			this.OnPropertyChanged("DisplayEndDay");
			this.Reload();
		}

		/// <summary>
		/// 前月
		/// </summary>
		public void PrevMonth()
		{
			if (this.DisplayStartDay == null)
			{
				return;
			}
			this.DisplayEndDay.Date = this.DisplayStartDay.Date -= this.DisplayStartDay.Date.Calendar.Date(0, 1, 0);
			this.OnPropertyChanged("DisplayStartDay");
			this.OnPropertyChanged("DisplayEndDay");
			this.Reload();
		}

		/// <summary>
		/// 翌年
		/// </summary>
		public void NextYear()
		{
			if (this.DisplayStartDay == null)
			{
				return;
			}
			this.DisplayEndDay.Date = this.DisplayStartDay.Date += this.DisplayStartDay.Date.Calendar.Date(1, 0, 0);
			this.OnPropertyChanged("DisplayStartDay");
			this.OnPropertyChanged("DisplayEndDay");
			this.Reload();
		}

		/// <summary>
		/// 前年
		/// </summary>
		public void PrevYear()
		{
			if (this.DisplayStartDay == null)
			{
				return;
			}
			this.DisplayEndDay.Date = this.DisplayStartDay.Date -= this.DisplayStartDay.Date.Calendar.Date(1, 0, 0);
			this.OnPropertyChanged("DisplayStartDay");
			this.OnPropertyChanged("DisplayEndDay");
			this.Reload();
		}

		/// <summary>
		/// タイムラインに表示する人物１人分
		/// </summary>
		public class PersonItem : INotifyPropertyChanged
		{
			public PersonEntity Person { get; internal set; }

			// この人物を画面に表示するか？
			private bool _isShow;
			public bool IsShow
			{
				get
				{
					return this._isShow;
				}
				internal set
				{
					this._isShow = value;
					this.OnPropertyChanged();
					this.OnPropertyChanged("IsShowWithBinding");
				}
			}
			public bool IsShowWithBinding
			{
				get
				{
					return this._isShow;
				}
				set
				{
					this._isShow = value;
					TimelineModel.Default.Reload();
					//TimelineModel.Default.TimelineChanged(this, new EventArgs());
				}
			}

			// 線を表示するか？
			private LineState _lineStatus;
			public LineState LineStatus
			{
				get
				{
					return this._lineStatus;
				}
				set
				{
					this._lineStatus = value;
					this.OnPropertyChanged();
				}
			}

			public enum LineState
			{
				Hide,
				ShowAll,			// 線を最初から最後まで表示
				//ShowWhenStart,		// 開始時点では表示（途中まで表示）
				//ShowWhenEnd,		// 終了時点では表示（途中から表示）
			}

			// 線を表示する場合、表示する長さ（全体を1とする：TL全体の表示開始日・終了日が同じ日でなくても1である）
			private double _lineLength;
			public double LineLength
			{
				get
				{
					return this._lineLength;
				}
				set
				{
					this._lineLength = value;
					this.OnPropertyChanged();
				}
			}

			#region INotifyPropertyChanged

			public event PropertyChangedEventHandler PropertyChanged = delegate { };
			protected void OnPropertyChanged([CallerMemberName] string name = null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(name));
			}

			#endregion
		}

		/// <summary>
		/// タイムラインに表示するシーン１つ分
		/// </summary>
		public class SceneItem				// structは値参照になってしまう。divisionsを上書きする時に困る
		{
			public SceneEntity Scene { get; internal set; }
			public int Divisions { get; internal set; }			// 分割数
			public int Numerator { get; internal set; }			// 分割したときの分子
			public bool IsSelected { get; internal set; }
			public StoryTime DisplayStartTime { get; internal set; }
			public StoryTime DisplayEndTime { get; internal set; }
		}

		/// <summary>
		/// 現在選択中のシーンアイテム
		/// </summary>
		private SceneItem _selectedSceneItem;
		public SceneItem SelectedSceneItem
		{
			get
			{
				return this._selectedSceneItem;
			}
			set
			{
				this._selectedSceneItem = value;
				foreach (var item in this.SceneItems)
				{
					foreach (var sceneItem in item.Value)
					{
						if (this._selectedSceneItem != null && this._selectedSceneItem.Scene.Id == sceneItem.Scene.Id)
						{
							sceneItem.IsSelected = true;
						}
						else
						{
							sceneItem.IsSelected = false;
						}
					}
				}
				this.OnPropertyChanged();
			}
		}

		/// <summary>
		/// タイムラインを更新する時発行されるイベント
		/// </summary>
		public event EventHandler TimelineChanged = delegate { };

		/// <summary>
		/// 表示の開始日
		/// </summary>
		private StoryDateTime _displayStartDay;
		public StoryDateTime DisplayStartDay
		{
			get
			{
				return this._displayStartDay;
			}
			set
			{
				this._displayStartDay = this.Model.StoryConfig.TimelineDisplayStartDateTime = value;
				this.DisplayEndDay = value;				// TODO
				this.OnPropertyChanged();

				this.Reload();
			}
		}

		/// <summary>
		/// 表示を終了する日
		/// </summary>
		private StoryDateTime _displayEndDay;
		public StoryDateTime DisplayEndDay
		{
			get
			{
				return this._displayEndDay;
			}
			set
			{
				this._displayEndDay = this.Model.StoryConfig.TimelineDisplayEndDateTime = value;
				this.OnPropertyChanged();

				this.Reload();
			}
		}

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged = delegate { };
		protected void OnPropertyChanged([CallerMemberName] string name = null)
		{
			this.PropertyChanged(this, new PropertyChangedEventArgs(name));
		}

		#endregion
	}
}
