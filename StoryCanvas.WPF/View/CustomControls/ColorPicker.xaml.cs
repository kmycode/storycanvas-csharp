using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using StoryCanvas.Shared.ViewModels;
using StoryCanvas.Shared.ViewTools.Resources;
using StoryCanvas.WPF.Converters.Common;

namespace StoryCanvas.WPF.View.CustomControls
{
	/// <summary>
	/// ColorPicker.xaml の相互作用ロジック
	/// </summary>
	public partial class ColorPicker : UserControl
	{
		private static ColorResourceColorConverter Converter = new ColorResourceColorConverter();
		private Color ColorBeforePopupOpen;
		private static Popup OpeningPopup;

		public ColorPicker()
		{
			InitializeComponent();
			var vm = new ColorViewModel();

			this.ColorPickerPopupInner.ColorPicked += (sender, e) =>
			{
				this.Color = e.SelectedColor;
				this.ColorPickerPopup.IsOpen = false;
			};
			this.ColorPickerPopupInner.ColorPickedAndContinue += (sender, e) =>
			{
				this.Color = e.SelectedColor;
			};

			// ポップアップを開いたら、開く前の色を保存する
			this.ColorPickerPopup.Opened += (sender, e) =>
			{
				// 他のポップアップが開いてないか？
				if (OpeningPopup != null)
				{
					OpeningPopup.IsOpen = false;
				}
				OpeningPopup = this.ColorPickerPopup;

				this.ColorBeforePopupOpen = this.Color;

				// 色の一覧を更新
				this.ColorPickerPopupInner.UpdateColors();
			};

			// ポップアップを閉じたら履歴に登録する
			this.ColorPickerPopup.Closed += (sender, e) =>
			{
				if (this.Color != this.ColorBeforePopupOpen)
				{
					vm.AddHistoryColor(Converter.ConvertBack(this.Color));
				}
				if (OpeningPopup == this.ColorPickerPopup)
				{
					OpeningPopup = null;
				}
			};
		}

		private void OpenPopup_Click(object sender, RoutedEventArgs e)
		{
			if (this.ColorPickerPopup.IsOpen)
			{
				this.ColorPickerPopup.IsOpen = false;
			}
			else
			{
				this.ColorPickerPopupInner.SetColor(this.Color);
				this.ColorPickerPopup.IsOpen = true;
			}
		}

		/// <summary>
		/// 色の依存プロパティ
		/// </summary>
		public static readonly DependencyProperty ColorProperty =
			DependencyProperty.Register(
				"Color",
				typeof(Color),
				typeof(ColorPicker),
				new FrameworkPropertyMetadata(
					Colors.Transparent,
					FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
					(d, e) =>
					{
						var view = d as ColorPicker;
						var newValue = (Color)e.NewValue;
						if (view != null)
						{
							// 色の表示を変更する
							view.SelectedColorDisplay.Background = new SolidColorBrush(newValue);
						}
					},
					(s, obj) =>
					{
						return obj;
					})
			);

		/// <summary>
		/// 色
		/// </summary>
		public Color Color
		{
			get
			{
				return (Color)GetValue(ColorProperty);
			}
			set
			{
				SetValue(ColorProperty, value);
			}
		}
	}
}
