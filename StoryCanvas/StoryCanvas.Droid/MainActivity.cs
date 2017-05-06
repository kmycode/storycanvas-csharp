using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using HockeyApp.Android;
using HockeyApp.Android.Metrics;
using Plugin.Permissions;
using Libraries.StoryCanvas.Native.Models.IO;
using StoryCanvas.Shared.Utils;
using Libraries.StoryCanvas.Native.Models.Cryptography;

namespace StoryCanvas.Droid
{
	[Activity(Label = "StoryCanvas", Icon = "@drawable/icon", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, Theme = "@style/Theme.StoryCanvas")]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
	{
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			AutofacUtil.OneDriveStorage = new OneDriveStorageModel();
			AutofacUtil.MD5 = new MD5();
			global::Xamarin.Forms.Forms.Init(this, bundle);

#if !DEBUG
			// HockeyApp
			CrashManager.Register(this, "27818e33812640c597c922ef52e81d25");
			MetricsManager.Register(this, Application, "27818e33812640c597c922ef52e81d25");
#endif

			LoadApplication(new App());
		}

		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
		{
			PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
		}
	}
}

