using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StoryCanvas.UWP.Behaviors
{
    /// <summary>
    /// ColorPickerのColorプロパティをバインディングしようとしても、
    /// ValueConverterのConvertBackメソッドには、なぜかColorではなく__ComObjectがvalueとして入ってくるので
    /// 代替としてビヘイビアを作成。UWPのバグであってほしい
    /// </summary>
    class ColorPickerBindingBehavior : Behavior<ColorPicker>
    {
        private ColorPicker lastAttached;

        /// <summary>
        /// 色
        /// </summary>
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register(
                nameof(Color),
                typeof(Color),
                typeof(ColorPickerBindingBehavior),
                new PropertyMetadata(Colors.Black, OnColorChanged));
        public Color Color
        {
            get => (Color)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        private static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var b = (ColorPickerBindingBehavior)d;
            if (b.AssociatedObject == null) return;

            var color = (Color)e.NewValue;
            if (color != b.AssociatedObject.Color)
            {
                b.AssociatedObject.Color = color;
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            this.DoDetach();
            this.AssociatedObject.Color = this.Color;
            this.AssociatedObject.ColorChanged += this.AssociatedObject_ColorChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.DoDetach();
        }

        private void DoDetach()
        {
            if (this.lastAttached != null)
            {
                this.lastAttached.ColorChanged -= this.AssociatedObject_ColorChanged;
                this.lastAttached = null;
            }
        }

        private void AssociatedObject_ColorChanged(ColorPicker sender, ColorChangedEventArgs args)
        {
            this.Color = this.AssociatedObject.Color;
        }
    }
}
