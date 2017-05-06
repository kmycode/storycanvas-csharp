using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoryCanvas.Shared.ViewTools.Resources;
using Xamarin.Forms;

namespace StoryCanvas.View.CustomControls
{
	class ImageResourceCell : ImageCell
	{
		/// <summary>
		/// アイテムソースのプロパティ
		/// </summary>
		public static readonly BindableProperty ImageResourceProperty =
			BindableProperty.Create<ImageResourceCell, ImageResource>(
				p => p.ImageResource,
				null,
				propertyChanged: (bindable, oldValue, newValue) =>
				{
					var view = bindable as ImageResourceCell;
					if (view != null)
					{
						// 外部からエンティティの値が設定されたら、リストでそれを選択させる
						if (view != null)
						{
							var res = newValue as ImageResource;
							if (res == null)
							{
								view.ImageSource = null;
							}
							else
							{
								if (res.ResourceType == ImageResource.Type.ApplicationResource)
								{
									view.ImageSource = ImageSource.FromFile(Device.OS == TargetPlatform.Android ? res.DroidPath : res.IOSPath);
								}
							}
						}
					}
				});

		/// <summary>
		/// アイテムソース
		/// </summary>
		public ImageResource ImageResource
		{
			get { return (ImageResource)GetValue(ImageResourceProperty); }
			set { SetValue(ImageResourceProperty, value); }
		}
	}
}
