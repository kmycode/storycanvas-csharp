using SkiaSharp;
using StoryCanvas.Shared.ViewTools.Resources;
using System;
using System.Collections.Generic;
using System.Text;

namespace StoryCanvas.Shared.View.Paint
{
    public static class CanvasUtil
    {
        public static SKPaint ToSKPaint(this SKColor color)
        {
            return new SKPaint
            {
                Color = color,
            };
        }

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
    }
}
