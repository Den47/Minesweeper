using System;
using Windows.UI.Xaml.Data;

namespace Minesweeper.UI.Converters
{
	public class InverseBoolConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			return !(bool)value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}
