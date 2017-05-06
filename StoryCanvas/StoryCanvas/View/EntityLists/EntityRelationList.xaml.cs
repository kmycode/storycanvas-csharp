using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoryCanvas.Shared.Messages.EditEntity;
using StoryCanvas.Shared.ViewTools;
using Xamarin.Forms;

namespace StoryCanvas.View.EntityLists
{
	public partial class EntityRelationList : ContentView
	{
		private PickNotRelatedEntityRequestedMessage newestMessage;

		public EntityRelationList()
		{
			InitializeComponent();

			this.NotRelatedEntityPicker.Picked += (sender, e) =>
			{
				if (this.newestMessage != null)
				{
					this.newestMessage.PickedAction(e.SelectedItem);
					this.newestMessage = null;
				}
			};

			Messenger.Default.Register<PickNotRelatedEntityRequestedMessage>((message) =>
			{
				this.newestMessage = message;
				this.NotRelatedEntityPicker.Unfocus();
				this.NotRelatedEntityPicker.Focus();
			});
		}
	}
}
