using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace StoryCanvas.Shared.Models.Entities
{
	[DataContract]
	public class PlaceEntity : TreeEntity
	{
		/// <summary>
		/// リソース名
		/// </summary>
		protected override string ResourceName
		{
			get
			{
				return "place";
			}
		}

		/// <summary>
		/// 中身が空であるか
		/// </summary>
		public override bool IsEmpty
		{
			get
			{
				return base.IsEmpty;
			}
		}
	}
}
