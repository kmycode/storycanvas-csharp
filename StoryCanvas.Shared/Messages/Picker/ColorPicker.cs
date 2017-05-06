using System;
using System.Collections.Generic;
using System.Text;
using StoryCanvas.Shared.ViewTools.Resources;

namespace StoryCanvas.Shared.Messages.Picker
{
    public class ColorPickerMessage
	{
		public Action<ColorResource> Action
		{
			get;
			private set;
		}
		public ColorResource DefaultValue
		{
			get;
			private set;
		}
		public ColorPickerMessage(ColorResource defaultValue, Action<ColorResource> action)
		{
			this.Action = action;
			this.DefaultValue = defaultValue;
		}
	}
}
