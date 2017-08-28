using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Globalization;
using System.Linq;

namespace StoryCanvas.Shared.Common
{
	/// <summary>
	/// 文字列リソースを取得
	/// </summary>
    public static class StringResourceResolver
    {
#if WINDOWS_UWP
		//private static Windows.ApplicationModel.Resources.ResourceLoader loader = new Windows.ApplicationModel.Resources.ResourceLoader();
#endif

		public static string Resolve(string key)
		{
#if WPF
			return (string)typeof(StoryCanvas.WPF.Properties.Resources).GetProperty(key).GetValue(null);
#elif WINDOWS_UWP
            return (string)typeof(AppResources)
                        .GetRuntimeProperties()
                        .SingleOrDefault(fi => fi.Name == key)?
                        .GetValue(null)
                        ?? throw new ArgumentException("適切なリソースが見つかりません :" + key);
            //return loader.GetString(key);
#elif XAMARIN_FORMS
			foreach (var fi in typeof(StoryCanvas.AppResources).GetRuntimeProperties())
			{
				if (fi.Name == key)
				{
					return (string)fi.GetValue(null);
				}
			}
			throw new ArgumentException("適切なリソースが見つかりません :" + key);
#endif
		}

		public static bool IsExistKey(string key)
		{
#if WPF
			return typeof(StoryCanvas.WPF.Properties.Resources).GetProperty(key) != null;
#elif WINDOWS_UWP
            return typeof(StoryCanvas.AppResources).GetRuntimeProperties().Any(p => p.Name == key);
            //return !string.IsNullOrEmpty(loader.GetString(key));
#elif XAMARIN_FORMS
			foreach (var fi in typeof(StoryCanvas.AppResources).GetRuntimeProperties())
			{
				if (fi.Name == key)
				{
					return true;
				}
			}
			return false;
#endif
		}
	}
}
