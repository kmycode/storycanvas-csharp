using System;
using System.Collections.Generic;
using System.Text;
using StoryCanvas.Shared.Models.Entities;

namespace StoryCanvas.Shared.Messages.EditEntity
{
	/// <summary>
	/// 章を編集するメッセージ。まだ編集するエンティティが設定されていない画面において有効
	/// </summary>
	public class EditSceneEntityNewMessage : EditEntityMessageBase<SceneEntity>
	{
		public EditSceneEntityNewMessage(SceneEntity entity) : base(entity)
		{
		}
	}

	/// <summary>
	/// 章を編集するメッセージ。IsPrimaryが指定された編集画面すべてに通知される
	/// </summary>
	public class EditSceneEntityPrimaryMessage : EditSceneEntityNewMessage
	{
		public EditSceneEntityPrimaryMessage(SceneEntity entity) : base(entity)
		{
		}
	}

	/// <summary>
	/// 新たにシーンコントロールを作って、シーンを編集。別途EditSceneEntityNewMessageを発行する必要あり
	/// </summary>
	public class StartEditSceneMessage
	{
		public Action ContinueWhenEditStarted { get; private set; }

		public event EventHandler EditorClosed = delegate { };

		public void OnEditorClosed()
		{
			this.EditorClosed(this, new EventArgs());
		}

		public StartEditSceneMessage(Action continueWhenEditStarted)
		{
			this.ContinueWhenEditStarted = continueWhenEditStarted;
		}
	}
}
