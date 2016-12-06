using System;
using System.Windows.Input;
using MyFormsLibrary.Effects;
using MyFormsLibrary.iOS.Effects;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using UIKit;
using CoreGraphics;
using System.Threading.Tasks;

[assembly: ResolutionGroupName("Xamarin")]
[assembly: ExportEffect(typeof(AddCommandPlatformEffect), nameof(AddCommand))]
namespace MyFormsLibrary.iOS.Effects
{
    public class AddCommandPlatformEffect : PlatformEffect
    {

        private ICommand command;
        private object commandParameter;
        private ICommand longCommand;
        private object longCommandParameter;
        private UITapGestureRecognizer tapGesture;
        private UILongPressGestureRecognizer longTapGesture;
        private UIView view;
        private UIView layer;

        protected override void OnAttached()
        {
            view = Control ?? Container;

            tapGesture = new UITapGestureRecognizer(async (obj) => {
                if (layer != null) {
                    layer.Frame = new CGRect(0, 0, Container.Bounds.Width, Container.Bounds.Height);
                    Container.AddSubview(layer);
                    Container.BringSubviewToFront(layer);
                    layer.Alpha = 1;
                    await UIView.AnimateAsync(0.3f, () => {
                        layer.Alpha = 0;
                    });
                    layer.RemoveFromSuperview();
                }

                command?.Execute(commandParameter ?? Element);
            });



            view.UserInteractionEnabled = true;
            view.AddGestureRecognizer(tapGesture);

            UpdateCommand();
            UpdateCommandParameter();
            UpdateLongCommand();
            UpdateLongCommandParameter();
            UpdateEffectColor();
        }

        protected override void OnDetached()
        {
            view.RemoveGestureRecognizer(tapGesture);
            tapGesture.Dispose();

            if (longTapGesture != null) {
                view.RemoveGestureRecognizer(longTapGesture);
                longTapGesture.Dispose();
            }

            if (layer != null) {
                layer.Dispose();
                layer = null;
            }
        }

        void UpdateCommand()
        {
            command = AddCommand.GetCommand(Element);
        }
        void UpdateCommandParameter()
        {
            commandParameter = AddCommand.GetCommandParameter(Element);
        }
        void UpdateLongCommand()
        {
            longCommand = AddCommand.GetLongCommand(Element);
            if (longTapGesture != null) {
                view.RemoveGestureRecognizer(longTapGesture);
                longTapGesture.Dispose();
            }
            if (longCommand == null) {
                return;
            }
            longTapGesture = new UILongPressGestureRecognizer(async (obj) => {
                if (obj.State == UIGestureRecognizerState.Began) {
                    longCommand?.Execute((longCommandParameter ?? commandParameter) ?? Element);

                    if (layer != null) {
                        layer.Frame = new CGRect(0, 0, Container.Bounds.Width, Container.Bounds.Height);
                        Container.AddSubview(layer);
                        Container.BringSubviewToFront(layer);
                        layer.Alpha = 1;
                        await UIView.AnimateAsync(1.5f, () => {
                            layer.Alpha = 0;
                        });
                        layer.RemoveFromSuperview();
                    }
                }
            });
            view.AddGestureRecognizer(longTapGesture);

        }
        void UpdateLongCommandParameter()
        {
            longCommandParameter = AddCommand.GetLongCommandParameter(Element);
        }

        void UpdateEffectColor()
        {

            if (layer != null) {
                layer.Dispose();
                layer = null;
            }

            var color = AddCommand.GetEffectColor(Element);
            if (color == Xamarin.Forms.Color.Default) {
                return;
            }

            layer = new UIView();
            layer.BackgroundColor = color.ToUIColor();

        }

        protected override void OnElementPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
        {
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

