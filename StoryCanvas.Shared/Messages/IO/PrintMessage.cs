using System;
using System.Collections.Generic;
using System.Text;
using StoryCanvas.Shared.Models.IO;

namespace StoryCanvas.Shared.Messages.IO
{
	/// <summary>
	/// テキストページを印刷するメッセージ
	/// </summary>
    public class PrintTextPageMessage
    {
		public PrintTextPageMessage(PrintTextPageInfo pageInfo, string title, ICollection<string> textes, ICollection<string> subtitles = null)
		{
			this.PageInfo = pageInfo;
			this.Title = title;
			this.Textes = textes;
			this.SubTitles = subtitles;
		}

		public PrintTextPageInfo PageInfo { get; private set; }

		public string Title { get; private set; }

		public ICollection<string> Textes { get; private set; }

		public ICollection<string> SubTitles { get; private set; }
    }
}
