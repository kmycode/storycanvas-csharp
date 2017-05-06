using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace StoryCanvas.Shared.Models.Date
{
	[DataContract]
	public class StoryDate : IComparable<StoryDate>
	{
		/// <summary>
		/// 日付に関連付けられた暦
		/// </summary>
		public StoryCalendar Calendar
		{
			get
			{
				return StoryCalendar.AnnoDomini;
			}
			private set
			{
			}
		}

		/// <summary>
		/// コンストラクタ。通常、StoryCalendar.Dateメソッドから生成する
		/// </summary>
		/// <param name="calendar">暦</param>
		public StoryDate(StoryCalendar calendar, int year, int month, int day)
		{
			this.Calendar = calendar;
			this._year = year;
			this._month = month;
			this._day = day;
		}

		/// <summary>
		/// 年
		/// </summary>
		[DataMember]
		private int _year;
		public int Year
		{
			get
			{
				return this._year;
			}
		}

		/// <summary>
		/// 月
		/// </summary>
		[DataMember]
		private int _month;
		public int Month
		{
			get
			{
				return this._month;
			}
		}

		/// <summary>
		/// 日
		/// </summary>
		[DataMember]
		private int _day;
		public int Day
		{
			get
			{
				return this._day;
			}
		}

		/// <summary>
		/// 日付が有効であるか
		/// </summary>
		public bool IsValid
		{
			get
			{
				// 整合性チェック
				if (this.Month <= 0 || this.Month > this.Calendar.Monthes.Count)
				{
					return false;
				}
				StoryCalendar.Month month = this.Calendar.Monthes[this.Month - 1];
				if (this.Day <= 0 ||
					this.Day > month.DayCount + this.Calendar.LeapSizeDelegate(this.Year, this.Month))
				{
					return false;
				}

				return true;
			}
		}
		
		/// <summary>
		/// 曜日を取得
		/// </summary>
		public StoryCalendar.Weekday Weekday
		{
			get
			{
				return this.Calendar.CalcWeekday(this);
			}
		}

		/// <summary>
		/// ２つの日付を加算
		/// </summary>
		/// <param name="date">加算相手</param>
		public StoryDate Add(StoryDate date)
		{
			int day = this.Day + date.Day;
			int month = this.Month + date.Month;
			int year = this.Year + date.Year;

			// 月に応じて、年を加算
			while (month > this.Calendar.Monthes.Count)
			{
				year++;
				month -= this.Calendar.Monthes.Count;
			}

			// 日数に応じて、月を加算
			while (true)
			{
				int decNum = this.Calendar.Monthes[month - 1].DayCount + this.Calendar.LeapSizeDelegate(year, month);
				if (day <= decNum)
				{
					break;
				}
				day -= decNum;
				month++;
				if (month > this.Calendar.Monthes.Count)
				{
					year++;
					month = 1;
				}
			}

			// 日付は不正であるか？（うるう年から普通の年に変わってないか）
			if (day > this.Calendar.Monthes[month - 1].DayCount + this.Calendar.LeapSizeDelegate(year, month))
			{
				day -= this.Calendar.Monthes[month - 1].DayCount;
				month++;
				if (month > this.Calendar.Monthes.Count)
				{
					month = 1;
					year++;
				}
			}

			return this.Calendar.Date(year, month, day);
		}

		/// <summary>
		/// ２つの日付を減算
		/// </summary>
		/// <param name="date">減算相手</param>
		/// <returns></returns>
		public StoryDate Sub(StoryDate date)
		{
			int day = this.Day - date.Day;
			int month = this.Month - date.Month;
			int year = this.Year - date.Year;

			// 月に応じて、年を減算
			while (month < 1)
			{
				year--;
				month += this.Calendar.Monthes.Count;
			}

			// 日数に応じて、月を減算
			while (day < 1)
			{
				int monthDayCount = this.Calendar.Monthes[month >= 2 ? month - 2 : this.Calendar.Monthes.Count - 1].DayCount;
				int addNum = monthDayCount + this.Calendar.LeapSizeDelegate(year, month - 1);
				day += addNum;
				month--;
				if (month < 1)
				{
					year--;
					month = this.Calendar.Monthes.Count;
				}
			}

			// 日付は不正であるか？（うるう年から普通の年に変わってないか）
			if (day > this.Calendar.Monthes[month - 1].DayCount + this.Calendar.LeapSizeDelegate(year, month))
			{
				day -= this.Calendar.Monthes[month - 1].DayCount;
				month++;
				if (month > this.Calendar.Monthes.Count)
				{
					month = 1;
					year++;
				}
			}

			return this.Calendar.Date(year, month, day);
		}

		/// <summary>
		/// 1年間の通算日数に変換
		/// </summary>
		/// <returns></returns>
		public int ToYday()
		{
			return this.Calendar.ToYday(this);
		}

		/// <summary>
		/// 日付を文字列に変換
		/// </summary>
		/// <returns>文字列</returns>
		public override string ToString()
		{
			return this.Year + "/" + this.Month + "/" + this.Day;
		}

		/// <summary>
		/// ２つの日付を比較して、どちらが先の日付かを判定。
		/// 暦は無視され、年月日の数字だけ見て判定される
		/// </summary>
		/// <param name="other">比較する対象</param>
		/// <returns>thisのほうが新しい時は1、そうでないときは-1</returns>
		public int CompareTo(StoryDate other)
		{
			if (this.Year > other.Year)
			{
				return 1;
			}
			else if (this.Year < other.Year)
			{
				return -1;
			}
			else
			{
				if (this.Month > other.Month)
				{
					return 1;
				}
				else if (this.Month < other.Month)
				{
					return -1;
				}
				else
				{
					if (this.Day > other.Day)
					{
						return 1;
					}
					else if (this.Day < other.Day)
					{
						return -1;
					}
					else
					{
						return 0;
					}
				}
			}
		}

		/// <summary>
		/// 指定された2つの日付が等しいか調べる
		/// </summary>
		/// <param name="obj">比較相手</param>
		/// <returns>等価ならtrue</returns>
		public override bool Equals(object obj)
		{
			var other = obj as StoryDate;
			if (other == null) throw new ArgumentException();

			return this.Year == other.Year && this.Month == other.Month && this.Day == other.Day;
		}

		/// <summary>
		/// 日付を比較。新しい方が大きい
		/// </summary>
		/// <param name="left">左辺</param>
		/// <param name="right">右辺</param>
		/// <returns></returns>
		public static bool operator <(StoryDate left, StoryDate right)
		{
			return left.CompareTo(right) < 0;
		}

		/// <summary>
		/// 日付を比較。新しい方が大きい
		/// </summary>
		/// <param name="left">左辺</param>
		/// <param name="right">右辺</param>
		/// <returns></returns>
		public static bool operator >(StoryDate left, StoryDate right)
		{
			return left.CompareTo(right) > 0;
		}

		/// <summary>
		/// 日付を比較。新しい方が大きい
		/// </summary>
		/// <param name="left">左辺</param>
		/// <param name="right">右辺</param>
		/// <returns></returns>
		public static bool operator <=(StoryDate left, StoryDate right)
		{
			return left < right || left == right;
		}

		/// <summary>
		/// 日付を比較。新しい方が大きい
		/// </summary>
		/// <param name="left">左辺</param>
		/// <param name="right">右辺</param>
		/// <returns></returns>
		public static bool operator >=(StoryDate left, StoryDate right)
		{
			return left > right || left == right;
		}

		/// <summary>
		/// 2つの日付が等しいか調べる
		/// </summary>
		/// <param name="left">左辺</param>
		/// <param name="right">右辺</param>
		/// <returns></returns>
		public static bool operator ==(StoryDate left, StoryDate right)
		{
			if ((object)right == null)
			{
				return (object)left == (object)right;
			}
			return left.Equals(right);
		}

		/// <summary>
		/// 2つの日付が等しくないか調べる
		/// </summary>
		/// <param name="left">左辺</param>
		/// <param name="right">右辺</param>
		/// <returns></returns>
		public static bool operator !=(StoryDate left, StoryDate right)
		{
			if ((object)right == null)
			{
				return (object)left != (object)right;
			}
			return !left.Equals(right);
		}

		/// <summary>
		/// 2つの日付を加算する
		/// </summary>
		/// <param name="left">左辺</param>
		/// <param name="right">右辺</param>
		/// <returns></returns>
		public static StoryDate operator +(StoryDate left, StoryDate right)
		{
			return left.Add(right);
		}

		/// <summary>
		/// 2つの日付を減算する
		/// </summary>
		/// <param name="left">左辺</param>
		/// <param name="right">右辺</param>
		/// <returns></returns>
		public static StoryDate operator -(StoryDate left, StoryDate right)
		{
			return left.Sub(right);
		}

		/// <summary>
		/// Date から DateTimeへの暗黙の型変換を許可
		/// </summary>
		/// <param name="val"></param>
		public static implicit operator StoryDateTime(StoryDate val)
		{
			if (val == null)
			{
				return null;
			}
			return new StoryDateTime
			{
				Date = val,
				Time = val.Calendar == null ? StoryCalendar.AnnoDomini.Time(0,0,0) : val.Calendar.Time(0, 0, 0)
			};
		}
	}
}
