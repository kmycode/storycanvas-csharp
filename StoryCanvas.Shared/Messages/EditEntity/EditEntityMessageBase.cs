using System;
using System.Collections.Generic;
using System.Text;
using StoryCanvas.Shared.Models.Entities;

namespace StoryCanvas.Shared.Messages.EditEntity
{
	/// <summary>
	/// エンティティを編集するメッセージの基本クラス
	/// </summary>
    public class EditEntityMessageBase<E> where E : Entity
    {
		public E Entity
		{
			get;
			private set;
		}

		public EditEntityMessageBase(E entity)
		{
			this.Entity = entity;
		}
    }
}
