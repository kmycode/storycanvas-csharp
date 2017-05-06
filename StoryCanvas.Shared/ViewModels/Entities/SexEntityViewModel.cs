using System;
using System.Collections.Generic;
using System.Text;
using StoryCanvas.Shared.Models.Entities;
using StoryCanvas.Shared.Models.EntitySet;
using StoryCanvas.Shared.Models.EntitySet;
using StoryCanvas.Shared.Models.Story;
using StoryCanvas.Shared.ViewTools;

namespace StoryCanvas.Shared.ViewModels.Entities
{
	public class SexEntityViewModel : EntityViewModelBase<SexEntity>
	{
		protected override SexEntity CreateDummyEntity()
		{
			return new SexEntity();
		}

		public SexEntityViewModel()
		{
			base.EntityChanged += (e, old) =>
			{
			};

			StoryEditorModel.Default.MainModeChanged += (o, n) =>
			{
				if (n == Types.MainMode.EditSex)
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
