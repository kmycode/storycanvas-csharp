using System;
using System.Collections.Generic;
using System.Text;
using StoryCanvas.Shared.ViewTools;

namespace StoryCanvas.Shared.Converters.Common
{
	public class StringEmptyBooleanConverter : ValueConverterBase<string, bool>
	{
		public override bool Convert(string value)
		{
			return !string.IsNullOrEmpty(value);
		}
	}
}
