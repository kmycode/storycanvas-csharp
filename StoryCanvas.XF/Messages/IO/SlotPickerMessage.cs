using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCLStorage;

namespace StoryCanvas.Messages.IO
{
	public abstract class SlotPickerMessage
	{
		public readonly Action<IFile> OnPicked;

		public SlotPickerMessage(Action<IFile> pickedAction)
		{
			this.OnPicked = pickedAction;
		}
	}

	public class SaveSlotPickerMessage : SlotPickerMessage
	{
		public SaveSlotPickerMessage(Action<IFile> pickedAction) : base(pickedAction)
		{
		}
	}

	public class OpenSlotPickerMessage : SlotPickerMessage
	{
		public OpenSlotPickerMessage(Action<IFile> pickedAction) : base(pickedAction)
		{
		}
	}
}
