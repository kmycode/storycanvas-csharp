using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using StoryCanvas.Shared.Messages.EditEntity;
using StoryCanvas.Shared.ViewTools;
using StoryCanvas.Shared.Common;
using StoryCanvas.Shared.Messages.Picker;
using Microsoft.Win32;
using StoryCanvas.Shared.Messages.Network;
using StoryCanvas.WPF.View.SubViews;
using StoryCanvas.Shared.Messages.Common;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using StoryCanvas.Shared.ViewModels;
using StoryCanvas.Shared.Types;
using StoryCanvas.Shared.Messages.Page;
using System.Diagnostics;
using StoryCanvas.WPF.View.Pages;
using StoryCanvas.Shared.Messages.IO;
using StoryCanvas.WPF.Models;
using StoryCanvas.Shared.Models.IO;
using StoryCanvas.Shared.Utils;
using Libraries.StoryCanvas.Native.Models.IO;
using StoryCanvas.Shared.Models.Cryptography;
using Libraries.StoryCanvas.Native.Models.Cryptography;

namespace StoryCanvas.WPF
{
	/// <summary>
	/// MainWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class MainWindow : MetroWindow, IDataContextHolder, IAuthBrowserProvider
	{
		private MainMode _beforeMainMode;
		private bool _isStartPage = true;
		private ProgressSubView _progressView;

		public MainWindow()
		{
			AutofacUtil.OneDriveStorage = new OneDriveStorageModel();

			// アプリケーション全体を初期化
			StoryCanvasApplication.Initialize();

			// UIスレッドで処理を行う
			Messenger.Default.Register<UIThreadInvokeMessage>((message) =>
			{
				this.Dispatcher.Invoke(message.Action);
			});

			InitializeComponent();
			this.DataContext = StoryViewModel.Default;

			this._beforeMainMode = StoryViewModel.Default.MainMode;

			this.RegisterMessages();

			// 本当はXAMLでIsOpenをBindingしたいのだが、なぜかうまくいかないのでここで
			((StoryViewModel)this.DataContext).PropertyChanged += (sender, e) =>
			{
				// メインモードが切り替えられた時
				if (e.PropertyName == "MainMode")
				{
					// フライアウト対応
					foreach (Flyout flyout in this.Flyouts.Items)
					{
						flyout.IsOpen = false;
					}

					// フライアウトを表示してもそれまでのデータが消えないよう、編集中のテキストボックスからフォーカスを外す
					this.DetermineCurrentInputing();

					// フライアウトを表示しても、それまでの編集画面が閉じないようにする
					MainMode mode = ((StoryViewModel)this.DataContext).MainMode;
					switch (mode)
					{
						// フライアウト
						case MainMode.StoragePage:
							((StoryViewModel)this.DataContext).MainMode = this._beforeMainMode;
							this.StorageFlyout.IsOpen = true;
							break;
						case MainMode.NetworkPage:
							((StoryViewModel)this.DataContext).MainMode = this._beforeMainMode;
							this.NetworkFlyout.IsOpen = true;
							break;
						case MainMode.AboutPage:
							((StoryViewModel)this.DataContext).MainMode = this._beforeMainMode;
							this.AboutFlyout.IsOpen = true;
							break;
						default:
							this._beforeMainMode = mode;

							// スタートページが表示されていれば閉じる
							if (this._isStartPage && mode != MainMode.StartPage)
							{
								this._isStartPage = false;
								this.MainFrame.Content = new MainPage();
							}

							break;
					}
				}
			};

			// Autofacに自身を登録
			AutofacUtil.AuthBrowserProvider = this;
			AutofacUtil.MD5 = new MD5();

			// 最新版確認
			this.Loaded += (sender, e) =>
			{
				new CheckNewestVersionModel().Check(this.Dispatcher);
			};
		}

		private void RegisterMessages()
		{
			Messenger.Default.Register<SaveFilePickerMessage>((message) =>
			{
				var fileDialog = new SaveFileDialog();
				fileDialog.FilterIndex = 1;
				fileDialog.Filter = "Story File(.sf)|*.sf|XML File(.xml)|*.xml|Text File(.txt)|*.txt|All Files (*.*)|*.*";
				bool? result = fileDialog.ShowDialog(this);
				if (result == true)
				{
					message.FileName = fileDialog.FileName;
				}
			});

			Messenger.Default.Register<OpenFilePickerMessage>((message) =>
			{
				var fileDialog = new OpenFileDialog();
				fileDialog.FilterIndex = 1;
				fileDialog.Filter = "Story File(.sf)|*.sf|XML File(.xml)|*.xml|Text File(.txt)|*.txt|All Files (*.*)|*.*";
				bool? result = fileDialog.ShowDialog(this);
				if (result == true)
				{
					message.FileName = fileDialog.FileName;
				}
			});

			Messenger.Default.Register<OpenImageFilePickerMessage>((message) =>
			{
				var fileDialog = new OpenFileDialog();
				fileDialog.FilterIndex = 1;
				fileDialog.Filter = "All Supported Images|*.jpg;*.jpeg;*.png|JPEG|*.jpg;*.jpeg|PNG|*.png";
				bool? result = fileDialog.ShowDialog(this);
				if (result == true)
				{
					message.FileName = fileDialog.FileName;
				}
			});

			// ネットワーク選択画面を表示
			Messenger.Default.Register<OpenNetworkChooseMessage>((message) =>
			{
				this.NetworkFlyout.IsOpen = false;
				this.ShowMetroDialogAsync(new NetworkSelectSubView());
			});

			// ネットワーク受信画面を表示
			Messenger.Default.Register<OpenNetworkReceiveMessage>((message) =>
			{
				this.NetworkFlyout.IsOpen = false;
				this.ShowMetroDialogAsync(new NetworkReceiveSubView());
				//var subView = new NetworkReceiveSubView();
				//subView.Owner = this;
				//subView.ShowDialog();
			});

			// ネットワーク送信画面を表示
			Messenger.Default.Register<OpenNetworkSendMessage>((message) =>
			{
				this.NetworkFlyout.IsOpen = false;
				this.ShowMetroDialogAsync(new NetworkSendSubView());
			});

			// シーン編集画面をモーダル表示
			Messenger.Default.Register<StartEditSceneMessage>((message) =>
			{
				var view = new SceneEditorSubView();
				view.SubViewClosed += (sender, e) => message.OnEditorClosed();
				this.ShowMetroDialogAsync(view);
				message.ContinueWhenEditStarted();
			});

			// 人物編集画面をモーダル表示
			Messenger.Default.Register<StartEditPersonMessage>((message) =>
			{
				var view = new PersonEditorSubView();
				view.SubViewClosed += (sender, e) => message.OnEditorClosed();
				this.ShowMetroDialogAsync(view);
				message.ContinueWhenEditStarted();
			});

			// 場所編集画面をモーダル表示
			Messenger.Default.Register<StartEditPlaceMessage>((message) =>
			{
				var view = new PlaceEditorSubView();
				view.SubViewClosed += (sender, e) => message.OnEditorClosed();
				this.ShowMetroDialogAsync(view);
				message.ContinueWhenEditStarted();
			});

			// メッセージボックス
			Messenger.Default.Register<AlertMessage>(async (message) =>
			{
				this.StorageFlyout.IsOpen = false;
				this.NetworkFlyout.IsOpen = false;
				this.AboutFlyout.IsOpen = false;
				MessageDialogResult result = default(MessageDialogResult);
				switch (message.Type)
				{
					case AlertMessageType.Ok:
						result = await this.ShowMessageAsync(this.Title, message.Text);
						break;
					case AlertMessageType.OkCancel:
						result = await this.ShowMessageAsync(this.Title, message.Text,
							MessageDialogStyle.AffirmativeAndNegative,
							new MetroDialogSettings { NegativeButtonText = StringResourceResolver.Resolve("Cancel") });
						break;
					case AlertMessageType.Yes:
						result = await this.ShowMessageAsync(this.Title, message.Text,
							MessageDialogStyle.Affirmative,
							new MetroDialogSettings { AffirmativeButtonText = StringResourceResolver.Resolve("Yes") });
						break;
					case AlertMessageType.YesNo:
						result = await this.ShowMessageAsync(this.Title, message.Text,
							MessageDialogStyle.AffirmativeAndNegative,
							new MetroDialogSettings
							{
								AffirmativeButtonText = StringResourceResolver.Resolve("Yes"),
								NegativeButtonText = StringResourceResolver.Resolve("No")
							});
						break;
					case AlertMessageType.YesNoCancel:
						result = await this.ShowMessageAsync(this.Title, message.Text,
							MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary,
							new MetroDialogSettings
							{
								AffirmativeButtonText = StringResourceResolver.Resolve("Yes"),
								NegativeButtonText = StringResourceResolver.Resolve("No"),
								FirstAuxiliaryButtonText = StringResourceResolver.Resolve("Cancel")
							});
						break;
				}
				switch (result)
				{
					case MessageDialogResult.Affirmative:
						message.Result = message.Type == AlertMessageType.Ok || message.Type == AlertMessageType.OkCancel ? AlertMessageResult.Ok : AlertMessageResult.Yes;
						break;
					case MessageDialogResult.Negative:
						message.Result = message.Type == AlertMessageType.OkCancel ? AlertMessageResult.Cancel : AlertMessageResult.No;
						break;
					case MessageDialogResult.FirstAuxiliary:
						message.Result = AlertMessageResult.Cancel;
						break;
				}
				message.Continue?.Invoke(message.Result);
			});
			Messenger.Default.Register<LightAlertMessage>((message) =>
			{
				this.StorageFlyout.IsOpen = false;
				this.NetworkFlyout.IsOpen = false;
				this.AboutFlyout.IsOpen = false;
				this.ShowMessageAsync(this.Title, message.Text);
				//MessageBox.Show(this, message.Text, this.Title);
			});

			Messenger.Default.Register<NavigationBackMessage>((message) =>
			{
				this.StorageFlyout.IsOpen = false;
			});
			Messenger.Default.Register<NavigationBackToRootMessage>((message) =>
			{
				this.StorageFlyout.IsOpen = false;
			});

			// URIを開く
			Messenger.Default.Register<OpenUrlMessage>((message) =>
			{
				Process.Start(message.Url);
			});

			// テキストページの印刷
			Messenger.Default.Register<PrintTextPageMessage>((message) =>
			{
				var model = new PrintTextPageModel(message);
				model.Print();
			});

			// テキストページの印刷設定画面を表示
			Messenger.Default.Register<ConfigPrintTextPageMessage>((message) =>
			{
				var dlg = new ConfigPrintTextPageSubView
				{
					DataContext = message.Info,
				};
				this.ShowMetroDialogAsync(dlg);
			});

			// 現在入力中の入力のバインディングを確定させる
			Messenger.Default.Register<DetermineInputingMessage>((message) =>
			{
				this.DetermineCurrentInputing();
			});

			// 進捗画面を表示
			Messenger.Default.Register<ProgressViewShowMessage>((message) =>
			{
				this._progressView = new ProgressSubView();
				this.ShowMetroDialogAsync(this._progressView, new MetroDialogSettings { AnimateShow = false, });
			});

			// 進捗画面を隠す
			Messenger.Default.Register<ProgressViewHideMessage>((message) =>
			{
				bool isSucceed = false;

				// 画面表示した瞬間隠すとエラーになるので、こうなる
				while (!isSucceed)
				{
					this.Dispatcher.Invoke(() =>
					{
						if (this._progressView != null)
						{
							try
							{
								this.HideMetroDialogAsync(this._progressView, new MetroDialogSettings { AnimateHide = false, });
								this._progressView = null;
								isSucceed = true;
							}
							catch
							{
								isSucceed = false;
							}
						}
						else
						{
							isSucceed = true;
						}
					});
				}
			});
		}

		/// <summary>
		/// ウィンドウ閉じる時
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private bool IsAllowClose = false;
		private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
#if !DEBUG
			// アラートが非同期で表示されてしまうので、
			// 終了するときはフラグをtrueにしてから、Closeイベントを再度呼び出すことになる
			if (!this.IsAllowClose)
			{
				e.Cancel = true;
				Messenger.Default.Send(null, new AlertMessage(StringResourceResolver.Resolve("IsQuitMessage"),
					AlertMessageType.YesNo,
					(result) =>
					{
						if (result == AlertMessageResult.Yes)
						{
							this.DetermineCurrentInputing();
							Shared.Models.Story.StoryModel.Current.Quit();
							this.IsAllowClose = true;
							this.Close();
						}
					}));
			}
#endif
		}

