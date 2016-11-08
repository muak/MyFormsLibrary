using System;
using System.Linq;
using Xamarin.Forms;

namespace MyFormsLibrary.Effects
{
    public class ShowBorder
    {
        public static readonly BindableProperty IsEnabledProperty =
        BindableProperty.CreateAttached(
                "IsEnabled",
                typeof(bool),
                typeof(ShowBorder),
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

            var enabled = (bool)newValue;
            if (enabled) {
                view.Effects.Add(new ShowBorderEffectRoutingEffect());
            }
            else {
                var toRemove = view.Effects.FirstOrDefault(e => e is ShowBorderEffectRoutingEffect);
                if (toRemove != null)
                    view.Effects.Remove(toRemove);
            }
        }

        public static readonly BindableProperty ColorProperty =
            BindableProperty.CreateAttached(
                    "Color",
                    typeof(Color),
                    typeof(ShowBorder),
                    default(Color)
                );

        public static void SetColor(BindableObject view, Color value) {
            view.SetValue(ColorProperty, value);
        }

        public static Color GetColor(BindableObject view) {
            return (Color)view.GetValue(ColorProperty);
        }

        public static readonly BindableProperty WidthProperty =
            BindableProperty.CreateAttached(
                    "Width",
                    typeof(float),
                    typeof(ShowBorder),
                    1.0f
                );

        public static void SetWidth(BindableObject view, float value) {
            view.SetValue(WidthProperty, value);
        }

        public static float GetWidth(BindableObject view) {
            return (float)view.GetValue(WidthProperty);
        }

        class ShowBorderEffectRoutingEffect : RoutingEffect
        {
            public ShowBorderEffectRoutingEffect() : base("Xamarin." + nameof(ShowBorder)) {

            }
        }
    }
}
