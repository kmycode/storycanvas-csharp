using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreGraphics;
using Foundation;
using StoryCanvas.iOS.Model.Image;
using StoryCanvas.Models.Image;
using UIKit;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(ImageResizer))]
namespace StoryCanvas.iOS.Model.Image
{
	class ImageResizer : IImageResizer
	{
		public Xamarin.Forms.Size GetSize(string imageFileName)
		{
			UIImage image = UIImage.FromFile(imageFileName);
			double w = image.Size.Width;
			double h = image.Size.Height;
			if (image.Orientation == UIImageOrientation.Left || image.Orientation == UIImageOrientation.LeftMirrored ||
				image.Orientation == UIImageOrientation.Right || image.Orientation == UIImageOrientation.RightMirrored)
			{
				double tmp = w;
				w = h;
				h = tmp;
			}
			return new Size(w, h);
		}

		public async Task<byte[]> Resize(byte[] data, float width, float height)
		{
			UIImage originalImage = ImageFromByteArray(data);
			UIImageOrientation orientation = originalImage.Orientation;

			//create a 24bit RGB image
			using (CGBitmapContext context = new CGBitmapContext(IntPtr.Zero,
												 (int)width, (int)height, 8,
												 (int)(4 * width), CGColorSpace.CreateDeviceRGB(),
												 CGImageAlphaInfo.PremultipliedFirst))
			{

				CGRect imageRect = new CGRect(0, 0, width, height);

				// draw the image
				context.DrawImage(imageRect, originalImage.CGImage);

				UIKit.UIImage resizedImage = UIKit.UIImage.FromImage(context.ToImage(), 0, orientation);

				// save the image as a jpeg
				return resizedImage.AsJPEG().ToArray();
			}
		}

		private static UIKit.UIImage ImageFromByteArray(byte[] data)
		{
			if (data == null)
			{
				return null;
			}

			UIKit.UIImage image;
			try
			{
				image = new UIKit.UIImage(Foundation.NSData.FromArray(data));
			}
			catch (Exception e)
			{
				Console.WriteLine("Image load failed: " + e.Message);
				return null;
			}
			return image;
		}
	}
}