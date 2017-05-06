using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoryCanvas.Shared.Models.Entities;

namespace StoryCanvas.Shared.Messages.StorylineDesigner
{
	/// <summary>
	/// シーンをデザイナで表示するメッセージ
	/// </summary>
	public class AddSceneToDesignerMessage
	{
		public readonly SceneEntity Scene;

		public AddSceneToDesignerMessage(SceneEntity scene)
		{
			this.Scene = scene;
		}
	}
}
