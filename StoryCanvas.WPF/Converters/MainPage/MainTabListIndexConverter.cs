using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoryCanvas.Shared.Types;
using StoryCanvas.Shared.ViewTools;

namespace StoryCanvas.WPF.Converters.MainPage
{
	/// <summary>
	/// メインタブの選択インデクス番号と、その選択項目をあらわす定数のコンバータ
	/// </summary>
	class MainTabListIndexConverter : ValueConverterBase<MainTab, int>
	{
		private static readonly MainTabItem[] MainList =
		{
			new MainTabItem { Mode = MainTab.Entity, Index = 0 },
			new MainTabItem { Mode = MainTab.SubEntity, Index = 1 },
			new MainTabItem { Mode = MainTab.Edit, Index = 2 },
			new MainTabItem { Mode = MainTab.View, Index = 3 },
		};

		private struct MainTabItem
		{
			public MainTab Mode;
			public int Index;
		}

		public override int Convert(MainTab value)
		{
			foreach (var item in MainList)
			{
				if (item.Mode == value)
				{
					return item.Index;
				}
			}
			return -1;
		}

		public override MainTab ConvertBack(int value)
		{
			foreach (var item in MainList)
			{
				if (item.Index == value)
				{
					return item.Mode;
				}
			}
			return MainTab.None;
		}
	}
}
