using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoryCanvas.Converters.Common;
using StoryCanvas.Shared.ViewTools.Resources;
using Xamarin.Forms;

namespace StoryCanvas.View.CustomControls
{
	public partial class EntityIconCell : ViewCell
	{
		private static readonly ColorResourceColorConverter ColorConverter = new ColorResourceColorConverter();

		/// <summary>
		/// デフォルトの高さ
		/// </summary>
		private double viewDefaultHeight;

		public EntityIconCell()
		{
			InitializeComponent();
			this.viewDefaultHeight = this.StackPanel.Height;
			this.StackPanel.MeasureInvalidated += (sender, e) =>
			{
				this.ForceUpdateSize();
			};
		}

		/// <summary>
		/// アイテムソースのプロパティ
		/// </summary>
		public static readonly BindableProperty ImageResourceProperty =
			BindableProperty.Create<EntityIconCell, ImageResource>(
				p => p.ImageResource,
				null,
				propertyChanged: (bindable, oldValue, newValue) =>
				{
					var view = bindable as EntityIconCell;
					if (view != null)
					{
						var res = newValue as ImageResource;
						view.Image.ImageResource = res;
					}
				});

		/// <summary>
		/// アイテムソース
		/// </summary>
		public ImageResource ImageResource
		{
			get { return (ImageResource)GetValue(ImageResourceProperty); }
			set { SetValue(ImageResourceProperty, value); }
		}

		/// <summary>
		/// 色のプロパティ
		/// </summary>
		public static readonly BindableProperty ColorProperty =
			BindableProperty.Create<EntityIconCell, Color>(
				p => p.Color,
				default(Color),
				propertyChanged: (bindable, oldValue, newValue) =>
				{
					var view = bindable as EntityIconCell;
					if (view != null)
					{
						view.ImageBackground.Color = newValue;
					}
				});

		/// <summary>
		/// 色
		/// </summary>
		public Color Color
		{
			get { return (Color)GetValue(ColorProperty); }
			set { SetValue(ColorProperty, value); }
		}

		/// <summary>
		/// 色ソースのプロパティ
		/// </summary>
		public static readonly BindableProperty ColorResourceProperty =
			BindableProperty.Create<EntityIconCell, ColorResource>(
				p => p.ColorResource,
				null,
				propertyChanged: (bindable, oldValue, newValue) =>
				{
					var view = bindable as EntityIconCell;
					if (view != null)
					{
						var res = newValue as ColorResource;
						view.ImageBackground.Color = newValue != null ? ColorConverter.Convert(newValue) : Color.Gray;
					}
				});

		/// <summary>
		/// 色ソース
		/// </summary>
		public ColorResource ColorResource
		{
			get { return (ColorResource)GetValue(ColorResourceProperty); }
			set { SetValue(ColorResourceProperty, value); }
		}

		/// <summary>
		/// テキストのプロパティ
		/// </summary>
		public static readonly BindableProperty TextProperty =
			BindableProperty.Create<EntityIconCell, string>(
				p => p.Text,
				null,
				propertyChanged: (bindable, oldValue, newValue) =>
				{
					var view = bindable as EntityIconCell;
					if (view != null)
					{
						var res = newValue as string;
						view.Label.Text = res;
					}
				});

		/// <summary>
		/// テキスト
		/// </summary>
		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}

		/// <summary>
		/// パディングのプロパティ
		/// </summary>
		public static readonly BindableProperty PaddingProperty =
			BindableProperty.Create<EntityIconCell, Thickness>(
				p => p.Padding,
				default(Thickness),
				propertyChanged: (bindable, oldValue, newValue) =>
				{
					var view = bindable as EntityIconCell;
					if (view != null)
					{
						var res = (Thickness)newValue;
						view.StackPanel.Padding = res;
					}
				});

		/// <summary>
		/// パディング
		/// </summary>
		public Thickness Padding
		{
			get { return (Thickness)GetValue(PaddingProperty); }
			set { SetValue(PaddingProperty, value); }
		}

		/// <summary>
		/// 可視のプロパティ
		/// </summary>
		public static readonly BindableProperty IsVisibleProperty =
			BindableProperty.Create<EntityIconCell, bool>(
				p => p.IsVisible,
				true,
				propertyChanged: (bindable, oldValue, newValue) =>
				{
					var view = bindable as EntityIconCell;
					if (view != null)
					{
						var res = (bool)newValue;
						if (!res)
						{
							view.Height = 1;
							view.StackPanel.IsVisible = false;
							view.StackPanel.HeightRequest = 0;
							view.Grid.HeightRequest = 0;
							view.Image.HeightRequest = 0;
						}
						else
						{
							view.Height = view.viewDefaultHeight;
							view.StackPanel.IsVisible = true;
							view.StackPanel.HeightRequest = view.viewDefaultHeight;
							view.Grid.HeightRequest = 44;
							view.Image.HeightRequest = 44;
						}
					}
				});

		/// <summary>
		/// 可視
		/// </summary>
		public bool IsVisible
		{
			get { return (bool)GetValue(IsVisibleProperty); }
			set { SetValue(IsVisibleProperty, value); }
		}
	}
}
