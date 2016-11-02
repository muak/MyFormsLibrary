using System;
using Xamarin.Forms;
using System.Windows.Input;
using MyFormsLibrary.CustomRenderers;

namespace MyFormsLibrary.CustomRenderers
{
    public class CommandCell:LabelCell
    {
        public CommandCell() {
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
