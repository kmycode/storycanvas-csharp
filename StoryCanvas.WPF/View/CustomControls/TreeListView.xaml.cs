using System;
using System.Collections.Generic;
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
using StoryCanvas.Shared.ViewTools.ControlModels;
using static StoryCanvas.Shared.ViewTools.ControlModels.TreeListViewControlModel;

namespace StoryCanvas.WPF.View.CustomControls
{
	/// <summary>
	/// TreeListView.xaml の相互作用ロジック
	/// </summary>
	public partial class TreeListView : UserControl
	{
		public TreeListView()
		{
			InitializeComponent();

			// リストで選択されているアイテムとSelectedEntityを連動させる
			this.ListView.SelectionChanged += (object sender, SelectionChangedEventArgs e) =>
			{
				object selectedItem = null;
				if (e.AddedItems.Count > 0)
				{
					selectedItem = e.AddedItems[0];
				}
				this.SelectedEntity = (selectedItem as TreeEntityListItem)?.Entity;
			};
		}

		/// <summary>
		/// コントロールモデルの依存プロパティ
		/// </summary>
		public static readonly DependencyProperty ControlModelProperty =
			DependencyProperty.Register(
				"ControlModel",
				typeof(object),
				typeof(TreeListView),
				new FrameworkPropertyMetadata(
					null,
					(d, e) =>
					{
						TreeListView view = d as TreeListView;
						if (view != null)
						{
							if (e.OldValue != null)
							{
								view.ListView.ItemsSource = null;
							}
							if (e.NewValue != null)
							{
								view.ListView.ItemsSource = (e.NewValue as TreeListViewControlModel)?.Entities;
							}
						}
					})
			);

		/// <summary>
		/// コントロールモデル
		/// </summary>
		public object ControlModel
		{
			get
			{
				return GetValue(ControlModelProperty);
			}
			set
			{
				SetValue(ControlModelProperty, value);
			}
		}

		/// <summary>
		/// 選択されているエンティティの依存プロパティ
		/// </summary>
		public static readonly DependencyProperty SelectedEntityProperty =
			DependencyProperty.Register(
				"SelectedEntity",
				typeof(TreeEntity),
				typeof(TreeListView),
				new FrameworkPropertyMetadata(
					null,
					(d, e) =>
					{
						// 外部からエンティティの値が設定されたら、リストでそれを選択させる
						TreeListView view = d as TreeListView;
						if (view != null)
						{
							if (view.SelectedEntity != view.ListView.SelectedItem)
							{
								if (e.OldValue != null)
								{
								}
								if (e.NewValue != null)
								{
									foreach (TreeEntityListItem item in view.ListView.ItemsSource)
									{
										if (item.Entity == e.NewValue)
										{
											view.ListView.SelectedItem = item;
											break;
										}
									}
								}
							}
						}
					})
			);

		/// <summary>
		/// 選択されているエンティティ
		/// </summary>
		public TreeEntity SelectedEntity
		{
			get
			{
				return GetValue(SelectedEntityProperty) as TreeEntity;
			}
			set
			{
				SetValue(SelectedEntityProperty, value);
			}
		}

		/// <summary>
		/// アイコン画像の依存プロパティ
		/// </summary>
		public static readonly DependencyProperty ItemTemplateProperty =
			DependencyProperty.Register(
				"ItemTemplate",
				typeof(ControlTemplate),
				typeof(TreeListView),
				new FrameworkPropertyMetadata(
					null,
					(d, e) =>
					{
						// 外部からエンティティの値が設定されたら、リストでそれを選択させる
						TreeListView view = d as TreeListView;
						if (view != null)
						{
							view.ItemTemplate.Template = (e.NewValue as ControlTemplate).Template;
						}
					})
			);

		/// <summary>
		/// アイコン画像
		/// </summary>
		public ControlTemplate ItemTemplate
		{
			get
			{
				return GetValue(ItemTemplateProperty) as ControlTemplate;
			}
			set
			{
				SetValue(ItemTemplateProperty, value);
			}
		}
	}
}
