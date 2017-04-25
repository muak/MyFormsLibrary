using System;
using System.Linq;
using Xamarin.Forms;
using System.Windows.Input;

namespace MyFormsLibrary.Effects
{
    public class NumberPicker
    {
        public static readonly BindableProperty IsEnabledProperty =
            BindableProperty.CreateAttached(
                    propertyName: "IsEnabled",
                    returnType: typeof(bool),
                    declaringType: typeof(NumberPicker),
                    defaultValue: false,
                    propertyChanged: OnIsEnabledChanged
                );

        public static void SetIsEnabled(BindableObject view, bool value) {
            view.SetValue(IsEnabledProperty, value);
        }

        public static bool GetIsEnabled(BindableObject view) {
            return (bool)view.GetValue(IsEnabledProperty);
        }

        public static readonly BindableProperty MaxProperty =
            BindableProperty.CreateAttached(
                    "Max",
                    typeof(int),
                    typeof(NumberPicker),
                    default(int)
                );

        public static void SetMax(BindableObject view, int value) {
            view.SetValue(MaxProperty, value);
        }

        public static int GetMax(BindableObject view) {
            return (int)view.GetValue(MaxProperty);
        }

        public static readonly BindableProperty MinProperty =
            BindableProperty.CreateAttached(
                    "Min",
                    typeof(int),
                    typeof(NumberPicker),
                    default(int)
                );

        public static void SetMin(BindableObject view, int value) {
            view.SetValue(MinProperty, value);
        }

        public static int GetMin(BindableObject view) {
            return (int)view.GetValue(MinProperty);
        }

        public static readonly BindableProperty SelectedItemProperty =
            BindableProperty.CreateAttached(
                    "SelectedItem",
                    typeof(int),
                    typeof(NumberPicker),
                    default(int)
                );

        public static void SetSelectedItem(BindableObject view, int value) {
            view.SetValue(SelectedItemProperty, value);
        }

        public static int GetSelectedItem(BindableObject view) {
            return (int)view.GetValue(SelectedItemProperty);
        }

        public static readonly BindableProperty TitleProperty =
            BindableProperty.CreateAttached(
                    "Title",
                    typeof(string),
                    typeof(NumberPicker),
                    default(string)
                );

        public static void SetTitle(BindableObject view, string value) {
            view.SetValue(TitleProperty, value);
        }

        public static string GetTitle(BindableObject view) {
            return (string)view.GetValue(TitleProperty);
        }


        public static readonly BindableProperty CommandProperty =
            BindableProperty.CreateAttached(
                    "Command",
                    typeof(ICommand),
                    typeof(NumberPicker),
                    default(ICommand)
                );

        public static void SetCommand(BindableObject view, ICommand value) {
            view.SetValue(CommandProperty, value);
        }

        public static ICommand GetCommand(BindableObject view) {
            return (ICommand)view.GetValue(CommandProperty);
        }

        private static void OnIsEnabledChanged(BindableObject bindable, object oldValue, object newValue) {
            var view = bindable as View;
            if (view == null)
                return;

            var enabled = (bool)newValue;
            if (enabled) {
                view.Effects.Add(new NumberPickerRoutingEffect());
            }
            else {
                var toRemove = view.Effects.FirstOrDefault(e => e is NumberPickerRoutingEffect);
                if (toRemove != null)
                    view.Effects.Remove(toRemove);
            }
        }

        class NumberPickerRoutingEffect : RoutingEffect
        {
            public NumberPickerRoutingEffect() : base("Xamarin." + nameof(NumberPicker)) {

            }
        }
    }
}
