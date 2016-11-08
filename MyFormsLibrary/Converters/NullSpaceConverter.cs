using System;
using System.Globalization;
using Xamarin.Forms;
namespace MyFormsLibrary.Converters
{
	public class NullSpaceConverter:IValueConverter
	{

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value != null) {
				return value;
			}

			return " ";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}

