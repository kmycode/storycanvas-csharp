using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using StoryCanvas.Shared.Models.EntitySet;
using StoryCanvas.Shared.Models.EntityRelate;
using System.Linq;

namespace StoryCanvas.Shared.Models.Entities
{
	[DataContract]
	public class ChapterEntity : Entity
	{
		/// <summary>
		/// 親となる編
		/// </summary>
		private WeakReference<PartEntity> _part = new WeakReference<PartEntity>(null);
		public PartEntity Part
		{
			get
			{
				PartEntity entity = null;
				this._part.TryGetTarget(out entity);
				return entity;
			}
			set
			{
				if (this.Part != value)
				{
					this.Part?.Chapters.Remove(this);
					if (value?.Chapters.Contains(this) != true)
					{
						value?.Chapters.Add(this);
					}
					this._part.SetTarget(value);
				}
			}
		}

		/// <summary>
		/// 初期化
		/// </summary>
		public ChapterEntity()
		{
		}

		/// <summary>
		/// 関連付けられたシーン
		/// </summary>
		public IEnumerable<SceneChapterEntityRelate> RelatedScenes
		{
			get
			{
				return this.StoryModel.SceneChapterRelation.FindRelated(this).OrderBy((relate) => relate.Entity1.Order);
			}
		}

		/// <summary>
		/// 関連付けられていないシーン
		/// </summary>
		public IEnumerable<SceneEntity> NotRelatedScenes
		{
			get
			{
				return this.StoryModel.SceneChapterRelation.FindNotRelated(this, this.StoryModel.Scenes);
			}
		}

		/// <summary>
		/// シーンの脚本をひとまとめにして取得
		/// </summary>
		/// <param name="isNewline">シーンとシーンの境に空行を入れるか</param>
		/// <returns></returns>
		public string GetScenesText(bool isNewline = true)
		{
			string result = "";
			var sceneItems = from item in this.RelatedScenes orderby item.Entity1.Order select item;
			foreach (var item in sceneItems)
			{
				result += item.Entity1.Text;
				if (isNewline)
				{
					result += "\n\n";
				}
				else
				{
					result += "\n";
				}
			}

			return result;
		}

		protected override string ResourceName
		{
			get
			{
				return "chapter";
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

		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="query"></param>
		protected override bool IsWordExists(string word)
		{
			if (!base.IsWordExists(word))
			{
				bool? r = false;
				foreach (var item in this.RelatedScenes)
				{
					r |= item.Entity1.Name?.IndexOf(word) >= 0;
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
