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
    public sealed partial class EditRow : UserControl
    {
        public EditRow()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// 名前
        /// </summary>
        public static readonly DependencyProperty RowNameProperty =
            DependencyProperty.Register(
                nameof(RowName),
                typeof(string),
                typeof(EditRow),
                new PropertyMetadata(null, null));
        public string RowName
        {
            get => (string)GetValue(RowNameProperty);
            set => SetValue(RowNameProperty, value);
        }

        /// <summary>
        /// 編集する要素
        /// </summary>
        public static readonly DependencyProperty EditColumnProperty =
            DependencyProperty.Register(
                nameof(EditColumn),
                typeof(UIElement),
                typeof(EditRow),
                new PropertyMetadata(null, null));
        public UIElement EditColumn
        {
            get => (UIElement)GetValue(EditColumnProperty);
            set => SetValue(EditColumnProperty, value);
        }
    }
}
