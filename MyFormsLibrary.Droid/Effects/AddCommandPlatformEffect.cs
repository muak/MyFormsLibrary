using System;
using Android.Views;
using Android.Widget;
using MyFormsLibrary.Droid.Effects;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using MyFormsLibrary.CustomViews;
using Android.Content;
using Android.Graphics.Drawables.Shapes;
using System.Threading.Tasks;
using MyFormsLibrary.Effects;
using System.Windows.Input;
using System.Collections.Generic;

[assembly: ResolutionGroupName("Xamarin")]
[assembly: ExportEffect(typeof(AddCommandPlatformEffect), nameof(AddCommand))]
namespace MyFormsLibrary.Droid.Effects
{
	
	public class AddCommandPlatformEffect:PlatformEffect
	{
		private ICommand command;
		private object commandParameter;
        private ICommand longCommand;
        private object longCommandParameter;
		private Android.Views.View view;
		private FrameLayout layer;


		protected override void OnAttached() {
			
			view = Control ?? Container;

			UpdateCommand();
			UpdateCommandParameter();
            UpdateLongCommand();
            UpdateLongCommandParameter();
			UpdateEffectColor();

			view.Click += OnClick;
		}

		protected override void OnDetached() {
			
			if (layer != null) {
				layer.Dispose();
				layer = null;
			}
			//view.Click -= OnClick;
			//view.Touch -= View_Touch;
		}


		void OnClick(object sender, EventArgs e) {
			command?.Execute(commandParameter ?? Element);
		}

        void OnLongClick(object sender, Android.Views.View.LongClickEventArgs e)
        {
            if (longCommand == null) {
                e.Handled = false;
                return;
            }

            longCommand?.Execute((longCommandParameter ?? commandParameter) ?? Element);

            e.Handled = true;
        }

        void View_Touch(object sender, Android.Views.View.TouchEventArgs e) {
			if (e.Event.Action == MotionEventActions.Down) {
				Container.AddView(layer);
				layer.Top = 0;
				layer.Left = 0;
				layer.Right = view.Width; // Container.Right;
				layer.Bottom = view.Height; //Container.Bottom;
				layer.BringToFront();
			}
			if (e.Event.Action == MotionEventActions.Up || e.Event.Action == MotionEventActions.Cancel) {
				Container.RemoveView(layer);
			}

			e.Handled = false;
		}

		void UpdateCommand() {
			command = AddCommand.GetCommand(Element);
		}
		void UpdateCommandParameter() {
			commandParameter = AddCommand.GetCommandParameter(Element);
		}

        void UpdateLongCommand()
        {
            if (longCommand != null) {
                view.LongClick -= OnLongClick;
            }
            longCommand = AddCommand.GetLongCommand(Element);
            if (longCommand == null) {
                return;
            }

            view.LongClick += OnLongClick;

        }
        void UpdateLongCommandParameter()
        {
            longCommandParameter = AddCommand.GetLongCommandParameter(Element);
        }

		void UpdateEffectColor() {
			
			view.Touch -= View_Touch;
			if (layer != null) {
				layer.Dispose();
				layer = null;
			}
			var color = AddCommand.GetEffectColor(Element);
			if (color == Xamarin.Forms.Color.Default) {
				return;
			}

			layer = new FrameLayout(Container.Context);
			layer.LayoutParameters = new ViewGroup.LayoutParams(-1, -1);
			layer.SetBackgroundColor(color.ToAndroid());
			view.Touch += View_Touch;
		}

		protected override void OnElementPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e) {
			base.OnElementPropertyChanged(e);

			if (e.PropertyName == AddCommand.CommandProperty.PropertyName) {
				UpdateCommand();
			}
			else if (e.PropertyName == AddCommand.CommandParameterProperty.PropertyName) {
				UpdateCommandParameter();
			}
			else if (e.PropertyName == AddCommand.EffectColorProperty.PropertyName) {
				UpdateEffectColor();
			}
            else if (e.PropertyName == AddCommand.LongCommandProperty.PropertyName) {
                UpdateLongCommand();
            }
            else if (e.PropertyName == AddCommand.LongCommandParameterProperty.PropertyName) {
                UpdateLongCommandParameter();
            }
		}
	}
}

