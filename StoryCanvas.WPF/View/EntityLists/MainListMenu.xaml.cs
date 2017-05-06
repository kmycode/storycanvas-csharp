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
using StoryCanvas.Shared.ViewModels;

namespace StoryCanvas.WPF.View.EntityLists
{
	/// <summary>
	/// MainListMenu.xaml の相互作用ロジック
	/// </summary>
	public partial class MainListMenu : ContentControlEx
	{
		public MainListMenu()
		{
			InitializeComponent();
			this.DataContext = StoryViewModel.Default;
		}
	}
}
