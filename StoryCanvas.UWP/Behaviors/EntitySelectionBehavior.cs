using Microsoft.Xaml.Interactivity;
using StoryCanvas.Shared.Models.Entities;
using StoryCanvas.Shared.ViewModels.EntityEditors;
using StoryCanvas.Shared.ViewTools.BehaviorHelpers;
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
    /// エンティティの選択を実行し、結果を返すビヘイビア
    /// </summary>
    class EntitySelectionBehavior : Behavior<Button>
    {
        /// <summary>
        /// エンティティの種類
        /// </summary>
        public static readonly DependencyProperty EntityTypeProperty =
            DependencyProperty.Register(
                nameof(EntityType),
                typeof(EntityType),
                typeof(EntitySelectionBehavior),
                new PropertyMetadata(EntityType.Person, null));
        public EntityType EntityType
        {
            get => (EntityType)GetValue(EntityTypeProperty);
            set => SetValue(EntityTypeProperty, value);
        }

        /// <summary>
        /// ヘルパ
        /// </summary>
        public static readonly DependencyProperty HelperProperty =
            DependencyProperty.Register(
                nameof(Helper),
                typeof(IEntitySelectionOpener),
                typeof(EntitySelectionBehavior),
                new PropertyMetadata(null, (sender, e) =>
                {
                    var view = (EntitySelectionBehavior)sender;
                    var oldHelper = (IEntitySelectionOpener)e.OldValue;
                    if (oldHelper != null)
                    {
                        oldHelper.SelectionRequested -= view.Helper_SelectionRequested;
                    }
                    var newHelper = (IEntitySelectionOpener)e.NewValue;
                    if (newHelper != null)
                    {
                        newHelper.SelectionRequested += view.Helper_SelectionRequested;
                    }
                }));
        public IEntitySelectionOpener Helper
        {
            get => (IEntitySelectionOpener)GetValue(HelperProperty);
            set => SetValue(HelperProperty, value);
        }

        /// <summary>
        /// 選択画面を表示することを要求された時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Helper_SelectionRequested(object sender, EventArgs e)
        {
            // フライアウトを設定
            var flyout = this.AssociatedObject.Flyout as Flyout;
            if (flyout == null)
            {
                this.AssociatedObject.Flyout = flyout = new Flyout();
            }

            // フライアウトのサイズを設定
            var style = new Style(typeof(FlyoutPresenter));
            style.Setters.Add(new Setter(FlyoutPresenter.MaxWidthProperty, Window.Current.CoreWindow.Bounds.Width));
            flyout.SetValue(Flyout.FlyoutPresenterStyleProperty, style);

            // フライアウトの中身を設定
            FrameworkElement content = null;
            switch (this.EntityType)
            {
                case EntityType.Person:
                    content = new EditPersonPage();
                    ((PersonEditorViewModel)content.DataContext).IsSelectOnlyMode = true;
                    break;
            }
            content.Width = 600;
            content.Height = 480;
            var okButton = new Button
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(12, 4, 12, 4),
                Content = "OK",
            };
            var grid = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition(),
                    new RowDefinition
                    {
                        Height = new GridLength(1, GridUnitType.Auto),
                    },
                },
                Children =
                {
                    content,
                    okButton,
                },
            };
            Grid.SetRow(okButton, 1);
            flyout.Content = grid;

            // OKボタンのイベントを設定
            okButton.Click += this.Flyout_Closed;
        }

        private void Flyout_Closed(object sender, object e)
        {
            var flyout = this.AssociatedObject.Flyout as Flyout;
            var content = (flyout.Content as Grid).Children.First() as FrameworkElement;

            this.Helper.NotifySelectedEntity(((IEntityPickerViewModel<Entity>)content.DataContext).SelectedEntity);

            flyout.Hide();
        }
    }
}
