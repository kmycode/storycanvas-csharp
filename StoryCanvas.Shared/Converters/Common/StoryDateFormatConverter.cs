using System;
using System.Collections.Generic;
using System.Text;
using StoryCanvas.Shared.Common;
using StoryCanvas.Shared.Models.Date;
using StoryCanvas.Shared.ViewTools;

namespace StoryCanvas.Shared.Converters.Common
{
	internal static class StoryDateTimeFormatUtil
	{
		public enum StoryDateTimeFormatType
		{
			Japan,
			USA,
			Undefined,
		}
		private static StoryDateTimeFormatType _formatType = StoryDateTimeFormatType.Undefined;
		public static StoryDateTimeFormatType FormatType
		{
			get
			{
				if (_formatType == StoryDateTimeFormatType.Undefined)
				{
					var type = StringResourceResolver.Resolve("LocationSetting_DateFormatType");
					_formatType = type == "USA" ? StoryDateTimeFormatType.USA : StoryDateTimeFormatType.Japan;
				}
				return _formatType;
			}
		}
		public static string GetDateFormat(StoryDate date)
		{
			switch (StoryDateTimeFormatUtil.FormatType)
			{
				case StoryDateTimeFormatUtil.StoryDateTimeFormatType.Japan:
					return $"{date.Year}/{date.Month}/{date.Day}";
				case StoryDateTimeFormatUtil.StoryDateTimeFormatType.USA:
					return $"{date.Month}/{date.Day}/{date.Year}";
			}
			return "-";
		}
		public static string GetTimeFormat(StoryTime time)
		{
			if (StoryDateTimeFormatUtil.FormatType == StoryDateTimeFormatUtil.StoryDateTimeFormatType.USA && (time.Calendar == null || time.Calendar == StoryCalendar.AnnoDomini))
			{
				string t = "AM";
				int hour = time.Hour;
				if (hour >= 12)
				{
					t = "PM";
					hour -= 12;
				}
				if (hour == 0)
				{
					hour = 12;
				}
				return $"{hour}:{time.Minute}:{time.Second} {t}";
			}
			return $"{time.Hour}:{time.Minute}:{time.Second}";
		}
	}

#if WPF
	public class StoryDateFormatConverter : ValueConverterBase<StoryDateTime, object>
#elif WINDOWS_UWP
	public class StoryDateFormatConverter : ValueConverterBase<StoryDateTime, object>
#elif XAMARIN_FORMS
	public class StoryDateFormatConverter : ValueConverterBase<StoryDateTime, string>
#endif
	{
#if WPF
		public override object Convert(StoryDateTime value)
#elif WINDOWS_UWP
		public override object Convert(StoryDateTime value)
#elif XAMARIN_FORMS
		public override string Convert(StoryDateTime value)
#endif
		{
			if (value != null)
			{
				return StoryDateTimeFormatUtil.GetDateFormat(value.Date);
			}
			return "-";
		}
	}

#if WPF
	public class StoryDateTimeFormatConverter : ValueConverterBase<StoryDateTime, object>
#elif WINDOWS_UWP
	public class StoryDateTimeFormatConverter : ValueConverterBase<StoryDateTime, object>
#elif XAMARIN_FORMS
	public class StoryDateTimeFormatConverter : ValueConverterBase<StoryDateTime, string>
#endif
	{
#if WPF
		public override object Convert(StoryDateTime value)
#elif WINDOWS_UWP
		public override object Convert(StoryDateTime value)
#elif XAMARIN_FORMS
		public override string Convert(StoryDateTime value)
#endif
		{
			if (value != null)
			{
				return $"{StoryDateTimeFormatUtil.GetDateFormat(value.Date)}  {StoryDateTimeFormatUtil.GetTimeFormat(value.Time)}";
			}
			return "-";
		}
	}

	public class StoryTimeFormatConverter : ValueConverterBase<StoryDateTime, string>
	{
		public override string Convert(StoryDateTime value)
		{
			if (value != null)
			{
				return StoryDateTimeFormatUtil.GetTimeFormat(value.Time);
			}
			return "-";
		}
	}
}
