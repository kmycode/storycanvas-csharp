using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using StoryCanvas.Shared.Models.Entities;
using StoryCanvas.Shared.Models.Story;
using StoryCanvas.Shared.ViewModels.Editors;
using StoryCanvas.WPF.Converters.Common;
using static StoryCanvas.Shared.Models.Story.TimelineModel;

namespace StoryCanvas.WPF.View.SubPages
{
	/// <summary>
	/// TimelineSubPage.xaml の相互作用ロジック
	/// このページは描画を多く含むため、VMとの密結合を多く持つ
	/// </summary>
	public partial class TimelineSubPage : UserControl
	{
		private static ColorResourceColorConverter ColorConverter = new ColorResourceColorConverter();

		private TimelineViewModel ViewModel = TimelineViewModel.Default;

		private struct SceneViewItem
		{
			public Border View;
			public TimelineModel.SceneItem SceneItem;
		}
		private Collection<SceneViewItem> SceneViewItems = new Collection<SceneViewItem>();

		private const int HourHeight = 80;				// １時間の縦の高さ

		private const int TimelinePersonWidth = 160;    // １人あたりのタイムラインの表示幅
		private const int TimelineLineWidth = 5;        // タイムラインの横幅

		private const int TimelineSceneHorizontalMargin = 10;       // シーンのマージン
		private const int TimelineSceneVerticalMargin = 10;
		private const int TimelineSceneHorizontalPadding = 4;
		private const int TimelineSceneVerticalPadding = 4;

		private static readonly Color TimelineSceneDefaultColor = new Color { R = 0xde, G = 0xef, B = 0xff, A = 0xff };
		private static readonly Color TimelineSceneSelectedColor = new Color { R = 0xff, G = 0xff, B = 0x99, A = 0xff };

		private int _personCount = 0;                   // 現在表示中の登場人物の人数
		private double Scale = 1;						// 画面の拡大率

		public TimelineSubPage()
		{
			InitializeComponent();
			this.DataContext = this.ViewModel;

			// スクロールバーを連動
			this.TimelineViewport.ScrollChanged += (sender, e) =>
			{
				this.NameViewport.ScrollToHorizontalOffset(e.HorizontalOffset);
				this.TimeDivisionViewport.ScrollToVerticalOffset(e.VerticalOffset);
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
					((SolidColorBrush)vitem.View.Background).Color = TimelineSceneSelectedColor;
				}
				else
				{
					((SolidColorBrush)vitem.View.Background).Color = TimelineSceneDefaultColor;
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
				double lineY = 0;
				double lineHeight = HourHeight * this.ViewModel.MainCalendar.HourMax;
				var timeline = new Line
				{
					StrokeThickness = TimelineLineWidth,
					Stroke = new SolidColorBrush(ColorConverter.Convert(item.Person.Color)),
					X1 = lineX,
					X2 = lineX,
					Y1 = lineY * this.Scale,
					Y2 = lineHeight * this.Scale,
				};
				this.TimelineArea.Children.Add(timeline);
			}

			// 名前
			double nameX = TimelinePersonWidth * this._personCount;
			var namePlate = new Label
			{
				Margin = new Thickness(nameX, 10, 0, 0),
				Content = item.Person.Name,
				Width = TimelinePersonWidth,
				HorizontalAlignment = HorizontalAlignment.Left,
				HorizontalContentAlignment = HorizontalAlignment.Center
			};
			this.NameArea.Children.Add(namePlate);

			this._personCount++;
			this.SceneItemArea.Width = this.TimelineArea.Width = this.NameArea.Width = TimelinePersonWidth * this._personCount;
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
					Content = i + ":00",
					Margin = new Thickness(0, timeY * this.Scale, 10, 0),
					FontSize = 14,
					HorizontalContentAlignment = HorizontalAlignment.Right,
				};
				this.TimeDivisionArea.Children.Add(timeLabel);
			}
			this.SceneItemArea.Height = this.TimeDivisionArea.Height = HourHeight * this.ViewModel.MainCalendar.HourMax * this.Scale;
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
					
					y1 = (int)(y1 * this.Scale);
					y2 = (int)(y2 * this.Scale);

					var sceneView = new Border
					{
						Background = new SolidColorBrush(TimelineSceneDefaultColor),
						BorderBrush = new SolidColorBrush(ColorConverter.Convert(sceneItem.Scene.Color)),
						BorderThickness = new Thickness(2),
						Width = (x2 - x1),
						Height = (y2 - y1),
						Cursor = Cursors.Hand,
					};
					sceneView.MouseLeftButtonUp += this.SelectSceneView;
					Canvas.SetLeft(sceneView, x1);
					Canvas.SetTop(sceneView, y1);
					double sceneNameHeight = (y2 - y1) - TimelineSceneVerticalPadding * 2;
					if (sceneNameHeight < 0) sceneNameHeight = 0;
					var sceneName = new TextBlock
					{
						Text = sceneItem.Scene.Name,
						Width = (x2 - x1) - TimelineSceneHorizontalPadding * 2,
						Height = sceneNameHeight,
						TextWrapping = TextWrapping.Wrap,
						Cursor = Cursors.Hand,
					};
					sceneName.MouseLeftButtonUp += this.SelectSceneView;
					//Canvas.SetLeft(sceneName, x1 + TimelineSceneHorizontalPadding);
					//Canvas.SetTop(sceneName, y1 + TimelineSceneVerticalPadding);
					this.SceneItemArea.Children.Add(sceneView);
					sceneView.Child = sceneName;

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
		private void SelectSceneView(object sender, MouseButtonEventArgs e)
		{
			var view = sender as Border;
			if (view == null)
			{
				var textview = sender as TextBlock;
				if (textview != null)
				{
					view = textview.Parent as Border;
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
			this.Scale += 0.2;
			this.MakeDivisionArea();
			this.Reload();
			this.TimelineViewport.ScrollToVerticalOffset(this.TimelineViewport.VerticalOffset * (this.Scale / (this.Scale - 0.2)));
		}

		/// <summary>
		/// 縮小ボタンクリックした時
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ScaleDownButton_Click(object sender, EventArgs e)
		{
			this.Scale -= 0.2;
			if (this.Scale < 0.2)
			{
				this.Scale = 0.2;
			}
			this.MakeDivisionArea();
			this.Reload();
			this.TimelineViewport.ScrollToVerticalOffset(this.TimelineViewport.VerticalOffset * (this.Scale / (this.Scale + 0.2)));
		}
	}
}
