using System;
using System.Windows;
using System.Windows.Data;

namespace UI.Desktop.Utils
{
	[ValueConversion(typeof(bool), typeof(Visibility))]
	public sealed class BoolToVisibilityConverter : IValueConverter
	{
		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return value != null && (bool)value ? Visibility.Visible : parameter == null ? Visibility.Collapsed : Visibility.Hidden;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotSupportedException("This converter do not support backward conversion.");
		}

		#endregion
	}
}
