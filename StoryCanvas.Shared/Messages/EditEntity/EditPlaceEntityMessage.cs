using System;
using System.Collections.Generic;
using System.Text;
using StoryCanvas.Shared.Models.Entities;

namespace StoryCanvas.Shared.Messages.EditEntity
{
	/// <summary>
	/// 場所を編集するメッセージ。まだ編集するエンティティが設定されていない画面において有効
	/// </summary>
	public class EditPlaceEntityNewMessage : EditEntityMessageBase<PlaceEntity>
	{
		public EditPlaceEntityNewMessage(PlaceEntity entity) : base(entity)
		{
		}
	}

	/// <summary>
	/// 場所を編集するメッセージ。IsPrimaryが指定された編集画面すべてに通知される
	/// </summary>
	public class EditPlaceEntityPrimaryMessage : EditPlaceEntityNewMessage
	{
		public EditPlaceEntityPrimaryMessage(PlaceEntity entity) : base(entity)
		{
		}
	}

	/// <summary>
	/// 新たに場所コントロールを作って、場所を編集。別途EditPlaceEntityNewMessageを発行する必要あり
	/// </summary>
	public class StartEditPlaceMessage
	{
		public Action ContinueWhenEditStarted { get; private set; }

		public event EventHandler EditorClosed = delegate { };

		public void OnEditorClosed()
		{
			this.EditorClosed(this, new EventArgs());
		}

		public StartEditPlaceMessage(Action continueWhenEditStarted)
		{
			this.ContinueWhenEditStarted = continueWhenEditStarted;
		}
	}
}
