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
using StoryCanvas.Shared.ViewTools.Resources;

namespace StoryCanvas.WPF.View.CustomControls
{
	/// <summary>
	/// ImageResourcePanel.xaml の相互作用ロジック
	/// </summary>
	public partial class ImageResourcePanel : Image
	{
		public ImageResourcePanel()
		{
			InitializeComponent();
		}

		/// <summary>
		/// アイコン画像の依存プロパティ
		/// </summary>
		public static readonly DependencyProperty ImageResourceProperty =
			DependencyProperty.Register(
				"ImageResource",
				typeof(ImageResource),
				typeof(ImageResourcePanel),
				new FrameworkPropertyMetadata(
					null,
					(d, e) =>
					{
						// 外部からエンティティの値が設定されたら、リストでそれを選択させる
						ImageResourcePanel view = d as ImageResourcePanel;
						if (view != null)
						{
							var res = e.NewValue as ImageResource;
							view.UpdateSource(res);
						}
					})
			);

		/// <summary>
		/// アイコン画像
		/// </summary>
		public ImageResource ImageResource
		{
			get
			{
				return GetValue(ImageResourceProperty) as ImageResource;
			}
			set
			{
				SetValue(ImageResourceProperty, value);
			}
		}

		/// <summary>
		/// ImageResourceの表示を更新する
		/// </summary>
		public void UpdateSource()
		{
			this.UpdateSource(this.ImageResource);
		}

		/// <summary>
		/// ImageResourceを更新する
		/// </summary>
		private void UpdateSource(ImageResource res)
		{
			if (res == null || res.IsEmpty)
			{
				this.Source = null;
				return;
			}

			if (res.ResourceType == ImageResource.Type.ApplicationResource)
			{
				BitmapImage logo = new BitmapImage();
				logo.BeginInit();
				logo.UriSource = new Uri(res.WPFPath);
				logo.EndInit();
				this.Source = logo;
			}
			else if (res.ResourceType == ImageResource.Type.UserImage)
			{
				try
				{
					BitmapImage logo = new BitmapImage();
					logo.BeginInit();
					logo.StreamSource = res.ImageDataStream;
					logo.EndInit();
					this.Source = logo;
				}
				catch
				{
					this.Source = null;
				}
			}
		}
	}
}
