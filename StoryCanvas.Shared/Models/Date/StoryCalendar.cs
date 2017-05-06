using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using StoryCanvas.Shared.ViewTools.Resources;
using System.Text.RegularExpressions;

namespace StoryCanvas.Shared.Models.Date
{
	[DataContract]
	public class StoryDateTime
	{
		[DataMember]
		private StoryDate _date;
		public StoryDate Date { get { return this._date; } set { this._date = value; } }
		[DataMember]
		private StoryTime _time;
		public StoryTime Time { get { return this._time; } set { this._time = value; } }
		
		/// <summary>
		/// DateTime から Date への暗黙の型変換を許可
		/// </summary>
		/// <param name="val"></param>
		public static implicit operator StoryDate(StoryDateTime val)
		{
			if (val != null)
			{
				return val.Date;
			}
			return null;
		}

		/// <summary>
		/// DateTime から Time への暗黙の型変換を許可
		/// </summary>
		/// <param name="val"></param>
		public static implicit operator StoryTime(StoryDateTime val)
		{
			if (val != null)
			{
				return val.Time;
			}
			return null;
		}
	}

	[DataContract]
	public class StoryCalendar
	{
		private static StoryCalendar _annoDomini = null;
		public static StoryCalendar AnnoDomini {
			get
			{
				if (_annoDomini == null)
				{
					// 西暦
					_annoDomini = new StoryCalendar();
					_annoDomini.Monthes.Add(new Month { Name = "January", DayCount = 31 });
					_annoDomini.Monthes.Add(new Month { Name = "Febrary", DayCount = 28 });
					_annoDomini.Monthes.Add(new Month { Name = "March", DayCount = 31 });
					_annoDomini.Monthes.Add(new Month { Name = "April", DayCount = 30 });
					_annoDomini.Monthes.Add(new Month { Name = "May", DayCount = 31 });
					_annoDomini.Monthes.Add(new Month { Name = "June", DayCount = 30 });
					_annoDomini.Monthes.Add(new Month { Name = "July", DayCount = 31 });
					_annoDomini.Monthes.Add(new Month { Name = "August", DayCount = 31 });
					_annoDomini.Monthes.Add(new Month { Name = "September", DayCount = 30 });
					_annoDomini.Monthes.Add(new Month { Name = "October", DayCount = 31 });
					_annoDomini.Monthes.Add(new Month { Name = "November", DayCount = 30 });
					_annoDomini.Monthes.Add(new Month { Name = "December", DayCount = 31 });
					_annoDomini.Weekdays.Add(new Weekday { Name = "Sun", Color = ColorResource.Red });
					_annoDomini.Weekdays.Add(new Weekday { Name = "Mon", Color = ColorResource.Black });
					_annoDomini.Weekdays.Add(new Weekday { Name = "Tue", Color = ColorResource.Black });
					_annoDomini.Weekdays.Add(new Weekday { Name = "Wed", Color = ColorResource.Black });
					_annoDomini.Weekdays.Add(new Weekday { Name = "Thu", Color = ColorResource.Black });
					_annoDomini.Weekdays.Add(new Weekday { Name = "Fri", Color = ColorResource.Black });
					_annoDomini.Weekdays.Add(new Weekday { Name = "Sat", Color = ColorResource.Blue });
					_annoDomini.HourMax = 24;
					_annoDomini.MinuteMax = 60;
					_annoDomini.SecondMax = 60;
					_annoDomini.IsLeapYearDelegate = (y) =>
					{
						if (y % 4 == 0)
						{
							if (y % 400 == 0)
							{
								return true;
							}
							else if (y % 100 == 0)
							{
								return false;
							}
							else
							{
								return true;
							}
						}
						return false;
					};
					_annoDomini.IsLeapMonthDelegate = (m) =>
					{
						return m == 2;
					};
					_annoDomini.ASunday = _annoDomini.Date(2, 1, 2000);
					_annoDomini.IsLocked = true;
				}
				return _annoDomini;
			}
		}

		/// <summary>
		/// この暦はロックされており変更不能であるか
		/// </summary>
		public bool IsLocked = false;

		/// <summary>
		/// 登録された月
		/// </summary>
		public readonly List<Month> Monthes;

		/// <summary>
		/// 登録された曜日
		/// </summary>
		public readonly List<Weekday> Weekdays;

		/// <summary>
		/// 初期化処理
		/// </summary>
		public StoryCalendar()
		{
			this.Monthes = new LockableList<Month>(this);
			this.Weekdays = new LockableList<Weekday>(this);
			this.LeapSizeDelegate = (y, m) =>
			{
				return this.IsLeapYearDelegate.Invoke(y) && this.IsLeapMonthDelegate.Invoke(m) ? 1 : 0;
			};
		}