		/*
		private void ShowSceneEditor(EditSceneEntityMessage message)
		{
			var editor = new View.EntityEditor.SceneEntityEditor();
			var view = new Window();
			view.Owner = this;
			view.Content = editor;
			view.Width = 300;
			view.Height = 500;
			view.Show();
			editor.OnEditEntityMessage(message);
		}
		*/

		/// <summary>
		/// フライアウト表示時など、現在編集中のデータが消えないように
		/// 現在入力中の項目のバインディングを確定させる
		/// </summary>
		private void DetermineCurrentInputing()
		{
			var direction = Keyboard.Modifiers == ModifierKeys.Shift ? FocusNavigationDirection.Previous : FocusNavigationDirection.Next;
			var directionBack = Keyboard.Modifiers == ModifierKeys.Shift ? FocusNavigationDirection.Next : FocusNavigationDirection.Previous;
			(FocusManager.GetFocusedElement(this) as FrameworkElement)?.MoveFocus(new TraversalRequest(direction));
			(FocusManager.GetFocusedElement(this) as FrameworkElement)?.MoveFocus(new TraversalRequest(directionBack));
		}

		/// <summary>
		/// ダイアログを表示する
		/// </summary>
		/// <param name="dialog"></param>
		/// <param name="settings"></param>
		public async void ShowMetroDialogImplAsync(BaseMetroDialog dialog, MetroDialogSettings settings = null)
		{
			await this.ShowMetroDialogAsync(dialog, settings);
		}

		/// <summary>
		/// ダイアログを隠す
		/// </summary>
		/// <param name="dialog"></param>
		/// <param name="settings"></param>
		public async void HideMetroDialogImplAsync(BaseMetroDialog dialog, MetroDialogSettings settings = null)
		{
			await this.HideMetroDialogAsync(dialog, settings);
		}

		public IAuthBrowser OpenBrowser()
		{
			var dlg = new AuthBrowserSubView();
			this.ShowMetroDialogAsync(dlg);
			return dlg;
		}

		private void MetroWindow_Activated(object sender, EventArgs e)
		{
			StorageModelBase.LastUseStorage?.CheckSaveConflict();
		}
	}
}
