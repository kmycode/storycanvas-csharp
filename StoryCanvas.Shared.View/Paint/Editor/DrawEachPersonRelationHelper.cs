using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;
using StoryCanvas.Shared.Models.Editor.Map;
using StoryCanvas.Shared.Models.Entities;
using StoryCanvas.Shared.Models.EntityRelate;

namespace StoryCanvas.Shared.View.Paint.Editor
{
    /// <summary>
    /// 人物同士の関連付けを描画する時のヘルパ
    /// </summary>
    class DrawEachPersonRelationHelper : IDrawRelationHelper
    {
        public void DrawRelation<E1, E2>(SKCanvas canvas, SKPaint paint, MapElement element1, MapElement element2, EntityRelateBase<E1, E2> relation)
            where E1 : Entity
            where E2 : Entity
        {
            // 引数の型チェック
            if (typeof(E1) != typeof(PersonEntity) || typeof(E2) != typeof(PersonEntity))
            {
                throw new ArgumentException();
            }
            var r = (FocusableEntityRelateBase<E1, E2>)relation;
            r.FocusedEntity = r.Entity1;

            // 線をひく
            paint.Color = SKColors.Gray;
            paint.StrokeWidth = 2;
            canvas.DrawLine(element1.X + 50, element1.Y + 50, element2.X + 50, element2.Y + 50, paint);

            paint.TextSize = 14;
            paint.Color = SKColors.Black;

            var text = r.Note.Length <= 20 ? r.Note : r.Note.Substring(0, 19) + "…";
            var textBoxWidth = (text.Length + 1) * paint.TextSize;

            // 関係を書くための座標を調べる
            var centerX = (element1.X + element2.X) / 2 + 50;
            var centerY = (element1.Y + element2.Y) / 2 + 50;
            var leftTopX = centerX - textBoxWidth / 2;
            var leftTopY = centerY - 8;
            if (leftTopX < 0) leftTopX = 0;
            if (leftTopY < 0) leftTopY = 0;

            // 関係を書く
            if (!string.IsNullOrEmpty(r.Note))
            {
                this.DrawRelateNote(canvas, paint, r.Note, ((Entity)r.FocusedEntity).Color.ToSKColor(), centerX, leftTopY);
            }
            leftTopY += (int)paint.TextSize + 4;

            // 関係その２
            if (!string.IsNullOrEmpty(r.OtherNote))
            {
                this.DrawRelateNote(canvas, paint, r.OtherNote, ((Entity)r.NotFocusedEntity).Color.ToSKColor(), centerX, leftTopY);
            }
        }

        private void DrawRelateNote(SKCanvas canvas, SKPaint paint, string text, SKColor color, int centerX, int leftTopY)
        {
            var textBoxWidth = (text.Length + 1) * paint.TextSize;

            // 下地
            paint.Color = SKColors.White;
            canvas.DrawRect(new SKRect(centerX - textBoxWidth / 2 - 4,
                                       leftTopY - paint.TextSize / 2 - 2,
                                       centerX + textBoxWidth / 2 + 4,
                                       leftTopY + paint.TextSize / 2 + 4), paint);

            // テキスト描画
            paint.Color = color;
            canvas.DrawText("●", centerX - textBoxWidth / 2, leftTopY + paint.TextSize / 2, paint);
            paint.Color = SKColors.Black;
            canvas.DrawText(text, centerX - textBoxWidth / 2 + paint.TextSize, leftTopY + paint.TextSize / 2, paint);
        }
    }
}
