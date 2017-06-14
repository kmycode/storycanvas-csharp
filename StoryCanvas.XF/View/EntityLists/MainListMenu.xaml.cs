using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoryCanvas.Shared.ViewModels;
using Xamarin.Forms;

namespace StoryCanvas.View.EntityLists
{
	public partial class MainListMenu : ContentView
	{
		public MainListMenu()
		{
			InitializeComponent();

			/*
			this.MainMenu.ItemsSource = new string[]
			{
				AppResources.People,
				AppResources.Place,
				AppResources.Scene,
				AppResources.Chapter,
			};
			*/
			this.BindingContext = StoryViewModel.Default;
		}
	}
}
