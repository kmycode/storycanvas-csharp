using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using StoryCanvas.Shared.ViewTools;
using StoryCanvas.Shared.ViewTools.Resources;

namespace StoryCanvas.WPF.Converters.Common
{
	class ColorResourceColorConverter : ValueConverterBase<ColorResource, Color>
	{
		public override Color Convert(ColorResource value)
		{
			return new Color { R = value.R, G = value.G, B = value.B, A = (byte)255 };
		}

		public override ColorResource ConvertBack(Color value)
		{
			return new ColorResource { R = value.R, G = value.G, B = value.B };
		}
	}

	class ColorResourceBrushConverter : ValueConverterBase<ColorResource, Brush>
	{
		public override Brush Convert(ColorResource value)
		{
			return new SolidColorBrush(new Color { R = value.R, G = value.G, B = value.B, A = (byte)255 });
		}

		public override ColorResource ConvertBack(Brush value)
		{
			var colorBrush = value as SolidColorBrush;
			if (colorBrush == null)
			{
				return ColorResource.Default;
			}
			return new ColorResource { R = colorBrush.Color.R, G = colorBrush.Color.G, B = colorBrush.Color.B };
		}
	}
}
