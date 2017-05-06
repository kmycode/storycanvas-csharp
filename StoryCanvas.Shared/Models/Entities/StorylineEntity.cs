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
	public class StorylineEntity : Entity
	{
		/// <summary>
		/// 子シーン
		/// </summary>
		[DataMember]
		private EntityListModel<SceneEntity> _scenes = new EntityListModel<SceneEntity>();
		public EntityListModel<SceneEntity> Scenes
		{
			get
			{
				return this._scenes;
			}
		}

		/// <summary>
		/// 初期化
		/// </summary>
		public StorylineEntity()
		{
			// シーンリストと、各シーンにおけるストーリーラインを自動的にリンクさせる
			this.Scenes.EntityAdded += (entity) =>
			{
				entity.Storyline = this;
			};
			this.Scenes.EntityRemoved += (entity) =>
			{
				if (entity.Storyline == this)
				{
					entity.Storyline = null;
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
				return "storyline";
			}
		}
	}
}
