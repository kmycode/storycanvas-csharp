using System;
using System.Collections.Generic;
using System.Text;

namespace StoryCanvas.Shared.ViewTools.BehaviorHelper
{
	/// <summary>
	/// 進捗を表すクラス
	/// </summary>
    public class ProgressView
    {
		/// <summary>
		/// 進捗管理が必要な作業が始まったことを通知
		/// </summary>
		public event StartProcessingEventHandler StartProcessing;
		public delegate void StartProcessingEventHandler(object sender, EventArgs e);

		/// <summary>
		/// 進捗管理が必要な作業の開始を記録
		/// </summary>
		public void OnStartProcessing()
		{
			this.StartProcessing?.Invoke(this, new EventArgs());
		}

		/// <summary>
		/// 進捗管理が必要な作業が終わったことを通知
		/// </summary>
		public event EndProcessingEventHandler EndProcessing;
		public delegate void EndProcessingEventHandler(object sender, EventArgs e);

		/// <summary>
		/// 進捗管理が必要な作業の終了を記録
		/// </summary>
		public void OnEndProcessing()
		{
			this.EndProcessing?.Invoke(this, new EventArgs());
		}
    }
}
