using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCLThinCanvas.Views;
using StoryCanvas.Converters.Common;
using StoryCanvas.Shared.Models.Entities;
using StoryCanvas.Shared.Models.Story;
using StoryCanvas.Shared.ViewModels.Editors;
using Xamarin.Forms;
using static StoryCanvas.Shared.Models.Story.TimelineModel;

namespace StoryCanvas.View.Pages
{
	public partial class TimelinePage : MasterDetailPage
	{
		private static ColorResourceColorConverter ColorConverter = new ColorResourceColorConverter();

		private TimelineViewModel ViewModel = TimelineViewModel.Default;

		private struct SceneViewItem
		{
			public SquareView View;
			public TimelineModel.SceneItem SceneItem;
		}
		private Collection<SceneViewItem> SceneViewItems = new Collection<SceneViewItem>();

		private const int HourHeight = 80;              // １時間の縦の高さ

		private const int TimelinePersonWidth = 140;    // １人あたりのタイムラインの表示幅
		private const int TimelineLineWidth = 5;        // タイムラインの横幅

		private const int TimelineSceneHorizontalMargin = 10;       // シーンのマージン
		private const int TimelineSceneVerticalMargin = 10;
		private const int TimelineSceneHorizontalPadding = 4;
		private const int TimelineSceneVerticalPadding = 4;

		private static readonly Color TimelineSceneDefaultColor = new Color((double)0xde / 0xff, (double)0xef / 0xff, (double)0xff / 0xff);
		private static readonly Color TimelineSceneSelectedColor = new Color((double)0xff / 0xff, (double)0xff / 0xff, (double)0x99 / 0xff);

		private int _personCount = 0;                   // 現在表示中の登場人物の人数
		private double ViewScale = 1;                       // 画面の拡大率

		private double timelineScrollX = 0;
		private double timelineScrollY = 0;

		public TimelinePage()
		{
			InitializeComponent();
			this.BindingContext = this.ViewModel;

			// スクロールバーを連動
			this.TimelineViewport.Scrolled += (sender, e) =>
			{
				// 少なくともAndroidでは、縦横両方にスクロールできるScrollViewを
				// 例えば縦だけにスクロールすると、ScrollYには正しい値が入るが、ScrollXは必ずゼロになる
				// イベントオブジェクトだけでなく、ScrollViewのプロパティでも同様
				// （Xamarin.Formsのバグ？）
				double scrollX = e.ScrollX;
				if (e.ScrollX > 0)
				{
					this.timelineScrollX = e.ScrollX;
				}
				else
				{
					scrollX = this.timelineScrollX;
				}

				double scrollY = e.ScrollY;
				if (e.ScrollY > 0)
				{
					this.timelineScrollY = e.ScrollY;
				}
				else
				{
					scrollY = this.timelineScrollY;
				}

				this.NameViewport.ScrollToAsync(scrollX, 0, false);
				this.TimeDivisionViewport.ScrollToAsync(0, scrollY, false);
			};

			StoryEditorModel.Default.MainModeChanged += (oldMode, newMode) =>
			{
				if (newMode == Shared.Types.MainMode.TimelinePage)
				{
					this.ViewModel.ChooseDefaultDisplayDayCommand.Execute(null);
					this.Reload();
				}
			};
			this.ViewModel.TimelineChanged += (sender, e) =>
			{
				this.Reload();
			};

			this.ViewModel.PropertyChanged += (sender, e) =>
			{
				if (e.PropertyName == "SelectedSceneItem")
				{
					this.ReloadSelectedSceneItem();
				}
			};

			this.MakeDivisionArea();
			this.Reload();
		}

		/// <summary>
		/// 画面をリロード
		/// </summary>
		private void Reload()
		{
			// this.ViewModel.ReloadCommand.Execute(null);

			// 登場人物を登録
			this.TimelineArea.Children.Clear();
			this.NameArea.Children.Clear();
			this._personCount = 0;
			foreach (var item in this.ViewModel.PersonItems)
			{
				if (item.IsShow)
				{
					this.AddPerson(item);
				}
			}

			// シーンを登録
			this.ReloadSceneItems();

			// シーンの選択状態を更新
			this.ReloadSelectedSceneItem();
		}

		/// <summary>
		/// 選択されているシーンの描画を更新
		/// </summary>
		private void ReloadSelectedSceneItem()
		{
			foreach (var vitem in this.SceneViewItems)
			{
				if (vitem.SceneItem.IsSelected)
				{
					vitem.View.FillColor = TimelineSceneSelectedColor;
				}
				else
				{
					vitem.View.FillColor = TimelineSceneDefaultColor;
				}
			}
		}

