using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoryCanvas.Shared.ViewModels;
using Xamarin.Forms;

namespace StoryCanvas.View.Pages
{
	public partial class NetworkSendPage : ContentPage
	{
		public NetworkSendPage()
		{
			InitializeComponent();
			this.BindingContext = new NetworkViewModel();

			// 画面を閉じた時、受信も終了する
			this.Disappearing += (sender, e) =>
			{
				((NetworkViewModel)this.BindingContext).CleaningUpReceiveModelCommand.Execute(null);
			};
		}
	}
}
