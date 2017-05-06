using System;
using System.Collections.Generic;
using System.Text;
using StoryCanvas.Shared.Models.Entities;

namespace StoryCanvas.Shared.Messages.EditEntity
{
	/// <summary>
	/// 章を編集するメッセージ。まだ編集するエンティティが設定されていない画面において有効
	/// </summary>
	public class EditPersonEntityNewMessage : EditEntityMessageBase<PersonEntity>
	{
		public EditPersonEntityNewMessage(PersonEntity entity) : base(entity)
		{
		}
	}

	/// <summary>
	/// 章を編集するメッセージ。IsPrimaryが指定された編集画面すべてに通知される
	/// </summary>
	public class EditPersonEntityPrimaryMessage : EditPersonEntityNewMessage
	{
		public EditPersonEntityPrimaryMessage(PersonEntity entity) : base(entity)
		{
		}
	}

	/// <summary>
	/// 新たに人物コントロールを作って、人物を編集。別途EditPersonEntityNewMessageを発行する必要あり
	/// </summary>
	public class StartEditPersonMessage
	{
		public Action ContinueWhenEditStarted { get; private set; }

		public event EventHandler EditorClosed = delegate { };

		public void OnEditorClosed()
		{
			this.EditorClosed(this, new EventArgs());
		}

		public StartEditPersonMessage(Action continueWhenEditStarted)
		{
			this.ContinueWhenEditStarted = continueWhenEditStarted;
		}
	}
}
