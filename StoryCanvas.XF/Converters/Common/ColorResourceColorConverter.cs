using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoryCanvas.Shared.ViewTools;
using StoryCanvas.Shared.ViewTools.Resources;
using Xamarin.Forms;

namespace StoryCanvas.Converters.Common
{
	class ColorResourceColorConverter : ValueConverterBase<ColorResource, Color>
	{
		public override Color Convert(ColorResource value)
		{
			return new Color(value.RedValue, value.GreenValue, value.BlueValue);
		}

		public override ColorResource ConvertBack(Color value)
		{
			return new ColorResource(value.R, value.G, value.B);
		}
	}
}
