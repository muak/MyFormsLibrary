using System;
using Xamarin.Forms;
namespace MyFormsLibrary.CustomRenderers
{
    public static class SectionAt
    {
        public static readonly BindableProperty IsVisibleProperty =
            BindableProperty.CreateAttached(
                    "IsVisible",
                    typeof(bool),
                    typeof(SectionAt),
                    default(bool),
                    propertyChanged:(bindable, oldValue, newValue) => {
                        var s = bindable as TableSection;
                       
                    }
                );

        public static void SetIsVisible(BindableObject view, bool value)
        {
            view.SetValue(IsVisibleProperty, value);
        }

        public static bool GetIsVisible(BindableObject view)
        {
            return (bool)view.GetValue(IsVisibleProperty);
        }


    }
}
