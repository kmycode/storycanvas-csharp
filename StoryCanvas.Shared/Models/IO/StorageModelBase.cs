using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using PCLStorage;
using StoryCanvas.Shared.Common;
using StoryCanvas.Shared.Messages.Common;
using StoryCanvas.Shared.Models.Story;
using StoryCanvas.Shared.ViewTools;

namespace StoryCanvas.Shared.Models.IO
{
    public abstract class StorageModelBase : INotifyPropertyChanged
	{
		private static readonly object lockObject = new object();
		private bool isCheckingSaveConflict = false;

		/// <summary>
		/// ワークスペースの配列
		/// </summary>
		public ObservableCollection<StorageWorkspaceBase> Workspaces { get; } = new ObservableCollection<StorageWorkspaceBase>();

		/// <summary>
		/// ワークスペースの数
		/// </summary>
		public int WorkspaceNum { get; protected set; }

		/// <summary>
		/// 選択されているワークスペース
		/// </summary>
		private StorageWorkspaceBase _selectedWorkspace;
		public StorageWorkspaceBase SelectedWorkspace
		{
			get
			{
				return this._selectedWorkspace;
			}
			set
			{
				this._selectedWorkspace = value;
				this.OnPropertyChanged();
				this.OnPropertyChanged("Slots");
			}
		}

		/// <summary>
		/// 選択されているワークスペースのスロット
		/// </summary>
		public List<StorageSlotBase> Slots
		{
			get
			{
				return this.SelectedWorkspace?.Slots;
			}
		}

		/// <summary>
		/// 選択されているスロット
		/// </summary>
		public StorageSlotBase SelectedSlot { get; set; }

		/// <summary>
		/// 最後に使用したスロット
		/// </summary>
		private static StorageSlotBase _lastUseSlot;
		public static StorageSlotBase LastUseSlot
		{
			get
			{
				return _lastUseSlot;
			}
			private set
			{
				_lastUseSlot = value;
				//StorageModel.Default.OnPropertyChanged();
			}
		}

		/// <summary>
		/// 最後に使用したワークスペース
		/// </summary>
		public static StorageWorkspaceBase LastUseWorkspace { get; private set; }

		private static StorageModelBase _lastUseStorage;
		public static StorageModelBase LastUseStorage
		{
			get
			{
				return _lastUseStorage ?? StorageModel.Default;
			}
			protected set
			{
				_lastUseStorage = value;
				LastUseStorageChanged?.Invoke(null, new EventArgs());
			}
		}

		public static event EventHandler LastUseStorageChanged;

		/// <summary>
		/// 保存時に利用するダミーのスロット（保存処理に失敗した時に元に戻せるように）
		/// ローカルのファイル固定
		/// </summary>
		protected StorageSlot DummySlot { get; set; }

		/// <summary>
		/// オートセーブが利用できるかの状態
		/// </summary>
		private AutoSaveStatus _autoSaveStatus;
		public AutoSaveStatus AutoSaveStatus
		{
			get
			{
				return this._autoSaveStatus;
			}
			private set
			{
				this._autoSaveStatus = value;
				this.OnPropertyChanged();
			}
		}

		/// <summary>
		/// ログイン処理中であるか？
		/// </summary>
		private bool _isLogining;
		public bool IsLogining
		{
			get
			{
				return this._isLogining;
			}
			protected set
			{
				this._isLogining = value;
				this.OnPropertyChanged();
			}
		}

		/// <summary>
		/// ログインしたか？
		/// </summary>
		private bool _hasLogined;
		public bool HasLogined
		{
			get
			{
				return this._hasLogined;
			}
			protected set
			{
				if (this._hasLogined == false && value == true)
				{
					this.Initialize();
					this.IsLogining = false;
				}
				this._hasLogined = value;
				this.OnPropertyChanged();
			}
		}

		private bool _isInitialized;

		protected StorageModelBase(int workspaces)
		{
			this.WorkspaceNum = workspaces;

#if WPF
			// バインディングされた配列を非同期で操作できるように
			// 初期化は必ずUIスレッドで！
			System.Windows.Data.BindingOperations.EnableCollectionSynchronization(this.Workspaces, new object());
#endif
		}

