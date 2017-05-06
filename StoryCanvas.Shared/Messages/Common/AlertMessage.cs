using System;
using System.Collections.Generic;
using System.Text;

namespace StoryCanvas.Shared.Messages.Common
{
    public abstract class AlertMessageBase
    {
		public string Text { get; private set; }
		public AlertMessageBase(string message)
		{
			this.Text = message;
		}
    }

	/// <summary>
	/// 画面操作を中断するモーダルメッセージボックスを表示
	/// </summary>
	public class AlertMessage : AlertMessageBase
	{
		public AlertMessageType Type { get; private set; }

		public AlertMessageResult Result { get; set; } = AlertMessageResult.WaitingResult;

		public Action<AlertMessageResult> Continue { get; private set; }

		public AlertMessage(string message, AlertMessageType type = AlertMessageType.Ok, Action<AlertMessageResult> con = null) : base(message)
		{
			this.Type = type;
			this.Continue = con;
		}
	}

	public enum AlertMessageType
	{
		Ok,
		OkCancel,
		Yes,
		YesNo,
		YesNoCancel,
	}

	public enum AlertMessageResult
	{
		WaitingResult,
		Ok,
		Cancel,
		Yes,
		No,
	}

	/// <summary>
	/// プラットフォームに、画面操作を中断しない程度のメッセージ機能（Toastとか）があればそれを使う
	/// なければモーダルのメッセージボックスを表示
	/// </summary>
	public class LightAlertMessage : AlertMessageBase
	{
		public LightAlertMessage(string message) : base(message)
		{
		}
	}
}
