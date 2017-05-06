using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using StoryCanvas.Shared.ViewTools;

namespace StoryCanvas.WPF.Converters.MainPage
{
	class ExistSlotColorConverter : ValueConverterBase<bool, Brush>
	{
		private static Brush ExistBrush = new SolidColorBrush(Colors.White);
		private static Brush NotExistBrush = new SolidColorBrush(Colors.Gray);

		public override Brush Convert(bool value)
		{
			return value ? ExistBrush : NotExistBrush;
		}
	}
}
