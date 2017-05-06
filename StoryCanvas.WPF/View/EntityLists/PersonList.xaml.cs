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
using StoryCanvas.Shared.ViewModels;

namespace StoryCanvas.WPF.View.EntityLists
{
	/// <summary>
	/// PersonList.xaml の相互作用ロジック
	/// </summary>
	public partial class PersonList : UserControl
	{
		public PersonList()
		{
			InitializeComponent();
			this.DataContext = StoryViewModel.Default;
		}

		private void ReorderButton_Click(object sender, RoutedEventArgs e)
		{
			this.ReorderButtonSet.Visibility = this.ReorderButtonSet.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
		}
	}
}
