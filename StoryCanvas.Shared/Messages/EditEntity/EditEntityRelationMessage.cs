using System;
using System.Collections.Generic;
using System.Text;
using StoryCanvas.Shared.Models.Entities;
using StoryCanvas.Shared.Models.EntityRelate;

namespace StoryCanvas.Shared.Messages.EditEntity
{
	[Obsolete]
    public class EditEntityRelationMessage
    {
		public IRelation Relation { get; private set; }
		public EditEntityRelationMessage(IRelation relation)
		{
			this.Relation = relation;
		}
    }

	public class EntityRelationEditorMessage
	{
		public EntityRelationEditorModel Model { get; private set; }
		public EntityRelationEditorMessage(EntityRelationEditorModel model)
		{
			this.Model = model;
		}
	}

	/// <summary>
	/// 要素関連付け編集画面で、まだ関連付けていない要素の中から
	/// １つ選択するための画面を表示するメッセージ（Xamarin.Forms向け）
	/// </summary>
	public class PickNotRelatedEntityRequestedMessage
	{
		public Action<object> PickedAction { get; set; }
	}
}
