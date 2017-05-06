using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Resources;
using System.Globalization;
using System.Reflection;
using StoryCanvas.Shared.Common;

namespace StoryCanvas
{
	public interface ILocalize
	{
		CultureInfo GetCurrentCultureInfo();

		void SetLocale();
	}

	// You exclude the 'Extension' suffix when using in Xaml markup
	[ContentProperty("Text")]
	public class TranslateExtension : IMarkupExtension
	{
		public string Text { get; set; }

		public object ProvideValue(IServiceProvider serviceProvider)
		{
			if (Text == null) return null;
			/*
			foreach (var fi in typeof(AppResources).GetRuntimeProperties())
			{
				if (fi.Name == Text)
				{
					return fi.GetValue(null);
				}
			}
			return "Translate Error";
			*/
			return StringResourceResolver.Resolve(this.Text);
		}
	}
}