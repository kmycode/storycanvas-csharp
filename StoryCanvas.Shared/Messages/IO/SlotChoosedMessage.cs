using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCLStorage;

namespace StoryCanvas.Shared.Messages.IO
{
	/// <summary>
	/// スロットを選択したことをあらわすメッセージ
	/// </summary>
	public class SlotChoosedMessage
	{
		public readonly IFile File;

		public SlotChoosedMessage(IFile file)
		{
			this.File = file;
		}
	}
}
