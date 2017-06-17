using StoryCanvas.Shared.ViewTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace StoryCanvas.UWP.Converters
{
    class EnumEquals2VisibilityConverter : ValueConverterBase<Enum, Visibility, string>
    {
        public override Visibility Convert(Enum value, string parameter)
        {
            return value.ToString() == parameter ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
