using System;
using System.Collections.Generic;
using System.Text;
using StoryCanvas.Shared.ViewTools;

namespace StoryCanvas.Shared.Converters.Common
{
	public class NegativeConverter : ValueConverterBase<bool, bool>
	{
		public override bool Convert(bool value)
		{
			return !value;
		}

		public override bool ConvertBack(bool value)
		{
			return !value;
		}
	}
}
