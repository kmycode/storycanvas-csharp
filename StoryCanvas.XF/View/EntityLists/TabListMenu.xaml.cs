using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoryCanvas.Shared.ViewModels;
using Xamarin.Forms;

namespace StoryCanvas.View.EntityLists
{
	public partial class TabListMenu : ContentView
	{
		public TabListMenu()
		{
			InitializeComponent();
			this.BindingContext = StoryViewModel.Default;
		}
	}
}
