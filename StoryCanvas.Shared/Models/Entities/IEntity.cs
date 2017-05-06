using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace StoryCanvas.Shared.Models.Entities
{
    public interface IEntity
    {
		long Id { get; }
		long Order { get; set; }

		void ReplaceOrder(IEntity other);
    }
}
