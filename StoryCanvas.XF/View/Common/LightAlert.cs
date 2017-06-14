using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace StoryCanvas.View.Common
{
	public interface ILightAlert
	{
		void Alert(string text);
	}

	public static class LightAlert
	{
		private static ILightAlert _instance;
		private static ILightAlert Instance
		{
			get
			{
				return _instance = _instance ?? DependencyService.Get<ILightAlert>();
			}
		}

		public static bool Alert(string text)
		{
			if (Instance == null)
			{
				return false;
			}
			Instance.Alert(text);
			return true;
		}
	}
}
