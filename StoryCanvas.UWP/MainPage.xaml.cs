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
    public sealed partial class MainPage
	{
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
        }
    }
}
