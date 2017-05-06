using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace StoryCanvas.Shared.Messages.Page
{
	public delegate void TextEditorClosedEventHandler(object sender, TextEditorClosedEventArgs e);
	public class TextEditorClosedEventArgs : EventArgs
	{
		public string Text { get; private set; }
		public TextEditorClosedEventArgs(string text)
		{
			this.Text = text;
		}
	}

    public class EditTextMessage : INotifyPropertyChanged
    {
		public string Text { get; set; }
		public event TextEditorClosedEventHandler TextEditorClosed = delegate { };
		public void OnTextEditorClosed()
		{
			this.TextEditorClosed(this, new TextEditorClosedEventArgs(this.Text));
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
