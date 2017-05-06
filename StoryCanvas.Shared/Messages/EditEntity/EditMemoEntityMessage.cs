using System;
using System.Collections.Generic;
using System.Text;
using StoryCanvas.Shared.Models.Entities;

namespace StoryCanvas.Shared.Messages.EditEntity
{
	/// <summary>
	/// 章を編集するメッセージ。まだ編集するエンティティが設定されていない画面において有効
	/// </summary>
	public class EditMemoEntityNewMessage : EditEntityMessageBase<MemoEntity>
	{
		public EditMemoEntityNewMessage(MemoEntity entity) : base(entity)
		{
		}
	}

	/// <summary>
	/// 章を編集するメッセージ。IsPrimaryが指定された編集画面すべてに通知される
	/// </summary>
	public class EditMemoEntityPrimaryMessage : EditMemoEntityNewMessage
	{
		public EditMemoEntityPrimaryMessage(MemoEntity entity) : base(entity)
		{
		}
	}
}
