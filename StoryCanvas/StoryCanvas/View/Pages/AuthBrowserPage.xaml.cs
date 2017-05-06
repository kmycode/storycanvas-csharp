using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoryCanvas.Shared.Messages.Page;
using StoryCanvas.Shared.Models.IO;
using StoryCanvas.Shared.ViewTools;
using Xamarin.Forms;

namespace StoryCanvas.View.Pages
{
	public partial class AuthBrowserPage : ContentPage, IAuthBrowser
	{
		public AuthBrowserPage()
		{
			InitializeComponent();

			this.Browser.Navigated += (sender, e) =>
			{
				this.UrlChanged?.Invoke(this, new AuthBrowserNavigationEventArgs(e.Url));
			};
			this.CloseButton.Clicked += (sender, e) =>
			{
				this.Aborted?.Invoke(this, new EventArgs());
				Messenger.Default.Send(this, new NavigationBackMessage());
			};
		}

		public event EventHandler Aborted;
		public event AuthBrowserNavigationEventHandler UrlChanged;

		public void GoToUrl(string url)
		{
			this.Browser.Source = url;
		}

		public void Close()
		{
			Messenger.Default.Send(this, new NavigationBackMessage());
		}
	}
}
