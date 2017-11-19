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

        /// <summary>
        /// ドラッグ中であるか
        /// </summary>
        private bool IsDragging { get; set; }

        /// <summary>
        /// ドラッグ可能であるか（ドラッグによって画面全体が動くかどうか。サブクラスのドラッグ処理は制御できない）
        /// </summary>
        protected bool CanDrag { get; set; } = true;

        /// <summary>
        /// 描画の時に使われる引数
        /// </summary>
        public object Argument { get; set; }

        /// <summary>
        /// 描画を開始するX座標。画面スクロールで変動
        /// </summary>
        public float X { get; private set; }

        /// <summary>
        /// 描画を開始するY座標。画面スクロールで変動
        /// </summary>
        public float Y { get; private set; }

        /// <summary>
        /// キャンバスの横幅
        /// </summary>
        public int Width { get; set; } = 500;

        /// <summary>
        /// キャンバスの高さ
        /// </summary>
        public int Height { get; set; } = 500;

        /// <summary>
        /// 画面への描画を行う
        /// </summary>
        /// <param name="canvas">キャンバス</param>
        public void Draw(SKSurface surface, SKColorType colorType)
        {
            // キャッシュがない、または無効化されたときは、描画しなおす
            if (this.cache == null)
            {
                // マップを最新のサイズに更新する
                this.ResizeMap();
                
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
                            this.DrawUpdating?.Invoke(this, EventArgs.Empty);
                            this.DrawUpdate(buffer.Canvas, paint);
                        }
                    }
                    this.cache = buffer.Snapshot().ToRasterImage();
                }
            }

            // キャッシュを描画
            surface.Canvas.Translate(this.X, this.Y);
            surface.Canvas.DrawImage(this.cache, 0, 0);

            // フローティング中の要素を描画
            using (var ppaint = new SKPaint
            {
                IsAntialias = true,
            })
            {
                this.DrawFloatingElements(surface.Canvas, ppaint);
            }
        }

        /// <summary>
        /// マップのリサイズを行う
        /// </summary>
        protected virtual void ResizeMap()
        {
        }

        /// <summary>
        /// 描画内容を更新する。X、Yなどで指定された部分ではなく、全範囲を描画する
        /// </summary>
        /// <param name="canvas">キャンバス</param>
        /// <param name="paint">ペイント</param>
        protected abstract void DrawUpdate(SKCanvas canvas, SKPaint paint);

        /// <summary>
        /// フローティング中の要素を描画する
        /// </summary>
        /// <param name="canvas">キャンバス</param>
        /// <param name="paint">ペイント</param>
        protected virtual void DrawFloatingElements(SKCanvas canvas, SKPaint paint)
        {
        }
        
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
            this.IsDragging = true;
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
            this.IsDragging = false;
        }

        /// <summary>
        /// ドラッグ（押しながら移動）された時に発生
        /// </summary>
        /// <param name="x">画面上のX</param>
        /// <param name="y">画面上のY</param>
        public virtual void OnTapMove(double x, double y)
        {
            if (this.IsDragging && this.CanDrag)
            {
                var dx = this.lastTapX - x;
                var dy = this.lastTapY - y;

                this.lastTapX = x;
                this.lastTapY = y;

                this.OnDragging(dx, dy);
            }
        }

        /// <summary>
        /// ドラッグ中である時に呼び出される
        /// </summary>
        /// <param name="dx">差分X</param>
        /// <param name="dy">差分Y</param>
        protected virtual void OnDragging(double dx, double dy)
        {
            if (this.X - (float)dx < -this.Width) dx = this.X + this.Width;
            if (this.Y - (float)dy < -this.Height) dy = this.Y + this.Height;

            this.Scroll(-(float)dx, -(float)dy);

            this.RequestRedraw();
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
        /// マップの左上座標を移動し、スクロールする
        /// </summary>
        /// <param name="x">移動量</param>
        /// <param name="y">移動量</param>
        public void Scroll(float x, float y)
        {
            this.X += x;
            this.Y += y;
            this.Scrolled?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// マップの左上座標を、現在の値によらない絶対値で指定する
        /// </summary>
        /// <param name="x">移動量</param>
        /// <param name="y">移動量</param>
        protected void SetMapTranslation(float x, float y)
        {
            this.Scroll(x - this.X, y - this.Y);
        }

        /// <summary>
        /// 再描画が要求された時に発行
        /// </summary>
        public event EventHandler RedrawRequested;

        /// <summary>
        /// 描画を更新する前に発行
        /// </summary>
        public event EventHandler DrawUpdating;

        /// <summary>
        /// マップがスクロール（X、Yプロパティが変更）されたときに発行
        /// </summary>
        protected event EventHandler Scrolled;
    }
}
