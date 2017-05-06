using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using StoryCanvas.Droid.View.CustomControls;
using StoryCanvas.View.CustomControls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

//[assembly: Xamarin.Forms.ExportRenderer(typeof(DynamicHeightEditor), typeof(DynamicHeightEditorRenderer))]
namespace StoryCanvas.Droid.View.CustomControls
{
	// https://forums.xamarin.com/discussion/38172/how-to-autosize-editor-height
	class DynamicHeightEditorRenderer : EditorRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
		{
			base.OnElementChanged(e);
			if (e.NewElement != null)
			{
				e.NewElement.TextChanged += this.InvalidateEditor;
			}
			if (e.OldElement != null)
			{
				e.OldElement.TextChanged -= this.InvalidateEditor;
			}
		}

		private void InvalidateEditor(object sender, TextChangedEventArgs e)
		{
			if (this.Element != null)
			{
				var method = typeof(Android.Views.View).GetMethod("InvalidateMeasure", BindingFlags.Instance | BindingFlags.NonPublic);
				method.Invoke(this.Element, null);
			}
		}
	}
}