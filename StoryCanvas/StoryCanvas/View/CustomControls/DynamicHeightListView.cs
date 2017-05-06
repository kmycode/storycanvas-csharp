using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace StoryCanvas.View.CustomControls
{
	/// <summary>
	/// 高さを自動調整するリストビュー
	/// </summary>
	class DynamicHeightListView : ListView
	{
		private double HeightValue;

		private INotifyCollectionChanged CurrentList;

		public DynamicHeightListView()
		{
			switch (Device.OS)
			{
				case TargetPlatform.iOS:
				case TargetPlatform.Android:
					this.RowHeight = 60;
					break;
				case TargetPlatform.Windows:
					this.RowHeight = 70;
					break;
			}
			
			this.ItemAppearing += (sender, e) =>
			{
				this.UpdateHeight();
			};
			this.ItemDisappearing += (sender, e) =>
			{
				this.UpdateHeight();
			};
			this.ItemSelected += (sender, e) =>
			{
				this.UpdateHeight();
			};
			this.PropertyChanged += (sender, e) =>
			{
				if (e.PropertyName == "ItemsSource")
				{
					this.UpdateHeight();

					var newList = this.ItemsSource as INotifyCollectionChanged;
					if (newList != null)
					{
						newList.CollectionChanged += this.OnCollectionChanged;
					}
					if (this.CurrentList != null)
					{
						this.CurrentList.CollectionChanged -= this.OnCollectionChanged;
					}
					this.CurrentList = newList;
				}
				else if (e.PropertyName == "RowHeight")
				{
					this.UpdateHeight();
				}
			};
			this.UpdateHeight();
		}

		private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			this.UpdateHeight();
		}

		private void UpdateHeight()
		{
			this.HeightValue = 0;
			if (this.ItemsSource != null)
			{
				int itemsCount = 0;
				foreach (var item in (IEnumerable)this.ItemsSource)
				{
					itemsCount++;
				}
				var height = this.RowHeight;
				this.HeightValue = height * itemsCount;
			}
			this.HeightRequest = this.HeightValue >= 0 ? this.HeightValue : 0;
		}
	}
}
