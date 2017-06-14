using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoryCanvas.Shared.ViewModels.Editors;
using Xamarin.Forms;

namespace StoryCanvas.View.Pages
{
	public partial class ChapterTextPage : MasterDetailPage
	{
		public ChapterTextPage()
		{
			InitializeComponent();
			this.BindingContext = ChapterTextViewModel.Default;

			// HTMLが変更された時に、ブラウザのHTMLを更新
			ChapterTextViewModel.Default.PropertyChanged += (sender, e) =>
			{
				if (e.PropertyName == "AllVerticalScenesTextHTML")
				{
					this.VerticalBrowser.Source = new HtmlWebViewSource { Html = ChapterTextViewModel.Default.AllVerticalScenesTextHTML, };
				}
			};

			// マスターページの表示モード変更
			this.BothModeButton.Clicked += (sender, e) =>
			{
				this.ShowBothMode();
			};
			this.SceneModeButton.Clicked += (sender, e) =>
			{
				this.ShowBothMode();
				this.RelationModeDisplay.IsVisible = false;
				this.SceneDetailDisplay.IsVisible = true;
				this.RelationRow.Height = GridLength.Auto;
				this.SceneRow.Height = GridLength.Star;
			};
			this.RelationModeButton.Clicked += (sender, e) =>
			{
				this.ShowBothMode();
				this.SceneModeDisplay.IsVisible = false;
				//this.RelatedEntitiesLabel.IsVisible = false;
				this.SceneRow.Height = GridLength.Auto;
				this.RelationRow.Height = GridLength.Star;
			};
		}

		private void ShowBothMode()
		{
			this.SceneModeDisplay.IsVisible = true;
			this.RelationModeDisplay.IsVisible = true;
			this.SceneRow.Height = GridLength.Star;
			this.RelationRow.Height = GridLength.Star;
			//this.RelatedEntitiesLabel.IsVisible = true;
			this.SceneDetailDisplay.IsVisible = false;
		}
	}
}
