using Microsoft.Xaml.Interactivity;
using StoryCanvas.Shared.ViewTools.BehaviorHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;

namespace StoryCanvas.UWP.Behaviors
{
    /// <summary>
    /// エラーメッセージを表示するビヘイビア
    /// </summary>
    class ErrorMessageBehavior : Behavior<FrameworkElement>
    {
        /// <summary>
        /// ヘルパ
        /// </summary>
        public static readonly DependencyProperty HelperProperty =
            DependencyProperty.Register(
                nameof(Helper),
                typeof(ErrorMessageHelper),
                typeof(ErrorMessageBehavior),
                new PropertyMetadata(null, (sender, e) =>
                {
                    var view = (ErrorMessageBehavior)sender;
                    if (e.OldValue != null)
                    {
                        var helper = (ErrorMessageHelper)e.OldValue;
                        helper.ErrorOccured -= view.Helper_ErrorOccured;
                    }
                    if (e.NewValue != null)
                    {
                        var helper = (ErrorMessageHelper)e.NewValue;
                        helper.ErrorOccured += view.Helper_ErrorOccured;
                    }
                }));
        public ErrorMessageHelper Helper
        {
            get => (ErrorMessageHelper)GetValue(HelperProperty);
            set => SetValue(HelperProperty, value);
        }

        private async void Helper_ErrorOccured(object sender, ErrorOccuredEventArgs e)
        {
            if (this.AssociatedObject != null)
            {
                var dlg = new MessageDialog(e.Message, AppResources.Error);
                dlg.Commands.Add(new UICommand("OK"));
                await dlg.ShowAsync();
            }
        }
    }
}
