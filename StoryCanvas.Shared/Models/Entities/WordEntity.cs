using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using StoryCanvas.Shared.Models.EntityRelate;
using System.Linq;

namespace StoryCanvas.Shared.Models.Entities
{
	[DataContract]
    public class WordEntity : TreeEntity
	{

		/// <summary>
		/// 関連付けられた人物
		/// </summary>
		public IEnumerable<WordPersonEntityRelate> RelatedPeople
		{
			get
			{
				return this.StoryModel.WordPersonRelation.FindRelated(this).OrderBy((relate) => relate.Entity2.Order);
			}
		}

		/// <summary>
		/// 関連付けられていない人物
		/// </summary>
		public IEnumerable<PersonEntity> NotRelatedPeople
		{
			get
			{
				return this.StoryModel.WordPersonRelation.FindNotRelated(this, this.StoryModel.People);
			}
		}

		/// <summary>
		/// 関連付けられたシーン
		/// </summary>
		public IEnumerable<WordSceneEntityRelate> RelatedScenes
		{
			get
			{
				return this.StoryModel.WordSceneRelation.FindRelated(this).OrderBy((relate) => relate.Entity2.Order);
			}
		}

		/// <summary>
		/// 関連付けられていないシーン
		/// </summary>
		public IEnumerable<SceneEntity> NotRelatedScenes
		{
			get
			{
				return this.StoryModel.WordSceneRelation.FindNotRelated(this, this.StoryModel.Scenes);
			}
		}

		/// <summary>
		/// リソース名
		/// </summary>
		protected override string ResourceName
		{
			get
			{
				return "word";
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
