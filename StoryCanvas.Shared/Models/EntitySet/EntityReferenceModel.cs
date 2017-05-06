using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using StoryCanvas.Shared.Models.Entities;

namespace StoryCanvas.Shared.Models.EntitySet
{
	/// <summary>
	/// IDによるエンティティへの参照
	/// </summary>
	[DataContract]
    public class EntityReferenceModel<E> where E : IEntity
    {
		[DataMember]
		private long _id = -1;
		public long Id
		{
			get
			{
				return this._id;
			}
			set
			{
				this._id = value;
			}
		}

		private E _entity;
		public E Entity
		{
			get
			{
				return this._entity;
			}
			set
			{
				this._entity = value;
				if (this._entity != null)
				{
					this._id = this._entity.Id;
				}
			}
		}

		public bool FindEntity(EntitySetModel<E> entitySet)
		{
			this.Entity = entitySet.FindId(this.Id);
			return this.Entity != null;
		}

		public EntityReferenceModel(E entity)
		{
			this.Entity = entity;
		}

		public EntityReferenceModel()
		{
		}
    }
}
