using System;
using System.Collections.Generic;
using System.Text;
using StoryCanvas.Shared.Common;
using StoryCanvas.Shared.Models.IO;
using StoryCanvas.Shared.ViewTools;

namespace StoryCanvas.Shared.Converters.Common
{
#if WPF
	public class AutoSaveStatusStringConverter : ValueConverterBase<AutoSaveStatus, object>
#elif WINDOWS_UWP
	public class AutoSaveStatusStringConverter : ValueConverterBase<AutoSaveStatus, object>
#elif XAMARIN_FORMS
	public class AutoSaveStatusStringConverter : ValueConverterBase<AutoSaveStatus, string>
#endif
	{
#if WPF
		public override object Convert(AutoSaveStatus value)
#elif WINDOWS_UWP
		public override object Convert(AutoSaveStatus value)
#elif XAMARIN_FORMS
		public override string Convert(AutoSaveStatus value)
#endif
		{
			switch (value)
			{
				case AutoSaveStatus.Enabled:
					//return StringResourceResolver.Resolve("AutoSaveEnabled");
					return "";
				case AutoSaveStatus.DisabledBecauseNoSlotSelected:
					return StringResourceResolver.Resolve("AutoSaveDisabledBecauseNoSlotSelected");
				case AutoSaveStatus.DisabledBecauseOfStoryConfig:
					return StringResourceResolver.Resolve("AutoSaveDisabledBecauseOfStoryConfig");
				case AutoSaveStatus.DisabledBecauseWaitingUserAction:
					return StringResourceResolver.Resolve("AutoSaveDisabledBecauseWaitingUserAction");
				case AutoSaveStatus.NetworkError:
					return StringResourceResolver.Resolve("NetworkConnectionFailed");
			}
			return "";
		}
	}
}
