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

namespace StoryCanvas.WPF.View.SubViews
{
	/// <summary>
	/// ConfigPrintTextPageSubView.xaml の相互作用ロジック
	/// </summary>
	public partial class ConfigPrintTextPageSubView : CustomDialog
	{
		public ConfigPrintTextPageSubView()
		{
			InitializeComponent();

			// 画面を閉じるボタン
			this.CloseButton.Click += (sender, e) =>
			{
				((MetroWindow)System.Windows.Application.Current.MainWindow).HideMetroDialogAsync(this);
			};
		}
	}
}
