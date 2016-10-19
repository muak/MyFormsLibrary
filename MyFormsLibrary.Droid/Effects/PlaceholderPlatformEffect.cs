using System;
using MyFormsLibrary.Droid.Effects;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Widget;
using MyFormsLibrary.Effects;

[assembly: ExportEffect(typeof(PlaceholderPlatformEffect), nameof(Placeholder))]
namespace MyFormsLibrary.Droid.Effects
{
	public class PlaceholderPlatformEffect:PlatformEffect
	{
		private EditText editText;

		protected override void OnAttached() {
			editText = Control as EditText;
			UpdateText();
			UpdateColor();
		}

		protected override void OnDetached() {
			
		}

		private void UpdateText() {
			editText.Hint = Placeholder.GetPlaceholder(Element);
		}

		private void UpdateColor() {
			editText.SetHintTextColor(Placeholder.GetPlaceholderColor(Element).ToAndroid());
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

