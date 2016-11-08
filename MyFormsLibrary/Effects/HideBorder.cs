using System;
using System.Linq;
using Xamarin.Forms;

namespace MyFormsLibrary.Effects
{
	public static class HideBorder
	{
		public static readonly BindableProperty IsEnabledProperty =
		BindableProperty.CreateAttached(
				"IsEnabled",
				typeof(bool),
				typeof(HideBorder),
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
			
			var hideBorder = (bool)newValue;
			if (hideBorder) {
				view.Effects.Add(new HideBorderEffectRoutingEffect());
			}
			else {
				var toRemove = view.Effects.FirstOrDefault(e => e is HideBorderEffectRoutingEffect);
				if (toRemove != null)
					view.Effects.Remove(toRemove);
			}
		}

		class HideBorderEffectRoutingEffect : RoutingEffect
		{
			public HideBorderEffectRoutingEffect() : base("Xamarin."+nameof(HideBorder)) {

			}
		}
	}
}

