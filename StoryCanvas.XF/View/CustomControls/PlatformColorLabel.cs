using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace StoryCanvas.View.CustomControls
{
	class PlatformColorLabel : Label
	{
		public PlatformColorLabel()
		{
			this.TextColor = Color.FromHex(
				Device.OS == TargetPlatform.Android ? "#33aedc" : (Device.OS == TargetPlatform.iOS ? "#000000" : "#000000")
				);
			this.LineBreakMode = LineBreakMode.TailTruncation;
		}
	}
}
