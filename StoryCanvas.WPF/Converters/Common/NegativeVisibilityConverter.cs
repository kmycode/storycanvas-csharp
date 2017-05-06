using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using StoryCanvas.Shared.ViewTools;

namespace StoryCanvas.WPF.Converters.Common
{
	class NegativeVisibilityConverter : ValueConverterBase<Visibility, Visibility>
	{
		public override Visibility Convert(Visibility value)
		{
			return value == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
		}

		public override Visibility ConvertBack(Visibility value)
		{
			return this.Convert(value);
		}
	}
}
