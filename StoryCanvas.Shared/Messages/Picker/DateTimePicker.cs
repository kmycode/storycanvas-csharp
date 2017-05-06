using System;
using System.Collections.Generic;
using System.Text;
using StoryCanvas.Shared.Models.Date;

namespace StoryCanvas.Shared.Messages.Picker
{
    public class DateTimePickerMessage
    {
		public Action<StoryDateTime> Action
		{
			get;
			private set;
		}
		public StoryCalendar Calendar
		{
			get;
			private set;
		}
		public StoryDateTime DefaultValue
		{
			get;
			private set;
		}
		public DateTimePickerMessage(StoryCalendar calendar, StoryDateTime defaultValue, Action<StoryDateTime> action)
		{
			this.Calendar = calendar;
			this.Action = action;
			this.DefaultValue = defaultValue;
		}
    }

	public class DatePickerMessage
	{
		public Action<StoryDate> Action
		{
			get;
			private set;
		}
		public StoryCalendar Calendar
		{
			get;
			private set;
		}
		public StoryDate DefaultValue
		{
			get;
			private set;
		}
		public DatePickerMessage(StoryCalendar calendar, StoryDate defaultValue, Action<StoryDate> action)
		{
			this.Calendar = calendar;
			this.Action = action;
			this.DefaultValue = defaultValue;
		}
	}

	public class TimePickerMessage
	{
		public Action<StoryTime> Action
		{
			get;
			private set;
		}
		public StoryCalendar Calendar
		{
			get;
			private set;
		}
		public StoryTime DefaultValue
		{
			get;
			private set;
		}
		public TimePickerMessage(StoryCalendar calendar, StoryTime defaultValue, Action<StoryTime> action)
		{
			this.Calendar = calendar;
			this.Action = action;
			this.DefaultValue = defaultValue;
		}
	}
}
