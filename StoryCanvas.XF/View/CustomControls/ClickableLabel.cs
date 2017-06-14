using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace StoryCanvas.View.CustomControls
{
	class ClickableLabel : Label
	{
		public ClickableLabel()
		{
			var tgr = new TapGestureRecognizer();
			tgr.Tapped += (sender, e) => this.OnClicked();
			this.GestureRecognizers.Add(tgr);
		}

		/// <summary>
		/// タップコマンドのプロパティ
		/// </summary>
		public static readonly BindableProperty TapCommandProperty =
			BindableProperty.Create<ClickableLabel, ICommand>(
				p => p.TapCommand,
				null,
				propertyChanged: (bindable, oldValue, newValue) =>
				{
					var view = bindable as ClickableLabel;
					if (view != null)
					{
						var res = newValue as ICommand;
						view.TapCommand = res;
					}
				});

		/// <summary>
		/// タップコマンド
		/// </summary>
		public ICommand TapCommand
		{
			get { return (ICommand)GetValue(TapCommandProperty); }
			set { SetValue(TapCommandProperty, value); }
		}

		/// <summary>
		/// タップコマンドのプロパティ
		/// </summary>
		public static readonly BindableProperty TapCommandParameterProperty =
			BindableProperty.Create<ClickableLabel, object>(
				p => p.TapCommandParameter,
				null,
				propertyChanged: (bindable, oldValue, newValue) =>
				{
					var view = bindable as ClickableLabel;
					if (view != null)
					{
						var res = newValue as object;
						view.TapCommandParameter = res;
					}
				});

		/// <summary>
		/// タップコマンド
		/// </summary>
		public object TapCommandParameter
		{
			get { return GetValue(TapCommandParameterProperty); }
			set { SetValue(TapCommandParameterProperty, value); }
		}

		private void OnClicked()
		{
			if (this.TapCommand != null)
			{
				this.TapCommand.Execute(this.TapCommandParameter);
			}
		}
	}
}
