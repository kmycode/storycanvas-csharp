using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace StoryCanvas.View.CustomControls
{
	class PlatformColorSecondaryLabel : Label
	{
		public PlatformColorSecondaryLabel()
		{
			this.TextColor = Color.FromHex(
				Device.OS == TargetPlatform.Android ? "#a8a8a8" : "#647d98");
			if (Device.OS == TargetPlatform.iOS) this.FontSize = 12;
			this.LineBreakMode = LineBreakMode.TailTruncation;
		}
	}
}
