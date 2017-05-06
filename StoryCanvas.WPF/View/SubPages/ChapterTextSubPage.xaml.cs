using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Xps;
using StoryCanvas.Shared.ViewModels.Editors;

namespace StoryCanvas.WPF.View.SubPages
{
	/// <summary>
	/// ChapterTextSubPage.xaml の相互作用ロジック
	/// </summary>
	public partial class ChapterTextSubPage : UserControl
	{
		public ChapterTextSubPage()
		{
			InitializeComponent();
			this.DataContext = ChapterTextViewModel.Default;

			// HTMLが変更された時に、ブラウザのHTMLを更新
			ChapterTextViewModel.Default.PropertyChanged += (sender, e) =>
			{
				if (e.PropertyName == "AllVerticalScenesTextHTML")
				{
					this.VerticalBrowser.NavigateToString(ChapterTextViewModel.Default.AllVerticalScenesTextHTML);
				}
			};
		}

		/*
		private void PrintButton_Click(object sender, RoutedEventArgs e)
		{
			// 印刷ダイアログを表示
			var dialog = new PrintDialog();

			if (dialog.ShowDialog() == true)
			{
				// 1.各種オブジェクトの生成
				LocalPrintServer lps = new LocalPrintServer();
				PrintQueue queue = dialog.PrintQueue;
				XpsDocumentWriter writer = PrintQueue.CreateXpsDocumentWriter(queue);

				// 2. 用紙サイズの設定
				PrintTicket ticket = dialog.PrintTicket;
				//ticket.PageMediaSize = new PageMediaSize(PageMediaSizeName.ISOA4);
				//ticket.PageOrientation = PageOrientation.Portrait;

				// ListBoxのScrollViewerを取得し、印刷のサイズを取得
				int pageCount = 1;
				var border = VisualTreeHelper.GetChild(this.MainListBox, 0) as Border;
				if (border != null)
				{
					var listBoxScroll = border.Child as ScrollViewer;
					if (listBoxScroll != null)
					{
						listBoxScroll.Visibility = Visibility.Collapsed;
						pageCount = (int)(listBoxScroll.ScrollableHeight / ticket.PageMediaSize.Height) + 1;
					}
				}
				var doc = new FixedDocument();

				for (int i = 0; i < pageCount; i++)
				{
					// 3. FixedPage の生成
					FixedPage page = new FixedPage();

					// 4. 印字データの作成
					//var printCanvas = new ChapterTextSubPage();
					var vb = new VisualBrush();
					vb.Stretch = Stretch.Uniform;
					vb.Visual = this.MainListBox;
					vb.AutoLayoutContent = true;
					var printCanvas = new Rectangle();
					printCanvas.Width = 500;
					printCanvas.Height = 1000;
					printCanvas.Fill = vb;
					//printCanvas.SelectChapterGrid.Visibility = Visibility.Collapsed;
					page.Children.Add(printCanvas);

					FixedPage.SetTop(printCanvas, ticket.PageMediaSize.Height.Value * -i);

					var pc = new PageContent();
					((IAddChild)pc).AddChild(page);
					doc.Pages.Add(pc);
				}

				// 5. 印刷の実行
				writer.Write(doc, ticket);
				//dialog.PrintDocument(page., "Print1");
			}
		}
		*/
	}
}
