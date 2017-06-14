using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace StoryCanvas.View.CustomControls.Cells
{
	/// <summary>
	/// 日付選択のセル
	/// </summary>
	public partial class DateCell : ViewCell
	{
		public DateCell()
		{
			InitializeComponent();
		}

		/// <summary>
		/// テキストの依存プロパティ
		/// </summary>
		public static readonly BindableProperty LabelProperty = BindableProperty.Create(
				"Label",								// プロパティ名
				typeof(string),                         // プロパティの型
				typeof(DateCell),						// プロパティを持つ View の型
				"",					                    // 初期値
				BindingMode.TwoWay,                     // バインド方向
				null,                                   // バリデーションメソッド
														// 変更後イベントハンドラ
				(bindable, oldValue, newValue) =>
				{
					var view = bindable as DateCell;
					if (view != null)
					{
						view.TextLabel.Text = newValue as string;
					}
				},
				null,                                   // 変更時イベントハンドラ
				null);                                  // BindableProperty.CoerceValueDelegate Xamarin 公式にも説明なしなので用途不明

		/// <summary>
		/// テキスト
		/// </summary>
		public string Label
		{
			get
			{
				return (string)this.GetValue(LabelProperty);
			}
			set
			{
				this.SetValue(LabelProperty, value);
			}
		}

		/// <summary>
		/// 日付の依存プロパティ
		/// </summary>
		public static readonly BindableProperty DateProperty = /*BindableProperty.Create(
				"Date",                                // プロパティ名
				typeof(DateTime?),                       // プロパティの型
				typeof(DateCell),                       // プロパティを持つ View の型
				default(DateTime),                      // 初期値
				BindingMode.TwoWay,                     // バインド方向
				null,                                   // バリデーションメソッド
														// 変更後イベントハンドラ
				(bindable, oldValue, newValue) =>
				{
					var view = bindable as DateCell;
					if (view != null)
					{
						view.DatePicker.Date = newValue is DateTime ? (DateTime)newValue : default(DateTime);
					}
				},
														// 変更時イベントハンドラ
				null,
				null);                                  // BindableProperty.CoerceValueDelegate Xamarin 公式にも説明なしなので用途不明
				*/
		BindableProperty.Create<DateCell, DateTime?>(p => p.DatePicker.Date, default(DateTime),
				propertyChanged: (bindable, oldValue, newValue) =>
					((DateCell)bindable).DatePicker.Date = newValue.HasValue ? newValue.Value : default(DateTime));

		/// <summary>
		/// テキスト
		/// </summary>
		public DateTime Date
		{
			get
			{
				return (DateTime)this.GetValue(DateProperty);
			}
			set
			{
				this.SetValue(DateProperty, value);
			}
		}
	}
}
