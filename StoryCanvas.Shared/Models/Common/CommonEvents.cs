using System;
using System.Collections.Generic;
using System.Text;

namespace StoryCanvas.Shared.Models.Common
{
    /// <summary>
    /// 値が変更された時に発行されるイベント
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ValueChangedEventArgs<T> : EventArgs
    {
        public T OldValue { get; set; }
        public T NewValue { get; set; }
    }

    public delegate void ValueChangedEventHandler<T>(object sender, ValueChangedEventArgs<T> e);
}
