using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using StoryCanvas.View.Common;
using Xamarin.Forms;

[assembly: Dependency(typeof(StoryCanvas.Droid.View.Common.LightAlert))]
namespace StoryCanvas.Droid.View.Common
{
	public class LightAlert : ILightAlert
	{
		public void Alert(string text)
		{
			Toast.MakeText(Android.App.Application.Context, text, ToastLength.Long).Show();
		}
	}
}