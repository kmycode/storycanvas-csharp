using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace StoryCanvas.View.CustomControls
{
	class ImageResourceButton : ImageResourcePanel
	{
		/// <summary>
		/// コマンドのプロパティ
		/// </summary>
		public static readonly BindableProperty CommandProperty =
			BindableProperty.Create<ImageResourceButton, ICommand>(
				p => p.Command,
				null,
				propertyChanged: (bindable, oldValue, newValue) =>
				{
					//var view = bindable as CommandCell;
					//if (view != null)
					//{
					//}
				});

		/// <summary>
		/// コマンド
		/// </summary>
		public ICommand Command
		{
			get { return (ICommand)GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
		}

		/// <summary>
		/// コマンドパラメータのプロパティ
		/// </summary>
		public static readonly BindableProperty CommandParameterProperty =
			BindableProperty.Create<ImageResourceButton, object>(
				p => p.CommandParameter,
				null,
				propertyChanged: (bindable, oldValue, newValue) =>
				{
					//var view = bindable as CommandParameterCell;
					//if (view != null)
					//{
					//}
				});

		/// <summary>
		/// コマンドパラメータ
		/// </summary>
		public object CommandParameter
		{
			get { return (object)GetValue(CommandParameterProperty); }
			set { SetValue(CommandParameterProperty, value); }
		}

		public ImageResourceButton()
		{
			this.GestureRecognizers.Add(new TapGestureRecognizer((view) =>
			{
				this.Command?.Execute(this.CommandParameter);
			}));
		}
	}
}
