using System;
using System.Collections.Generic;
using System.Text;
using StoryCanvas.Shared.Models.Story;

namespace StoryCanvas.Shared.Common
{
	public static class StoryCanvasApplication
	{
		/// <summary>
		/// アプリケーションを初期化
		/// </summary>
		public static void Initialize()
		{
			var story = new StoryModel();
			StoryModel.Current = story;

			story.SetTestData();
		}
	}
}
