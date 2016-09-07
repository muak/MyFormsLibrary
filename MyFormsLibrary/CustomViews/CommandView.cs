﻿using System;
using System.Windows.Input;
using Xamarin.Forms;
namespace MyFormsLibrary.CustomViews
{
	public class CommandView:ContentView
	{
		public static readonly BindableProperty CommandParameterProperty =
			BindableProperty.Create(
				propertyName: nameof(CommandParameter),
				returnType: typeof(object),
				declaringType: typeof(CommandView),
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
				declaringType: typeof(CommandView),
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

		public CommandView() {
			var tgr = new TapGestureRecognizer();
			tgr.Tapped += (s, e) => {
				this.Command?.Execute(this.CommandParameter ?? this);
			};
			this.GestureRecognizers.Add(tgr);
		}


	}
}

