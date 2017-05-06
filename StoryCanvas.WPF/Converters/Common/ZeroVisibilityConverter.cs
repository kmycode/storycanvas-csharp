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
	/// 中身がからのリストを隠すかどうか決める
	/// </summary>
	public class ZeroVisibilityConverter : ValueConverterBase<int, Visibility>
	{
		public override Visibility Convert(int value)
		{
			return value == 0 ? Visibility.Collapsed : Visibility.Visible;
		}
	}
}
