using System;
using MyFormsLibrary.iOS.Effects;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using UIKit;
using CoreGraphics;
using Foundation;
using System.Linq;
using MyFormsLibrary.Effects;
using System.Runtime.CompilerServices;
using ObjCRuntime;
using System.Threading.Tasks;

[assembly: ExportEffect(typeof(PlaceholderPlatformEffect), nameof(Placeholder))]
namespace MyFormsLibrary.iOS.Effects
{
	public class PlaceholderPlatformEffect:PlatformEffect
	{

		private UITextView textView;
		private UILabel placeHolderLabel;
		private NSLayoutConstraint[] constraint;

		protected override void OnAttached() {

			textView = Control as UITextView;
			placeHolderLabel = new UILabel();

			placeHolderLabel.LineBreakMode = UILineBreakMode.WordWrap;
			placeHolderLabel.Lines = 0;
			placeHolderLabel.Font = textView.Font;
			placeHolderLabel.BackgroundColor = UIColor.Clear;

			placeHolderLabel.Alpha = 0;

			UpdateText();
			UpdateColor();

			textView.AddSubview(placeHolderLabel);

			textView.AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth;
			placeHolderLabel.TranslatesAutoresizingMaskIntoConstraints = false;
			constraint = new NSLayoutConstraint[]{
				NSLayoutConstraint.Create(
					placeHolderLabel,
					NSLayoutAttribute.Top,
					NSLayoutRelation.Equal,
					textView,
					NSLayoutAttribute.Top,
					1,
					8
				),
				NSLayoutConstraint.Create(
					placeHolderLabel,
					NSLayoutAttribute.Left,
					NSLayoutRelation.Equal,
					textView,
					NSLayoutAttribute.Left,
					1,
					4
				),
				NSLayoutConstraint.Create(
					placeHolderLabel,
					NSLayoutAttribute.Right,
					NSLayoutRelation.Equal,
					textView,
					NSLayoutAttribute.Right,
					1,
					4
				),
				NSLayoutConstraint.Create(
					placeHolderLabel,
					NSLayoutAttribute.Bottom,
					NSLayoutRelation.Equal,
					textView,
					NSLayoutAttribute.Bottom,
					1,
					8
				),
				NSLayoutConstraint.Create(
					placeHolderLabel,
					NSLayoutAttribute.Width,
					NSLayoutRelation.Equal,
					textView,
					NSLayoutAttribute.Width,
					1,
					-8
				),
				
			};
			textView.AddConstraints(constraint);
			placeHolderLabel.SizeToFit();
			textView.SendSubviewToBack(placeHolderLabel);

			if (textView.Text.Length == 0 && placeHolderLabel.Text.Length > 0) {
				placeHolderLabel.Alpha = 1;
			}

			textView.Changed += TextChanged;
		}

		protected override void OnDetached() {
			textView.Changed -= TextChanged;
			Control.RemoveConstraints(constraint);
			placeHolderLabel.RemoveFromSuperview();
			placeHolderLabel.Dispose();
		}


		private void UpdateText(){
			placeHolderLabel.Text = Placeholder.GetPlaceholder(Element);
		}

		private void UpdateColor() {
			placeHolderLabel.TextColor = Placeholder.GetPlaceholderColor(Element).ToUIColor();
		}

		private void TextChanged(object sender, EventArgs e) {
			if (placeHolderLabel.Text.Length == 0) {
				return;
			}
			if (textView.Text.Length == 0) {
				placeHolderLabel.Alpha = 1;
			}
			else {
				placeHolderLabel.Alpha = 0;
			}
		}

		protected override void OnElementPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e) {
			base.OnElementPropertyChanged(e);

			if (e.PropertyName == Placeholder.PlaceholderProperty.PropertyName) {
				UpdateText();
			}
			else if (e.PropertyName == Placeholder.PlaceholderColorProperty.PropertyName) {
				UpdateColor();
			}
		}


	}
}

