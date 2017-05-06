using System;
using System.Collections.Generic;
using System.Text;
using StoryCanvas.Shared.ViewTools;

namespace StoryCanvas.Shared.Converters.Common
{
	public class TypeNameBooleanConverter : ValueConverterBase<object, bool, string>
	{
		public override bool Convert(object value, string parameter)
		{
			if (value == null)
			{
				return false;
			}
			return value.GetType().Name == parameter;
		}
	}
}
