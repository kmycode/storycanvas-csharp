using System;
using System.Collections.Generic;
using System.Text;

namespace StoryCanvas.Shared.Messages.Network
{
    public class OpenUrlMessage
    {
		public readonly string Url;

		public OpenUrlMessage(string url)
		{
			this.Url = url;
		}
    }
}
