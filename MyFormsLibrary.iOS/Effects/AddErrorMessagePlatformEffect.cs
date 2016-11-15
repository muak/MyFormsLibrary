using System;
using MyFormsLibrary.Effects;
using MyFormsLibrary.iOS.Effects;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;


[assembly: ExportEffect(typeof(AddErrorMessagePlatformEffect), nameof(AddErrorMessage))]
namespace MyFormsLibrary.iOS.Effects
{
    public class AddErrorMessagePlatformEffect:PlatformEffect
    {
        private UILabel errorLabel;
        private NSLayoutConstraint[] constraint;

        protected override void OnAttached() {
            errorLabel = new UILabel();
            errorLabel.LineBreakMode = UILineBreakMode.Clip;
            errorLabel.Lines = 1;
            errorLabel.BackgroundColor = UIColor.FromWhiteAlpha(1, .5f);
            errorLabel.TextColor = UIColor.Red.ColorWithAlpha(0.8f);
            errorLabel.TintAdjustmentMode = UIViewTintAdjustmentMode.Automatic;
            errorLabel.AdjustsFontSizeToFitWidth = true;
            errorLabel.BaselineAdjustment = UIBaselineAdjustment.AlignCenters;
            errorLabel.TextAlignment = UITextAlignment.Center;
            errorLabel.AdjustsLetterSpacingToFitWidth = true;
            errorLabel.Font = errorLabel.Font.WithSize(10);

            UpdateText();

            Container.AddSubview(errorLabel);

            errorLabel.TranslatesAutoresizingMaskIntoConstraints = false;
            constraint = new NSLayoutConstraint[]{
                NSLayoutConstraint.Create(
                    errorLabel,
                    NSLayoutAttribute.Top,
                    NSLayoutRelation.Equal,
                    Container,
                    NSLayoutAttribute.Top,
                    1,
                    0
                ),
                NSLayoutConstraint.Create(
                    errorLabel,
                    NSLayoutAttribute.Right,
                    NSLayoutRelation.Equal,
                    Container,
                    NSLayoutAttribute.Right,
                    1,
                    0
                ),
                NSLayoutConstraint.Create(
                    errorLabel,
                    NSLayoutAttribute.Height,
                    NSLayoutRelation.Equal,
                    null,
                    NSLayoutAttribute.Height,
                    1,
                    14
                )
            };

            Container.AddConstraints(constraint);
            errorLabel.SizeToFit();

        }

        protected override void OnDetached() {
            Control.RemoveConstraints(constraint);
            errorLabel.RemoveFromSuperview();
            errorLabel.Dispose();
        }

        private void UpdateText() {
            var msg = AddErrorMessage.GetErrorMessage(Element);
            errorLabel.Text = $" {msg} ";
            errorLabel.Hidden = string.IsNullOrEmpty(msg);
        }

        protected override void OnElementPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e) {
            base.OnElementPropertyChanged(e);

            if (e.PropertyName == AddErrorMessage.ErrorMessageProperty.PropertyName) {
                UpdateText();
            }

        }
    }
}
