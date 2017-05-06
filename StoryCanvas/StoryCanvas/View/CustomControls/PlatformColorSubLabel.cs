using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace StoryCanvas.View.CustomControls
{
	class PlatformColorSubLabel : Label
	{
		public PlatformColorSubLabel()
		{
			this.TextColor = Color.FromHex(
				Device.OS == TargetPlatform.Android ? "#a8a8a8" : "#a8a8a8");
			this.LineBreakMode = LineBreakMode.TailTruncation;
		}
	}
}
