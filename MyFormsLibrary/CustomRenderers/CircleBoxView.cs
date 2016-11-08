using System;
using Xamarin.Forms;

namespace MyFormsLibrary.CustomRenderers
{
	public class CircleBoxView:BoxView
	{
		public static BindableProperty RadiusProperty =
			BindableProperty.Create(
				nameof(Radius),
				typeof(int),
				typeof(CircleBoxView),
				default(int),
				defaultBindingMode: BindingMode.OneWay
			);

		public int Radius {
			get { return (int)GetValue(RadiusProperty); }
			set { SetValue(RadiusProperty, value); }
		}
	}
}

