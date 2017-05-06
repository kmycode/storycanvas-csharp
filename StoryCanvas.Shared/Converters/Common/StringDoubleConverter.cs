using System;
using System.Collections.Generic;
using System.Text;
using StoryCanvas.Shared.ViewTools;

namespace StoryCanvas.Shared.Converters.Common
{
	public class StringDoubleConverter : ValueConverterBase<string, double>
	{
		public override double Convert(string value)
		{
			double r;
			if (!double.TryParse(value, out r))
			{
				r = default(double);
			}
			return r;
		}

		public override string ConvertBack(double value)
		{
			return value.ToString();
		}
	}
}
