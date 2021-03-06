﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoryCanvas.Shared.Types;
using StoryCanvas.Shared.ViewTools;

namespace StoryCanvas.WPF.Converters.MainPage
{
	/// <summary>
	/// メインメニュー（ビュータブ）の選択インデクス番号と、その選択項目をあらわす定数のコンバータ
	/// </summary>
	class MainViewModeListIndexConverter : ValueConverterBase<MainMode, int>
	{
		private static readonly MainListItem[] MainList =
		{
			new MainListItem { Mode = MainMode.TimelinePage, Index = 0 },
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
