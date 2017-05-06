using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
#if WPF
using System.Windows;
#elif XAMARIN_FORMS
using Xamarin.Forms;
#endif

namespace StoryCanvas.Shared.Models.IO
{
	[DataContract]
	public class PrintTextPageInfo : INotifyPropertyChanged
	{
		internal PrintTextPageInfo()
		{
		}

		/// <summary>
		/// 行の高さ（フォントサイズに対する倍率）
		/// </summary>
		[DataMember]
		private double _lineHeight = 1;
		public double LineHeight
		{
			get
			{
				return this._lineHeight;
			}
			set
			{
				this._lineHeight = value;
				this.OnPropertyChanged();
			}
		}

		/// <summary>
		/// フォントサイズ（px）
		/// </summary>
		[DataMember]
		private double _fontSize = 12;
		public double FontSize
		{
			get
			{
				return this._fontSize;
			}
			set
			{
				this._fontSize = value;
				this.OnPropertyChanged();
			}
		}

		/// <summary>
		/// タイトルのフォントサイズ
		/// </summary>
		[DataMember]
		private double _titleFontSize = 20;
		public double TitleFontSize
		{
			get
			{
				return this._titleFontSize;
			}
			set
			{
				this._titleFontSize = value;
				this.OnPropertyChanged();
			}
		}

		/// <summary>
		/// タイトルの下のマージン
		/// </summary>
		[DataMember]
		private double _titleMarginBottom = 20;
		public double TitleMarginBottom
		{
			get
			{
				return this._titleMarginBottom;
			}
			set
			{
				this._titleMarginBottom = value;
				this.OnPropertyChanged();
			}
		}

		/// <summary>
		/// タイトルを印刷するか
		/// </summary>
		[DataMember]
		private bool _isTitle = true;
		public bool IsTitle
		{
			get
			{
				return this._isTitle;
			}
			set
			{
				this._isTitle = value;
				this.OnPropertyChanged();
			}
		}

		/// <summary>
		/// サブタイトルのフォントサイズ
		/// </summary>
		[DataMember]
		private double _subTitleFontSize = 16;
		public double SubTitleFontSize
		{
			get
			{
				return this._subTitleFontSize;
			}
			set
			{
				this._subTitleFontSize = value;
				this.OnPropertyChanged();
			}
		}

		/// <summary>
		/// サブタイトルの下のマージン
		/// </summary>
		[DataMember]
		private double _subTitleMarginBottom = 48;
		public double SubTitleMarginBottom
		{
			get
			{
				return this._subTitleMarginBottom;
			}
			set
			{
				this._subTitleMarginBottom = value;
				this.OnPropertyChanged();
			}
		}

		/// <summary>
		/// サブタイトルを印刷するか
		/// </summary>
		[DataMember]
		private bool _isSubTitle = true;
		public bool IsSubTitle
		{
			get
			{
				return this._isSubTitle;
			}
			set
			{
				this._isSubTitle = value;
				this.OnPropertyChanged();
			}
		}

		/// <summary>
		/// テキストの下のマージン
		/// </summary>
		[DataMember]
		private double _textMarginBottom = 48;
		public double TextMarginBottom
		{
			get
			{
				return this._textMarginBottom;
			}
			set
			{
				this._textMarginBottom = value;
				this.OnPropertyChanged();
			}
		}

		/// <summary>
		/// Text の最後の改行を自動で取り除くか
		/// </summary>
		[DataMember]
		private bool _isRemoveLastNewline = true;
		public bool IsRemoveLastNewline
		{
			get
			{
				return this._isRemoveLastNewline;
			}
			set
			{
				this._isRemoveLastNewline = value;
				this.OnPropertyChanged();
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
