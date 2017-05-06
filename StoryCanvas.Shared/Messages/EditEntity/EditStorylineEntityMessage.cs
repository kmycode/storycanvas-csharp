using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoryCanvas.Shared.Models.Entities;

namespace StoryCanvas.Shared.Messages.EditEntity
{
	public class EditStorylineEntityMessage
	{
		public readonly StorylineEntity Entity;

		public EditStorylineEntityMessage(StorylineEntity entity)
		{
			this.Entity = entity;
		}
	}
}
