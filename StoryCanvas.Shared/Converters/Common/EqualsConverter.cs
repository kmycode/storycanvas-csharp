using System;
using System.Collections.Generic;
using System.Text;
using StoryCanvas.Shared.ViewTools;

namespace StoryCanvas.Shared.Converters.Common
{
	/// <summary>
	/// 2つの値がEqualsであればtrueを返すコンバータ
	/// </summary>
	public class EqualsConverter : ValueConverterBase<object, bool, object, object>
	{
		public override bool Convert(object value, object parameter)
		{
			return value?.ToString().Equals(parameter) ?? false;
		}
	}

	public class NotEqualsConverter : ValueConverterBase<object, bool, object, object>
	{
		public override bool Convert(object value, object parameter)
		{
			return !(value?.ToString().Equals(parameter) ?? false);
		}
	}
}
