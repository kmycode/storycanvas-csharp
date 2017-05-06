using System;
using System.Collections.Generic;
using System.Text;
using StoryCanvas.iOS.Models.IO;
using StoryCanvas.Models.IO;
using Xamarin.Forms;

[assembly: Dependency(typeof(FolderPath))]
namespace StoryCanvas.iOS.Models.IO
{
	public class FolderPath : IFolderPath
	{
		public string DocumentPath
		{
			get
			{
				return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			}
		}
	}
}
