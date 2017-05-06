using System;
using System.Collections.Generic;
using System.Text;

namespace StoryCanvas.Shared.Messages.Common
{
    public class UIThreadInvokeMessage
    {
		public Action Action { get; }

		public UIThreadInvokeMessage(Action action)
		{
			this.Action = action;
		}
	}
}
