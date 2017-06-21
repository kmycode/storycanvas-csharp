using StoryCanvas.Shared.Models.Entities;
using StoryCanvas.Shared.ViewModels.EntityEditControls;
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

namespace StoryCanvas.UWP.View.EntityEditor
{
    public sealed partial class PersonEditor : UserControl
    {
        public PersonEditor()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// 編集するエンティティ
        /// </summary>
        public static readonly DependencyProperty EntityProperty =
            DependencyProperty.Register(
                nameof(Entity),
                typeof(PersonEntity),
                typeof(PersonEditor),
                new PropertyMetadata(null, (sender, e) =>
                {
                    var view = (PersonEditor)sender;
                    ((PersonEditControlViewModel)view.MainPanel.DataContext).Entity = (PersonEntity)e.NewValue;
                }));
        public PersonEntity Entity
        {
            get => (PersonEntity)GetValue(EntityProperty);
            set => SetValue(EntityProperty, value);
        }
    }
}
