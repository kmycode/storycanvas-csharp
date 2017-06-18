using SkiaSharp;
using StoryCanvas.Shared.Models.Editor.Map;
using StoryCanvas.Shared.ViewTools.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StoryCanvas.Shared.View.Paint
{
    public static class CanvasUtil
    {
        public static SKColor ToSKColor(this ColorResource color)
        {
            return new SKColor(color.R, color.G, color.B, 255);
        }

        public static SKTypeface LoadFont()
        {
#if WINDOWS_UWP
            return SKTypeface.FromFamilyName("Yu Gothic");
#else
            throw new NotImplementedException();
#endif
        }

        /// <summary>
        /// 2つの要素と、ある点のあいだの距離を求める
        /// </summary>
        /// <param name="e1">要素１</param>
        /// <param name="e2">要素２</param>
        /// <param name="x">ある点の座標X</param>
        /// <param name="y">ある点の座標Y</param>
        /// <returns>距離。ただし２乗されているので注意</returns>
        public static float GetDistanceBetweenLimitedLineAndPosition(MapElement e1, MapElement e2, float x, float y)
        {
            var dl = MathUtil.GetDistanceBetweenLimitedLineAndPosition(x, y, e1.X, e1.Y, e2.X, e2.Y);
            return dl;
        }
    }
}
