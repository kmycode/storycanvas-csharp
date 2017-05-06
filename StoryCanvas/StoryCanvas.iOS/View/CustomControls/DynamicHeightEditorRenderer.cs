using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using StoryCanvas.iOS.View.CustomControls;
using StoryCanvas.View.CustomControls;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(DynamicHeightEditor), typeof(DynamicHeightEditorRenderer))]
namespace StoryCanvas.iOS.View.CustomControls
{
	class DynamicHeightEditorRenderer : EditorRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
		{
			base.OnElementChanged(e);

			if (Control != null)
			{
				Control.ScrollEnabled = false;
			}
		}
	}
}