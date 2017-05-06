using System;
using System.Collections.Generic;
using System.Text;
using StoryCanvas.Shared.ViewTools;

namespace StoryCanvas.Shared.Converters.Common
{
	public class NullBooleanConverter : ValueConverterBase<object, bool, string>
	{
		public override bool Convert(object value, string parameter)
		{
			bool result = value != null;
			return parameter != "Negative" ? result : !result;
		}
	}
}
