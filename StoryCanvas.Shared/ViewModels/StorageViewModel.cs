using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using StoryCanvas.Shared.Common;
using StoryCanvas.Shared.Messages.Common;
using StoryCanvas.Shared.Messages.IO;
using StoryCanvas.Shared.Messages.Page;
using StoryCanvas.Shared.Models.IO;
using StoryCanvas.Shared.Models.Story;
using StoryCanvas.Shared.Utils;
using StoryCanvas.Shared.ViewTools;

namespace StoryCanvas.Shared.ViewModels
{
    public class StorageViewModel : ViewModelBase
	{
		public ObservableCollection<StorageModelBase> StorageModels { get; } = new ObservableCollection<StorageModelBase>
		{
			StorageModel.Default,
			AutofacUtil.OneDriveStorage,
			DropboxStorageModel.Default,
		};

		private StorageModelBase _model;
		public StorageModelBase Model
		{
			get
			{
				return this._model ?? this.StorageModels[0];
			}
			set
			{
				this._model = value;
				this.OnPropertyChanged("");
			}
		}

		public StorageViewModel()
		{
			foreach (var model in this.StorageModels)
			{
				this.StoreModel(model);
			}
			StorageModel.Default.Login();
		}

		/// <summary>
		/// ワークスペースの配列
		/// </summary>
		public ObservableCollection<StorageWorkspaceBase> Workspaces
		{
			get
			{
				return this.Model.Workspaces;
			}
		}

		/// <summary>
		/// 選択されているワークスペース
		/// </summary>
		public StorageWorkspaceBase SelectedWorkspace
		{
			get
			{
				return this.Model.SelectedWorkspace;
			}
			set
			{
				if (this.Model.Workspaces.Contains(value))
				{
					this.Model.SelectedWorkspace = value;
				}
				else
				{
					this.Model.SelectedWorkspace = this.Model.Workspaces.FirstOrDefault();
				}
			}
		}

		public List<StorageSlotBase> Slots
		{
			get
			{
				return this.Model.Slots;
			}
		}

		/// <summary>
		/// 選択されているスロット
		/// </summary>
		public StorageSlotBase SelectedSlot
		{
			get
			{
				return this.Model.SelectedSlot;
			}
			set
			{
				this.Model.SelectedSlot = value;
			}
		}

		/// <summary>
		/// ログイン中であるか
		/// </summary>
		public bool IsLogining => this.Model.IsLogining;

		/// <summary>
		/// ログインされているか
		/// </summary>
		public bool HasLogined => this.Model.HasLogined;

		/// <summary>
		/// スロットを選択してメッセージを発行、その後の処理を行う
		/// </summary>
		private RelayCommand _saveCommand;
		public RelayCommand SaveCommand
		{
			get
			{
				return this._saveCommand = this._saveCommand ?? new RelayCommand((obj) =>
				{
					this.Model.Save();
				});
			}
		}

		/// <summary>
		/// 最後に保存したスロットに対して保存
		/// </summary>
		private RelayCommand _saveLastSlotCommand;
		public RelayCommand SaveLastSlotCommand
		{
			get
			{
				return this._saveLastSlotCommand = this._saveLastSlotCommand ?? new RelayCommand((obj) =>
				{
					this.Model.SaveLastSlot();
				});
			}
		}

		/// <summary>
		/// スロットを選択してメッセージを発行、その後の処理を行う
		/// </summary>
		private RelayCommand _loadCommand;
		public RelayCommand LoadCommand
		{
			get
			{
				return this._loadCommand = this._loadCommand ?? new RelayCommand((obj) =>
				{
					this.Model.Load();
#if WPF
					StoryEditorModel.Default.MainMode = Types.MainMode.EditPerson;
#endif
				});
			}
		}

		/// <summary>
		/// スロットを削除
		/// </summary>
		private RelayCommand _deleteCommand;
		public RelayCommand DeleteCommand
		{
			get
			{
				return this._deleteCommand = this._deleteCommand ?? new RelayCommand((obj) =>
				{
					this.Model.Delete();
				});
			}
		}

		/// <summary>
		/// ストーリーを新規作成
		/// </summary>
		private RelayCommand _createNewCommand;
		public RelayCommand CreateNewCommand
		{
			get
			{
				return this._createNewCommand = this._createNewCommand ?? new RelayCommand((obj) =>
				{
					StoryModel.Current.CreateNew();
#if WPF
					StoryEditorModel.Default.MainMode = Types.MainMode.EditPerson;
#endif
				});
			}
		}

		/// <summary>
		/// （従来）上書き保存
		/// </summary>
		private RelayCommand _saveFileCommand;
		public RelayCommand SaveFileCommand
		{
			get
			{
				return this._saveFileCommand = this._saveFileCommand ?? new RelayCommand((obj) =>
				{
					StoryModel.Current.SaveFile();
				});
			}
		}

		/// <summary>
		/// （従来）名前をつけて保存
		/// </summary>
		private RelayCommand _saveAsFileCommand;
		public RelayCommand SaveAsFileCommand
		{
			get
			{
				return this._saveAsFileCommand = this._saveAsFileCommand ?? new RelayCommand((obj) =>
				{
					StoryModel.Current.SaveAsFile();
				});
			}
		}

		/// <summary>
		/// （従来）ファイルを開く
		/// </summary>
		private RelayCommand _openFileCommand;
		public RelayCommand OpenFileCommand
		{
			get
			{
				return this._openFileCommand = this._openFileCommand ?? new RelayCommand((obj) =>
				{
					StoryModel.Current.OpenFile();
					Messenger.Default.Send(this, new NavigationBackToRootMessage());
#if WPF
					StoryEditorModel.Default.MainMode = Types.MainMode.EditPerson;
#endif
				});
			}
		}

		/// <summary>
		/// クラウドなどへのログイン
		/// </summary>
		private RelayCommand _loginCommand;
		public RelayCommand LoginCommand
		{
			get
			{
				return this._loginCommand = this._loginCommand ?? new RelayCommand((obj) =>
				{
					try
					{
						this.Model.Login();
					}
					catch (Exception e)
					{
						Messenger.Default.Send(this, new AlertMessage(StringResourceResolver.Resolve("LoginFailedMessage")));
					}
				});
			}
		}
	}
}
