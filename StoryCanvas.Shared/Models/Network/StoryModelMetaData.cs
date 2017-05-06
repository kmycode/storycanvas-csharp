using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using StoryCanvas.Shared.Common;
using StoryCanvas.Shared.Models.Story;

namespace StoryCanvas.Shared.Models.Network
{
	/// <summary>
	/// ストーリーモデルをネットワークで送受信するときにやり取りするメタデータ
	/// </summary>
	[DataContract]
    public class StoryModelMetaData
    {
		public StoryModelMetaData(StoryModel model)
		{
			this.StoryTitle = model.StoryConfig.Title;
		}

		protected StoryModelMetaData(StoryModelMetaData data)
		{
			this.ApplicationVersion = data.ApplicationVersion;
			this.StoryTitle = data.StoryTitle;
		}

		[DataMember]
		public string ApplicationVersion { get; set; } = StringResourceResolver.Resolve("ApplicationVersion");

		[DataMember]
		public string StoryTitle { get; private set; }
    }

	[DataContract]
	public class StoryModelReceivedMetaData : StoryModelMetaData
	{
		public StoryModelReceivedMetaData(StoryModelMetaData data) : base(data)
		{
		}
	}
}