		/// <summary>
		/// ログイン後の初期化
		/// </summary>
		private void Initialize()
		{
			if (this._isInitialized) return;
			this._isInitialized = true;

			this.CheckOldSlots();

			// 非同期処理で、どういう順番になるかわからないので、
			// あらかじめ固定長の配列を作って格納して、あとからthis.Workspaceにうつす
			var workspacesRaw = new StorageWorkspaceBase[this.WorkspaceNum];
			for (int i = 0; i < this.WorkspaceNum; i++)
			{
				var wid = i + 1;
				Action cmdAdd = () =>
				{
					var wo = this.NewStorageWorkspace(wid, false);
					workspacesRaw[wid - 1] = wo;
					//this.Workspaces.Add(this.NewStorageWorkspace(wid, false));
				};
				Task.Run(cmdAdd);
			}

			Action cmdAfter = () =>
			{
				while (workspacesRaw.Count(w => w != null) < this.WorkspaceNum)
				{
					Task.Delay(10).Wait();
				}

#if XAMARIN_FORMS
				Messenger.Default.Send(this, new UIThreadInvokeMessage(() =>
				{
#endif
					foreach (var w in workspacesRaw)
					{
						this.Workspaces.Add(w);
					}

					this.SelectedWorkspace = this.Workspaces[0];

					this.CreateDummySlot();

					StoryModel.Current.NewStoryCreating += (sender, e) =>
					{
						this.AutoSave();
					};
					StoryModel.Current.NewStoryCreated += (sender, e) =>
					{
						this.ResetLastSlot();
					};
					// XFのオートセーブを元に戻す時は、StoryModelのSetNextAutoSaveEventも修正のこと
					StoryModel.Current.AutoSaveRequested += (sender, e) =>
					{
						this.AutoSave();
					};

					// オートセーブの値を更新するためのイベントを設定する
					StoryModel.Current.PropertyChanged += (sender, e) =>
					{
						if (e.PropertyName == "StoryConfig")
						{
							this.SetStoryConfigEvent();
							this.UpdateAutoSaveStatus();
						}
					};
					this.PropertyChanged += (sender, e) =>
					{
						if (e.PropertyName == "LastUseSlot")
						{
							this.UpdateAutoSaveStatus();
						}
					};
					this.SetStoryConfigEvent();
					this.UpdateAutoSaveStatus();
#if XAMARIN_FORMS
				}));
#endif
			};
			Task.Run(cmdAfter).Wait();
		}

		/// <summary>
		/// 新しいワークスペースのインスタンスを取得
		/// </summary>
		/// <param name="number"></param>
		/// <param name="isReadOnly"></param>
		/// <returns></returns>
		protected abstract StorageWorkspaceBase NewStorageWorkspace(int number, bool isReadOnly);

		/// <summary>
		/// StoryConfigが変更された時に、StoryConfigのプロパティが変更された時のイベントを設定し直す
		/// </summary>
		private void SetStoryConfigEvent()
		{
			StoryModel.Current.StoryConfig.PropertyChanged += (sender, e) =>
			{
				if (e.PropertyName == "IsAutoSave")
				{
					this.UpdateAutoSaveStatus();
				}
			};
		}

		/// <summary>
		/// ローカルでv1.1でスロットの仕様変更したのでそれに対応（もうv1.0使ってる人はいないと思うけど一応）
		/// クラウドとかでは実装する必要はないよ
		/// </summary>
		protected virtual void CheckOldSlots() { }

