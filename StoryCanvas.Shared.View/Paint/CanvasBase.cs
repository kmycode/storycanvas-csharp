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
        /// 描画の時に使われる引数
        /// </summary>
        public object Argument { get; set; }

        /// <summary>
        /// 描画のサイズを取得する
        /// </summary>
        /// <returns></returns>
        public virtual Tuple<int, int> GetCanvasSize()
        {
            return new Tuple<int, int>(500, 500);
        }

        /// <summary>
        /// 画面への描画を行う
        /// </summary>
        /// <param name="canvas">キャンバス</param>
        public abstract void Draw(SKCanvas canvas);
        
        /// <summary>
        /// タップが開始された時に発生
        /// </summary>
        /// <param name="x">画面上のX</param>
        /// <param name="y">画面上のY</param>
        public virtual void OnTapStart(double x, double y)
        {
        }

        /// <summary>
        /// タップが終了した時に発生
        /// </summary>
        /// <param name="x">画面上のX</param>
        /// <param name="y">画面上のY</param>
        public virtual void OnTapEnd(double x, double y)
        {
        }

        /// <summary>
        /// ドラッグ（押しながら移動）された時に発生
        /// </summary>
        /// <param name="x">画面上のX</param>
        /// <param name="y">画面上のY</param>
        public virtual void OnTapMove(double x, double y)
        {
        }

        /// <summary>
        /// 再描画を要求する
        /// </summary>
        public void RequestRedraw()
        {
            this.RedrawRequested?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 再描画が要求された時に発行
        /// </summary>
        public event EventHandler RedrawRequested;
    }
}
