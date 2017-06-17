using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;
using StoryCanvas.Shared.View.Paint.Editor;
using StoryCanvas.Shared.Models.Entities;
using StoryCanvas.Shared.Models.Editor.Map;

namespace StoryCanvas.Shared.View.Paint
{
    /// <summary>
    /// 人物マップのキャンバス
    /// </summary>
    public class PersonMapCanvas : EntityEditorCanvasBase<PersonEntity>
    {
        public SimpleEntityMap<PersonEntity> Map { get; set; } = new SimpleEntityMap<PersonEntity>();

        protected override void DrawUpdate(SKCanvas canvas, SKPaint paint)
        {
            paint.TextSize = 18;

            foreach (var element in this.Map.Elements)
            {
                var size = 100.0f;

                var stream = element.Entity.DisplayIcon.ImageStream;
                var data = SKData.Create(stream);
                var codec = SKCodec.Create(data);
                var scale = size / (codec.Info.Width > codec.Info.Height ? codec.Info.Width : codec.Info.Height);

                canvas.Translate(element.X, element.Y);

                paint.Color = element.Entity.Color.ToSKColor();
                canvas.DrawRect(new SKRect(0, 0, 100, 100), paint);
                canvas.DrawText(element.Entity.Name, 8, 124, paint);

                canvas.Scale(scale);

                canvas.DrawImage(SKImage.FromEncodedData(data), 0, 0);

                canvas.Scale(1 / scale);
                canvas.Translate(-element.X, -element.Y);
            }
        }
    }
}
