using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace StoryCanvas.View.CustomControls
{
	/// <summary>
	/// ItemsView2 風 Xamarin.Forms.View
	/// </summary>
	public class ItemsView2 : ContentView
	{
		#region ItemsPanel

		/// <summary>
		/// ItemsPanel BindableProperty
		/// </summary>
		public static readonly BindableProperty ItemsPanelProperty = BindableProperty.Create<ItemsView2, Layout<Xamarin.Forms.View>>(
			p => p.ItemsPanel,
			new StackLayout(),
			BindingMode.OneWay,
			null,
			OnItemsPanelChanged);

		/// <summary>
		/// ItemsPanel CLR プロパティ
		/// </summary>
		public Layout<Xamarin.Forms.View> ItemsPanel
		{
			get { return (Layout<Xamarin.Forms.View>)this.GetValue(ItemsPanelProperty); }
			set { this.SetValue(ItemsPanelProperty, value); }
		}

		/// <summary>
		/// ItemsPanel 変更イベントハンドラ
		/// </summary>
		/// <param name="bindable">BindableObject</param>
		/// <param name="oldValue">古い値</param>
		/// <param name="newValue">新しい値</param>
		private static void OnItemsPanelChanged(BindableObject bindable, Layout<Xamarin.Forms.View> oldValue, Layout<Xamarin.Forms.View> newValue)
		{
			var control = bindable as ItemsView2;
			if (control == null)
			{
				return;
			}

			if (oldValue != null)
			{
				oldValue.Children.Clear();
			}

			if (newValue == null)
			{
				return;
			}

			foreach (var item in control.ItemsSource)
			{
				var content = control.ItemTemplate.CreateContent();
				Xamarin.Forms.View view;
				var cell = content as ViewCell;
				if (cell != null)
				{
					view = cell.View;
				}
				else
				{
					view = (Xamarin.Forms.View)content;
				}

				view.BindingContext = item;
				control.ItemsPanel.Children.Add(view);
			}

			control.Content = newValue;
			control.UpdateChildrenLayout();
			control.InvalidateLayout();
		}

		#endregion //ItemsPanel

		#region ItemsSource

		/// <summary>
		/// ItemsSource BindableProperty
		/// </summary>
		public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create<ItemsView2, IEnumerable>(
			p => p.ItemsSource,
			new ObservableCollection<object>(),
			BindingMode.OneWay,
			null,
			OnItemsSourceChanged);

		/// <summary>
		/// ItemsSource CLR プロパティ
		/// </summary>
		public IEnumerable ItemsSource
		{
			get { return (IEnumerable)this.GetValue(ItemsSourceProperty); }
			set { this.SetValue(ItemsSourceProperty, value); }
		}

		/// <summary>
		/// ItemsSource 変更イベントハンドラ
		/// </summary>
		/// <param name="bindable">BindableObject</param>
		/// <param name="oldValue">古い値</param>
		/// <param name="newValue">新しい値</param>
		private static void OnItemsSourceChanged(BindableObject bindable, IEnumerable oldValue, IEnumerable newValue)
		{
			var control = bindable as ItemsView2;
			if (control == null)
			{
				return;
			}

			var oldCollection = oldValue as INotifyCollectionChanged;
			if (oldCollection != null)
			{
				oldCollection.CollectionChanged -= control.OnCollectionChanged;
			}

			if (newValue == null)
			{
				return;
			}

			control.ItemsPanel.Children.Clear();

			foreach (var item in newValue)
			{
				var content = control.ItemTemplate.CreateContent();
				Xamarin.Forms.View view;
				var cell = content as ViewCell;
				if (cell != null)
				{
					view = cell.View;
				}
				else
				{
					view = (Xamarin.Forms.View)content;
				}

				view.BindingContext = item;
				control.ItemsPanel.Children.Add(view);
			}

			var newCollection = newValue as INotifyCollectionChanged;
			if (newCollection != null)
			{
				newCollection.CollectionChanged += control.OnCollectionChanged;
			}

			control.UpdateChildrenLayout();
			control.InvalidateLayout();
		}

		#endregion //ItemsSource

		#region ItemTemplate

		/// <summary>
		/// ItemTemplate BindableProperty
		/// </summary>
		public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create<ItemsView2, DataTemplate>(
			p => p.ItemTemplate,
			default(DataTemplate));

		/// <summary>
		/// ItemTemplate CLR プロパティ
		/// </summary>
		public DataTemplate ItemTemplate
		{
			get { return (DataTemplate)this.GetValue(ItemTemplateProperty); }
			set { this.SetValue(ItemTemplateProperty, value); }
		}

		#endregion //ItemTemplate

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ItemsView2()
		{
			this.Content = this.ItemsPanel;
		}

		/// <summary>
		/// Items の変更イベントハンドラ
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.OldItems != null)
			{
				this.ItemsPanel.Children.RemoveAt(e.OldStartingIndex);
				this.UpdateChildrenLayout();
				this.InvalidateLayout();
			}

			var collection = this.ItemsSource as ObservableCollection<object>;
			if (e.NewItems == null || collection == null)
			{
				return;
			}
			foreach (var item in e.NewItems)
			{
				var content = this.ItemTemplate.CreateContent();

				Xamarin.Forms.View view;
				var cell = content as ViewCell;
				if (cell != null)
				{
					view = cell.View;
				}
				else
				{
					view = (Xamarin.Forms.View)content;
				}

				view.BindingContext = item;
				this.ItemsPanel.Children.Insert(collection.IndexOf(item), view);
			}

			this.UpdateChildrenLayout();
			this.InvalidateLayout();
		}
	}
}