		/// <summary>
		/// 時間の最大
		/// </summary>
		private int _hourMax = 24;
		public int HourMax
		{
			get
			{
				return this._hourMax;
			}
			set
			{
				if (!this.IsLocked)
				{
					this._hourMax = value;
				}
			}
		}

		/// <summary>
		/// 分の最大
		/// </summary>
		private int _minuteMax = 60;
		public int MinuteMax
		{
			get
			{
				return this._minuteMax;
			}
			set
			{
				if (!this.IsLocked)
				{
					this._minuteMax = value;
				}
			}
		}

		/// <summary>
		/// 秒の最大
		/// </summary>
		private int _secondMax = 60;
		public int SecondMax
		{
			get
			{
				return this._secondMax;
			}
			set
			{
				if (!this.IsLocked)
				{
					this._secondMax = value;
				}
			}
		}

		/// <summary>
		/// 曜日を計算する時、基準となる日付。日曜日（もしくは最初の曜日）
		/// </summary>
		private StoryDate _aSunday;
		public StoryDate ASunday
		{
			get
			{
				return this._aSunday;
			}
			set
			{
				if (!this.IsLocked)
				{
					this._aSunday = value;
				}
			}
		}

		/// <summary>
		/// 年、月を渡して、その月は通常より何日長いかを返す
		/// </summary>
		private Func<int, int, int> _leapSizeDelegate;
		public Func<int, int, int> LeapSizeDelegate
		{
			get
			{
				return this._leapSizeDelegate;
			}
			set
			{
				if (!this.IsLocked)
				{
					this._leapSizeDelegate = value;
				}
			}
		}

		/// <summary>
		/// 年を渡して、その年にうるう日が存在するかを返す
		/// </summary>
		private Predicate<int> _isLeapYearDelegate;
		public Predicate<int> IsLeapYearDelegate
		{
			get
			{
				return this._isLeapYearDelegate;
			}
			set
			{
				if (!this.IsLocked)
				{
					this._isLeapYearDelegate = value;
				}
			}
		}

		/// <summary>
		/// 月を渡して、その月にうるう日が存在するかを返す
		/// </summary>
		private Predicate<int> _isLeapMonthDelegate;
		public Predicate<int> IsLeapMonthDelegate
		{
			get
			{
				return this._isLeapMonthDelegate;
			}
			set
			{
				if (!this.IsLocked)
				{
					this._isLeapMonthDelegate = value;
				}
			}
		}

		/// <summary>
		/// 指定年月の日数を返す
		/// </summary>
		/// <param name="year">年</param>
		/// <param name="month">月</param>
		/// <returns></returns>
		public int GetDayLength(int year, int month)
		{
			if (month > 0 && month <= this.Monthes.Count)
			{
				var m = this.Monthes[month - 1];
				return m.DayCount + this.LeapSizeDelegate.Invoke(year, month);
			}
			return 0;
		}
		
		/// <summary>
		/// 現在日付（西暦）
		/// </summary>
		public static StoryDate CurrentDate
		{
			get
			{
				DateTime now = DateTime.Now;
				return StoryCalendar.AnnoDomini.Date(now.Year, now.Month, now.Day);
			}
		}

		/// <summary>
		/// 現在時刻（西暦）
		/// </summary>
		public static StoryTime CurrentTime
		{
			get
			{
				DateTime now = DateTime.Now;
				return StoryCalendar.AnnoDomini.Time(now.Hour, now.Minute, now.Second);
			}
		}

		/// <summary>
		/// 指定した日付のオブジェクトを生成
		/// </summary>
		/// <param name="year">年</param>
		/// <param name="month">月</param>
		/// <param name="day">日</param>
		/// <returns>生成された日付オブジェクト</returns>
		public StoryDate Date(int year, int month, int day)
		{
			StoryDate d = new StoryDate(this, year, month, day);
			return d;
		}

		/// <summary>
		/// 指定した時刻のオブジェクトを生成
		/// </summary>
		/// <param name="hour">時間</param>
		/// <param name="minute">分</param>
		/// <param name="second">秒</param>
		/// <returns>生成された時刻オブジェクト</returns>
		public StoryTime Time(int hour, int minute, int second)
		{
			StoryTime t = new StoryTime(this, hour, minute, second);
			return t;
		}

