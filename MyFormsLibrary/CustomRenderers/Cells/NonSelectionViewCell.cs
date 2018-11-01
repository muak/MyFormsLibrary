using System.Windows.Input;
using MyFormsLibrary.CustomRenderers.Cells;
using Xamarin.Forms;

namespace MyFormsLibrary.CustomRenderers
{
    public class NonSelectionViewCell:ViewCell,IContextMenuCell
	{

		public static BindableProperty CommandProperty =
			BindableProperty.Create(
				nameof(Command),
				typeof(ICommand),
				typeof(NonSelectionViewCell),
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
				typeof(NonSelectionViewCell),
				default(object),
				defaultBindingMode: BindingMode.OneWay
			);

		public object CommandParameter {
			get { return (object)GetValue(CommandParameterProperty); }
			set { SetValue(CommandParameterProperty, value); }
		}

		public static BindableProperty ContextMenuTitleProperty =
			BindableProperty.Create(
				nameof(ContextMenuTitle),
				typeof(string),
				typeof(NonSelectionViewCell),
				default(string),
				defaultBindingMode: BindingMode.OneWay
			);

		public string ContextMenuTitle {
			get { return (string)GetValue(ContextMenuTitleProperty); }
			set { SetValue(ContextMenuTitleProperty, value); }
		}
	}
}

