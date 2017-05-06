using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoryCanvas.Shared.ViewModels;
using Xamarin.Forms;

namespace StoryCanvas.View.EntityLists
{
	public partial class WordList : ContentView
	{
		public WordList()
		{
			InitializeComponent();
			this.BindingContext = StoryViewModel.Default;
		}

		private void ReorderButton_Click(object sender, EventArgs e)
		{
			this.ReorderButtonSet.IsVisible = !this.ReorderButtonSet.IsVisible;
		}
	}
}
