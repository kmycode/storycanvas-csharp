using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using StoryCanvas.Shared.ViewTools;
using Xamarin.Forms;

namespace StoryCanvas.Converters.MainPage
{
	class TreeListViewMarginLevelConverter : ValueConverterBase<int, Thickness>
	{
		private static int MarginSize = 15;

		public override Thickness Convert(int value)
		{
			return new Thickness(value * MarginSize, 0, 0, 0);
		}

		public override int ConvertBack(Thickness value)
		{
			return (int)value.Left / MarginSize;
		}
	}
}
