using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Libraries.StoryCanvas.Native.Models.IO;
using StoryCanvas.Shared.Common;
using StoryCanvas.Shared.Messages.Common;
using StoryCanvas.Shared.Messages.EditEntity;
using StoryCanvas.Shared.Messages.Network;
using StoryCanvas.Shared.Models.IO;
using StoryCanvas.Shared.Types;
using StoryCanvas.Shared.Utils;
using StoryCanvas.Shared.ViewModels;
using StoryCanvas.Shared.ViewTools;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 空白ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 を参照してください

namespace StoryCanvas.UWP
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : Page, IAuthBrowserProvider
	{
		private MainMode _beforeMainMode;
		private bool _isStartPage = true;

		public MainPage()
        {
			AutofacUtil.OneDriveStorage = new OneDriveStorageModel();

			// アプリケーション全体を初期化
			StoryCanvasApplication.Initialize();

			// UIスレッドで処理を行う
			Messenger.Default.Register<UIThreadInvokeMessage>(async (message) =>
			{
				await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => message.Action());
			});
            this.InitializeComponent();
			this.DataContext = StoryViewModel.Default;

			this._beforeMainMode = StoryViewModel.Default.MainMode;

			this.RegisterMessages();

			// 画面遷移
			((StoryViewModel)this.DataContext).PropertyChanged += (sender, e) =>
			{
				// メインモードが切り替えられた時
				if (e.PropertyName == "MainMode")
				{
					throw new NotImplementedException();
					/*
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
					*/
				}
			};

			// Autofacに自身を登録
			AutofacUtil.AuthBrowserProvider = this;

			// UWPタイトルバーとか
			CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = false;
		}

		public IAuthBrowser OpenBrowser()
		{
			throw new NotImplementedException();
		}

		private void RegisterMessages()
		{
			// ネットワーク選択画面を表示
			Messenger.Default.Register<OpenNetworkChooseMessage>((message) =>
			{
				throw new NotImplementedException();
			});

			// ネットワーク受信画面を表示
			Messenger.Default.Register<OpenNetworkReceiveMessage>((message) =>
			{
				throw new NotImplementedException();
			});

			// ネットワーク送信画面を表示
			Messenger.Default.Register<OpenNetworkSendMessage>((message) =>
			{
				throw new NotImplementedException();
			});

			// シーン編集画面をモーダル表示
			Messenger.Default.Register<StartEditSceneMessage>((message) =>
			{
				throw new NotImplementedException();
				//view.SubViewClosed += (sender, e) => message.OnEditorClosed();
				// ここで画面表示
				//message.ContinueWhenEditStarted();
			});

			// 人物編集画面をモーダル表示
			Messenger.Default.Register<StartEditPersonMessage>((message) =>
			{
				throw new NotImplementedException();
				//view.SubViewClosed += (sender, e) => message.OnEditorClosed();
				// ここで画面表示
				//message.ContinueWhenEditStarted();
			});

			// 場所編集画面をモーダル表示
			Messenger.Default.Register<StartEditPlaceMessage>((message) =>
			{
				throw new NotImplementedException();
				//view.SubViewClosed += (sender, e) => message.OnEditorClosed();
				// ここで画面表示
				//message.ContinueWhenEditStarted();
			});
		}
    }
}