		/// <summary>
		/// 登場人物を画面に追加
		/// </summary>
		/// <param name="person">登場人物</param>
		private void AddPerson(PersonItem item)
		{
			// ライン
			if (item.LineStatus == PersonItem.LineState.ShowAll)
			{
				double lineX = TimelinePersonWidth * (this._personCount + 0.5) - TimelineLineWidth / 2.0;
				double lineHeight = HourHeight * this.ViewModel.MainCalendar.HourMax;

				var timeline = new LineView
				{
					LineWidth = TimelineLineWidth,
					LineColor = ColorConverter.Convert(item.Person.Color),
				};
				AbsoluteLayout.SetLayoutFlags(timeline, AbsoluteLayoutFlags.None);
				AbsoluteLayout.SetLayoutBounds(timeline, new Rectangle(lineX,
																		0,
																		TimelineLineWidth,
																		lineHeight * this.ViewScale));
				this.TimelineArea.Children.Add(timeline);
			}

			// 名前
			double nameX = TimelinePersonWidth * this._personCount;
			var namePlate = new Label
			{
				Margin = new Thickness(0, 10, 0, 0),
				Text = item.Person.Name,
				WidthRequest = TimelinePersonWidth,
				HorizontalTextAlignment = TextAlignment.Center,
			};
			AbsoluteLayout.SetLayoutFlags(namePlate, AbsoluteLayoutFlags.None);
			AbsoluteLayout.SetLayoutBounds(namePlate, new Rectangle(nameX,
																	10,
																	TimelinePersonWidth,
																	this.NameArea.Height));
			this.NameArea.Children.Add(namePlate);

			this._personCount++;
			this.SceneItemArea.WidthRequest = this.TimelineArea.WidthRequest = this.NameArea.WidthRequest = TimelinePersonWidth * this._personCount;
		}

		/// <summary>
		/// 時刻を表示するエリアを作成
		/// </summary>
		private void MakeDivisionArea()
		{
			this.TimeDivisionArea.Children.Clear();
			for (var i = 0; i < this.ViewModel.MainCalendar.HourMax; i++)
			{
				double timeY = HourHeight * i;
				var timeLabel = new Label
				{
					Text = i + ":00",
					Margin = new Thickness(0, 0, 10, 0),
					FontSize = 14,
					HorizontalTextAlignment = TextAlignment.End,
					VerticalTextAlignment = TextAlignment.Start,
				};
				AbsoluteLayout.SetLayoutFlags(timeLabel, AbsoluteLayoutFlags.None);
				AbsoluteLayout.SetLayoutBounds(timeLabel, new Rectangle(0,
																		timeY * this.ViewScale,
																		this.TimeDivisionArea.Width,
																		HourHeight));
				this.TimeDivisionArea.Children.Add(timeLabel);
			}
			this.SceneItemArea.HeightRequest = this.TimelineArea.HeightRequest = this.TimeDivisionArea.HeightRequest = HourHeight * this.ViewModel.MainCalendar.HourMax * this.ViewScale;
		}

