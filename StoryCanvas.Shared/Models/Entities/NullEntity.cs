using System;
using System.Collections.Generic;
using System.Text;

namespace StoryCanvas.Shared.Models.Entities
{
	/// <summary>
	/// 存在しないエンティティ
	/// </summary>
	class NullEntity : Entity
	{
		protected override string ResourceName
		{
			get
			{
				return "";
			}
		}
	}
}
