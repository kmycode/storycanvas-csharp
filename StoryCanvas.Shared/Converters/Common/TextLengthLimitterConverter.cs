using System;
using System.Collections.Generic;
using System.Text;
using StoryCanvas.Shared.ViewTools;

namespace StoryCanvas.Shared.Converters.Common
{
	public class TextLengthLimitterConverter : ValueConverterBase<string, string>
	{
		public override string Convert(string value)
		{
			//if (value.Length <= 8)
			//{
				return value;
			//}
			//return value.Substring(0, 7) + "...";
		}
	}
}
