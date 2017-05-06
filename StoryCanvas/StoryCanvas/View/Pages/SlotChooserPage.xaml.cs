using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoryCanvas.ViewModels.IO;
using Xamarin.Forms;

namespace StoryCanvas.View.Pages
{
	public partial class SlotChooserPage : ContentPage
	{
		public SlotChooserPage(bool isReadOnly = false)
		{
			InitializeComponent();
			this.BindingContext = new StorageSlotViewModel(isReadOnly);
			this.Title = isReadOnly ? AppResources.PickLoadSlot : AppResources.PickSaveSlot;
		}
	}
}
