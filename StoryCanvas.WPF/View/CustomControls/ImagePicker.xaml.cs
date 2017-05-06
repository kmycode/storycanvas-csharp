using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
using Microsoft.Win32;
using StoryCanvas.Shared.Messages.Picker;
using StoryCanvas.Shared.Utils;
using StoryCanvas.Shared.ViewTools;
using StoryCanvas.Shared.ViewTools.Resources;

namespace StoryCanvas.WPF.View.CustomControls
{
	/// <summary>
	/// ImagePicker.xaml の相互作用ロジック
	/// </summary>
	public partial class ImagePicker : UserControl
	{
		public ImagePicker()
		{
			InitializeComponent();
			this.SelectedImageDisplay.ImageResource = this.ImageResource;

			// 画像を選択
			this.ImagePickButton.Click += (sender, e) =>
			{
				var message = new OpenImageFilePickerMessage();
				Messenger.Default.Send(null, message);

				if (message.IsSelected)
				{
					// 画像縮小
					var image = new Bitmap(message.FileName);
					var resizedImage = this.ResizeBitmap(image);

					// バイナリをデータに設定
					var source = this.GetBitmapSource(resizedImage);
					if (source != null)
					{
						this.ImageResource = this.GetImageResource(source);
					}
				}
			};

			// 画像を貼り付け
			this.ImagePasteButton.Click += (sender, e) =>
			{
				if (Clipboard.ContainsImage())
				{
					//クリップボードにあるデータの取得
					var img = Clipboard.GetImage();
					if (img != null)
					{
						// 画像縮小
						var image = this.GetBitmap(img);
						var resizedImage = this.ResizeBitmap(image);

						var source = this.GetBitmapSource(resizedImage);
						if (source != null)
						{
							this.ImageResource = this.GetImageResource(source);
						}
					}
				}
			};

			// 画像を削除
			this.ImageDeleteButton.Click += (sender, e) =>
			{
				this.ImageResource = null;
			};
		}

		/// <summary>
		/// 画像の依存プロパティ
		/// </summary>
		public static readonly DependencyProperty ImageResourceProperty =
			DependencyProperty.Register(
				"ImageResource",
				typeof(ImageResource),
				typeof(ImagePicker),
				new FrameworkPropertyMetadata(
					new ImageResource { ResourceType = StoryCanvas.Shared.ViewTools.Resources.ImageResource.Type.UserImage, },
					FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
					(d, e) =>
					{
						var view = d as ImagePicker;
						var newValue = (ImageResource)e.NewValue;
						if (view != null)
						{
							// 画像の表示を変更する
							view.SelectedImageDisplay.ImageResource = newValue;
						}
						else
						{
							view.SelectedImageDisplay.ImageResource = null;
						}
					},
					(s, obj) =>
					{
						return obj;
					})
			);

		/// <summary>
		/// 画像
		/// </summary>
		public ImageResource ImageResource
		{
			get
			{
				return (ImageResource)GetValue(ImageResourceProperty);
			}
			set
			{
				SetValue(ImageResourceProperty, value);
			}
		}

		private BitmapSource GetBitmapSource(Bitmap bitmap)
		{
			var bitmapData = bitmap.LockBits(
				new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
				System.Drawing.Imaging.ImageLockMode.ReadOnly, bitmap.PixelFormat);

			PixelFormat format;
			switch (bitmap.PixelFormat)
			{
				case System.Drawing.Imaging.PixelFormat.Format24bppRgb:
					format = PixelFormats.Bgr24;
					break;
				case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
					format = PixelFormats.Bgra32;
					break;
				case System.Drawing.Imaging.PixelFormat.Format32bppRgb:
					format = PixelFormats.Bgr32;
					break;
				default:
					MessageBox.Show(Properties.Resources.NoSupportedPixelFormatMessage);
					return null;
			}

			var bitmapSource = BitmapSource.Create(
				bitmapData.Width, bitmapData.Height, 96, 96, format, null,
				bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, bitmapData.Stride);

			bitmap.UnlockBits(bitmapData);
			return bitmapSource;
		}

		private Bitmap GetBitmap(BitmapSource bitmapsource)
		{
			Bitmap bitmap;
			using (var outStream = new MemoryStream())
			{
				BitmapEncoder enc = new BmpBitmapEncoder();
				enc.Frames.Add(BitmapFrame.Create(bitmapsource));
				enc.Save(outStream);
				bitmap = new Bitmap(outStream);
			}
			return bitmap;
		}

		private ImageResource GetImageResource(BitmapSource source)
		{
			ImageResource ir = null;

			using (MemoryStream fs = new MemoryStream())
			{
				var enc = new JpegBitmapEncoder();
				enc.Frames.Add(BitmapFrame.Create(source));
				enc.Save(fs);

				ir = new ImageResource
				{
					ImageData = fs.ToArray(),
					ResourceType = ImageResource.Type.UserImage,
				};
			}

			return ir;
		}

		private Bitmap ResizeBitmap(Bitmap image)
		{
			var newImageSize = ImageUtil.ResizeImage(image.Width, image.Height);
			Bitmap resizedImage = new Bitmap((int)newImageSize.Item1, (int)newImageSize.Item2, image.PixelFormat);
			using (Graphics g = Graphics.FromImage(resizedImage))
			{
				g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
				g.DrawImage(image, 0, 0, (int)newImageSize.Item1, (int)newImageSize.Item2);
			}

			return resizedImage;
		}
	}
}
