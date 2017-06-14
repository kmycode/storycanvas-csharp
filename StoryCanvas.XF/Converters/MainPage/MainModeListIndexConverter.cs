using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoryCanvas.Shared.Types;
using StoryCanvas.Shared.ViewTools;

namespace StoryCanvas.Converters.MainPage
{
	/// <summary>
	/// メインメニューの選択インデクス番号と、その選択項目をあらわす定数のコンバータ
	/// </summary>
	class MainModeListIndexConverter : ValueConverterBase<MainMode, string>
	{
		private static readonly MainListItem[] MainList =
		{
			new MainListItem { Mode = MainMode.EditPerson, Item = AppResources.People },
			new MainListItem { Mode = MainMode.EditPlace, Item = AppResources.Place },
			new MainListItem { Mode = MainMode.EditScene, Item = AppResources.Scene },
			new MainListItem { Mode = MainMode.EditChapter, Item = AppResources.Chapter },
		};

		private struct MainListItem
		{
			public MainMode Mode;
			public string Item;
		}

		public override string Convert(MainMode value)
		{
			foreach (var item in MainList)
			{
				if (item.Mode == value)
				{
					return item.Item;
				}
			}
			return default(string);
		}

		public override MainMode ConvertBack(string value)
		{
			foreach (var item in MainList)
			{
				if (item.Item == value)
				{
					return item.Mode;
				}
			}
			return MainMode.None;
		}
	}
}
