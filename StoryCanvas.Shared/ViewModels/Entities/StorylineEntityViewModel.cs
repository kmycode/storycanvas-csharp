using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoryCanvas.Shared.Models.Entities;
using StoryCanvas.Shared.Messages.StorylineDesigner;
using StoryCanvas.Shared.Messages.EditEntity;

namespace StoryCanvas.Shared.ViewModels.Entities
{
	public class StorylineEntityViewModel : EntityViewModelBase<StorylineEntity>
	{
		protected override StorylineEntity CreateDummyEntity()
		{
			return new StorylineEntity();
		}
		
		/// <summary>
		/// ストーリーラインを編集する時
		/// </summary>
		/// <param name="message">メッセージ</param>
		public void OnEditEntityMessage(EditStorylineEntityMessage message)
		{
			this.Entity = message.Entity;
		}
	}
}
