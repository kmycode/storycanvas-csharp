using StoryCanvas.Shared.ViewTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace StoryCanvas.UWP.Converters
{
    class Boolean2VisibilityConverter : ValueConverterBase<bool, Visibility>
    {
        public override Visibility Convert(bool value)
        {
            return value ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    class Boolean2NegativeVisibilityConverter : ValueConverterBase<bool, Visibility>
    {
        public override Visibility Convert(bool value)
        {
            return !value ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
