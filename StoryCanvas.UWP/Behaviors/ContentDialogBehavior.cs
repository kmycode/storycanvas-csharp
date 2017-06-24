using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StoryCanvas.UWP.Behaviors
{
    /// <summary>
    /// ボタンをクリックするとContentDialogを表示するビヘイビア
    /// </summary>
    class ContentDialogBehavior : Behavior<Button>
    {
        /// <summary>
        /// ダイアログ
        /// </summary>
        public static readonly DependencyProperty DialogProperty =
            DependencyProperty.Register(
                nameof(Dialog),
                typeof(ContentDialog),
                typeof(ContentDialogBehavior),
                new PropertyMetadata(null, null));
        public ContentDialog Dialog
        {
            get => (ContentDialog)GetValue(DialogProperty);
            set => SetValue(DialogProperty, value);
        }

        /// <summary>
        /// コマンドパラメータ
        /// </summary>
        public static readonly DependencyProperty PrimaryCommandParameterProperty =
            DependencyProperty.Register(
                nameof(PrimaryCommandParameter),
                typeof(object),
                typeof(ContentDialogBehavior),
                new PropertyMetadata(null, null));
        public object PrimaryCommandParameter
        {
            get => (object)GetValue(PrimaryCommandParameterProperty);
            set => SetValue(PrimaryCommandParameterProperty, value);
        }

        /// <summary>
        /// コマンドパラメータ
        /// </summary>
        public static readonly DependencyProperty SecondaryCommandParameterProperty =
            DependencyProperty.Register(
                nameof(SecondaryCommandParameter),
                typeof(object),
                typeof(ContentDialogBehavior),
                new PropertyMetadata(null, null));
        public object SecondaryCommandParameter
        {
            get => (object)GetValue(SecondaryCommandParameterProperty);
            set => SetValue(SecondaryCommandParameterProperty, value);
        }

        private Button lastButton;

        protected override void OnAttached()
        {
            base.OnAttached();
            this.DetachObject();

            this.AssociatedObject.Click += this.AssociatedObject_Click;
            this.lastButton = this.AssociatedObject;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.DetachObject();
        }

        private void DetachObject()
        {
            if (this.lastButton != null)
            {
                this.lastButton.Click -= this.AssociatedObject_Click;
                this.lastButton = null;
            }
        }

        private async void AssociatedObject_Click(object sender, RoutedEventArgs e)
        {
            if (this.Dialog != null)
            {
                this.Dialog.PrimaryButtonCommandParameter = this.PrimaryCommandParameter;
                this.Dialog.SecondaryButtonCommandParameter = this.SecondaryCommandParameter;
                await this.Dialog.ShowAsync();
            }
        }
    }
}
