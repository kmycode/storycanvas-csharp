using System;
using System.Collections.Generic;
using System.Text;
using StoryCanvas.Shared.Common;
using StoryCanvas.Shared.ViewTools;

namespace StoryCanvas.Shared.Converters.Common
{
	/// <summary>
	/// タイトルが空だったら無題とかつける
	/// </summary>
	public class TitleDisplayConverter : ValueConverterBase<string, string>
	{
		public override string Convert(string value)
		{
			if (string.IsNullOrWhiteSpace(value))
			{
				return StringResourceResolver.Resolve("EmptyTitle");
			}
			return value;
		}
	}
}
