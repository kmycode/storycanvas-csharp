using SkiaSharp;
using StoryCanvas.Shared.Models.Editor.Map;
using StoryCanvas.Shared.Models.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace StoryCanvas.Shared.View.Paint.Editor
{
    /// <summary>
    /// エンティティエディタのキャンバスの基底クラス
    /// </summary>
    public abstract class EntityEditorCanvasBase<T> : CanvasBase
        where T : Entity
    {
        /// <summary>
        /// 描画ロジック
        /// </summary>
        protected IEntityEditorCanvasContainer<T> Container { get; }

        /// <summary>
        /// タップしている時間を計測するストップウォッチ
        /// </summary>
        private readonly Stopwatch tapping = new Stopwatch();

        /// <summary>
        /// 選択中のエンティティを含む要素
        /// </summary>
        public MapElement SelectedElement
        {
            get => this.Container.SelectedElement;
            set => this.Container.SelectedElement = value;
        }

        protected EntityEditorCanvasBase(IEntityEditorCanvasContainer<T> container)
        {
            this.Container = container;
            this.Container.Canvas = this;
            this.Container.SelectedEntityChanged += (sender, e) => this.SelectedEntityChanged?.Invoke(this, e);
        }

        protected override void ResizeMap()
        {
            base.ResizeMap();
            this.Container.ResizeMap();
        }

        public override void OnTapStart(double x, double y)
        {
            base.OnTapStart(x, y);

            // タップかどうか判定するためのタイマーを開始する
            this.tapping.Reset();
            this.tapping.Start();

            this.Container.OnTapStart(x, y);
        }

        protected override void OnDragging(double dx, double dy)
        {
            if (this.Container.CanDragMap)
            {
                base.OnDragging(dx, dy);
            }

            this.Container.OnDragging(dx, dy);
        }

        public override void OnTapEnd(double x, double y)
        {
            // タップであるか判定
            this.tapping.Stop();
            if (this.tapping.ElapsedMilliseconds < 200)
            {
                this.OnTapped(x, y);
            }

            this.Container.OnTapEnd(x, y);

            base.OnTapEnd(x, y);
        }

        protected virtual void OnTapped(double x, double y)
        {
            this.Container.OnTapped(x, y);
        }

        protected override void DrawUpdate(SKCanvas canvas, SKPaint paint)
        {
            this.Container.DrawUpdate(canvas, paint);
        }

        /// <summary>
        /// 現在選択中の要素を強調する
        /// </summary>
        /// <param name="canvas">キャンバス</param>
        /// <param name="paint">ペイント</param>
        protected override void DrawFloatingElements(SKCanvas canvas, SKPaint paint)
        {
            base.DrawFloatingElements(canvas, paint);

            if (this.SelectedElement != null)
            {
                paint.IsStroke = true;
                paint.StrokeWidth = 4;
                paint.Color = SKColors.Blue;

                canvas.DrawRect(new SKRect(this.SelectedElement.X - 8,
                                           this.SelectedElement.Y - 8,
                                           this.SelectedElement.X + this.SelectedElement.ViewWidth + 8,
                                           this.SelectedElement.Y + this.SelectedElement.ViewHeight + 8
                                           ), paint);

                paint.IsStroke = false;
            }
        }

        /// <summary>
        /// 指定座標上に存在する要素を探して検出する
        /// </summary>
        /// <typeparam name="E">エンティティの型</typeparam>
        /// <param name="elements">エンティティの入った複数の要素</param>
        /// <param name="x">調べる座標X</param>
        /// <param name="y">調べる座標Y</param>
        /// <returns>検出された要素。なければNULL</returns>
        public MapEntityElement<E> GetEntityFromPosition<E>(IEnumerable<MapEntityElement<E>> elements, double x, double y)
            where E : Entity
        {
            x -= this.X;
            y -= this.Y;
            return elements.LastOrDefault(e => e.X < x && x < e.X + e.ViewWidth && e.Y < y && y < e.Y + e.ViewHeight);
        }

        /// <summary>
        /// 選択中のエンティティが変更された時に呼び出し
        /// </summary>
        public event EventHandler SelectedEntityChanged;
    }

    static class EntityEditorCanvasUtil
    {
        /// <summary>
        /// マップ上の要素を描画する
        /// </summary>
        /// <typeparam name="E">エンティティの型</typeparam>
        /// <param name="element">エンティティの入った要素</param>
        /// <param name="canvas">キャンバス</param>
        /// <param name="paint">ペイント</param>
        public static void DrawMapElement<E>(MapEntityElement<E> element, SKCanvas canvas, SKPaint paint)
            where E : Entity
        {
            var size = 100.0f;

            var stream = element.Entity.DisplayIcon.ImageStream;
            var data = SKData.Create(stream);
            var codec = SKCodec.Create(data);
            var scale = size / (codec.Info.Width > codec.Info.Height ? codec.Info.Width : codec.Info.Height);

            canvas.Translate(element.X, element.Y);

            // 名前とアイコン背景色を描画する
            paint.TextSize = 18;
            paint.Color = element.Entity.Color.ToSKColor();
            canvas.DrawRect(new SKRect(0, 0, 100, 100), paint);
            canvas.DrawText(element.Entity.Name, 8, 120, paint);

            canvas.Scale(scale);

            // アイコンを描画する
            canvas.DrawImage(SKImage.FromEncodedData(data), 0, 0);

            canvas.Scale(1 / scale);
            canvas.Translate(-element.X, -element.Y);

            // 要素の大きさを記録する
            element.ViewWidth = (int)size;
            element.ViewHeight = (int)size + 24;
        }
    }
}
