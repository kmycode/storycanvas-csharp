using StoryCanvas.Shared.ViewTools;
using StoryCanvas.Shared.ViewTools.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace StoryCanvas.UWP.Converters
{
    class ColorResource2ColorConverter : ValueConverterBase<ColorResource, Color>
    {
        public ColorResource2ColorConverter()
        {
            this.IsCheckTargetType = false;
        }

        public override Color Convert(ColorResource value)
        {
            return new Color
            {
                R = value.R,
                G = value.G,
                B = value.B,
                A = 255,
            };
        }

        public override ColorResource ConvertBack(Color value)
        {
            return new ColorResource
            {
                R = value.R,
                G = value.G,
                B = value.B,
            };
        }
    }
}
