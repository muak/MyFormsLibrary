using System;
using Xamarin.Forms;
using System.Windows.Input;
using MyFormsLibrary.CustomRenderers;

namespace MyFormsLibrary.CustomRenderers
{
    public class CommandCell:CellBase
    {
        public CommandCell() {
        }

        public static BindableProperty ValueTextProperty =
            BindableProperty.Create(
                nameof(ValueText),
                typeof(string),
                typeof(CommandCell),
                default(string),
                defaultBindingMode: BindingMode.OneWay
            );

        public string ValueText {
            get { return (string)GetValue(ValueTextProperty); }
            set { SetValue(ValueTextProperty, value); }
        }

        public static BindableProperty ValueTextColorProperty =
            BindableProperty.Create(
                nameof(ValueTextColor),
                typeof(Color),
                typeof(CommandCell),
                default(Color),
                defaultBindingMode: BindingMode.OneWay
            );

        public Color ValueTextColor {
            get { return (Color)GetValue(ValueTextColorProperty); }
            set { SetValue(ValueTextColorProperty, value); }
        }

        public static BindableProperty ValueTextFontSizeProperty =
            BindableProperty.Create(
                nameof(ValueTextFontSize),
                typeof(double),
                typeof(CommandCell),
                -1.0d,
                defaultBindingMode: BindingMode.OneWay
            );

        [TypeConverter(typeof(FontSizeConverter))]
        public double ValueTextFontSize {
            get { return (double)GetValue(ValueTextFontSizeProperty); }
            set { SetValue(ValueTextFontSizeProperty, value); }
        }

        public static BindableProperty CommandProperty =
            BindableProperty.Create(
                nameof(Command),
                typeof(ICommand),
                typeof(CommandCell),
                default(ICommand),
                defaultBindingMode: BindingMode.OneWay
            );

        public ICommand Command {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static BindableProperty CommandParameterProperty =
            BindableProperty.Create(
                nameof(CommandParameter),
                typeof(object),
                typeof(CommandCell),
                default(object),
                defaultBindingMode: BindingMode.OneWay
            );

        public object CommandParameter {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }
    }
}
