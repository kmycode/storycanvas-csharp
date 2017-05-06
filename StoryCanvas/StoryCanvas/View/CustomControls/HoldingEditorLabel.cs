using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoryCanvas.Shared.Messages.Page;
using StoryCanvas.Shared.ViewTools;
using Xamarin.Forms;

namespace StoryCanvas.View.CustomControls
{
	/// <summary>
	/// クリックで編集画面へとぶラベル
	/// </summary>
	class HoldingEditorLabel : StackLayout
	{
		private ClickableLabel Label;

		public HoldingEditorLabel()
		{
			this.Label = new ClickableLabel();
			this.Label.TapCommand = new RelayCommand((obj) =>
			{
				var message = new EditTextMessage { Text = this.Label.Text, };
				message.TextEditorClosed += (sender, e) =>
				{
					this.Text = e.Text;
				};
				Messenger.Default.Send(this, message);
			});

			var scroll = new ScrollView();
			scroll.Content = this.Label;
			scroll.HeightRequest = 300;

			var button = new Button();
			button.Text = AppResources.Edit;
			button.Command = this.Label.TapCommand;

			this.Children.Add(scroll);
			this.Children.Add(button);
		}

		/// <summary>
		/// テキストのプロパティ
		/// </summary>
		public static readonly BindableProperty TextProperty =
			BindableProperty.Create<HoldingEditorLabel, string>(
				p => p.Text,
				null,
				defaultBindingMode: BindingMode.TwoWay,
				propertyChanged: (bindable, oldValue, newValue) =>
				{
					var view = bindable as HoldingEditorLabel;
					if (view != null)
					{
						var res = newValue as string;
						view.Label.Text = res;
					}
				});

		/// <summary>
		/// テキスト
		/// </summary>
		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
	}
}
