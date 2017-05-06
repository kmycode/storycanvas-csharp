using System;
using System.Collections.Generic;
using System.Text;
using StoryCanvas.Shared.Models.Entities;
using StoryCanvas.Shared.Models.Story;

namespace StoryCanvas.Shared.ViewModels.Entities
{
	public class PlaceEntityViewModel : EntityViewModelBase<PlaceEntity>
	{
		protected override PlaceEntity CreateDummyEntity()
		{
			return new PlaceEntity();
		}

		public PlaceEntityViewModel()
		{
			base.EntityChanged += (e, old) =>
			{
			};

			StoryEditorModel.Default.MainModeChanged += (o, n) =>
			{
				if (n == Types.MainMode.EditPlace)
				{
					if (!this.IsDummy)
					{
						this.Entity = this.Entity;
					}
				}
			};
		}
	}
}
