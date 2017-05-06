using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoryCanvas.Converters.Common;
using StoryCanvas.Shared.Messages.Page;
using StoryCanvas.Shared.Messages.Picker;
using StoryCanvas.Shared.ViewModels;
using StoryCanvas.Shared.ViewTools;
using StoryCanvas.Shared.ViewTools.Resources;
using Xamarin.Forms;

namespace StoryCanvas.View.CustomControls.Pickers
{
	public partial class ColorPicker : ContentPage
	{
		private ColorPickerMessage Message;

		private ColorViewModel ViewModel = new ColorViewModel();
		private static ColorResourceColorConverter Converter = new ColorResourceColorConverter();

		private static ObservableCollection<ColorItem> _colorItems;
		public static ObservableCollection<ColorItem> ColorItems
		{
			get
			{
				if (_colorItems == null)
				{
					_colorItems = new ObservableCollection<ColorItem>();
					_colorItems.Add(new ColorItem { Color = new ColorResource { R = 0x00, G = 0x00, B = 0x00 }, });
					_colorItems.Add(new ColorItem { Color = new ColorResource { R = 0x69, G = 0x69, B = 0x69 }, });
					_colorItems.Add(new ColorItem { Color = new ColorResource { R = 0x80, G = 0x80, B = 0x80 }, });
					_colorItems.Add(new ColorItem { Color = new ColorResource { R = 0xa9, G = 0xa9, B = 0xa9 }, });
					_colorItems.Add(new ColorItem { Color = new ColorResource { R = 0xd3, G = 0xd3, B = 0xd3 }, });

					_colorItems.Add(new ColorItem { Color = new ColorResource { R = 0x00, G = 0x64, B = 0x00 }, });
					_colorItems.Add(new ColorItem { Color = new ColorResource { R = 0x00, G = 0x80, B = 0x00 }, });
					_colorItems.Add(new ColorItem { Color = new ColorResource { R = 0x90, G = 0xee, B = 0x90 }, });
					_colorItems.Add(new ColorItem { Color = new ColorResource { R = 0x9a, G = 0xcd, B = 0x32 }, });
					_colorItems.Add(new ColorItem { Color = new ColorResource { R = 0x00, G = 0xff, B = 0x00 }, });

					_colorItems.Add(new ColorItem { Color = new ColorResource { R = 0x00, G = 0x00, B = 0x8b }, });
					_colorItems.Add(new ColorItem { Color = new ColorResource { R = 0x00, G = 0x00, B = 0xff }, });
					_colorItems.Add(new ColorItem { Color = new ColorResource { R = 0xad, G = 0xd8, B = 0xe6 }, });
					_colorItems.Add(new ColorItem { Color = new ColorResource { R = 0x00, G = 0xff, B = 0xff }, });
					_colorItems.Add(new ColorItem { Color = new ColorResource { R = 0x80, G = 0x00, B = 0x80 }, });

					_colorItems.Add(new ColorItem { Color = new ColorResource { R = 0x8b, G = 0x00, B = 0x00 }, });
					_colorItems.Add(new ColorItem { Color = new ColorResource { R = 0xff, G = 0x00, B = 0x00 }, });
					_colorItems.Add(new ColorItem { Color = new ColorResource { R = 0xfb, G = 0x80, B = 0x72 }, });
					_colorItems.Add(new ColorItem { Color = new ColorResource { R = 0xff, G = 0xa0, B = 0x7a }, });
					_colorItems.Add(new ColorItem { Color = new ColorResource { R = 0xff, G = 0xc0, B = 0xcb }, });

					_colorItems.Add(new ColorItem { Color = new ColorResource { R = 0xff, G = 0x14, B = 0x93 }, });
					_colorItems.Add(new ColorItem { Color = new ColorResource { R = 0xee, G = 0x82, B = 0xee }, });
					_colorItems.Add(new ColorItem { Color = new ColorResource { R = 0xa5, G = 0x2a, B = 0x2a }, });
					_colorItems.Add(new ColorItem { Color = new ColorResource { R = 0xde, G = 0xb8, B = 0x87 }, });
					_colorItems.Add(new ColorItem { Color = new ColorResource { R = 0xff, G = 0xff, B = 0x00 }, });
				}
				return _colorItems;
			}
		}

