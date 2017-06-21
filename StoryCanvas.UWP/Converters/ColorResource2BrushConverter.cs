using StoryCanvas.Shared.ViewTools;
using StoryCanvas.Shared.ViewTools.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace StoryCanvas.UWP.Converters
{
    class ColorResource2BrushConverter : ValueConverterBase<ColorResource, Brush>
    {
        public override Brush Convert(ColorResource value)
        {
            return new SolidColorBrush(new Color
            {
                R = value.R,
                G = value.G,
                B = value.B,
                A = 255,
            });
        }

        public override ColorResource ConvertBack(Brush value)
        {
            if (value is SolidColorBrush b)
            {
                return new ColorResource
                {
                    R = b.Color.R,
                    G = b.Color.G,
                    B = b.Color.B,
                };
            }
            return new ColorResource();
        }
    }
}
