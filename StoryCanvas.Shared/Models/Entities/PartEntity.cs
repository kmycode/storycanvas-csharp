using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using StoryCanvas.Shared.Models.EntitySet;

namespace StoryCanvas.Shared.Models.Entities
{
	[DataContract]
	public class PartEntity : Entity
	{
		/// <summary>
		/// 子チャプター
		/// </summary>
		[DataMember]
		private EntityListModel<ChapterEntity> _chapters = new EntityListModel<ChapterEntity>();
		public EntityListModel<ChapterEntity> Chapters
		{
			get
			{
				return this._chapters;
			}
		}

		/// <summary>
		/// 初期化
		/// </summary>
		public PartEntity()
		{
			// 章リストと、各章における編を自動的にリンクさせる
			this.Chapters.EntityAdded += (entity) =>
			{
				entity.Part = this;
			};
			this.Chapters.EntityRemoved += (entity) =>
			{
				if (entity.Part == this)
				{
					entity.Part = null;
				}
			};
		}

		/// <summary>
		/// リソース名
		/// </summary>
		protected override string ResourceName
		{
			get
			{
				return "part";
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
