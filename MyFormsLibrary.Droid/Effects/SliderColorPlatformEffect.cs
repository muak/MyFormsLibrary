using System;
using MyFormsLibrary.Droid.Effects;
using MyFormsLibrary.Effects;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Widget;
using Android.Graphics;
using XfColor = Xamarin.Forms.Color;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using Android.Content.Res;

[assembly: ExportEffect(typeof(SliderColorPlatformEffect), nameof(SliderColor))]
namespace MyFormsLibrary.Droid.Effects
{
	public class SliderColorPlatformEffect:PlatformEffect
	{
		private SeekBar slider;
		private Drawable thumb;
		private LayerDrawable progress;

		protected override void OnAttached() {
			slider = Control as SeekBar;

			UpdateTrackColor();
			UpdateThumbColor();
		}

		protected override void OnDetached() {
			thumb.Dispose();
			progress.Dispose();
			thumb = null;
			progress = null;
		}

		void UpdateTrackColor() {

			var minColor = SliderColor.GetMinTrackColor(Element);
			var maxColor = SliderColor.GetMaxTrackColor(Element);

			if (minColor == XfColor.Default && maxColor == XfColor.Default) {
				return;
			}

			progress = (LayerDrawable)(progress ?? slider.ProgressDrawable.Current.GetConstantState().NewDrawable());

			if (maxColor != XfColor.Default) {
				progress.GetDrawable(0).SetColorFilter(maxColor.ToAndroid(), PorterDuff.Mode.SrcIn);
			}
			if (minColor != XfColor.Default) {
				progress.GetDrawable(2).SetColorFilter(minColor.ToAndroid(), PorterDuff.Mode.SrcIn);
			}

			slider.ProgressDrawable = progress;
		}

		void UpdateThumbColor() {
			var color = SliderColor.GetThumbColor(Element);
			if (color != XfColor.Default) {
				thumb = thumb ?? slider.Thumb.GetConstantState().NewDrawable();
				thumb.SetColorFilter(color.ToAndroid(), PorterDuff.Mode.SrcIn);
				slider.SetThumb(thumb);
			}
		}

		protected override void OnElementPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e) {
			base.OnElementPropertyChanged(e);

			if (e.PropertyName == SliderColor.MaxTrackColorProperty.PropertyName) {
				UpdateTrackColor();
			}
			else if (e.PropertyName == SliderColor.MinTrackColorProperty.PropertyName) {
				UpdateTrackColor();
			}
			else if (e.PropertyName == SliderColor.ThumbColorProperty.PropertyName) {
				UpdateThumbColor();
			}
		}
	}
}

