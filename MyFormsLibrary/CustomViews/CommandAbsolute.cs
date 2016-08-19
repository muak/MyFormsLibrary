using System;
using System.Windows.Input;
using Xamarin.Forms;
namespace MyFormsLibrary.CustomViews
{
	public class CommandAbsolute:AbsoluteLayout
	{
		public static readonly BindableProperty CommandParameterProperty =
			BindableProperty.Create(
				propertyName: nameof(CommandParameter),
				returnType: typeof(object),
				declaringType: typeof(CommandAbsolute),
				defaultValue: default(object),
				defaultBindingMode: BindingMode.OneWay
			);

		public object CommandParameter {
			get {
				return this.GetValue(CommandParameterProperty);
			}
			set {
				this.SetValue(CommandParameterProperty, value);
			}
		}

		public static readonly BindableProperty CommandProperty =
			BindableProperty.Create(
				propertyName: nameof(Command),
				returnType: typeof(ICommand),
				declaringType: typeof(CommandAbsolute),
				defaultValue: default(Command),
				defaultBindingMode: BindingMode.OneWay
			);

		public ICommand Command {
			get {
				return (ICommand)this.GetValue(CommandProperty);
			}
			set {
				this.SetValue(CommandProperty, value);
			}
		}

		public CommandAbsolute() {
			//var tgr = new TapGestureRecognizer();
			//tgr.Tapped += (s, e) => OnTapped();
			//this.GestureRecognizers.Add(tgr);
		}

		protected void OnTapped() {
			if (this.Command != null) {
				this.Command.Execute(this.CommandParameter ?? this);
			}
			if (this.Parent is ViewCell) {
				var cell = this.Parent as IListViewController;
				//cell.NotifyRowTapped();
			}

		}
	}
}

