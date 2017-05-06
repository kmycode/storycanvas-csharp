﻿using System;
using System.Collections.Generic;
using System.Text;
using StoryCanvas.Shared.Models.Entities;

namespace StoryCanvas.Shared.Messages.EditEntity
{
	/// <summary>
	/// 章を編集するメッセージ。まだ編集するエンティティが設定されていない画面において有効
	/// </summary>
	public class EditSexEntityNewMessage : EditEntityMessageBase<SexEntity>
	{
		public EditSexEntityNewMessage(SexEntity entity) : base(entity)
		{
		}
	}

	/// <summary>
	/// 章を編集するメッセージ。IsPrimaryが指定された編集画面すべてに通知される
	/// </summary>
	public class EditSexEntityPrimaryMessage : EditSexEntityNewMessage
	{
		public EditSexEntityPrimaryMessage(SexEntity entity) : base(entity)
		{
		}
	}
}
