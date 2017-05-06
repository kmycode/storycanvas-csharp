using System;
using System.Collections.Generic;
using System.Text;
using StoryCanvas.Shared.Models.Entities;

namespace StoryCanvas.Shared.Messages.EditEntity
{
	/// <summary>
	/// 用語を編集するメッセージ。まだ編集するエンティティが設定されていない画面において有効
	/// </summary>
	public class EditWordEntityNewMessage : EditEntityMessageBase<WordEntity>
	{
		public EditWordEntityNewMessage(WordEntity entity) : base(entity)
		{
		}
	}

	/// <summary>
	/// 用語を編集するメッセージ。IsPrimaryが指定された編集画面すべてに通知される
	/// </summary>
	public class EditWordEntityPrimaryMessage : EditWordEntityNewMessage
	{
		public EditWordEntityPrimaryMessage(WordEntity entity) : base(entity)
		{
		}
	}
}
