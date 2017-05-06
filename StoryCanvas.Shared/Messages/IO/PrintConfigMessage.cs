using System;
using System.Collections.Generic;
using System.Text;
using StoryCanvas.Shared.Models.IO;

namespace StoryCanvas.Shared.Messages.IO
{
	/// <summary>
	/// 印刷設定画面を表示する
	/// </summary>
	public class ConfigPrintTextPageMessage
	{
		public PrintTextPageInfo Info { get; private set; }
		public ConfigPrintTextPageMessage(PrintTextPageInfo info)
		{
			this.Info = info;
		}
	}
}
