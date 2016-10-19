using System;
using System.Linq;
using Xamarin.Forms;

namespace MyFormsLibrary.Effects
{
	public static class Placeholder
	{
		public static readonly BindableProperty IsEnabledProperty =
			BindableProperty.CreateAttached(
					propertyName: "IsEnabled",
					returnType: typeof(bool),
					declaringType: typeof(Placeholder),
					defaultValue: false,
					propertyChanged: OnIsEnabledChanged
				);

		public static void SetIsEnabled(BindableObject view, bool value) {
			view.SetValue(IsEnabledProperty, value);
		}

		public static bool GetIsEnabled(BindableObject view) {
			return (bool)view.GetValue(IsEnabledProperty);
		}

		private static void OnIsEnabledChanged(BindableObject bindable, object oldValue, object newValue) {
			var view = bindable as View;
			if (view == null)
				return;
			if (!(view is Editor)) {
				return;
			}
			var enabled = (bool)newValue;
			if (enabled) {
				view.Effects.Add(new PlaceholderRoutingEffect());
			}
			else {
				var toRemove = view.Effects.FirstOrDefault(e => e is PlaceholderRoutingEffect);
				if (toRemove != null)
					view.Effects.Remove(toRemove);
			}
		}

		public static readonly BindableProperty PlaceholderProperty =
		BindableProperty.CreateAttached(
				"Placeholder",
				typeof(string), 
				typeof(Placeholder), 
				default(string)
			);

		public static void SetPlaceholder(BindableObject view, string value) {
			view.SetValue(PlaceholderProperty, value);
		}

		public static string GetPlaceholder(BindableObject view) {
			return (string)view.GetValue(PlaceholderProperty);
		}

		public static readonly BindableProperty PlaceholderColorProperty =
		BindableProperty.CreateAttached(
				"PlaceholderColor",
				typeof(Color),
				typeof(Placeholder),
				Color.Silver
			);

		public static void SetPlaceholderColor(BindableObject view, Color value) {
			view.SetValue(PlaceholderColorProperty, value);
		}

		public static Color GetPlaceholderColor(BindableObject view) {
			return (Color)view.GetValue(PlaceholderColorProperty);
		}


		class PlaceholderRoutingEffect : RoutingEffect
		{
			public PlaceholderRoutingEffect() : base("Xamarin."+nameof(Placeholder)) {

			}
		}
	}
}

