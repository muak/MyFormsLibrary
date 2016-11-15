using System;
using System.Linq;
using Xamarin.Forms;

namespace MyFormsLibrary.Effects
{
    public class AddErrorMessage
    {
        public static readonly BindableProperty IsEnabledProperty =
            BindableProperty.CreateAttached(
                    propertyName: "IsEnabled",
                    returnType: typeof(bool),
                    declaringType: typeof(AddErrorMessage),
                    defaultValue: default(bool),
                    propertyChanged: OnIsEnabledChanged
                );

        public static void SetIsEnabled(BindableObject view, bool value) {
            view.SetValue(IsEnabledProperty, value);
        }

        public static bool GetIsEnabled(BindableObject view) {
            return (bool)view.GetValue(IsEnabledProperty);
        }

        public static readonly BindableProperty ErrorMessageProperty =
            BindableProperty.CreateAttached(
                    propertyName: "ErrorMessage",
                    returnType: typeof(string),
                    declaringType: typeof(AddErrorMessage),
                    defaultValue: null
                );

        public static void SetErrorMessage(BindableObject view, string value) {
            view.SetValue(ErrorMessageProperty, value);
        }

        public static string GetErrorMessage(BindableObject view) {
            return (string)view.GetValue(ErrorMessageProperty);
        }

        private static void OnIsEnabledChanged(BindableObject bindable, object oldValue, object newValue) {
            var view = bindable as View;
            if (view == null)
                return;

            var enabled = (bool)newValue;
            if (enabled) {
                view.Effects.Add(new AddErrorMessageRoutingEffect());
            }
            else {
                var toRemove = view.Effects.FirstOrDefault(e => e is AddErrorMessageRoutingEffect);
                if (toRemove != null)
                    view.Effects.Remove(toRemove);
            }
        }

        class AddErrorMessageRoutingEffect : RoutingEffect
        {
            public AddErrorMessageRoutingEffect() : base("Xamarin." + nameof(AddErrorMessage)) {

            }
        }
    }
}
