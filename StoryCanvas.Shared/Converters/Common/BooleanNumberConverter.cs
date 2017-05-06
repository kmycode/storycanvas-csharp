using System;
using System.Collections.Generic;
using System.Text;
using StoryCanvas.Shared.ViewTools;

namespace StoryCanvas.Shared.Converters.Common
{
	/// <summary>
	/// booleanを数値に変換するコンバータ
	/// </summary>
	public class BooleanNumberConverter : ValueConverterBase<bool, int, string>
	{
		public override int Convert(bool value, string parameter)
		{
			int param1, param2;
			string[] paramString = parameter.Split('_');
			if (int.TryParse(paramString[0], out param1) && int.TryParse(paramString[1], out param2))
			{
				return value ? param1 : param2;
			}
			return -1;
		}
	}
}
