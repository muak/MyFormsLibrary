using System;
using System.Linq;
using Xamarin.Forms;

namespace MyFormsLibrary.Effects
{
    public class MaxLength
    {
        public static readonly BindableProperty OnProperty =
            BindableProperty.CreateAttached(
                    propertyName: "On",
                    returnType: typeof(bool),
                    declaringType: typeof(MaxLength),
                    defaultValue: false,
                    propertyChanged: OnOffChanged
                );

        public static void SetOn(BindableObject view, bool value)
        {
            view.SetValue(OnProperty, value);
        }

        public static bool GetOn(BindableObject view)
        {
            return (bool)view.GetValue(OnProperty);
        }

        public static readonly BindableProperty LengthProperty =
            BindableProperty.CreateAttached(
                    "Length",
                    typeof(int),
                    typeof(MaxLength),
                    default(int)
                );

        public static void SetLength(BindableObject view, int value)
        {
            view.SetValue(LengthProperty, value);
        }

        public static int GetLength(BindableObject view)
        {
            return (int)view.GetValue(LengthProperty);
        }

        private static void OnOffChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = bindable as View;
            if (view == null)
                return;

            if ((bool)newValue) {
                view.Effects.Add(new MaxLengthRoutingEffect());
            }
            else {
                var toRemove = view.Effects.FirstOrDefault(e => e is MaxLengthRoutingEffect);
                if (toRemove != null)
                    view.Effects.Remove(toRemove);
            }
        }

        class MaxLengthRoutingEffect : RoutingEffect
        {
            public MaxLengthRoutingEffect() : base("Xamarin." + nameof(MaxLength)) { }
        }
    }
}
