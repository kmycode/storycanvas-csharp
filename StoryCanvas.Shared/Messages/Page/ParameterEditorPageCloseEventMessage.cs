using System;
using System.Collections.Generic;
using System.Text;

namespace StoryCanvas.Shared.Messages.Page
{
	/// <summary>
	/// パラメータ編集画面が閉じるときの動作を指定するメッセージ
	/// </summary>
    public class ParameterEditorPageCloseEventMessage
    {
		public Action CloseAction { get; private set; }
		public ParameterEditorPageCloseEventMessage(Action closeAction)
		{
			this.CloseAction = closeAction;
		}
    }
}
