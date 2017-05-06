using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using StoryCanvas.Shared.ViewModels;

namespace StoryCanvas.WPF.View.SubViews
{
	/// <summary>
	/// NetworkSendSubView.xaml の相互作用ロジック
	/// </summary>
	public partial class NetworkSendSubView : CustomDialog
	{
		public NetworkSendSubView()
		{
			InitializeComponent();
			this.DataContext = new NetworkViewModel();

			// 画面を閉じる時、ネットワークからの受信を中止
			this.CloseButton.Click += (sender, e) =>
			{
				((NetworkViewModel)this.DataContext).CleaningUpSendModelCommand.Execute(null);
				((MetroWindow)System.Windows.Application.Current.MainWindow).HideMetroDialogAsync(this);
			};
		}
	}
}
