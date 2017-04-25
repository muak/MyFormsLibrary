using System;
using Xamarin.Forms;

namespace MyFormsLibrary.CustomRenderers
{
    public class LabelCell:CellBase
    {
        public LabelCell() {
        }

        public static BindableProperty ValueTextProperty =
            BindableProperty.Create(
                nameof(ValueText),
                typeof(string),
                typeof(LabelCell),
                default(string),
                defaultBindingMode: BindingMode.OneWay
            );

        public string ValueText {
            get { return (string)GetValue(ValueTextProperty); }
            set { SetValue(ValueTextProperty, value); }
        }

    }
}
