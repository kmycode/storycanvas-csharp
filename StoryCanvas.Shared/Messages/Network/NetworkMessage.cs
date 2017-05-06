using System;
using System.Collections.Generic;
using System.Text;

namespace StoryCanvas.Shared.Messages.Network
{
	/// <summary>
	/// ネットワークの送受信をするための画面を開くメッセージ
	/// </summary>
	public class OpenNetworkPageMessage
	{
	}

	/// <summary>
	/// ネットワークを選択するための画面を開くメッセージ
	/// </summary>
	public class OpenNetworkChooseMessage
	{
	}

	/// <summary>
	/// ネットワークから受信するための画面を開くメッセージ
	/// </summary>
	public class OpenNetworkReceiveMessage
    {
	}

	/// <summary>
	/// ネットワークへ送信するための画面を開くメッセージ
	/// </summary>
	public class OpenNetworkSendMessage
	{
	}

	/// <summary>
	/// ネットワークから受信するための画面を閉じるメッセージ
	/// ※画面を閉じた時に画面側から受信終了処理を呼び出すため、このメッセージは作らない
	/// </summary>
	//public class CloseNetworkReceiveMessage
	//{
	//}
}
