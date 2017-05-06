using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using StoryCanvas.Shared.Models.IO;

namespace StoryCanvas.WPF.View.SubViews
{
	/// <summary>
	/// AuthBrowserSubView.xaml の相互作用ロジック
	/// </summary>
	public partial class AuthBrowserSubView : CustomDialog, IAuthBrowser
	{
		public AuthBrowserSubView()
		{
			InitializeComponent();

			this.Browser.Navigated += (sender, e) =>
			{
				this.UrlChanged?.Invoke(this, new AuthBrowserNavigationEventArgs(e.Uri.AbsoluteUri));
			};
			this.CloseButton.Click += (sender, e) =>
			{
				this.Aborted?.Invoke(this, new EventArgs());
				((MetroWindow)System.Windows.Application.Current.MainWindow).HideMetroDialogAsync(this);
			};
		}

		public event EventHandler Aborted;
		public event AuthBrowserNavigationEventHandler UrlChanged;

		public void GoToUrl(string url)
		{
			this.Browser.Navigate(url);
		}

		public void Close()
		{
			((MetroWindow)System.Windows.Application.Current.MainWindow).HideMetroDialogAsync(this);
		}
	}
}
