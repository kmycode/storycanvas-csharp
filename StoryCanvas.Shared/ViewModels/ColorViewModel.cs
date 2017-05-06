using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using StoryCanvas.Shared.Models.Story;
using StoryCanvas.Shared.ViewTools;
using StoryCanvas.Shared.ViewTools.Resources;

namespace StoryCanvas.Shared.ViewModels
{
    public class ColorViewModel : ViewModelBase
    {
		public ReadOnlyCollection<ColorResourceWrapper> ColorHistory
		{
			get
			{
				return StoryModel.Current.StoryConfig.ColorHistory;
			}
		}

		public ReadOnlyCollection<ColorResourceWrapper> ColorCustom
		{
			get
			{
				return StoryModel.Current.StoryConfig.ColorCustom;
			}
		}
		
		public void AddHistoryColor(ColorResource color)
		{
			StoryModel.Current.StoryConfig.AddHistoryColor(color);
		}
	}
}
