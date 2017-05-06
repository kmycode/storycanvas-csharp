using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using HockeyApp.iOS;
using Libraries.StoryCanvas.Native.Models.IO;
using StoryCanvas.Shared.Utils;
using UIKit;
using Libraries.StoryCanvas.Native.Models.Cryptography;

namespace StoryCanvas.iOS
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the 
	// User Interface of the application, as well as listening (and optionally responding) to 
	// application events from iOS.
	[Register("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		protected PCLThinCanvas.DummyClassForLoadAssembly _dummy1
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		protected PCLThinCanvas.iOS.DummyClassForLoadAssembly _dummy2
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		//
		// This method is invoked when the application has loaded and is ready to run. In this 
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		//
		// You have 17 seconds to return from this method, or iOS will terminate your application.
		//
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			var a = new PCLThinCanvas.iOS.DummyClassForLoadAssembly();
			AutofacUtil.OneDriveStorage = new OneDriveStorageModel();
			AutofacUtil.MD5 = new MD5();

			global::Xamarin.Forms.Forms.Init();

#if !DEBUG
			// HockeyApp
			var manager = BITHockeyManager.SharedHockeyManager;
			manager.Configure("bb4db9d351ee442f9cadfa91ce939b85");
			manager.StartManager();
#endif

			LoadApplication(new App());

			// #3B8940
			UINavigationBar.Appearance.BarTintColor = new UIColor((nfloat)(0x3b / 256.0), (nfloat)(0x89 / 256.0), (nfloat)(0x40 / 256.0), (nfloat)1.0);
			UINavigationBar.Appearance.TintColor = new UIColor((nfloat)(0x6b / 256.0), (nfloat)(0xff / 256.0), (nfloat)(0xcc / 256.0), (nfloat)1.0);
			UINavigationBar.Appearance.SetTitleTextAttributes(new UITextAttributes
			{
				TextColor = UIColor.White
			});

#if DEBUG
			// メモリ使用量を出力
			System.Threading.Tasks.Task.Run(() =>
			{
				while(true)
				{
					var td = MemoryChecker.GetTaskInfo();
					System.Diagnostics.Debug.WriteLine("RAM: " + td.ResidentSize);
					System.Threading.Tasks.Task.Delay(30000).Wait();
				}
			});
#endif

			return base.FinishedLaunching(app, options);
		}
	}
}
