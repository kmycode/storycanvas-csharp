using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoryCanvas.Shared.ViewModels;
using Xamarin.Forms;

namespace StoryCanvas.View.Pages
{
	public partial class EditPeoplePage : MasterDetailPage
	{
		public EditPeoplePage()
		{
			InitializeComponent();
			this.BindingContext = StoryViewModel.Default;
		}
	}
}