		public ColorPicker(ColorPickerMessage message)
		{
			InitializeComponent();
			this.Message = message;
			this.SelectedColor = message.DefaultValue;
			this.ColorList.ItemsSource = ColorItems;

			// 色選択時のイベントを設定
			this.ColorList.ItemSelected += (sender, e) =>
			{
				this.SelectedColor = (e.SelectedItem as ColorItem)?.Color;
			};
			this.CustomList.ItemSelected += (sender, e) =>
			{
				this.SelectedColor = (e.SelectedItem as ColorResourceWrapper)?.Color;
			};
			this.HistoryList.ItemSelected += (sender, e) =>
			{
				this.SelectedColor = (e.SelectedItem as ColorResourceWrapper)?.Color;
			};

			// タブボタンクリック時のイベントを設定
			this.PresetButton.Clicked += (sender, e) =>
			{
				this.HideAllTab();
				this.ColorList.IsVisible = true;
			};
			this.CustomButton.Clicked += (sender, e) =>
			{
				this.HideAllTab();
				this.CustomList.IsVisible = true;
			};
			this.RGBButton.Clicked += (sende, e) =>
			{
				this.HideAllTab();
				this.RGBLayout.IsVisible = true;

				// スライダーの値を、現在の選択色に設定
				if (this.SelectedColor != null)
				{
					Tuple<byte, byte, byte> rgb = new Tuple<byte, byte, byte>(this.SelectedColor.R, this.SelectedColor.G, this.SelectedColor.B);
					this.RSlider.Value = rgb.Item1;
					this.GSlider.Value = rgb.Item2;
					this.BSlider.Value = rgb.Item3;
				}
			};
			this.HistoryButton.Clicked += (sender, e) =>
			{
				this.HideAllTab();
				this.HistoryList.IsVisible = true;
			};

			// RGB入力時の値を設定
			this.RSlider.ValueChanged += (sender, e) => this.UpdateRGBPreview();
			this.GSlider.ValueChanged += (sender, e) => this.UpdateRGBPreview();
			this.BSlider.ValueChanged += (sender, e) => this.UpdateRGBPreview();

			// リストの項目を設定
			this.CustomList.ItemsSource = this.ViewModel.ColorCustom;
			this.HistoryList.ItemsSource = this.ViewModel.ColorHistory;

			// ページを閉じた時に選択を確定
			this.Disappearing += (sender, e) =>
			{
				this.Submit();
			};
		}

		/// <summary>
		/// 全てのタブを隠す
		/// </summary>
		private void HideAllTab()
		{
			this.ColorList.IsVisible = this.CustomList.IsVisible = this.RGBLayout.IsVisible = this.HistoryList.IsVisible = false;
		}

		/// <summary>
		/// RGBのプレビューを更新する
		/// </summary>
		private void UpdateRGBPreview()
		{
			var color = new ColorResource(this.RSlider.Value / 255.0,
												this.GSlider.Value / 255.0,
												this.BSlider.Value / 255.0);
			this.RGBPreview.Color = Converter.Convert(color);
			this.SelectedColor = color;
		}

		/// <summary>
		/// 選択を確定
		/// </summary>
		private void Submit()
		{
			Message.Action(this.SelectedColor);

			// 履歴に追加
			this.ViewModel.AddHistoryColor(this.SelectedColor);
		}

		private ColorResource _selectedColor;
		private ColorResource SelectedColor
		{
			get
			{
				return this._selectedColor;
			}
			set
			{
				this._selectedColor = value;
			}
		}

		public class ColorItem
		{
			public ColorResource Color { get; set; }
			public string Name
			{
				get
				{
					return $"({this.Color.R},{this.Color.G},{this.Color.B})";
				}
			}
		}
	}
}
