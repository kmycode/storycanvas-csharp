using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using StoryCanvas.Shared.ViewTools;

namespace StoryCanvas.WPF.Converters.Common
{
	public class DebugModeVisibilityConverter : ValueConverterBase<object, Visibility>
	{
		public override Visibility Convert(object value)
		{
#if DEBUG
			return Visibility.Visible;
#else
			return Visibility.Collapsed;
#endif
		}
	}
}
