using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoryCanvas.Shared.ViewModels;
using Xamarin.Forms;

namespace StoryCanvas.View.Pages
{
	public partial class EditScenePage : MasterDetailPage
	{
		public EditScenePage()
		{
			InitializeComponent();
			this.BindingContext = StoryViewModel.Default;
		}
	}
}
