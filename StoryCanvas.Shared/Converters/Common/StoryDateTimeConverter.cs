using System;
using System.Collections.Generic;
using System.Text;
using StoryCanvas.Shared.Models.Date;
using StoryCanvas.Shared.ViewTools;

namespace StoryCanvas.Shared.Converters.Common
{
    /// <summary>
    /// システムの日付を、ストーリー内で使われる形式に変換するコンバータ
    /// </summary>
#if !WINDOWS_UWP
	public class StoryDateTimeConverter : ValueConverterBase<StoryDateTime, System.DateTime?>
#else
    public class StoryDateTimeConverter : ValueConverterBase<StoryDateTime, DateTimeOffset>
#endif
    {
#if !WINDOWS_UWP
        public override DateTime? Convert(StoryDateTime value)
		{
			return new System.DateTime?(new System.DateTime(value.Date.Year, value.Date.Month, value.Date.Day,
				value.Time.Hour, value.Time.Minute, value.Time.Second));
		}

		public override StoryDateTime ConvertBack(DateTime? value)
		{
			if (value.HasValue)
			{
				return new StoryDateTime
				{
					Date = StoryCalendar.AnnoDomini.Date(value.Value.Year, value.Value.Month, value.Value.Day),
					Time = StoryCalendar.AnnoDomini.Time(value.Value.Hour, value.Value.Minute, value.Value.Second)
				};
			}
			else
			{
				return null;
			}
		}
#else
        public StoryDateTimeConverter()
        {
            this.IsCheckTargetType = false;
        }

        public override DateTimeOffset Convert(StoryDateTime value)
        {
            return new System.DateTimeOffset(new System.DateTime(value.Date.Year, value.Date.Month, value.Date.Day,
                value.Time.Hour, value.Time.Minute, value.Time.Second));
        }

        public override StoryDateTime ConvertBack(DateTimeOffset value)
        {
            return new StoryDateTime
            {
                Date = StoryCalendar.AnnoDomini.Date(value.Year, value.Month, value.Day),
                Time = StoryCalendar.AnnoDomini.Time(value.Hour, value.Minute, value.Second)
            };
        }
#endif
    }
}
