using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoryCanvas.Shared.Messages.Page;
using StoryCanvas.Shared.ViewModels;
using StoryCanvas.Shared.ViewTools;
using Xamarin.Forms;

namespace StoryCanvas.View.Pages
{
	public partial class MainPage : TabbedPage
	{
		public MainPage()
		{
			InitializeComponent();
			this.BindingContext = StoryViewModel.Default;
		}

		protected override void OnSizeAllocated(double width, double height)
		{
			base.OnSizeAllocated(width, height);
			Messenger.Default.Send(this, new PageResizedMessage(width, height));
		}
	}
}
