using System;
using System.Linq;
using Xamarin.Forms;

namespace MyFormsLibrary.Effects
{
	public static class LineHeight
	{
		public static readonly BindableProperty IsEnabledProperty =
		BindableProperty.CreateAttached(
				"IsEnabled",
				typeof(bool),
				typeof(LineHeight),
				false,
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
			if (!(view is Label))
				return;
			var enabled = (bool)newValue;
			if (enabled) {
				view.Effects.Add(new LineHeightEffectRoutingEffect());
			}
			else {
				var toRemove = view.Effects.FirstOrDefault(e => e is LineHeightEffectRoutingEffect);
				if (toRemove != null)
					view.Effects.Remove(toRemove);
			}
		}

		public static readonly BindableProperty HeightProperty =
			BindableProperty.CreateAttached(
					"Height",
					typeof(double),
					typeof(LineHeight),
					default(double)
				);

		public static void SetHeight(BindableObject view, double value) {
			view.SetValue(HeightProperty, value);
		}

		public static double GetHeight(BindableObject view) {
			return (double)view.GetValue(HeightProperty);
		}


		class LineHeightEffectRoutingEffect : RoutingEffect
		{
			public LineHeightEffectRoutingEffect() : base("Xamarin." + nameof(LineHeight)) {

			}
		}
	}
}

