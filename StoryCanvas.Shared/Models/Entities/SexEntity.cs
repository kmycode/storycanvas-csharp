using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using StoryCanvas.Shared.ViewTools.Resources;
using StoryCanvas.Shared.Common;

namespace StoryCanvas.Shared.Models.Entities
{
	[DataContract]
	public class SexEntity : Entity
	{
		/// <summary>
		/// 男（定数）
		/// </summary>
		private static SexEntity _male = null;
		public static SexEntity Male
		{
			get
			{
				if (_male == null)
				{
					_male = NewMale();
				}
				return _male;
			}
		}
		public static SexEntity NewMale()
		{
            var entity = new SexEntity()
            {
                Name = StringResourceResolver.Resolve("Male"),
                Color = ColorResource.Blue,
            };
            return entity;
		}

		/// <summary>
		/// 女（定数）
		/// </summary>
		private static SexEntity _female = null;
		public static SexEntity Female
		{
			get
			{
				if (_female == null)
				{
					_female = NewFemale();
				}
				return _female;
			}
		}
		public static SexEntity NewFemale()
		{
            var entity = new SexEntity()
            {
                Name = StringResourceResolver.Resolve("Female"),
                Color = ColorResource.Red,
            };
            return entity;
		}

		/// <summary>
		/// リソース名
		/// </summary>
		protected override string ResourceName
		{
			get
			{
				return "sex";
			}
		}

		/// <summary>
		/// 中身が空であるか
		/// </summary>
		public override bool IsEmpty
		{
			get
			{
				return base.IsEmpty;
			}
		}
	}
}
