using System;
using System.Collections.Generic;
using System.Text;
using StoryCanvas.Shared.ViewTools;
using StoryCanvas.Shared.ViewTools.Resources;

namespace StoryCanvas.Shared.Converters.Common
{
	/// <summary>
	/// 色を、カラーコードに変換
	/// </summary>
#if WPF
	public class ColorResourceCodeConverter : ValueConverterBase<ColorResource, object>
#elif WINDOWS_UWP
	public class ColorResourceCodeConverter : ValueConverterBase<ColorResource, object>
#elif XAMARIN_FORMS
	public class ColorResourceCodeConverter : ValueConverterBase<ColorResource, string>
#endif
	{
#if WPF
		public override object Convert(ColorResource value)
#elif WINDOWS_UWP
		public override object Convert(ColorResource value)
#elif XAMARIN_FORMS
		public override string Convert(ColorResource value)
#endif
		{
			return "#" + value.R.ToString("X2") + value.G.ToString("X2") + value.B.ToString("X2");
		}
	}
}
