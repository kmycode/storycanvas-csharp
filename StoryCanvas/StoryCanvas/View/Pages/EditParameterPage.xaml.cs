using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoryCanvas.Shared.Messages.Page;
using StoryCanvas.Shared.ViewModels;
using StoryCanvas.Shared.ViewTools;
using Xamarin.Forms;

namespace StoryCanvas.View.Pages
{
	public partial class EditParameterPage : MasterDetailPage
	{
		public EditParameterPage()
		{
			InitializeComponent();
			this.BindingContext = StoryViewModel.Default;

			this.Disappearing += (sender, e) => CloseEventMessageDone();
		}

		/// <summary>
		/// 次にパラメータページを閉じる時に起こすイベントを指定したメッセージ
		/// </summary>
		private static ParameterEditorPageCloseEventMessage CloseEventMessage = null;
		static EditParameterPage()
		{
			Messenger.Default.Register<ParameterEditorPageCloseEventMessage>((message) =>
			{
				CloseEventMessage = message;
			});
		}
		private static void CloseEventMessageDone()
		{
			if (CloseEventMessage != null)
			{
				CloseEventMessage.CloseAction();
				CloseEventMessage = null;
			}
		}
	}
}
