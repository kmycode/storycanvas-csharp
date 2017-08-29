using System;
#if WPF
using System.Windows.Data;
#endif
#if WINDOWS_UWP
using Windows.UI.Xaml.Data;
#endif
#if XAMARIN_FORMS
using Xamarin.Forms;
#endif

namespace StoryCanvas.Shared.ViewTools
{
	public abstract class ValueConverterBase<FROM, TO, PARAM, PARAMBACK> : IValueConverter
	{
		protected System.Globalization.CultureInfo culture;

		public abstract TO Convert(FROM value, PARAM parameter);

#if WINDOWS_UWP
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			return this.Convert(value, targetType, parameter, default(System.Globalization.CultureInfo));
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			return this.ConvertBack(value, targetType, parameter, default(System.Globalization.CultureInfo));
		}
#endif

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			this.culture = culture;
			if (value is FROM && typeof(TO) == targetType && parameter is PARAM)
			{
				return this.Convert((FROM)value, (PARAM)parameter);
			}
			return null;
		}

		public virtual FROM ConvertBack(TO value, PARAMBACK parameter)
		{
			return default(FROM);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			this.culture = culture;
			if (value is TO && typeof(FROM) == targetType && parameter is PARAMBACK)
			{
				return this.ConvertBack((TO)value, (PARAMBACK)parameter);
			}
			return null;
		}
	}

	public abstract class ValueConverterBase<FROM, TO, PARAM> : IValueConverter
	{
		protected System.Globalization.CultureInfo culture;

		public abstract TO Convert(FROM value, PARAM parameter);

#if WINDOWS_UWP
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			return this.Convert(value, targetType, parameter, default(System.Globalization.CultureInfo));
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			return this.ConvertBack(value, targetType, parameter, default(System.Globalization.CultureInfo));
		}
#endif

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			this.culture = culture;
			if ((value == null || value is FROM) && typeof(TO) == targetType && parameter is PARAM)
			{
				return this.Convert((FROM)value, (PARAM)parameter);
			}
			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return null;
		}
	}

	public abstract class ValueConverterBase<FROM, TO> : IValueConverter
	{
		protected System.Globalization.CultureInfo culture;

        protected bool IsCheckTargetType { get; set; } = true;

		public abstract TO Convert(FROM value);

#if WINDOWS_UWP
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			return this.Convert(value, targetType, parameter, default(System.Globalization.CultureInfo));
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			return this.ConvertBack(value, targetType, parameter, default(System.Globalization.CultureInfo));
		}
#endif

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			this.culture = culture;
			if (value is FROM && (!this.IsCheckTargetType || typeof(TO) == targetType))
			{
				return this.Convert((FROM)value);
			}
			return null;
		}

		public virtual FROM ConvertBack(TO value)
		{
			return default(FROM);
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			this.culture = culture;
			if (value is TO && (!this.IsCheckTargetType || typeof(FROM) == targetType))
			{
				return this.ConvertBack((TO)value);
			}
			return null;
		}
	}
}
