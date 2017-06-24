using Microsoft.Xaml.Interactivity;
using StoryCanvas.Shared.Types;
using StoryCanvas.UWP.View.Frame;
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
    /// MainModeに追従して、フレームの画面遷移をおこなうクラス
    /// </summary>
    class MainModeFrameBehavior : Behavior<Frame>
    {
        /// <summary>
        /// メインモード
        /// </summary>
        public static readonly DependencyProperty MainModeProperty =
            DependencyProperty.Register(
                nameof(MainMode),
                typeof(MainMode),
                typeof(MainModeFrameBehavior),
                new PropertyMetadata(MainMode.None, (s, e) =>
                {
                    ((MainModeFrameBehavior)s).OnMainModeUpdated();
                }));
        public MainMode MainMode
        {
            get => (MainMode)GetValue(MainModeProperty);
            set => SetValue(MainModeProperty, value);
        }

        private void OnMainModeUpdated()
        {
            if (this.AssociatedObject == null)
            {
                return;
            }

            switch (this.MainMode)
            {
                case MainMode.EditPerson:
                    this.AssociatedObject.Navigate(typeof(EditPersonPage));
                    break;
                case MainMode.StoragePage:
                    this.AssociatedObject.Navigate(typeof(StoragePage));
                    break;
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            this.OnMainModeUpdated();
        }
    }
}
