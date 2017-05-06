using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace StoryCanvas.Models.Image
{
	public interface IImageResizer
	{
		Size GetSize(string imageFileName);

		Task<byte[]> Resize(byte[] data, float width, float height);
	}
}
