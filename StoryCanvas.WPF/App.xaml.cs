using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.HockeyApp;

namespace StoryCanvas.WPF
{
	/// <summary>
	/// App.xaml の相互作用ロジック
	/// </summary>
	public partial class App : Application
	{
		protected override async void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

#if !DEBUG
			// HockeyApp
			HockeyClient.Current.Configure("8c1294f9d7c84c9cbeed4c02efbbea15");
			await HockeyClient.Current.SendCrashesAsync();
#endif
		}
	}
}
