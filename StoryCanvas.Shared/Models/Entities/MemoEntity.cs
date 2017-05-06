using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using StoryCanvas.Shared.Models.EntitySet;
using StoryCanvas.Shared.Models.EntitySet;

namespace StoryCanvas.Shared.Models.Entities
{
	[DataContract]
	public class MemoEntity : Entity
	{
		/// <summary>
		/// 初期化
		/// </summary>
		public MemoEntity()
		{
		}

		/// <summary>
		/// メモ本文
		/// </summary>
		[DataMember]
		private string _text;
		public string Text
		{
			get
			{
				return this._text;
			}
			set
			{
				this._text = value;
				this.OnPropertyChanged();
			}
		}

		protected override string ResourceName
		{
			get
			{
				return "memo";
			}
		}

		/// <summary>
		/// 中身が空であるか
		/// </summary>
		public override bool IsEmpty
		{
			get
			{
				return base.IsEmpty &&
					string.IsNullOrEmpty(this.Text);
			}
		}

		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="query"></param>
		protected override bool IsWordExists(string word)
		{
			if (!base.IsWordExists(word))
			{
				bool? r = false;
				r |= this.Text?.IndexOf(word) >= 0;

				return r == true;
			}
			else
			{
				return true;
			}
		}
	}
}
