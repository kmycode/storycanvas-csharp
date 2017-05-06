using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoryCanvas.Shared.Types;
using StoryCanvas.Shared.ViewTools;
using StoryCanvas.WPF.Properties;

namespace StoryCanvas.WPF.Converters.MainPage
{
	class NetworkReceiveStatusMessageConverter : ValueConverterBase<NetworkReceiveStatus, object>
	{
		public override object Convert(NetworkReceiveStatus value)
		{
			switch (value)
			{
				case NetworkReceiveStatus.None:
					return Resources.NetworkStatusNone;
				case NetworkReceiveStatus.MakingTcp:
					return Resources.NetworkStatusMakingTcp;
				case NetworkReceiveStatus.WaitingForSenderSignal:
					return Resources.NetworkStatusReady;
				case NetworkReceiveStatus.ReceivingMetaData:
					return Resources.NetworkStatusReceivingMetaData;
				case NetworkReceiveStatus.SendingReceivedMetaData:
					return Resources.NetworkStatusSendingMetaData;
				case NetworkReceiveStatus.WaitingForSenderData:
					return Resources.NetworkStatusWaitingForData;
				case NetworkReceiveStatus.Receiving:
					return Resources.NetworkStatusReceiving;
				case NetworkReceiveStatus.Received:
					return Resources.NetworkStatusReceived;
				case NetworkReceiveStatus.Deserializing:
					return Resources.NetworkStatusDeserializing;

				case NetworkReceiveStatus.Abort:
					return Resources.NetworkStatusAbort;
				case NetworkReceiveStatus.ErrorAndAbort:
					return Resources.NetworkStatusErrorAndAbort;
			}
			return "";
		}
	}

	class NetworkSendStatusMessageConverter : ValueConverterBase<NetworkSendStatus, object>
	{
		public override object Convert(NetworkSendStatus value)
		{
			switch (value)
			{
				case NetworkSendStatus.None:
					return Resources.NetworkStatusNone;
				case NetworkSendStatus.MakingTcp:
					return Resources.NetworkStatusMakingTcp;
				case NetworkSendStatus.SendingMetaData:
					return Resources.NetworkStatusSendingMetaData;
				case NetworkSendStatus.WaitingForReceiverSignal:
					return Resources.NetworkStatusWaitingForSignal;
				case NetworkSendStatus.ReceivingReceivedMetaData:
					return Resources.NetworkStatusReceivingMetaData;
				case NetworkSendStatus.Sending:
					return Resources.NetworkStatusSending;
				case NetworkSendStatus.Sent:
					return Resources.NetworkStatusSent;

				case NetworkSendStatus.ErrorAndAbort:
					return Resources.NetworkStatusErrorAndAbort;
				case NetworkSendStatus.NoSignalsAndAbort:
					return Resources.NetworkStatusNoSignalsAndAbort;
			}
			return "";
		}
	}
}
