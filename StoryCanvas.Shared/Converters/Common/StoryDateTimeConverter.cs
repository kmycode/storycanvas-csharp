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
	public class StoryDateTimeConverter : ValueConverterBase<StoryDateTime, System.DateTime?>
	{
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
	}
}
