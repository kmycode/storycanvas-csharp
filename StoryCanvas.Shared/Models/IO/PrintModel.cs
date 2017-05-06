using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using StoryCanvas.Shared.Messages.IO;
using StoryCanvas.Shared.Models.Entities;
using StoryCanvas.Shared.ViewTools;

namespace StoryCanvas.Shared.Models.IO
{
	/// <summary>
	/// 印刷するロジック
	/// </summary>
    public class PrintModel : INotifyPropertyChanged
	{
		/// <summary>
		/// ページ設定
		/// </summary>
		public PrintTextPageInfo PageInfo
		{
			get;
			set;
		} = new PrintTextPageInfo();

		/// <summary>
		/// テキストページを印刷
		/// </summary>
		/// <param name="title">ページのタイトル</param>
		/// <param name="text">本文</param>
		public void PrintTextPage(string title, ICollection<string> textes, ICollection<string> subTitles = null)
		{
			Messenger.Default.Send(this, new PrintTextPageMessage(this.PageInfo, title, textes, subTitles));
		}

		/// <summary>
		/// 章とそれに属する全てのシーンを、テキストページとして印刷
		/// </summary>
		/// <param name="chapter">章</param>
		/// <param name="scenes"></param>
		public void PrintScenes(ChapterEntity chapter)
		{
			var textes = new Collection<string>();
			var subTitles = new Collection<string>();
			var scenes = chapter.RelatedScenes;

			foreach (var item in scenes)
			{
				textes.Add(item.Entity1.Text);
				subTitles.Add(item.Entity1.Name);
			}

			this.PrintTextPage(chapter.Name, textes, subTitles);
		}

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged([CallerMemberName] string name = null)
		{
			if (PropertyChanged == null) return;

			PropertyChanged(this, new PropertyChangedEventArgs(name));
		}

		#endregion
	}
}
