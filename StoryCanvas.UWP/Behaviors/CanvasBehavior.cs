using Microsoft.Xaml.Interactivity;
using SkiaSharp;
using StoryCanvas.Shared.View.Paint;
using StoryCanvas.UWP.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace StoryCanvas.UWP.Behaviors
{
    /// <summary>
    /// SkiaSharpとの連携を行うビヘイビア
    /// </summary>
    public class CanvasBehavior : Behavior<Border>
    {
        private bool isInvalidated;

        private Timer redrawTimer;

        /// <summary>
        /// キャンバスへの描画ロジック
        /// </summary>
        public static readonly DependencyProperty CanvasProperty =
            DependencyProperty.Register(
                nameof(Canvas),
                typeof(CanvasBase),
                typeof(CanvasBehavior),
                new PropertyMetadata(null, (s, e) =>
                {
                    var view = (CanvasBehavior)s;
                    var oldValue = e.OldValue as CanvasBase;
                    if (oldValue != null)
                    {
                        oldValue.RedrawRequested -= view.Canvas_RedrawRequested;
                    }
                    var newValue = e.NewValue as CanvasBase;
                    if (newValue != null)
                    {
                        newValue.RedrawRequested += view.Canvas_RedrawRequested;
                        view.RequestRedraw();
                    }
                }));
        public CanvasBase Canvas
        {
            get => (CanvasBase)GetValue(CanvasProperty);
            set => SetValue(CanvasProperty, value);
        }

        /// <summary>
        /// キャンバスへの描画ロジックにわたす引数
        /// </summary>
        public static readonly DependencyProperty ArgumentProperty =
            DependencyProperty.Register(
                nameof(Argument),
                typeof(object),
                typeof(CanvasBehavior),
                new PropertyMetadata(null, (s, e) =>
                {
                    ((CanvasBehavior)s).RequestRedraw();
                }));
        public object Argument
        {
            get => (object)GetValue(ArgumentProperty);
            set => SetValue(ArgumentProperty, value);
        }

        public CanvasBehavior()
        {
            // 再描画のタイマーをセットする
            this.redrawTimer = new Timer(async (state) =>
            {
                await this.Dispatcher.RunIdleAsync((e) =>
                {
                    if (this.isInvalidated)
                    {
                        this.isInvalidated = false;
                        this.Draw();
                    }
                });
            }, null, 0, 1000 / 30);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.SizeChanged += this.AssociatedObject_LayoutUpdated;
            this.AssociatedObject.PointerPressed += this.AssociatedObject_PointerPressed;
            this.AssociatedObject.PointerMoved += this.AssociatedObject_PointerMoved;
            this.AssociatedObject.PointerReleased += this.AssociatedObject_PointerReleased;
            this.AssociatedObject.PointerExited += this.AssociatedObject_PointerExited;
            this.isInvalidated = true;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.SizeChanged -= this.AssociatedObject_LayoutUpdated;
            this.AssociatedObject.PointerPressed -= this.AssociatedObject_PointerPressed;
            this.AssociatedObject.PointerMoved -= this.AssociatedObject_PointerMoved;
            this.AssociatedObject.PointerReleased -= this.AssociatedObject_PointerReleased;
            this.AssociatedObject.PointerExited -= this.AssociatedObject_PointerExited;
            this.isInvalidated = false;
        }

        private void AssociatedObject_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var p = e.GetCurrentPoint(this.AssociatedObject);
            this.Canvas?.OnTapEnd(p.Position.X, p.Position.Y);
        }

        private void AssociatedObject_PointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var p = e.GetCurrentPoint(this.AssociatedObject);
            this.Canvas?.OnTapEnd(p.Position.X, p.Position.Y);
        }

        private void AssociatedObject_PointerMoved(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var p = e.GetCurrentPoint(this.AssociatedObject);
            this.Canvas?.OnTapMove(p.Position.X, p.Position.Y);
        }

        private void AssociatedObject_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var p = e.GetCurrentPoint(this.AssociatedObject);
            this.Canvas?.OnTapStart(p.Position.X, p.Position.Y);
        }

        private void AssociatedObject_LayoutUpdated(object sender, object e)
        {
            this.RequestRedraw();
        }

        /// <summary>
        /// 再描画要求
        /// </summary>
        private void RequestRedraw()
        {
            //this.Draw();
            this.isInvalidated = true;
        }

        /// <summary>
        /// 描画
        /// </summary>
        private void Draw()
        {
            if (this.Canvas == null) return;
            if (this.AssociatedObject == null) return;
            if (this.AssociatedObject.ActualWidth < 1 && this.AssociatedObject.ActualHeight < 1) return;

            this.Canvas.Argument = this.Argument;

            using (var surface = SKSurface.Create((int)this.AssociatedObject.ActualWidth, (int)this.AssociatedObject.ActualHeight, SKColorType.Bgra8888, SKAlphaType.Premul))
            {
                if (surface == null) return;
                var canvas = surface.Canvas;

                // 描画処理
                this.Canvas.Draw(surface, SKColorType.Bgra8888);

                // 描画結果を画面に反映
                var stream = surface.Snapshot().Encode().AsStream();

                if (this.AssociatedObject.Child is Image img)
                {
                    img.Source = stream.ToImageSource();
                }
                else
                {
                    this.AssociatedObject.Child = new Image
                    {
                        Source = stream.ToImageSource(),
                        Stretch = Stretch.None,
                        HorizontalAlignment = HorizontalAlignment.Left,
                    };
                }
            }
        }

        /// <summary>
        /// 再描画が要求された時に呼び出される
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Canvas_RedrawRequested(object sender, EventArgs e)
        {
            this.RequestRedraw();
        }
    }
}
