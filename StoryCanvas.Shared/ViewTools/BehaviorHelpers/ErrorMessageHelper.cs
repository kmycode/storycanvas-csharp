using System;
using System.Collections.Generic;
using System.Text;

namespace StoryCanvas.Shared.ViewTools.BehaviorHelpers
{
    /// <summary>
    /// 画面上にエラーメッセージを表示するヘルパ
    /// </summary>
    public class ErrorMessageHelper
    {
        public void OnError(string message)
        {
            this.ErrorOccured?.Invoke(this, new ErrorOccuredEventArgs
            {
                Message = message,
            });
        }

        public event EventHandler<ErrorOccuredEventArgs> ErrorOccured;
    }

    public class ErrorOccuredEventArgs : EventArgs
    {
        public string Message { get; set; }
    }
}
