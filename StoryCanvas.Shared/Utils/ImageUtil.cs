using System;
using System.Collections.Generic;
using System.Text;

namespace StoryCanvas.Shared.Utils
{
	/// <summary>
	/// 画像にまつわるユーティリティクラス
	/// </summary>
    public static class ImageUtil
    {
		/// <summary>
		/// 画像のサイズを、指定された最大値におさまるよう、アスペクト比を維持しつつ調整
		/// </summary>
		/// <param name="width">画像横幅</param>
		/// <param name="height">画像縦幅</param>
		/// <param name="widthMax">画像横幅最大</param>
		/// <param name="heightMax">画像縦幅最大</param>
		/// <returns>リサイズ後のサイズ（横、盾）</returns>
		public static Tuple<double, double> ResizeImage(double width, double height, double widthMax = 200, double heightMax = 200)
		{
			double newWidth = width, newHeight = height;

			if (newWidth > widthMax)
			{
				newHeight *= (widthMax / newWidth);
				newWidth = widthMax;
			}

			if (newHeight > heightMax)
			{
				newWidth *= (heightMax / newHeight);
				newHeight = heightMax;
			}

			System.Diagnostics.Debug.WriteLine($"{width} , {height} -> {newWidth} , {newHeight}");
			System.Diagnostics.Debug.WriteLine($"{width / height} -> {newWidth / newHeight}");

			return new Tuple<double, double>(newWidth, newHeight);
		}
    }
}