		/// <summary>
		/// ダミーのスロットを作成
		/// （ローカルでもクラウドでも、必ずローカル上のものを使用する）
		/// </summary>
		protected void CreateDummySlot()
		{
			Func<Task> cmd = async () =>
			{
#if WPF
				var dummyFolderRoot = await FileSystem.Current.GetFolderFromPathAsync(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
				var dummyFolderRoot2 = await dummyFolderRoot.CreateFolderAsync("StoryCanvas", CreationCollisionOption.OpenIfExists);
				var dummyFolder = await dummyFolderRoot2.CreateFolderAsync("dummy", CreationCollisionOption.OpenIfExists);
#elif WINDOWS_UWP
				var dummyFolder = await FileSystem.Current.LocalStorage.CreateFolderAsync("dummy", CreationCollisionOption.OpenIfExists);
#elif XAMARIN_FORMS
				var dummyFolder = await FileSystem.Current.LocalStorage.CreateFolderAsync("dummy", CreationCollisionOption.OpenIfExists);
#endif
				this.DummySlot = new StorageSlot(dummyFolder, 0, "Dummy", "Dummy", false);
			};
			Task.Run(cmd);
		}

		/// <summary>
		/// スロットを保存
		/// </summary>
		public void Save()
		{
			if (this.SelectedSlot != null)
			{
				if (this.SelectedSlot.IsExists)
				{
					var message = new AlertMessage(
						StringResourceResolver.Resolve("IsOverwriteSlotMessage"),
						AlertMessageType.YesNo,
						(result) =>
						{
							if (result == AlertMessageResult.Yes)
							{
								this.Save_Private();
							}
						});
					Messenger.Default.Send(this, message);
				}
				else
				{
					this.Save_Private();
				}
			}
		}

		/// <summary>
		/// 最後に保存／読み込みしたスロットに対して保存
		/// </summary>
		public void SaveLastSlot()
		{
			if (LastUseSlot != null && LastUseSlot.IsExists)
			{
				LastUseStorage.SaveDummySlot(() => LastUseSlot);
			}
			else
			{
				Messenger.Default.Send(this, new AlertMessage(StringResourceResolver.Resolve("SaveFailedForNotChoosingSlot")));
			}
		}

		protected void Save_Private()
		{
			this.SaveDummySlot(() => this.SelectedSlot);
			LastUseSlot = this.SelectedSlot;
			LastUseWorkspace = this.SelectedWorkspace;
			LastUseStorage = this;
		}

		// slotGetter: 非同期処理でオートセーブと別スロットの読み込みが衝突し、LastUseSlotの値が変わることがあるため
		//　　　　　　　ラムダ式にしてある
		protected void SaveDummySlot(Func<StorageSlotBase> slotGetter, bool isMessage = true, bool isAutoSave = false)
		{
			lock (lockObject)
			{
				if (StoryModel.Current.SaveSlot(this.DummySlot, isMessage))
				{
					this.SaveReallySlot(slotGetter, isAutoSave);
				}
			}
		}

		/// <summary>
		/// ダミースロットへの保存が完了した時のコールバック
		/// ここから実スロットへの保存を行う
		/// </summary>
		/// <param name="slotGetter">実スロットを取得するためのデリゲート</param>
		protected abstract void SaveReallySlot(Func<StorageSlotBase> slotGetter, bool isAutoSave);

		/// <summary>
		/// オートセーブ
		/// </summary>
		private void AutoSave()
		{
			if (LastUseStorage != this) return;

			this.UpdateAutoSaveStatus();
			if (this.AutoSaveStatus == AutoSaveStatus.Enabled)
			{
				this.SaveDummySlot(() => LastUseSlot, false, true);
			}
		}

		/// <summary>
		/// オートセーブが利用できるかどうかの状態を更新
		/// </summary>
		private void UpdateAutoSaveStatus()
		{
			AutoSaveStatus value = AutoSaveStatus.Enabled;
			if (LastUseSlot == null || !LastUseSlot.IsExists)
			{
				value = AutoSaveStatus.DisabledBecauseNoSlotSelected;
			}
			else if (!StoryModel.Current.StoryConfig.IsAutoSave)
			{
				value = AutoSaveStatus.DisabledBecauseOfStoryConfig;
			}
			else if (this.isCheckingSaveConflict)
			{
				value = AutoSaveStatus.DisabledBecauseWaitingUserAction;
			}
			this.AutoSaveStatus = value;
		}

		/// <summary>
		/// オートセーブをネットワークエラーに設定する
		/// </summary>
		protected void AutoSaveNetworkError()
		{
			this.AutoSaveStatus = AutoSaveStatus.NetworkError;
		}

		/// <summary>
		/// データに競合の可能性がないか確認する
		/// （クラウドデータを他のデバイスで編集したときとか）
		/// </summary>
		/// <returns>trueなら競合の可能性あり</returns>
		private bool IsSaveConfilictFared()
		{
			var slot = LastUseSlot;
			if (slot != null)
			{
				return slot.IsConfilictFeared();
			}
			return false;
		}

		/// <summary>
		/// データに競合の可能性がないか確認し、確認とれるまでおーとせーぶなどを止める
		/// </summary>
		public void CheckSaveConflict()
		{
			if (this.isCheckingSaveConflict) return;

			this.isCheckingSaveConflict = true;
			if (this.IsSaveConfilictFared())
			{
				Messenger.Default.Send(this, new AlertMessage(StringResourceResolver.Resolve("SaveDataConflictMessage"),
					AlertMessageType.YesNo,
					(result) =>
					{
						if (result == AlertMessageResult.No)
						{
							this.ResetLastSlot();
						}
						else
						{
							LastUseSlot?.UpdateLastSaveDateTimeForce();
						}
						this.isCheckingSaveConflict = false;
					}));
			}
			else
			{
				this.isCheckingSaveConflict = false;
			}
		}

		/// <summary>
		/// 最後に保存／読み込みしたスロットのキャッシュを破棄
		/// </summary>
		public void ResetLastSlot()
		{
			LastUseSlot = null;
			LastUseWorkspace = null;
			LastUseStorage = null;
			this.UpdateAutoSaveStatus();
		}

		/// <summary>
		/// スロットを読み込み
		/// </summary>
		public void Load()
		{
			// 読み込む前に自動保存
			this.AutoSave();

			if (this.SelectedSlot != null && this.SelectedSlot.IsExists)
			{
				StoryModel.Current.LoadSlot(this.SelectedSlot);
				LastUseSlot = this.SelectedSlot;
				LastUseWorkspace = this.SelectedWorkspace;
				LastUseStorage = this;

				this.UpdateAutoSaveStatus();
			}
		}

		/// <summary>
		/// スロットを削除
		/// </summary>
		public void Delete()
		{
			if (this.SelectedSlot != null && this.SelectedSlot.IsExists)
			{
				this.SelectedSlot.Delete();
			}
		}

		/// <summary>
		/// ログイン（クラウドなどへの）
		/// </summary>
		public abstract void Login();


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
