using System;
using Xamarin.Forms;
namespace MyFormsLibrary.CustomRenderers
{
    public class EntryCellEx:EntryCell
    {
        public static BindableProperty TextColorProperty =
            BindableProperty.Create(
                nameof(TextColor),
                typeof(Color),
                typeof(EntryCellEx),
                default(Color),
                defaultBindingMode: BindingMode.OneWay
            );

        public Color TextColor {
            get { return (Color)GetValue(TextColorProperty); }
            set { SetValue(TextColorProperty, value); }
        }
    }
}
