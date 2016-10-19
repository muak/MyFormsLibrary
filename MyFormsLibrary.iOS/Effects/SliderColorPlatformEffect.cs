using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using MyFormsLibrary.Effects;
using MyFormsLibrary.iOS.Effects;
using UIKit;

[assembly: ExportEffect(typeof(SliderColorPlatformEffect), nameof(SliderColor))]
namespace MyFormsLibrary.iOS.Effects
{
	public class SliderColorPlatformEffect : PlatformEffect
	{
		private UISlider slider;

		protected override void OnAttached() {
			slider = Control as UISlider;

			UpdateMaxTrackColor();
			UpdateMinTrackColor();
			UpdateThumbColor();
		}

		protected override void OnDetached() {
			
		}

		void UpdateMaxTrackColor() {
			var color = SliderColor.GetMaxTrackColor(Element);
			if (color != Color.Default) {
				slider.MaximumTrackTintColor = color.ToUIColor();
			}
		}
		void UpdateMinTrackColor() {
			var color = SliderColor.GetMinTrackColor(Element);
			if (color != Color.Default) {
				slider.MinimumTrackTintColor = color.ToUIColor();
			}
		}
		void UpdateThumbColor() {
			var color = SliderColor.GetThumbColor(Element);
			if (color != Color.Default) {
				//枠は消えてしまう
				slider.ThumbTintColor = color.ToUIColor();
			}
		}

		protected override void OnElementPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e) {
			base.OnElementPropertyChanged(e);

			if (e.PropertyName == SliderColor.MaxTrackColorProperty.PropertyName) {
				UpdateMaxTrackColor();
			}
			else if (e.PropertyName == SliderColor.MinTrackColorProperty.PropertyName) {
				UpdateMinTrackColor();
			}
			else if (e.PropertyName == SliderColor.ThumbColorProperty.PropertyName){
				UpdateThumbColor();
			}
		}
	}
}

