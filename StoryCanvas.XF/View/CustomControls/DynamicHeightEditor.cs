using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace StoryCanvas.View.CustomControls
{
	public class DynamicHeightEditor : Editor
	{
		public DynamicHeightEditor()
		{
			this.TextChanged += (sender, e) => { this.InvalidateMeasure(); };
		}
	}
}
