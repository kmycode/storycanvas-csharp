using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StoryCanvas.UWP.Behaviors
{
    /// <summary>
    /// 本当にやるか二択できくビヘイビア
    /// </summary>
    class PromptBehavior : Behavior<Button>
    {
        /// <summary>
        /// コマンド
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(
                nameof(Command),
                typeof(ICommand),
                typeof(PromptBehavior),
                new PropertyMetadata(null, null));
        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        /// <summary>
        /// コマンドパラメータ
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register(
                nameof(CommandParameter),
                typeof(object),
                typeof(PromptBehavior),
                new PropertyMetadata(null, null));
        public object CommandParameter
        {
            get => (object)GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        /// <summary>
        /// 本当に削除するか尋ねるメッセージ
        /// </summary>
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register(
                nameof(Message),
                typeof(object),
                typeof(PromptBehavior),
                new PropertyMetadata(null, null));
        public object Message
        {
            get => (object)GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }

        private Button lastAttached;

        protected override void OnAttached()
        {
            base.OnAttached();
            this.DetachEvents();
            this.lastAttached = this.AssociatedObject;

            if (this.AssociatedObject != null)
            {
                this.AssociatedObject.Click += this.AssociatedObject_Click;
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.DetachEvents();
        }

        private void DetachEvents()
        {
            if (this.lastAttached == null) return;
            this.lastAttached.Click -= this.AssociatedObject_Click;
            this.lastAttached = null;
        }

        private async void AssociatedObject_Click(object sender, RoutedEventArgs e)
        {
            if (this.Command != null)
            {
                if (this.Command.CanExecute(this.CommandParameter))
                {
                    var dlg = new MessageDialog(this.Message?.ToString() ?? "", AppResources.ApplicationName);
                    dlg.Commands.Add(new UICommand(AppResources.Yes));
                    dlg.Commands.Add(new UICommand(AppResources.No));
                    var result = await dlg.ShowAsync();

                    if (result.Label == AppResources.Yes)
                    {
                        this.Command.Execute(this.CommandParameter);
                    }
                }
            }
        }
    }
}
