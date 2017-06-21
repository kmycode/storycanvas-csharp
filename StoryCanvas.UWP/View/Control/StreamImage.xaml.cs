using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace StoryCanvas.UWP.View.Control
{
    public sealed partial class StreamImage : UserControl
    {
        public StreamImage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// 画像のストリーム
        /// </summary>
        public static readonly DependencyProperty StreamProperty =
            DependencyProperty.Register(
                nameof(Stream),
                typeof(Stream),
                typeof(StreamImage),
                new PropertyMetadata(null, (sender, e) =>
                {
                    var view = (StreamImage)sender;
                    var stream = (Stream)e.NewValue;
                    if (stream != null)
                    {
                        view.Image.Source = view.Stream.ToImageSource();
                    }
                    else
                    {
                        view.Image.Source = null;
                    }
                }));
        public Stream Stream
        {
            get => (Stream)GetValue(StreamProperty);
            set => SetValue(StreamProperty, value);
        }

        /// <summary>
        /// 画像のストレッチ
        /// </summary>
        public static readonly DependencyProperty StretchProperty =
            DependencyProperty.Register(
                nameof(Stretch),
                typeof(Stretch),
                typeof(StreamImage),
                new PropertyMetadata(Stretch.Uniform, null));
        public Stretch Stretch
        {
            get => (Stretch)GetValue(StretchProperty);
            set => SetValue(StretchProperty, value);
        }
    }
}