		/// <summary>
		/// 文字列を日付に変換
		/// </summary>
		/// <param name="str">変換元の文字列</param>
		/// <returns>変換された日付オブジェクト</returns>
		public StoryDate FromDateString(string str)
		{
			string pattern = @"(\d+)/(\d+)/(\d+)";
			if (Regex.IsMatch(str, pattern))
			{
				MatchCollection mc = Regex.Matches(str, pattern);
				if (mc.Count >= 1)
				{
					StoryDate date = this.Date(int.Parse(mc[0].Groups[1].Value),
						int.Parse(mc[0].Groups[2].Value),
						int.Parse(mc[0].Groups[3].Value));
					if (date.IsValid)
					{
						return date;
					}
				}
			}
			return null;
		}

		/// <summary>
		/// うるうが入る可能性のある月を全てピックアップ
		/// </summary>
		public List<Month> LeapMonthes
		{
			get
			{
				var leapMonthes = new List<Month>();
				for (int i = 0; i < this.Monthes.Count; i++)
				{
					if (this.IsLeapMonthDelegate.Invoke(i + 1))
					{
						leapMonthes.Add(this.Monthes[i]);
					}
				}
				return leapMonthes;
			}
		}

		/// <summary>
		/// うるうが入る可能性のある年を全てピックアップ。fromはtoより小さい必要がある
		/// </summary>
		/// <param name="from">確認する最初の年</param>
		/// <param name="to">確認する最後の年</param>
		/// <returns>うるう年になる可能性のある年のリスト</returns>
		public List<int> PickupLeapYears(int from, int to)
		{
			var leapYears = new List<int>();
			for (int i = from; i <= to; i++)
			{
				if (this.IsLeapYearDelegate.Invoke(i))
				{
					leapYears.Add(i);
				}
			}
			return leapYears;
		}

		/// <summary>
		/// 1年間の通算日数に変換
		/// </summary>
		/// <param name="date">日付</param>
		/// <returns></returns>
		public int ToYday(StoryDate date)
		{
			int yday = date.Day;
			for (int m = 0; m < date.Month - 1; m++)
			{
				yday += this.Monthes[m].DayCount + this.LeapSizeDelegate(date.Year, date.Month);
			}
			return yday;
		}

		/// <summary>
		/// 1年間の通算日数を日付に変換
		/// </summary>
		/// <param name="yday">1年間の通算日数</param>
		/// <param name="year">年</param>
		/// <returns></returns>
		public StoryDate FromYday(int yday, int year = 0)
		{
			int day = yday;
			int month = 1;
			foreach (var mth in this.Monthes)
			{
				int monthCount = mth.DayCount + this.LeapSizeDelegate(year, month);
				if (day - monthCount <= 0)
				{
					break;
				}
				day -= monthCount;
				month++;
			}
			return this.Date(year, month, day);
		}

		/// <summary>
		/// 通年の日数
		/// </summary>
		public int DaysOfYear
		{
			get
			{
				int daysOfYear = 0;
				foreach (Month m in this.Monthes)
				{
					daysOfYear += m.DayCount;
				}
				return daysOfYear;
			}
		}
		
		/// <summary>
		/// ２つの日付の日数差を取得
		/// </summary>
		/// <param name="from">古い日付</param>
		/// <param name="to">新しい日付</param>
		/// <returns>日数差。古い日付として渡された日付が新しい日付より新しければ、負の数として返される</returns>
		public int CalcDistanceDays(StoryDate from, StoryDate to)
		{
			// 古い方を古い方にする
			bool isReplaced = false;
			if (from.CompareTo(to) > 0)
			{
				StoryDate tmp = from;
				from = to;
				to = tmp;
				isReplaced = true;
			}

			// うるうが入る可能性のある月をピックアップ
			List<Month> leapMonthes = this.LeapMonthes;

			// うるうが入る年をピックアップ
			List<int> leapYears = this.PickupLeapYears(from.Year, to.Year);

			// うるうによって余計に増えた日数を計算
			int leapDays = 0;
			int monthCount = this.Monthes.Count;
			foreach (int year in leapYears)
			{
				for (int m = 1; m <= monthCount; m++)
				{
					leapDays += this.LeapSizeDelegate.Invoke(year, m);
				}
			}

			// 最初の年と最後の年のうるうによる影響日数をそれぞれ引く
			for (int m = 1; m < from.Month; m++)
			{
				leapDays -= this.LeapSizeDelegate.Invoke(from.Year, m);
			}
			for (int m = to.Month; m <= monthCount; m++)
			{
				leapDays -= this.LeapSizeDelegate.Invoke(to.Year, m);
			}

			// 通年の日数を計算
			int daysOfYear = this.DaysOfYear;

			// 日数計算
			int days = leapDays;
			if (to.Year != from.Year)
			{
				days += (to.Year - from.Year - 1) * daysOfYear;
				for (int m = 1; m <= to.Month - 1; m++)
				{
					days += this.Monthes[m - 1].DayCount;
				}
				for (int m = from.Month + 1; m <= monthCount; m++)
				{
					days += this.Monthes[m - 1].DayCount;
				}
				days += this.Monthes[from.Month - 1].DayCount - from.Day;
				days += to.Day;
			}
			else
			{
				for (int m = from.Month + 1; m <= to.Month - 1; m++)
				{
					days += this.Monthes[m - 1].DayCount;
				}
				if (to.Month == from.Month)
				{
					days += to.Day - from.Day;
				}
				else
				{
					days += this.Monthes[from.Month - 1].DayCount - from.Day;
					days += to.Day;
				}
			}

			return isReplaced ? -days : days;
		}
		
