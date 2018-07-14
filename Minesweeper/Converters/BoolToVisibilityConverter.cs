using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Minesweeper.Converters
{
	public class BoolToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			var direct = parameter != null ? bool.Parse(parameter.ToString()) : true;
			if (direct)
				return (bool)value ? Visibility.Visible : Visibility.Collapsed;
			else
				return (bool)value ? Visibility.Collapsed : Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}
