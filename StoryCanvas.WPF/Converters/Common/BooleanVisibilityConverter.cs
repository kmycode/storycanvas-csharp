using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using StoryCanvas.Shared.ViewTools;

namespace StoryCanvas.WPF.Converters.Common
{
	public class BooleanVisibilityConverter : ValueConverterBase<bool, Visibility>
	{
		public override Visibility Convert(bool value)
		{
			return value ? Visibility.Visible : Visibility.Collapsed;
		}

		public override bool ConvertBack(Visibility value)
		{
			return value == Visibility.Visible;
		}
	}
	public class NegativeBooleanVisibilityConverter : ValueConverterBase<bool, Visibility>
	{
		public override Visibility Convert(bool value)
		{
			return value ? Visibility.Collapsed : Visibility.Visible;
		}

		public override bool ConvertBack(Visibility value)
		{
			return value == Visibility.Collapsed;
		}
	}
}
