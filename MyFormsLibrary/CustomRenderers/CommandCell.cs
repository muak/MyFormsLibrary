using System;
using Xamarin.Forms;
using System.Windows.Input;

namespace MyFormsLibrary.CustomRenderers
{
    public class CommandCell:Cell
    {
        public CommandCell() {
        }

        public static BindableProperty LabelTextProperty =
            BindableProperty.Create(
                nameof(LabelText),
                typeof(string),
                typeof(CommandCell),
                default(string),
                defaultBindingMode: BindingMode.OneWay
            );

        public string LabelText {
            get { return (string)GetValue(LabelTextProperty); }
            set { SetValue(LabelTextProperty, value); }
        }

        public static BindableProperty TextProperty =
            BindableProperty.Create(
                nameof(Text),
                typeof(string),
                typeof(CommandCell),
                default(string),
                defaultBindingMode: BindingMode.OneWay
            );

        public string Text {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
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
