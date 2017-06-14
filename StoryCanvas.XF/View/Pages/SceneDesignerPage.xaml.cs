using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoryCanvas.Shared.ViewModels.Editors;
using Xamarin.Forms;

namespace StoryCanvas.View.Pages
{
	public partial class SceneDesignerPage : MasterDetailPage
	{
		public SceneDesignerPage()
		{
			InitializeComponent();
			this.BindingContext = SceneDesignerViewModel.Default;
		}
	}
}
