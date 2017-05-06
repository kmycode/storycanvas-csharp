using System;
using System.Collections.Generic;
using System.Text;
using StoryCanvas.Shared.Models.Entities;

namespace StoryCanvas.Shared.Messages.EditEntity
{
	public class EditGroupEntityNewMessage : EditEntityMessageBase<GroupEntity>
	{
		public EditGroupEntityNewMessage(GroupEntity entity) : base(entity)
		{
		}
	}

	public class EditGroupEntityPrimaryMessage : EditGroupEntityNewMessage
	{
		public EditGroupEntityPrimaryMessage(GroupEntity entity) : base(entity)
		{
		}
	}
}
