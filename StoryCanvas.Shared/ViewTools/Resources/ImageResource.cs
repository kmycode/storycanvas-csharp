using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.IO;

namespace StoryCanvas.Shared.ViewTools.Resources
{
	[DataContract]
	public class ImageResource
	{
		public bool IsEmpty
		{
			get
			{
				if (this.ResourceType == Type.ApplicationResource)
				{
#if WPF
					return string.IsNullOrEmpty(this.WPFPath);
#elif WINDOWS_UWP
					return string.IsNullOrEmpty(this.UWPPath);
#elif XAMARIN_FORMS
					return Xamarin.Forms.Device.OS == Xamarin.Forms.TargetPlatform.iOS ? string.IsNullOrEmpty(this.IOSPath) : string.IsNullOrEmpty(this.DroidPath);
#endif
				}
				else
				{
					return this.ImageData == null;
				}
			}
		}

		// アプリに埋め込まれている画像のときのみ有効
		[DataMember]
		public string WPFPath { get; set; }
		[DataMember]
		public string UWPPath { get; set; }
		[DataMember]
		public string IOSPath { get; set; }
		[DataMember]
		public string DroidPath { get; set; }

		// ユーザが定義した画像のときのみ有効
		[DataMember]
		public byte[] ImageData { get; set; } = null;
		public Stream ImageDataStream
		{
			get
			{
				return new MemoryStream(this.ImageData ?? new byte[] { });
			}
		}

        // 両方
        public Stream ImageStream
        {
            get
            {
                Stream fs = null;
#if WINDOWS_UWP
                Task.Run(async () =>
                {
                    var file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri(this.UWPPath, UriKind.Absolute));
                    fs = await file.OpenStreamForReadAsync();
                }).Wait();
#else
                throw new NotImplementedException();
#endif
                return fs;
            }
        }

		// 画像が保存されている場所の種類
		[DataMember]
		public Type ResourceType { get; set; } = Type.ApplicationResource;

		public enum Type
		{
			ApplicationResource,			// アプリに埋め込まれている画像
			UserImage,						// ユーザが登録した画像
		}
	}
}
