using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoryCanvas.Shared.Types;
using StoryCanvas.Shared.ViewTools;

namespace StoryCanvas.Converters.MainPage
{
	class NetworkReceiveStatusMessageConverter : ValueConverterBase<NetworkReceiveStatus, string>
	{
		public override string Convert(NetworkReceiveStatus value)
		{
			switch (value)
			{
				case NetworkReceiveStatus.None:
					return AppResources.NetworkStatusNone;
				case NetworkReceiveStatus.MakingTcp:
					return AppResources.NetworkStatusMakingTcp;
				case NetworkReceiveStatus.WaitingForSenderSignal:
					return AppResources.NetworkStatusReady;
				case NetworkReceiveStatus.ReceivingMetaData:
					return AppResources.NetworkStatusReceivingMetaData;
				case NetworkReceiveStatus.SendingReceivedMetaData:
					return AppResources.NetworkStatusSendingMetaData;
				case NetworkReceiveStatus.WaitingForSenderData:
					return AppResources.NetworkStatusWaitingForData;
				case NetworkReceiveStatus.Receiving:
					return AppResources.NetworkStatusReceiving;
				case NetworkReceiveStatus.Received:
					return AppResources.NetworkStatusReceived;
				case NetworkReceiveStatus.Deserializing:
					return AppResources.NetworkStatusDeserializing;

				case NetworkReceiveStatus.Abort:
					return AppResources.NetworkStatusAbort;
				case NetworkReceiveStatus.ErrorAndAbort:
					return AppResources.NetworkStatusErrorAndAbort;
			}
			return "";
		}
	}

	class NetworkSendStatusMessageConverter : ValueConverterBase<NetworkSendStatus, string>
	{
		public override string Convert(NetworkSendStatus value)
		{
			switch (value)
			{
				case NetworkSendStatus.None:
					return AppResources.NetworkStatusNone;
				case NetworkSendStatus.MakingTcp:
					return AppResources.NetworkStatusMakingTcp;
				case NetworkSendStatus.SendingMetaData:
					return AppResources.NetworkStatusSendingMetaData;
				case NetworkSendStatus.WaitingForReceiverSignal:
					return AppResources.NetworkStatusWaitingForSignal;
				case NetworkSendStatus.ReceivingReceivedMetaData:
					return AppResources.NetworkStatusReceivingMetaData;
				case NetworkSendStatus.Sending:
					return AppResources.NetworkStatusSending;
				case NetworkSendStatus.Sent:
					return AppResources.NetworkStatusSent;

				case NetworkSendStatus.ErrorAndAbort:
					return AppResources.NetworkStatusErrorAndAbort;
				case NetworkSendStatus.NoSignalsAndAbort:
					return AppResources.NetworkStatusNoSignalsAndAbort;
			}
			return "";
		}
	}
}
