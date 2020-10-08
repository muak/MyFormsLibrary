using System;
using Xamarin.Forms;

namespace MyFormsLibrary.CustomRenderers
{
    public class MyToolbarItem:ToolbarItem
    {
        public static BindableProperty IsVisibleProperty = BindableProperty.Create(
            nameof(IsVisible),
            typeof(bool),
            typeof(MyToolbarItem),
            default(bool),
            defaultBindingMode: BindingMode.OneWay
        );

        public bool IsVisible
        {
            get { return (bool)GetValue(IsVisibleProperty); }
            set { SetValue(IsVisibleProperty, value); }
        }
    }
}
