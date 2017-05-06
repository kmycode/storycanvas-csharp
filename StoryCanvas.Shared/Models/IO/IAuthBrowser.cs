using System;
using System.Collections.Generic;
using System.Text;

namespace StoryCanvas.Shared.Models.IO
{
    public interface IAuthBrowser
    {
		event AuthBrowserNavigationEventHandler UrlChanged;

		event EventHandler Aborted;

		void GoToUrl(string url);
		void Close();
	}

	public delegate void AuthBrowserNavigationEventHandler(object sender, AuthBrowserNavigationEventArgs e);

	public class AuthBrowserNavigationEventArgs : EventArgs
	{
		public string Url { get; }
		public AuthBrowserNavigationEventArgs(string url)
		{
			this.Url = url;
		}
	}
}
