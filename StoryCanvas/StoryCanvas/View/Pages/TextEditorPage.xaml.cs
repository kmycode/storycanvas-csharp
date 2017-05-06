using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoryCanvas.Shared.Messages.Page;
using Xamarin.Forms;

namespace StoryCanvas.View.Pages
{
	public partial class TextEditorPage : ContentPage
	{
		public TextEditorPage(EditTextMessage message)
		{
			InitializeComponent();
			this.BindingContext = message;

			this.Disappearing += (sender, e) =>
			{
				message.OnTextEditorClosed();
			};
		}
	}
}
