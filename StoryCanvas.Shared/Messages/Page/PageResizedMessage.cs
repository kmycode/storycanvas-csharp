using System;
using System.Collections.Generic;
using System.Text;

namespace StoryCanvas.Shared.Messages.Page
{
	/// <summary>
	/// 画面サイズが変更された時に発行されるメッセージ
	/// </summary>
    public class PageResizedMessage
    {
		private double _width;
		public double Width
		{
			get
			{
				return this._width;
			}
		}
		private double _height;
		public double Height
		{
			get
			{
				return this._height;
			}
		}

		public PageResizedMessage(double w, double h)
		{
			this._width = w;
			this._height = h;
		}
    }
}
