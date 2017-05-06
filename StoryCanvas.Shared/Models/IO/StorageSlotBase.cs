using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using StoryCanvas.Shared.Common;
using StoryCanvas.Shared.Messages.Common;
using StoryCanvas.Shared.Messages.Page;
using StoryCanvas.Shared.ViewTools;
using StoryCanvas.Shared.Utils;

namespace StoryCanvas.Shared.Models.IO
{
    public abstract class StorageSlotBase : INotifyPropertyChanged
	{
		public delegate void SlotInfoChangedEventHandler();
		public event SlotInfoChangedEventHandler SlotInfoChanged = delegate { };

		public bool IsReadOnly { get; set; }

		public bool IsEnabled
		{
			get
			{
				return this.IsExists || !this.IsReadOnly;
			}
		}

		/// <summary>
		/// ファイル名
		/// </summary>
		public string FileName
		{
			get
			{
				return "slot" + this.SlotNumber + ".dat";
			}
		}

		/// <summary>
		/// スロット番号
		/// </summary>
		private int SlotNumber;

		/// <summary>
		/// スロット名
		/// </summary>
		private string _slotName;
		public string SlotName
		{
			get
			{
				return string.IsNullOrEmpty(this._slotName) ? StringResourceResolver.Resolve("Slot") + this.SlotNumber : this._slotName;
			}
			set
			{
				if (this._slotName != value)
				{
					this._slotName = value;
					if (!this.SlotInfoChanging)
					{
						this.SlotInfoChanged();
					}
					this.OnPropertyChanged();
				}
			}
		}

		/// <summary>
		/// スロットの名前がからであるか
		/// </summary>
		public bool IsEmptySlotName
		{
			get
			{
				return string.IsNullOrEmpty(this._slotName);
			}
		}

		/// <summary>
		/// スロットのコメント
		/// </summary>
		private string _slotComment;
		public string SlotComment
		{
			get
			{
				return this._slotComment;
			}
			set
			{
				this.CheckExists();
				var newValue =
					string.IsNullOrEmpty(value) ?
					(this.IsExists ? "" : StringResourceResolver.Resolve("EmptySlot"))
					: value;
				if (this._slotComment != newValue)
				{
					this._slotComment = newValue;
					if (!this.SlotInfoChanging)
					{
						this.SlotInfoChanged();
					}
					this.OnPropertyChanged();
				}
			}
		}

		/// <summary>
		/// スロット情報を変更中であるか。
		/// trueのままだと、スロットを変更したという情報は外部に送信されない
		/// </summary>
		private bool _slotInfoChanging = true;
		protected bool SlotInfoChanging
		{
			get
			{
				return this._slotInfoChanging;
			}
			set
			{
				this._slotInfoChanging = value;
			}
		}

		/// <summary>
		/// スロットがすでに存在しているか
		/// </summary>
		private bool _isExists;
		public bool IsExists
		{
			get
			{
				return this._isExists;
			}
			set
			{
				this._isExists = value;
				this.OnPropertyChanged();
			}
		}

		/// <summary>
		/// スロットの詳細情報
		/// </summary>
		public string SlotDetail
		{
			get
			{
				return this.IsExists ? StringResourceResolver.Resolve("ExistingSlot") : StringResourceResolver.Resolve("EmptySlot");
			}
		}

		/// <summary>
		/// 最後に保存したデータのMD5ハッシュ
		/// </summary>
		protected string LastSavedMD5 { get; private set; }

		public StorageSlotBase(int slotNumber, string name, string comment, bool isReadOnly = false)
		{
			this.SlotNumber = slotNumber;
			this.IsReadOnly = isReadOnly;
			this.SlotName = name;
		}

		/// <summary>
		/// スロットを削除
		/// </summary>
		public void Delete()
		{
			if (this.IsExists)
			{
				Messenger.Default.Send(this, new AlertMessage(StringResourceResolver.Resolve("IsDeleteSlotMessage"), AlertMessageType.YesNo, (result) =>
				{
					if (result == AlertMessageResult.Yes)
					{
						this.Delete_Private();
						this.CheckExists();
						this.SlotName = "";
						this.SlotComment = "";
						this.lastConflictCheckDate = default(DateTime);
						Messenger.Default.Send(this, new NavigationBackMessage());
					}
				}));
			}
		}

		/// <summary>
		/// データのMD5ハッシュをチェック
		/// </summary>
		/// <param name="stream"></param>
		/// <returns>ハッシュが、最後に保存したデータのハッシュと違っているか</returns>
		public bool CheckDataMD5(Stream stream)
		{
			var bytes = new byte[stream.Length];
			stream.Seek(0, SeekOrigin.Begin);
			stream.Read(bytes, 0, (int)stream.Length);
			return this.CheckDataMD5(bytes);
		}

		public bool CheckDataMD5(byte[] bytes)
		{
			var hash = AutofacUtil.MD5.FromByteArray(bytes);
			if (hash == this.LastSavedMD5)
			{
				return false;
			}
			this.LastSavedMD5 = hash;
			return true;
		}

		/// <summary>
		/// スロットの存在確認
		/// </summary>
		public abstract void CheckExists();

		/// <summary>
		/// スロットを開くストリームを取得
		/// </summary>
		/// <returns></returns>
		public abstract Stream OpenStream();

		/// <summary>
		/// スロットに書き込むストリームを取得
		/// </summary>
		/// <returns></returns>
		public abstract Stream WriteStream();

		/// <summary>
		/// スロット削除
		/// </summary>
		protected abstract void Delete_Private();

		/// <summary>
		/// スロットの最終更新日を取得
		/// </summary>
		protected DateTime LastSaveDateTime { get; set; }

		/// <summary>
		/// 最終更新日をクラウドなどに接続して確認
		/// </summary>
		/// <returns>最終更新日</returns>
		protected abstract DateTime CheckLastSaveDateTime();

		/// <summary>
		/// 最後に競合を確認した時刻
		/// </summary>
		private DateTime lastConflictCheckDate;

		/// <summary>
		/// 競合のおそれがあるか。trueならあり
		/// </summary>
		public bool IsConfilictFeared()
		{
			if (this.LastSaveDateTime == default(DateTime))
			{
				return false;
			}

			// 最後の確認から5分以上経過しないと次の確認をしない（ネット接続を頻繁にしないようにする）
			var now = DateTime.Now;
			if (this.lastConflictCheckDate.AddMinutes(5) > now)
			{
				return false;
			}
			this.lastConflictCheckDate = now;

			// 2.5分以上の差があればtrue（自身の端末の保存が誤って検出されないように）
			return (this.LastSaveDateTime.AddMinutes(2.5) < this.CheckLastSaveDateTime());
		}

		/// <summary>
		/// 最終更新日時を強制的に更新する
		/// </summary>
		public void UpdateLastSaveDateTimeForce()
		{
			this.LastSaveDateTime = DateTime.Now;
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
