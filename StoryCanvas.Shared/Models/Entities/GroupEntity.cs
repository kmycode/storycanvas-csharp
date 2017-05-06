using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using StoryCanvas.Shared.Models.EntitySet;
using StoryCanvas.Shared.Models.EntityRelate;
using System.Linq;

namespace StoryCanvas.Shared.Models.Entities
{
	/// <summary>
	/// 集団
	/// </summary>
	[DataContract]
	public class GroupEntity : TreeEntity
	{
		/// <summary>
		/// 関連付けられた人物に設定するパラメータ
		/// </summary>
		[DataMember]
		private EntityListModel<ParameterEntity> _relatedPersonParameters = new EntityListModel<ParameterEntity>();
		public EntityListModel<ParameterEntity> RelatedPersonParameters
		{
			get
			{
				if (this._relatedPersonParameters == null)
				{
					this._relatedPersonParameters = new EntityListModel<ParameterEntity>();
				}
				return this._relatedPersonParameters;
			}
		}

		/// <summary>
		/// 関連付けられた人物
		/// </summary>
		public IEnumerable<GroupPersonEntityRelate> RelatedPeople
		{
			get
			{
				return this.StoryModel.GroupPersonRelation.FindRelated(this).OrderBy((relate) => relate.Entity2.Order);
			}
		}

		/// <summary>
		/// 関連付けられていない人物
		/// </summary>
		public IEnumerable<PersonEntity> NotRelatedPeople
		{
			get
			{
				return this.StoryModel.GroupPersonRelation.FindNotRelated(this, this.StoryModel.People);
			}
		}

		protected override string ResourceName
		{
			get
			{
				return "group";
			}
		}

		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="query"></param>
		protected override bool IsWordExists(string word)
		{
			if (!base.IsWordExists(word))
			{
				bool? r = false;
				foreach (var item in this.RelatedPeople)
				{
					r |= item.Entity2.Name?.IndexOf(word) >= 0;
					r |= item.Note?.IndexOf(word) >= 0;
				}

				return r == true;
			}
			else
			{
				return true;
			}
		}
	}
}
