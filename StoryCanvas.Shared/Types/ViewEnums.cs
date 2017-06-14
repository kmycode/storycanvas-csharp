using System;
using System.Collections.Generic;
using System.Text;

namespace StoryCanvas.Shared.Types
{
	/// <summary>
	/// メインページの編集・表示モード
	/// 現在どの種類の編集画面を開いているか
	/// </summary>
    public enum MainMode
	{
		None,
		StartPage,

		EditPerson,
		EditGroup,
		EditPlace,
		EditScene,
		EditStoryline,
		EditChapter,
		EditWord,

		EditSex,
		EditParameter,
		EditMemo,

		StorySettingPage,
		ChapterTextPage,
		SceneDesignerPage,

		TimelinePage,

		NetworkPage,
		StoragePage,
		SlotPage,
		AboutPage,
	}

	/*
	 * 1. Receiver が UDP を立ち上げ
	 * 2. Receiver が TCP を立ち上げ
	 * 3. Sender が TCP を立ち上げ
	 * 4. Sender が Receiver の UDP に参加して、メタデータを送信
	 * 5. Receiver が Sender からのメタデータを UDP で受信
	 * 6. Receiver が Sender の TCP に折り返しメタデータを送信
	 * 7. Sender が Receiver からの折り返しメタデータを TCP で受信
	 * 8. Sender が Receiver の TCP にモデルデータを送信
	 * 9. Receiver が Sender からのモデルデータを TCP で受信、受信完了
	 */

	/// <summary>
	/// ネットワークで情報を送信するときの状態
	/// </summary>
	public enum NetworkSendStatus
	{
		None,
		MakingTcp,				// 3. TCP立ち上げ
		SendingMetaData,		// 4. メタデータ送信中
		WaitingForReceiverSignal,	// 7を待機. 受信側が折り返しメタデータを送ってくるのを待つ
		ReceivingReceivedMetaData,	// 7. 折り返しメタデータの受信
		Sending,				// 8. モデルデータ送信中
		Sent,					// 9. モデルデータ送信完了

		ErrorAndAbort,			// エラーが発生したので中止
		NoSignalsAndAbort,		// 受信側からの信号が受け取れなかったので中止
	}

	/// <summary>
	/// ネットワークから情報を受信する時の状態
	/// </summary>
	public enum NetworkReceiveStatus
	{
		None,
		MakingUdp,				// 1. UDP立ち上げ中
		MakingTcp,				// 2. TCP立ち上げ中
		WaitingForSenderSignal,	// 5を待機. UDPマルチキャストで信号が送られてくるのを待つ
		ReceivingMetaData,		// 5. メタデータ受信
		SendingReceivedMetaData,	// 6. 折り返しメタデータの送信
		WaitingForSenderData,	// TCPで送信者からデータが来るのを待つ
		Receiving,				// 9. モデルデータ受信中
		Received,               // 9. モデルデータ受信完了
		Deserializing,          // モデルデータをデシリアライズ中

		Abort,
		ErrorAndAbort,			// エラーが発生したので中止
	}
}