		/// <summary>
		/// シーンアイテムを更新
		/// </summary>
		private void ReloadSceneItems()
		{
			this.SceneItemArea.Children.Clear();
			this.SceneViewItems.Clear();

			int personCount = 0;

			// Dictionary
			foreach (var item in this.ViewModel.SceneItems)
			{
				int x1Base = personCount * TimelinePersonWidth + TimelineSceneHorizontalMargin;
				int x2Base = x1Base + TimelinePersonWidth - TimelineSceneHorizontalMargin * 2;
				int widthBase = x2Base - x1Base;

				// Collection (SceneItems)
				foreach (var sceneItem in item.Value)
				{
					var calendar = sceneItem.Scene.EndDateTime.Time.Calendar;

					int x1 = (widthBase / sceneItem.Divisions) * sceneItem.Numerator + x1Base;
					int x2 = (widthBase / sceneItem.Divisions) * (sceneItem.Numerator + 1) + x1Base;
					var startTime = sceneItem.DisplayStartTime;
					int y1 = startTime.Hour * HourHeight + (int)((startTime.Minute / (double)startTime.Calendar.MinuteMax) * HourHeight);
					var endTime = sceneItem.DisplayEndTime;
					int y2 = endTime.Hour * HourHeight + (int)((endTime.Minute / (double)endTime.Calendar.MinuteMax) * HourHeight);

					y1 = (int)(y1 * this.ViewScale);
					y2 = (int)(y2 * this.ViewScale);

					var sceneView = new SquareView
					{
						FillColor = TimelineSceneDefaultColor,
						LineColor = ColorConverter.Convert(sceneItem.Scene.Color),
						LineWidth = 2,
						WidthRequest = (x2 - x1),
						HeightRequest = (y2 - y1),
					};
					var sceneViewTap = new TapGestureRecognizer();
					sceneViewTap.Tapped += (sender, e) => this.SelectSceneView(sender);
					sceneView.GestureRecognizers.Add(sceneViewTap);
					AbsoluteLayout.SetLayoutFlags(sceneView, AbsoluteLayoutFlags.None);
					AbsoluteLayout.SetLayoutBounds(sceneView, new Rectangle(x1, y1, (x2 - x1), (y2 - y1)));
					var sceneName = new Label
					{
						Text = sceneItem.Scene.Name,
						TextColor = Color.Black,
						WidthRequest = (x2 - x1) - TimelineSceneHorizontalPadding * 2,
						HeightRequest = (y2 - y1) - TimelineSceneVerticalPadding * 2,
					};
					var sceneNameTap = new TapGestureRecognizer();
					sceneNameTap.Tapped += (sender, e) => this.SelectSceneView(sender);
					sceneName.GestureRecognizers.Add(sceneNameTap);
					AbsoluteLayout.SetLayoutFlags(sceneName, AbsoluteLayoutFlags.None);
					AbsoluteLayout.SetLayoutBounds(sceneName, new Rectangle(x1, y1, (x2 - x1) - TimelineSceneHorizontalPadding * 2, (y2 - y1) - TimelineSceneHorizontalPadding * 2));

					// ビューを子の配列に格納
					this.SceneItemArea.Children.Add(sceneView);
					this.SceneItemArea.Children.Add(sceneName);

					this.SceneViewItems.Add(new SceneViewItem { View = sceneView, SceneItem = sceneItem });
				}

				personCount++;
			}
		}

		/// <summary>
		/// シーンを選択したときに送られるイベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SelectSceneView(object sender)
		{
			var view = sender as SquareView;
			if (view == null)
			{
				var textview = sender as Label;
				if (textview != null)
				{
					// SceneItemArea.Children配列の順番は、ReloadSceneItemsメソッド呼び出し時に決まっている
					// （奇数）SquareView、（偶数）Label
					int textviewIndex = this.SceneItemArea.Children.IndexOf(textview);
					if (textviewIndex > 0)
					{
						view = this.SceneItemArea.Children[textviewIndex - 1] as SquareView;
					}
				}
			}

			// ビューとシーンアイテムを関連付けたディクショナリを走査して、どのシーンが選択されたかを特定する
			if (view != null)
			{
				foreach (var vitem in this.SceneViewItems)
				{
					if (view == vitem.View)
					{
						this.ViewModel.SelectedSceneItem = vitem.SceneItem;
						break;
					}
				}
			}
		}

		/// <summary>
		/// 拡大ボタンクリックした時
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ScaleUpButton_Click(object sender, EventArgs e)
		{
			this.ViewScale += 0.5;
			this.MakeDivisionArea();
			this.Reload();

			double correction = (this.ViewScale / (this.ViewScale - 0.5));
			this.TimelineViewport.ScrollToAsync(this.TimelineViewport.ScrollX,
												this.TimeDivisionViewport.ScrollY * correction, false);
		}

		/// <summary>
		/// 縮小ボタンクリックした時
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ScaleDownButton_Click(object sender, EventArgs e)
		{
			this.ViewScale -= 0.5;
			if (this.ViewScale < 0.5)
			{
				this.ViewScale = 0.5;
			}
			this.MakeDivisionArea();
			this.Reload();

			double correction = (this.ViewScale / (this.ViewScale + 0.5));
			this.TimelineViewport.ScrollToAsync(this.TimelineViewport.ScrollX,
												this.TimeDivisionViewport.ScrollY * correction, false);
		}

		private void ReorderButton_Click(object sender, EventArgs e)
		{
			this.DateOperationButtons.IsVisible ^= true;
		}

		private void ShowMenuButton_Click(object sender, EventArgs e)
		{
			this.IsPresented ^= true;
		}
	}
}
