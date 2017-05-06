using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using StoryCanvas.Shared.Common;
using StoryCanvas.Shared.Messages.Common;
using StoryCanvas.Shared.Messages.Network;
using StoryCanvas.Shared.ViewTools;

namespace StoryCanvas.WPF.Models
{
	class CheckNewestVersionModel
	{
		public void Check(Dispatcher dispatcher)
		{
			Task.Run(() =>
			{
				var currentVersion = new ApplicationVersionResolver(Properties.Resources.ApplicationVersion);

				try
				{
					// ネットからversion.jsをダウンロード
					System.Net.WebClient wc = new System.Net.WebClient();
					wc.Encoding = System.Text.Encoding.UTF8;
					string source = wc.DownloadString(
						currentVersion.Modifier == ApplicationVersionModifier.Stable ? 
								"https://storycanvas.kmycode.net/scripts/version.js" :
								"https://storycanvas.kmycode.net/scripts/version_beta.js"
						);
					wc.Dispose();

					// バージョン番号を取得
					var lines = source.Split(';');
					foreach (var line in lines)
					{
						var param = line.Replace("var ", "").Replace(" ", "").Trim('\r', '\n').Split('=');
						if (param[0] == "version_num")
						{
							// バージョン番号を比較
							var versionNum = param[1].Replace("'", "");
							var newestVersion = new ApplicationVersionResolver(versionNum);
							if (newestVersion > currentVersion)
							{
								// メッセージを送る
								dispatcher.BeginInvoke(new Action(() =>
							   {
								   Messenger.Default.Send(null, new AlertMessage(StoryCanvas.WPF.Properties.Resources.NewestVersionAvaliabledMessage.Replace("%Version%", versionNum),
																				   AlertMessageType.YesNo,
																				   (r) =>
																				   {
																					   if (r == AlertMessageResult.Yes)
																					   {
																						   Messenger.Default.Send(null, new OpenUrlMessage("https://storycanvas.kmycode.net/" + Properties.Resources.Language + "/documents/download.html"));
																					   }
																				   }));
							   }));
							}
						}
					}
				}
				catch
				{
					// インターネットからのダウンロードに失敗した時
				}
			});
		}
	}
}
