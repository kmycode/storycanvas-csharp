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
	public class MemoEntityViewModel : EntityViewModelBase<MemoEntity>
	{
		protected override MemoEntity CreateDummyEntity()
		{
			return new MemoEntity();
		}

		public MemoEntityViewModel()
		{
			base.EntityChanged += (e, old) =>
			{
				this.OnPropertyChanged("Text");
			};

			StoryEditorModel.Default.MainModeChanged += (o, n) =>
			{
				if (n == Types.MainMode.EditMemo)
				{
					if (!this.IsDummy)
					{
						this.Entity = this.Entity;
					}
				}
			};
		}

		public string Text
		{
			get
			{
				return this.Entity.Text;
			}
			set
			{
				this.Entity.Text = value;
			}
		}
	}
}
