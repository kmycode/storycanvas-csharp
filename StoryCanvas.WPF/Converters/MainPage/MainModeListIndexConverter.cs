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
	/// メインメニューの選択インデクス番号と、その選択項目をあらわす定数のコンバータ
	/// </summary>
	class MainModeListIndexConverter : ValueConverterBase<MainMode, int>
	{
		private static readonly MainListItem[] MainList =
		{
			new MainListItem { Mode = MainMode.EditPerson, Index = 0 },
			new MainListItem { Mode = MainMode.EditGroup, Index = 1 },
			new MainListItem { Mode = MainMode.EditPlace, Index = 2 },
			new MainListItem { Mode = MainMode.EditScene, Index = 3 },
			new MainListItem { Mode = MainMode.EditChapter, Index = 4 },
			new MainListItem { Mode = MainMode.EditWord, Index = 5 },
		};

		private struct MainListItem
		{
			public MainMode Mode;
			public int Index;
		}

		public override int Convert(MainMode value)
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

		public override MainMode ConvertBack(int value)
		{
			foreach (var item in MainList)
			{
				if (item.Index == value)
				{
					return item.Mode;
				}
			}
			return MainMode.None;
		}
	}
}
