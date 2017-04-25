using System;
using Xamarin.Forms;

namespace MyFormsLibrary.CustomRenderers
{
    public class SwitchCellAlt:CellBase
    {
        public SwitchCellAlt() {
        }

        public static BindableProperty OnProperty =
            BindableProperty.Create(
                nameof(On),
                typeof(bool),
                typeof(SwitchCellAlt),
                default(bool),
                defaultBindingMode: BindingMode.TwoWay
            );

        public bool On {
            get { return (bool)GetValue(OnProperty); }
            set { SetValue(OnProperty, value); }
        }

        public static BindableProperty AccentColorProperty =
            BindableProperty.Create(
                nameof(AccentColor),
                typeof(Color),
                typeof(SwitchCellAlt),
                default(Color),
                defaultBindingMode: BindingMode.OneWay
            );

        public Color AccentColor {
            get { return (Color)GetValue(AccentColorProperty); }
            set { SetValue(AccentColorProperty, value); }
        }
    }
}
