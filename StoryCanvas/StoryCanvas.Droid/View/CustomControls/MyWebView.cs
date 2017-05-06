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
using StoryCanvas.Droid.View.CustomControls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: Xamarin.Forms.ExportRenderer(typeof(WebView), typeof(MyWebViewRenderer))]
namespace StoryCanvas.Droid.View.CustomControls
{
	public class MyWebViewRenderer : WebViewRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<WebView> e)
		{
			base.OnElementChanged(e);
			if (e.NewElement != null)
			{
				// lets get a reference to the native control
				var webView = (global::Android.Webkit.WebView)Control;
				// do whatever you want to the WebView here!
				webView.HorizontalScrollBarEnabled = true;
				webView.VerticalScrollBarEnabled = false;
				//webView.SetInitialScale(-1);
			}
		}
	}
}