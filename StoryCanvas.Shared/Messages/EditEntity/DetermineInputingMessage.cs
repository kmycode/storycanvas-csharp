using System;
using System.Collections.Generic;
using System.Text;

namespace StoryCanvas.Shared.Messages.EditEntity
{
	/// <summary>
	/// 現在入力中の項目の入力を確定させる
	/// （BindingのPropertyChangeTriggerによっては、値変更が即時モデルに反映されないことがあるため　テキストボックスとか）
	/// </summary>
    public class DetermineInputingMessage
    {
    }
}