		/// <summary>
		/// ２つの時刻の秒数差を計算する
		/// </summary>
		/// <param name="from">古い時刻</param>
		/// <param name="to">新しい時刻</param>
		/// <returns>時刻差を秒数で。古い時刻として渡された時刻が新しい時刻より新しければ、負の数として返される</returns>
		public int CalcDistanceSeconds(StoryTime from, StoryTime to)
		{
			// 古い方を古い方にする
			bool isReplaced = false;
			if (from.CompareTo(to) > 0)
			{
				StoryTime tmp = from;
				from = to;
				to = tmp;
				isReplaced = true;
			}

			int seconds = 0;

			// 秒数差
			seconds += (to.Hour - from.Hour) * this.MinuteMax * this.SecondMax;
			seconds += (to.Minute - from.Minute) * this.SecondMax;
			seconds += (to.Second - from.Second);

			return isReplaced ? -seconds : seconds;
		}

		///**
		// * 指定した日付の曜日を取得
		// * @param date 指定した日付
		// * @return 曜日
		// */
		public Weekday CalcWeekday(StoryDate date)
		{
			int days = this.CalcDistanceDays(this.ASunday, date);
			int weekdaynum = days % this.Weekdays.Count;
			if (days < 0)
			{
				weekdaynum += this.Weekdays.Count;
				if (weekdaynum == this.Weekdays.Count)
				{
					weekdaynum = 0;
				}
			}
			if (--weekdaynum == -1)
			{
				weekdaynum = this.Weekdays.Count - 1;
			}
			return this.Weekdays[weekdaynum];
		}

		/// <summary>
		/// 月
		/// </summary>
		public struct Month
		{
			/// <summary>
			/// 月の名前
			/// </summary>
			public string Name;

			/// <summary>
			/// 月の日数
			/// </summary>
			public int DayCount;
		}

		/// <summary>
		/// 曜日
		/// </summary>
		public struct Weekday
		{
			/// <summary>
			/// 曜日の名前
			/// </summary>
			public string Name;

			/// <summary>
			/// 曜日の色
			/// </summary>
			public ColorResource Color;
		}
		
		/// <summary>
		/// ロック可能なリスト
		/// </summary>
		/// <typeparam name="E">リストに格納するアイテムの型</typeparam>
		private class LockableList<E> : List<E>
		{
			/// <summary>
			/// リストに関連付けられる暦
			/// </summary>
			private WeakReference<StoryCalendar> Calendar;

			/// <summary>
			/// リストに関連付けられている暦がロックされているかいなか
			/// </summary>
			public bool IsLocked
			{
				get
				{
					StoryCalendar calendar;
					if (this.Calendar.TryGetTarget(out calendar))
					{
						return calendar.IsLocked;
					}
					return false;
				}
			}

			/// <summary>
			/// リストを作成
			/// </summary>
			/// <param name="calendar">リストに関連付けられる暦</param>
			public LockableList(StoryCalendar calendar)
			{
				this.Calendar = new WeakReference<StoryCalendar>(calendar);
			}

			public new void Add(E e)
			{
				if (!this.IsLocked)
				{
					base.Add(e);
				}
			}

			public new void AddRange(IEnumerable<E> collection)
			{
				if (!this.IsLocked)
				{
					base.AddRange(collection);
				}
			}

			public new void Remove(E e)
			{
				if (!this.IsLocked)
				{
					base.Remove(e);
				}
			}

			public new void RemoveAll(Predicate<E> match)
			{
				if (!this.IsLocked)
				{
					base.RemoveAll(match);
				}
			}

			public new void RemoveAt(int index)
			{
				if (!this.IsLocked)
				{
					base.RemoveAt(index);
				}
			}

			public new void RemoveRange(int index, int count)
			{
				if (!this.IsLocked)
				{
					base.RemoveRange(index, count);
				}
			}
		}
	}
}
