using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace StoryCanvas.Shared.Models.Date
{
	[DataContract]
	public class StoryTime : IComparable<StoryTime>
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
		/// コンストラクタ。通常、StoryCalendar.Timeメソッドから生成する
		/// </summary>
		/// <param name="calendar">暦</param>
		public StoryTime(StoryCalendar calendar, int hour, int minute, int second)
		{
			this.Calendar = calendar;
			this._hour = hour;
			this._minute = minute;
			this._second = second;
		}

		/// <summary>
		/// 時間
		/// </summary>
		[DataMember]
		private int _hour;
		public int Hour
		{
			get
			{
				return this._hour;
			}
		}

		/// <summary>
		/// 分
		/// </summary>
		[DataMember]
		private int _minute;
		public int Minute
		{
			get
			{
				return this._minute;
			}
		}

		/// <summary>
		/// 秒
		/// </summary>
		[DataMember]
		private int _second;
		public int Second
		{
			get
			{
				return this._second;
			}
		}

		/// <summary>
		/// 時刻が有効であるかどうか
		/// </summary>
		public bool IsValid
		{
			get
			{
				if (this.Hour < 0 || this.Hour >= this.Calendar.HourMax)
				{
					return false;
				}
				if (this.Minute < 0 || this.Minute >= this.Calendar.MinuteMax)
				{
					return false;
				}
				if (this.Second < 0 || this.Second >= this.Calendar.SecondMax)
				{
					return false;
				}
				return true;
			}
		}

		/// <summary>
		/// 時刻を文字列に変換
		/// </summary>
		/// <returns>文字列</returns>
		public override string ToString()
		{
			return this.Hour + ":" + this.Minute + ":" + this.Second;
		}

		/// <summary>
		/// ２つの日付を比較して、どちらが先の日付かを判定。
		/// 暦は無視され、年月日の数字だけ見て判定される
		/// </summary>
		/// <param name="other">比較する対象</param>
		/// <returns>thisのほうが新しい時は1、そうでないときは-1</returns>
		public int CompareTo(StoryTime other)
		{
			if (this.Hour > other.Hour)
			{
				return 1;
			}
			else if (this.Hour < other.Hour)
			{
				return -1;
			}
			else
			{
				if (this.Minute > other.Minute)
				{
					return 1;
				}
				else if (this.Minute < other.Minute)
				{
					return -1;
				}
				else
				{
					if (this.Second > other.Second)
					{
						return 1;
					}
					else if (this.Second < other.Second)
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
		/// 指定された2つの時刻が等しいか調べる
		/// </summary>
		/// <param name="obj">比較相手</param>
		/// <returns>等価ならtrue</returns>
		public override bool Equals(object obj)
		{
			var other = obj as StoryTime;
			if (other == null) throw new ArgumentException();

			return this.Hour == other.Hour && this.Minute == other.Minute && this.Second == other.Second;
		}

		/// <summary>
		/// 時刻を比較。新しい方が大きい
		/// </summary>
		/// <param name="left">左辺</param>
		/// <param name="right">右辺</param>
		/// <returns></returns>
		public static bool operator <(StoryTime left, StoryTime right)
		{
			return left.CompareTo(right) < 0;
		}

		/// <summary>
		/// 時刻を比較。新しい方が大きい
		/// </summary>
		/// <param name="left">左辺</param>
		/// <param name="right">右辺</param>
		/// <returns></returns>
		public static bool operator >(StoryTime left, StoryTime right)
		{
			return left.CompareTo(right) > 0;
		}

		/// <summary>
		/// 時刻を比較。新しい方が大きい
		/// </summary>
		/// <param name="left">左辺</param>
		/// <param name="right">右辺</param>
		/// <returns></returns>
		public static bool operator <=(StoryTime left, StoryTime right)
		{
			return left < right || left == right;
		}

		/// <summary>
		/// 時刻を比較。新しい方が大きい
		/// </summary>
		/// <param name="left">左辺</param>
		/// <param name="right">右辺</param>
		/// <returns></returns>
		public static bool operator >=(StoryTime left, StoryTime right)
		{
			return left > right || left == right;
		}

		/// <summary>
		/// 2つの時刻が等しいか調べる
		/// </summary>
		/// <param name="left">左辺</param>
		/// <param name="right">右辺</param>
		/// <returns></returns>
		public static bool operator ==(StoryTime left, StoryTime right)
		{
			if ((object)right == null)
			{
				return (object)left == (object)right;
			}
			return left.Equals(right);
		}

		/// <summary>
		/// 2つの時刻が等しくないか調べる
		/// </summary>
		/// <param name="left">左辺</param>
		/// <param name="right">右辺</param>
		/// <returns></returns>
		public static bool operator !=(StoryTime left, StoryTime right)
		{
			if ((object)right == null)
			{
				return (object)left != (object)right;
			}
			return !left.Equals(right);
		}

		/// <summary>
		/// Time から DateTimeへの暗黙の型変換を許可
		/// </summary>
		/// <param name="val"></param>
		public static implicit operator StoryDateTime(StoryTime val)
		{
			return new StoryDateTime
			{
				Date = val.Calendar.Date(0, 1, 1),
				Time = val
			};
		}
	}
}
