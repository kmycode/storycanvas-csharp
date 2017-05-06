using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoryCanvas.Shared.Messages.EditEntity;
using StoryCanvas.Shared.ViewModels.Entities;
using Xamarin.Forms;

namespace StoryCanvas.View.Pages
{
	public partial class EntityRelationPage : MasterDetailPage
	{
		public EntityRelationPage(EntityRelationEditorMessage message)
		{
			InitializeComponent();
			EntityRelationViewModel vm = new EntityRelationViewModel(message);
			this.RelationListPage.BindingContext = vm;
			this.RelationEditorPage.BindingContext = vm;
		}
	}
}
