using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using StoryCanvas.Droid.Model.Image;
using StoryCanvas.Models.Image;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(ImageResizer))]
namespace StoryCanvas.Droid.Model.Image
{
	class ImageResizer : IImageResizer
	{
		public Size GetSize(string imageFileName)
		{
			var options = new BitmapFactory.Options
			{
				InJustDecodeBounds = true
			};
			/*
			imageFileName = imageFileName.Replace('-', '_').Replace(".png", "");
			var resId = Forms.Context.Resources.GetIdentifier(
				imageFileName, "drawable", Forms.Context.PackageName);
			BitmapFactory.DecodeResource(
				Forms.Context.Resources, resId, options);
				*/
			BitmapFactory.DecodeFile(imageFileName, options);
			return new Size((double)options.OutWidth, (double)options.OutHeight);
		}

		public async Task<byte[]> Resize(byte[] data, float width, float height)
		{
			// Load the bitmap
			Bitmap originalImage = BitmapFactory.DecodeByteArray(data, 0, data.Length);
			Bitmap resizedImage = Bitmap.CreateScaledBitmap(originalImage, (int)width, (int)height, false);

			using (MemoryStream ms = new MemoryStream())
			{
				resizedImage.Compress(Bitmap.CompressFormat.Jpeg, 100, ms);
				return ms.ToArray();
			}
		}
	}
}