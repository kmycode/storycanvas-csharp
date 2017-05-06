using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using StoryCanvas.Shared.Models.Date;
using StoryCanvas.Shared.Models.IO;
using System.Collections.ObjectModel;
using StoryCanvas.Shared.ViewTools.Resources;

namespace StoryCanvas.Shared.Models.Story
{
	[DataContract]
	public class StoryConfig : INotifyPropertyChanged
	{
		public StoryConfig()
		{
			this.Initialize(default(StreamingContext));
		}

		[OnDeserialized]
		private void Initialize(StreamingContext context)
		{
			if (this._colorCustom == null)
			{
				this._colorCustom = new List<ColorResourceWrapper>();
			}
			if (this._colorHistory == null)
			{
				this._colorHistory = new List<ColorResourceWrapper>();
			}

			// 色配列の要素数が25になるようにする
			Action<List<ColorResourceWrapper>> adjustColorCount = (colors) =>
			{
				if (colors.Count < 25)
				{
					for (var i = colors.Count; i < 25; i++)
					{
						colors.Add(new ColorResourceWrapper { Color = ColorResource.Default });
					}
				}
				else if (colors.Count > 25)
				{
					int count = colors.Count;
					for (var i = 25; i < count; i++)
					{
						colors.RemoveAt(24);
					}
				}
			};
			adjustColorCount(this._colorCustom);
			adjustColorCount(this._colorHistory);
		}

		/// <summary>
		/// 題名
		/// </summary>
		[DataMember]
		private string _title = "";
		public string Title
		{
			get
			{
				return this._title;
			}
			set
			{
				this._title = value;
				this.OnPropertyChanged();
			}
		}

		/// <summary>
		/// 作者名
		/// </summary>
		[DataMember]
		private string _authorName = "";
		public string AuthorName
		{
			get
			{
				return this._authorName;
			}
			set
			{
				this._authorName = value;
				this.OnPropertyChanged();
			}
		}

		/// <summary>
		/// 一行コメント（概要の縮小版）
		/// </summary>
		[DataMember]
		private string _comment = "";
		public string Comment
		{
			get
			{
				return this._comment;
			}
			set
			{
				this._comment = value;
				this.OnPropertyChanged();
			}
		}

		/// <summary>
		/// 概要
		/// </summary>
		[DataMember]
		private string _overview = "";
		public string Overview
		{
			get
			{
				return this._overview;
			}
			set
			{
				this._overview = value;
				this.OnPropertyChanged();
			}
		}

		/// <summary>
		/// オートセーブ
		/// ( 1.5.0で追加された仕様かつデフォルトがtrueなので、bool?にして保存 )
		/// </summary>
		[DataMember]
		private bool? _isAutoSave = true;
		public bool IsAutoSave
		{
			get
			{
#if WPF
				return this.IsAutoSaveWPF;
#elif WINDOWS_UWP
				return this.IsAutoSaveWPF;
#elif XAMARIN_FORMS
				return this.IsAutoSaveXF;
#endif
			}
		}

		/// <summary>
		/// オートセーブ（WPF）
		/// ( 1.5.0で追加された仕様かつデフォルトがtrueなので、bool?にして保存 )
		/// </summary>
		[DataMember]
		private bool? _isAutoSaveWPF = true;
		public bool IsAutoSaveWPF
		{
			get
			{
				if (this._isAutoSaveWPF == null)
				{
					this._isAutoSaveWPF = true;
				}
				return this._isAutoSaveWPF == true;
			}
			set
			{
				this._isAutoSaveWPF = value;
				this.OnPropertyChanged();
				this.OnPropertyChanged("IsAutoSave");
			}
		}

		/// <summary>
		/// オートセーブ（Xamarin.Forms＝スマホ）
		/// ( 2.0.1で追加 )
		/// </summary>
		[DataMember]
		private bool _isAutoSaveXF = false;
		public bool IsAutoSaveXF
		{
			get
			{
				return this._isAutoSaveXF;
			}
			set
			{
				this._isAutoSaveXF = value;
				this.OnPropertyChanged();
				this.OnPropertyChanged("IsAutoSave");
			}
		}

		/// <summary>
		/// メインで使う暦
		/// </summary>
		// 1.5.0追記：
		// 1.4で、StoryCalendarの各メンバにDataMemberをつけないまま、シリアライズ対象のフィールドを作ってしまった
		// なので、今後暦を保存する場合は、フィールド名として以下の名前を使わないようにする
		//[DataMember]
		//private StoryCalendar _mainCalendar = StoryCalendar.AnnoDomini;
		public StoryCalendar MainCalendar
		{
			get
			{
				return StoryCalendar.AnnoDomini;		// 1.4で追加のためnull処理
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// タイムラインで、データロード直後に表示する開始日付
		/// </summary>
		[DataMember]
		private StoryDateTime _timelineDisplayStartDateTime = null;
		public StoryDateTime TimelineDisplayStartDateTime
		{
			get
			{
				return this._timelineDisplayStartDateTime;
			}
			set
			{
				this._timelineDisplayStartDateTime = value;
			}
		}

		/// <summary>
		/// タイムラインで、データロード直後に表示する終了日付
		/// </summary>
		[DataMember]
		private StoryDateTime _timelineDisplayEndDateTime = null;
		public StoryDateTime TimelineDisplayEndDateTime
		{
			get
			{
				return this._timelineDisplayEndDateTime;
			}
			set
			{
				this._timelineDisplayEndDateTime = value;
			}
		}

		/// <summary>
		/// 章と脚本画面の印刷設定
		/// </summary>
		[DataMember]
		private PrintTextPageInfo _printChapterTextInfo = null;
		public PrintTextPageInfo PrintChapterTextInfo
		{
			get
			{
				// 2.0.0で追加したためnull処理
				if (this._printChapterTextInfo == null)
				{
					this._printChapterTextInfo = new PrintTextPageInfo();
				}
				return this._printChapterTextInfo;
			}
		}

		/// <summary>
		/// 色選択の履歴
		/// </summary>
		[DataMember]
		private List<ColorResourceWrapper> _colorHistory = null;
		public ReadOnlyCollection<ColorResourceWrapper> ColorHistory
		{
			get
			{
				return new ReadOnlyCollection<ColorResourceWrapper>(this._colorHistory);
			}
		}

		public void AddHistoryColor(ColorResource color)
		{
			for (int i = 24; i >= 1; i--)
			{
				this._colorHistory[i].Color = this._colorHistory[i - 1].Color;
			}
			this._colorHistory[0].Color = color;
		}

		/// <summary>
		/// カスタムカラー
		/// </summary>
		[DataMember]
		private List<ColorResourceWrapper> _colorCustom = null;
		public ReadOnlyCollection<ColorResourceWrapper> ColorCustom
		{
			get
			{
				return new ReadOnlyCollection<ColorResourceWrapper>(this._colorCustom);
			}
		}

#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged([CallerMemberName] string name = null)
		{
			if (PropertyChanged == null) return;

			PropertyChanged(this, new PropertyChangedEventArgs(name));
		}

#endregion
	}
}
