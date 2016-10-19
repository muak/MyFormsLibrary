using System;
using System.Linq;
using Xamarin.Forms;

namespace MyFormsLibrary.Effects
{
	public static class SliderColor
	{
		public static readonly BindableProperty IsEnabledProperty =
			BindableProperty.CreateAttached(
					propertyName: "IsEnabled",
					returnType: typeof(bool),
					declaringType: typeof(SliderColor),
					defaultValue: false,
					propertyChanged: OnIsEnabledChanged
				);

		public static void SetPropertyName(BindableObject view, bool value) {
			view.SetValue(IsEnabledProperty, value);
		}

		public static bool GetPropertyName(BindableObject view) {
			return (bool)view.GetValue(IsEnabledProperty);
		}

		private static void OnIsEnabledChanged(BindableObject bindable, object oldValue, object newValue) {
			var view = bindable as View;
			if (view == null)
				return;
			if (!(view is Slider)) {
				return;
			}
			var enabled = (bool)newValue;
			if (enabled) {
				view.Effects.Add(new SliderColorRoutingEffect());
			}
			else {
				var toRemove = view.Effects.FirstOrDefault(e => e is SliderColorRoutingEffect);
				if (toRemove != null)
					view.Effects.Remove(toRemove);
			}
		}



		public static readonly BindableProperty MinTrackColorProperty =
			BindableProperty.CreateAttached(
					"MinTrackColor",
					typeof(Color),
					typeof(SliderColor),
					default(Color)
				);

		public static void SetMinTrackColor(BindableObject view, Color value) {
			view.SetValue(MinTrackColorProperty, value);
		}

		public static Color GetMinTrackColor(BindableObject view) {
			return (Color)view.GetValue(MinTrackColorProperty);
		}

		public static readonly BindableProperty MaxTrackColorProperty =
			BindableProperty.CreateAttached(
					"MaxTrackColor",
					typeof(Color),
					typeof(SliderColor),
					default(Color)
				);

		public static void SetMaxTrackColor(BindableObject view, Color value) {
			view.SetValue(MaxTrackColorProperty, value);
		}

		public static Color GetMaxTrackColor(BindableObject view) {
			return (Color)view.GetValue(MaxTrackColorProperty);
		}

		public static readonly BindableProperty ThumbColorProperty =
			BindableProperty.CreateAttached(
					"ThumbColor",
					typeof(Color),
					typeof(SliderColor),
					default(Color)
				);

		public static void SetThumbColor(BindableObject view, Color value) {
			view.SetValue(ThumbColorProperty, value);
		}

		public static Color GetThumbColor(BindableObject view) {
			return (Color)view.GetValue(ThumbColorProperty);
		}

		class SliderColorRoutingEffect : RoutingEffect
		{
			public SliderColorRoutingEffect() : base("Xamarin." + nameof(SliderColor)) {

			}
		}
	}
}

