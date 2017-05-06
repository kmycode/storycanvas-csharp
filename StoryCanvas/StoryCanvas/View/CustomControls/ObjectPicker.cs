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
	public delegate void PickedEventHandler(object sender, PickedEventArgs e);
	public class PickedEventArgs : EventArgs
	{
		public object SelectedItem { get; private set; }
		public PickedEventArgs(object selectedItem)
		{
			this.SelectedItem = selectedItem;
		}
	}

	/// <summary>
	/// オブジェクトの選択肢を設定可能なピッカー
	/// </summary>
	class ObjectPicker : Picker
	{
		private IList ObjectItems;

		public ObjectPicker()
		{
			// 選択されたインデクスが変わったらアイテムも変わるようにする
			base.SelectedIndexChanged += (sender, e) =>
			{
				if (this.SelectedIndex >= 0) this.SelectedItem = this.ObjectItems[this.SelectedIndex];
				else this.SelectedItem = null;

				// Androidは、OKボタンを押した瞬間にSelectedIndexChangedが発行される
				if (Device.OS == TargetPlatform.Android)
				{
					this.Picked(this, new PickedEventArgs(this.SelectedItem));
				}
			};

			// iOSは、完了ボタンを押したらUnfocusedイベントが発行される
			// 完了ボタンを押す前に選択肢を変更しただけでもイベント発行されるのでこういうことをしている
			base.Unfocused += (sender, e) =>
			{
				if (Device.OS == TargetPlatform.iOS)
				{
					this.Picked(this, new PickedEventArgs(this.SelectedItem));
				}
			};
		}

		public event PickedEventHandler Picked = delegate { };

		/// <summary>
		/// アイテムソースのプロパティ
		/// </summary>
		public static readonly BindableProperty ItemsSourceProperty =
			BindableProperty.Create<ObjectPicker, IList>(
				p => p.ItemsSource,
				null,
				propertyChanged: (bindable, oldValue, newValue) =>
				{
					var view = bindable as ObjectPicker;
					if (view != null)
					{
						if (oldValue != null)
						{
							if (oldValue is INotifyCollectionChanged)
							{
								((INotifyCollectionChanged)oldValue).CollectionChanged -= view.UpdateItems;
							}
							view.ObjectItems = null;
						}
						if (newValue != null)
						{
							if (newValue is INotifyCollectionChanged)
							{
								((INotifyCollectionChanged)newValue).CollectionChanged += view.UpdateItems;
							}
							view.ObjectItems = newValue;
						}
						view.UpdateItems(null, null);
					}
				});

		/// <summary>
		/// アイテムソース
		/// </summary>
		public IList ItemsSource
		{
			get { return (IList)GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
		}

		/// <summary>
		/// アイテムの一覧を更新
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void UpdateItems(object sender, NotifyCollectionChangedEventArgs e)
		{
			var oldSelectedItem = this.SelectedItem;
			base.Items.Clear();
			if (this.ObjectItems != null)
			{
				foreach (var obj in this.ObjectItems)
				{
					base.Items.Add(obj.ToString());
				}
				this.SelectedItem = oldSelectedItem;
				this.SelectedItem_Event(oldSelectedItem);
			}
		}

		/// <summary>
		/// 選択されたアイテムのプロパティ
		/// </summary>
		public static readonly BindableProperty SelectedItemProperty =
			BindableProperty.Create<ObjectPicker, object>(
				p => p.SelectedItem,
				null,
				propertyChanged: (bindable, oldValue, newValue) =>
				{
					var view = bindable as ObjectPicker;
					if (view != null)
					{
						view.SelectedItem_Event(newValue);
					}
				});

		private void SelectedItem_Event(object newValue)
		{
			if (this.ObjectItems.Contains(newValue))
			{
				this.SelectedIndex = this.ObjectItems.IndexOf(newValue);
			}
			else
			{
				this.SelectedIndex = -1;
			}
		}

		/// <summary>
		/// 選択されたアイテム
		/// </summary>
		public object SelectedItem
		{
			get { return GetValue(SelectedItemProperty); }
			set { SetValue(SelectedItemProperty, value); }
		}
	}
}
