using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using PCLStorage;

namespace StoryCanvas.Models.IO
{
	/// <summary>
	/// セーブ・ロードに使うスロット
	/// </summary>
	class StorageSlot : INotifyPropertyChanged
	{
		public bool IsReadOnly { get; private set; }

		public bool IsEnabled
		{
			get
			{
				return this.IsExists || !this.IsReadOnly;
			}
		}

		public StorageSlot(int slotNumber, bool isReadOnly = false)
		{
			this.SlotNumber = slotNumber;
			this.IsReadOnly = isReadOnly;
			this._slotName = AppResources.Slot + this.SlotNumber;

			Func<Task> cmd = async () =>
			{
				var rootFolder = FileSystem.Current.LocalStorage;
				this.IsExists = (await rootFolder.CheckExistsAsync(this.FileName)) == ExistenceCheckResult.FileExists;
			};

			Task.Run(cmd).Wait();
		}

		/// <summary>
		/// ファイルオブジェクトを取得
		/// </summary>
		/// <returns>ファイルオブジェクト</returns>
		public IFile GetFile()
		{
			Func<Task<IFile>> cmd = async () =>
			{
				if (this.IsExists)
				{
					return await FileSystem.Current.LocalStorage.GetFileAsync(this.FileName);
				}
				else if (!this.IsReadOnly)
				{
					return await FileSystem.Current.LocalStorage.CreateFileAsync(this.FileName, CreationCollisionOption.OpenIfExists);
				}
				return null;
			};
			var result = Task.Run(cmd);
			result.Wait();
			return result.Result;
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
				return this._slotName;
			}
			private set
			{
				this._slotName = value;
			}
		}

		/// <summary>
		/// スロットがすでに存在しているか
		/// </summary>
		private bool IsExists;

		/// <summary>
		/// スロットの詳細情報
		/// </summary>
		public string SlotDetail
		{
			get
			{
				return this.IsExists ? AppResources.ExistingSlot : AppResources.EmptySlot;
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
