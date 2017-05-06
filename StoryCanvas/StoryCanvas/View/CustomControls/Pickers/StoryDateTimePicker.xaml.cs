using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoryCanvas.Shared.Messages.Page;
using StoryCanvas.Shared.Messages.Picker;
using StoryCanvas.Shared.Models.Date;
using StoryCanvas.Shared.ViewTools;
using Xamarin.Forms;

namespace StoryCanvas.View.CustomControls.Pickers
{
	public partial class StoryDateTimePicker : ContentPage
	{
		private bool IsEnabledValue;
		private DateTimePickerMessage Message;
		private bool isCleared = false;

		public StoryDateTimePicker(DateTimePickerMessage message)
		{
			InitializeComponent();

			this.Message = message;
			for (int i = 0; i < 12; i++)
			{
				this.MonthInput.Items.Add((i + 1).ToString());
			}

			// 標準Pickerの値変更時のイベント
			this.NativePicker.DateSelected += (sender, e) =>
			{
				this.UpdateManualPicker();
			};

			// カスタム入力部分でバリデーションチェック
			this.YearInput.TextChanged += (sender, e) =>
			{
				this.ValidationCheck();
				this.UpdateNativePicker();
				this.SetDaySelectItems();
			};
			this.MonthInput.SelectedIndexChanged += (sender, e) =>
			{
				this.ValidationCheck();
				this.UpdateNativePicker();
				this.SetDaySelectItems();
			};
			this.DayInput.SelectedIndexChanged += (sender, e) =>
			{
				this.ValidationCheck();
				this.UpdateNativePicker();
			};

			// ページ閉じた時のイベント
			this.Disappearing += (sender, e) =>
			{
				if (!this.isCleared)
				{
					if (this.IsEnabledValue)
					{
						this.Message.Action.Invoke(
							new StoryDateTime
							{
								Date = this.Message.Calendar.Date(this.GetYear(), this.GetMonth(), this.GetDay()),
								Time = this.Message.Calendar.Time(this.NativeTimePicker.Time.Hours,
																	this.NativeTimePicker.Time.Minutes,
																	this.NativeTimePicker.Time.Seconds),
							});
					}
				}
			};

			// クリアボタンクリック時のイベント
			this.Clear.Clicked += (sender, e) =>
			{
				this.Message.Action.Invoke(null);
				this.isCleared = true;
				Messenger.Default.Send(this, new NavigationBackMessage());
			};

			// メッセージで指定されたデフォルトの日付を記入する
			if (message.DefaultValue != null)
			{
				this.YearInput.Text = message.DefaultValue.Date.Year.ToString();
				this.MonthInput.SelectedIndex = message.DefaultValue.Date.Month - 1;
				this.DayInput.SelectedIndex = message.DefaultValue.Date.Day - 1;
				this.NativeTimePicker.Time = new TimeSpan(message.DefaultValue.Time.Hour,
															message.DefaultValue.Time.Minute,
															message.DefaultValue.Time.Second);
			}

			// DatePickerのDateに格納するDateTimeはstructであり、nullをもてない
			// ここでは、DatePickerの値を手動入力の値に反映する
			this.UpdateManualPicker();
		}

		private int GetYear()
		{
			int year = 0;
			int.TryParse(this.YearInput.Text, out year);
			return year;
		}

		private bool IsValidatedYear
		{
			get
			{
				int year;
				return int.TryParse(this.YearInput.Text, out year);
			}
		}

		private int GetMonth()
		{
			return this.MonthInput.SelectedIndex + 1;
		}

		private int GetDay()
		{
			return this.DayInput.SelectedIndex + 1;
		}

		private void SetDaySelectItems()
		{
			int oldSelectedIndex = this.DayInput.SelectedIndex;

			this.DayInput.Items.Clear();
			if (!this.IsValidatedYear)
			{
				return;
			}
			int daynum = this.Message.Calendar.GetDayLength(this.GetYear(), this.GetMonth());
			for (int i = 0; i < daynum; i++)
			{
				this.DayInput.Items.Add((i + 1).ToString());
			}

			this.DayInput.SelectedIndex = oldSelectedIndex < daynum ? oldSelectedIndex : daynum - 1;
		}

		private void ValidationCheck()
		{
			this.IsEnabledValue = (this.DayInput.SelectedIndex >= 0 && this.MonthInput.SelectedIndex >= 0 && this.IsValidatedYear);
		}

		private bool IsChangingPickerByCode = false;
		private void UpdateNativePicker()
		{
			if (this.IsEnabledValue && !this.IsChangingPickerByCode)
			{
				this.IsChangingPickerByCode = true;
				try
				{
					this.NativePicker.Date = new DateTime(this.GetYear(), this.GetMonth(), this.GetDay());
					this.NativePicker.IsEnabled = this.IsSameValues();
				}
				catch (ArgumentOutOfRangeException)
				{
					this.NativePicker.IsEnabled = false;
				}
				this.IsChangingPickerByCode = false;
			}
		}

		private void UpdateManualPicker()
		{
			if (!this.IsChangingPickerByCode)
			{
				this.IsChangingPickerByCode = true;
				this.YearInput.Text = this.NativePicker.Date.Year.ToString();
				this.MonthInput.SelectedIndex = this.NativePicker.Date.Month - 1;
				this.DayInput.SelectedIndex = this.NativePicker.Date.Day - 1;
				this.IsChangingPickerByCode = false;
			}
		}

		private bool IsSameValues()
		{
			return this.NativePicker.Date.Year == this.GetYear() && this.NativePicker.Date.Month == this.GetMonth() && this.NativePicker.Date.Day == this.GetDay();
		}
	}
}
