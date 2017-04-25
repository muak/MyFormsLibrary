using System;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace MyFormsLibrary.Effects
{
	public static class AddCommand
	{
		public static readonly BindableProperty IsEnabledProperty =
			BindableProperty.CreateAttached(
					propertyName: "IsEnabled",
					returnType: typeof(bool),
					declaringType: typeof(AddCommand),
					defaultValue: false,
					propertyChanged: OnIsEnabledChanged
				);

		public static void SetIsEnabled(BindableObject view, bool value) {
			view.SetValue(IsEnabledProperty, value);
		}

		public static bool GetIsEnabled(BindableObject view) {
			return (bool)view.GetValue(IsEnabledProperty);
		}

		private static void OnIsEnabledChanged(BindableObject bindable, object oldValue, object newValue) {
			var view = bindable as View;
			if (view == null)
				return;
			
			var enabled = (bool)newValue;
			if (enabled) {
				view.Effects.Add(new AddCommandRoutingEffect());
			}
			else {
				var toRemove = view.Effects.FirstOrDefault(e => e is AddCommandRoutingEffect);
				if (toRemove != null)
					view.Effects.Remove(toRemove);
			}
		}

		public static readonly BindableProperty CommandProperty =
			BindableProperty.CreateAttached(
					"Command",
					typeof(ICommand),
					typeof(AddCommand),
					default(ICommand)
				);

		public static void SetCommand(BindableObject view, ICommand value) {
			view.SetValue(CommandProperty, value);
		}

		public static ICommand GetCommand(BindableObject view) {
			return (ICommand)view.GetValue(CommandProperty);
		}


		public static readonly BindableProperty CommandParameterProperty =
			BindableProperty.CreateAttached(
					"CommandParameter",
					typeof(object),
					typeof(AddCommand),
					default(object)
				);

		public static void SetCommandParameter(BindableObject view, object value) {
			view.SetValue(CommandParameterProperty, value);
		}

		public static object GetCommandParameter(BindableObject view) {
			return (object)view.GetValue(CommandParameterProperty);
		}

		public static readonly BindableProperty EffectColorProperty =
			BindableProperty.CreateAttached(
					"EffectColor",
					typeof(Color),
					typeof(AddCommand),
					Color.Default
				);

		public static void SetEffectColor(BindableObject view, Color value) {
			view.SetValue(EffectColorProperty, value);
		}

		public static Color GetEffectColor(BindableObject view) {
			return (Color)view.GetValue(EffectColorProperty);
		}

        public static readonly BindableProperty LongCommandProperty =
            BindableProperty.CreateAttached(
                    "LongCommand",
                    typeof(ICommand),
                    typeof(AddCommand),
                    default(ICommand)
                );

        public static void SetLongCommand(BindableObject view, ICommand value)
        {
            view.SetValue(LongCommandProperty, value);
        }

        public static ICommand GetLongCommand(BindableObject view)
        {
            return (ICommand)view.GetValue(LongCommandProperty);
        }

        public static readonly BindableProperty LongCommandParameterProperty =
            BindableProperty.CreateAttached(
                    "LongCommandParameter",
                    typeof(object),
                    typeof(AddCommand),
                    default(object)
                );

        public static void SetLongCommandParameter(BindableObject view, object value)
        {
            view.SetValue(LongCommandParameterProperty, value);
        }

        public static object GetLongCommandParameter(BindableObject view)
        {
            return (object)view.GetValue(LongCommandParameterProperty);
        }


		class AddCommandRoutingEffect : RoutingEffect
		{
			public AddCommandRoutingEffect() : base("Xamarin." + nameof(AddCommand)) {

			}
		}
	}
}

