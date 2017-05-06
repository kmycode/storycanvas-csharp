using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoryCanvas.Shared.ViewTools.Resources;
using Xamarin.Forms;

namespace StoryCanvas.View.CustomControls
{
	class ImageResourcePanel : Image
	{

		/// <summary>
		/// アイテムソースのプロパティ
		/// </summary>
		public static readonly BindableProperty ImageResourceProperty =
			BindableProperty.Create<ImageResourcePanel, ImageResource>(
				p => p.ImageResource,
				null,
				propertyChanged: (bindable, oldValue, newValue) =>
				{
					var view = bindable as ImageResourcePanel;
					if (view != null)
					{
						// 外部からエンティティの値が設定されたら、リストでそれを選択させる
						if (view != null)
						{
							var res = newValue as ImageResource;
							if (res == null)
							{
								view.Source = null;
							}
							else
							{
								if (res.ResourceType == ImageResource.Type.ApplicationResource)
								{
									view.Source = ImageSource.FromFile(Device.OS == TargetPlatform.Android ? res.DroidPath : res.IOSPath);
								}
								else if (res.ResourceType == ImageResource.Type.UserImage)
								{
									view.Source = ImageSource.FromStream(() => res.ImageDataStream);
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
