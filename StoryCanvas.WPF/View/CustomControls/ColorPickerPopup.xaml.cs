using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using StoryCanvas.Shared.ViewModels;
using StoryCanvas.WPF.Converters.Common;

namespace StoryCanvas.WPF.View.CustomControls
{
	public delegate void ColorPickedEventHandler(object sender, ColorPickedEventArgs e);
	public class ColorPickedEventArgs : EventArgs
	{
		public ColorPickedEventArgs(Color color)
		{
			this.SelectedColor = color;
		}
		public Color SelectedColor { get; private set; }
	}

	/// <summary>
	/// ColorPickerPopup.xaml の相互作用ロジック
	/// </summary>
	public partial class ColorPickerPopup : UserControl
	{
		private static ColorResourceColorConverter Converter = new ColorResourceColorConverter();
		private Border[] HistoryBorders = new Border[25];
		private Border[] CustomBorders = new Border[25];
		private ColorViewModel ViewModel = new ColorViewModel();

		public ColorPickerPopup()
		{
			InitializeComponent();

			// プリセット
			var cells = new Border[]
			{
				this.Color00, this.Color01, this.Color02, this.Color03, this.Color04,
				this.Color10, this.Color11, this.Color12, this.Color13, this.Color14,
				this.Color20, this.Color21, this.Color22, this.Color23, this.Color24,
				this.Color30, this.Color31, this.Color32, this.Color33, this.Color34,
				this.Color40, this.Color41, this.Color42, this.Color43, this.Color44,
			};
			foreach (var cell in cells)
			{
				this.SetBorderCellEvent(cell);
			}

			// カスタム
			for (var i = 0; i < 25; i++)
			{
				var border = new Border
				{
					Background = new SolidColorBrush(Converter.Convert(this.ViewModel.ColorCustom.ElementAt(i).Color)),
				};
				this.CustomBorders[i] = border;
				Grid.SetRow(border, ((int)(i / 5)));
				Grid.SetColumn(border, i % 5);
				this.CustomSelect.Children.Add(border);
				this.SetBorderCellEvent(border);
			}

			// 履歴
			for (var i = 0; i < 25; i++)
			{
				var border = new Border
				{
					Background = new SolidColorBrush(Converter.Convert(this.ViewModel.ColorHistory.ElementAt(i).Color)),
				};
				this.HistoryBorders[i] = border;
				Grid.SetRow(border, ((int)(i / 5)));
				Grid.SetColumn(border, i % 5);
				this.HistorySelect.Children.Add(border);
				this.SetBorderCellEvent(border);
			}

			// RGB入力
			this.RSlider.ValueChanged += (sender, e) => this.UpdateRGBValue();
			this.GSlider.ValueChanged += (sender, e) => this.UpdateRGBValue();
			this.BSlider.ValueChanged += (sender, e) => this.UpdateRGBValue();
		}

		/// <summary>
		/// 履歴、カスタムカラーを更新する
		/// </summary>
		public void UpdateColors()
		{
			for (var i = 0; i < 25; i++)
			{
				((SolidColorBrush)this.CustomBorders[i].Background).Color = Converter.Convert(this.ViewModel.ColorCustom[i].Color);
				((SolidColorBrush)this.HistoryBorders[i].Background).Color = Converter.Convert(this.ViewModel.ColorHistory[i].Color);
			}
		}

		public void SetColor(Color color)
		{
			// RGB入力の値を変更
			this.RSlider.Value = color.R;
			this.GSlider.Value = color.G;
			this.BSlider.Value = color.B;
		}

		private void SetBorderCellEvent(Border cell)
		{
			cell.PreviewMouseLeftButtonDown += (sender, e) =>
			{
				var color = (((Border)sender).Background as SolidColorBrush)?.Color ?? default(Color);
				this.ColorPicked(this, new ColorPickedEventArgs(color));
			};
		}

		private void UpdateRGBValue()
		{
			var brush = this.RGBPreview.Background as SolidColorBrush;
			if (brush == null)
			{
				brush = new SolidColorBrush();
				this.RGBPreview.Background = brush;
			}
			brush.Color = new Color { R = (byte)(uint)this.RSlider.Value, G = (byte)(uint)this.GSlider.Value, B = (byte)(uint)this.BSlider.Value, A = 255, };
			this.ColorPickedAndContinue(this, new ColorPickedEventArgs(brush.Color));
		}

		public event ColorPickedEventHandler ColorPicked = delegate { };
		public event ColorPickedEventHandler ColorPickedAndContinue = delegate { };
	}
}
