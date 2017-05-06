using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCLStorage;
using Plugin.Media;
using Plugin.Media.Abstractions;
using StoryCanvas.Models.Image;
using StoryCanvas.Shared.Utils;
using StoryCanvas.Shared.ViewTools.Resources;
using Xamarin.Forms;

namespace StoryCanvas.View.CustomControls.Pickers
{
	public partial class ImagePicker : ContentView
	{
		private enum SelectionResult
		{
			Cancel,
			TakePicture,			// 写真を撮る
			SelectPicture,			// 写真を選択
			DeletePicture,			// 削除
		}

		public ImagePicker()
		{
			CrossMedia.Current.Initialize();
			InitializeComponent();

			this.EditButton.Clicked += (sender, e) => this.EditPicture();
		}

		private async void EditPicture()
		{
			if (!CrossMedia.Current.IsPickPhotoSupported || !CrossMedia.Current.IsTakePhotoSupported)
			{
				await this.GetPage().DisplayAlert(AppResources.ApplicationName, AppResources.CannotAccessCamera, AppResources.Cancel);
				return;
			}

			IFile file = null;

			try
			{
				// 選択肢を出す
				var r = await this.GetPage().DisplayActionSheet(AppResources.Image, AppResources.Cancel, AppResources.DeletePicture, AppResources.TakePicture, AppResources.SelectPicture);
				var media = await this.GetPictureFile(this.ShowSelection(r));
				if (media == null)
				{
					return;
				}
				file = await FileSystem.Current.GetFileFromPathAsync(media.Path);
				if (file == null)
				{
					file = await FileSystem.Current.GetFileFromPathAsync(media.AlbumPath);
					if (file == null)
					{
						return;
					}
				}
			}
			catch
			{
				await this.GetPage().DisplayAlert(AppResources.ApplicationName, AppResources.CannotAccessCamera, AppResources.Cancel);
				return;
			}

			byte[] binary = null;
			byte[] resizedBinary = null;

			try
			{
				// 画像バイナリ取得
				using (var stream = await file.OpenAsync(FileAccess.Read))
				{
					binary = this.ReadFully(stream);
				}

				// 画像縮小処理
				var resizer = DependencyService.Get<IImageResizer>();
				var size = resizer.GetSize(file.Path);
				var newSize = ImageUtil.ResizeImage(size.Width, size.Height);
				resizedBinary = await resizer.Resize(binary, (int)newSize.Item1, (int)newSize.Item2);
			}
			catch
			{
				await this.GetPage().DisplayAlert(AppResources.ApplicationName, AppResources.ImageLoadFailed, AppResources.Cancel);
				return;
			}

			try
			{
				// 画像を保存
				this.ImageResource = new ImageResource
				{
					ResourceType = ImageResource.Type.UserImage,
					ImageData = resizedBinary,
				};

				// ファイルを消す
				await file.DeleteAsync();
			}
			catch
			{
				await this.GetPage().DisplayAlert(AppResources.ApplicationName, AppResources.ImageSaveFailed, AppResources.Cancel);
			}
		}

		private byte[] ReadFully(Stream input)
		{
			byte[] buffer = new byte[16 * 1024];
			using (MemoryStream ms = new MemoryStream())
			{
				int read;
				while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
				{
					ms.Write(buffer, 0, read);
				}
				return ms.ToArray();
			}
		}

		private async Task<MediaFile> GetPictureFile(SelectionResult selection)
		{
			MediaFile media = null;
			switch (selection)
			{
				case SelectionResult.TakePicture:
					media = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
					{
						SaveToAlbum = true,
					});
					break;
				case SelectionResult.SelectPicture:
					media = await CrossMedia.Current.PickPhotoAsync();
					break;
				case SelectionResult.DeletePicture:
					this.ImageResource = null;
					break;
				default:
					return null;
			}

			return media;
		}

		private SelectionResult ShowSelection(string r)
		{
			SelectionResult result = SelectionResult.Cancel;
			if (r == AppResources.TakePicture)
			{
				result = SelectionResult.TakePicture;
			}
			else if (r == AppResources.SelectPicture)
			{
				result = SelectionResult.SelectPicture;
			}
			else if (r == AppResources.DeletePicture)
			{
				result = SelectionResult.DeletePicture;
			}
				
			return result;
		}

		private Page GetPage()
		{
			Page page = null;
			Element view = this;
			while (view.Parent != null)
			{
				if (view.Parent is App)
				{
					page = (Page)view;
					break;
				}
				view = view.Parent;
			}
			return page;
		}

		/// <summary>
		/// 画像のプロパティ
		/// </summary>
		public static readonly BindableProperty ImageResourceProperty =
			BindableProperty.Create<ImagePicker, ImageResource>(
				p => p.ImageResource,
				null,
				defaultBindingMode: BindingMode.TwoWay,
				propertyChanged: (bindable, oldValue, newValue) =>
				{
					var view = bindable as ImagePicker;
					if (view != null)
					{
						var res = newValue as ImageResource;
						view.Image.ImageResource = res;
					}
				});

		/// <summary>
		/// 画像
		/// </summary>
		public ImageResource ImageResource
		{
			get { return (ImageResource)GetValue(ImageResourceProperty); }
			set { SetValue(ImageResourceProperty, value); }
		}
	}
}
