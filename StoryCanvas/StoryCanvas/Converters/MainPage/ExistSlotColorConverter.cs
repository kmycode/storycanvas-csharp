using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoryCanvas.Shared.ViewTools;
using Xamarin.Forms;

namespace StoryCanvas.Converters.MainPage
{
	class ExistSlotColorConverter : ValueConverterBase<bool, Color>
	{
		private static Color ExistBrush = Device.OS == TargetPlatform.Android ? Color.FromRgb((int)0x33, (int)0xae, (int)0xdc) :
											Device.OS == TargetPlatform.iOS ? Color.Black : default(Color);
		private static Color NotExistBrush = Color.FromRgb((int)0xcc, (int)0xcc, (int)0xcc);

		public override Color Convert(bool value)
		{
			return value ? ExistBrush : NotExistBrush;
		}
	}
}
