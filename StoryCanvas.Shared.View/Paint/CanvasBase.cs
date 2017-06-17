using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace StoryCanvas.Shared.View.Paint
{
    /// <summary>
    /// 画面に2D描画を行うベースクラス。このクラスはViewから直接呼び出すことを想定している
    /// </summary>
    public abstract class CanvasBase
    {
        /// <summary>
        /// キャッシュ
        /// </summary>
        private SKImage cache;

        private double lastTapX;

        private double lastTapY;

        private bool isDragging;

        /// <summary>
        /// 描画の時に使われる引数
        /// </summary>
        public object Argument { get; set; }

        /// <summary>
        /// 描画を開始するX座標。画面スクロールで変動
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// 描画を開始するY座標。画面スクロールで変動
        /// </summary>
        public float Y { get; set; }

        /// <summary>
        /// キャンバスの横幅
        /// </summary>
        public int Width { get; protected set; } = 500;

        /// <summary>
        /// キャンバスの高さ
        /// </summary>
        public int Height { get; protected set; } = 500;

        /// <summary>
        /// 画面への描画を行う
        /// </summary>
        /// <param name="canvas">キャンバス</param>
        public void Draw(SKSurface surface, SKColorType colorType)
        {
            surface.Canvas.Translate(this.X, this.Y);

            if (this.cache == null)
            {
                using (var buffer = SKSurface.Create(this.Width, this.Height, colorType, SKAlphaType.Premul))
                {
                    using (var paint = new SKPaint
                    {
                        IsAntialias = true,
                    })
                    {
                        using (var tf = CanvasUtil.LoadFont())
                        {
                            paint.Typeface = tf;
                            this.DrawUpdate(buffer.Canvas, paint);
                        }
                    }
                    this.cache = buffer.Snapshot().ToRasterImage();
                }
            }

            surface.Canvas.DrawImage(this.cache, 0, 0);
        }

        /// <summary>
        /// 描画内容を更新する。X、Yなどで指定された部分ではなく、全範囲を描画する
        /// </summary>
        /// <param name="canvas">キャンバス</param>
        protected abstract void DrawUpdate(SKCanvas canvas, SKPaint paint);
        
        /// <summary>
        /// 描画のキャッシュを無効にし、描画内容を更新する
        /// </summary>
        public void InvalidateSurface()
        {
            this.cache = null;
        }

        /// <summary>
        /// タップが開始された時に発生
        /// </summary>
        /// <param name="x">画面上のX</param>
        /// <param name="y">画面上のY</param>
        public virtual void OnTapStart(double x, double y)
        {
            this.isDragging = true;
            this.lastTapX = x;
            this.lastTapY = y;
        }

        /// <summary>
        /// タップが終了した時に発生
        /// </summary>
        /// <param name="x">画面上のX</param>
        /// <param name="y">画面上のY</param>
        public virtual void OnTapEnd(double x, double y)
        {
            this.isDragging = false;
        }

        /// <summary>
        /// ドラッグ（押しながら移動）された時に発生
        /// </summary>
        /// <param name="x">画面上のX</param>
        /// <param name="y">画面上のY</param>
        public virtual void OnTapMove(double x, double y)
        {
            if (this.isDragging)
            {
                var dx = this.lastTapX - x;
                var dy = this.lastTapY - y;

                this.X -= (float)dx;
                this.Y -= (float)dy;
                
                if (this.X < -this.Width) this.X = -this.Width;
                if (this.Y < -this.Height) this.Y = -this.Height;

                this.lastTapX = x;
                this.lastTapY = y;

                this.RequestRedraw();
            }
        }

        /// <summary>
        /// 再描画を要求する
        /// </summary>
        public void RequestRedraw(bool isInvalidated = false)
        {
            if (isInvalidated)
            {
                this.InvalidateSurface();
            }
            this.RedrawRequested?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 再描画が要求された時に発行
        /// </summary>
        public event EventHandler RedrawRequested;
    }
}
