using System;
using Xamarin.Forms;
using System.Windows.Input;

namespace MyFormsLibrary.CustomRenderers
{
    public class NumberPickerCell:CellBase
    {
        public NumberPickerCell()
        {
        }

        public static BindableProperty NumberProperty =
            BindableProperty.Create(
                nameof(Number),
                typeof(int),
                typeof(NumberPickerCell),
                default(int),
                defaultBindingMode: BindingMode.TwoWay
            );

        public int Number {
            get { return (int)GetValue(NumberProperty); }
            set { SetValue(NumberProperty, value); }
        }

        public static BindableProperty MinProperty =
            BindableProperty.Create(
                nameof(Min),
                typeof(int),
                typeof(NumberPickerCell),
                0,
                defaultBindingMode: BindingMode.OneWay
            );

        public int Min {
            get { return (int)GetValue(MinProperty); }
            set { SetValue(MinProperty, value); }
        }

        public static BindableProperty MaxProperty =
            BindableProperty.Create(
                nameof(Max),
                typeof(int),
                typeof(NumberPickerCell),
                9999,
                defaultBindingMode: BindingMode.OneWay
            );

        public int Max {
            get { return (int)GetValue(MaxProperty); }
            set { SetValue(MaxProperty, value); }
        }

        public static BindableProperty TitleProperty =
            BindableProperty.Create(
                nameof(Title),
                typeof(string),
                typeof(NumberPickerCell),
                default(string),
                defaultBindingMode: BindingMode.OneWay
            );

        public string Title {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static BindableProperty CommandProperty =
            BindableProperty.Create(
                nameof(Command),
                typeof(ICommand),
                typeof(NumberPickerCell),
                default(ICommand),
                defaultBindingMode: BindingMode.OneWay
            );

        public ICommand Command {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

    }
}
