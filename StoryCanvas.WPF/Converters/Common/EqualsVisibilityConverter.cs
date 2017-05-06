using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using StoryCanvas.Shared.ViewTools;

namespace StoryCanvas.WPF.Converters.Common
{
	/// <summary>
	/// 2つの値が等しければVisibleを返す
	/// </summary>
	public class EqualsVisibilityConverter : ValueConverterBase<object, Visibility, object>
	{
		public override Visibility Convert(object value, object parameter)
		{
			return (value?.ToString().Equals(parameter.ToString()) ?? false) ? Visibility.Visible : Visibility.Collapsed;
		}
	}
	public class NotEqualsVisibilityConverter : ValueConverterBase<object, Visibility, object>
	{
		public override Visibility Convert(object value, object parameter)
		{
			return (value?.ToString().Equals(parameter.ToString()) ?? false) ? Visibility.Collapsed : Visibility.Visible;
		}
	}
}
