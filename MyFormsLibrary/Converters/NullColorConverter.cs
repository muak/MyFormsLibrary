using System;
using System.Globalization;
using Xamarin.Forms;
namespace MyFormsLibrary.Converters
{
	public class NullColorConverter:IValueConverter
	{
		
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return value ?? Color.Default;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}

