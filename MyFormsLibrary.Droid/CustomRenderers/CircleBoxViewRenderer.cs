using System;
using Android.Graphics;
using MyFormsLibrary.CustomRenderers;
using Xamarin.Forms.Platform.Android;
using MyFormsLibrary.Droid.CustomRenderers;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(CircleBoxView), typeof(CircleBoxViewRenderer))]
namespace MyFormsLibrary.Droid.CustomRenderers
{
	public class CircleBoxViewRenderer:BoxRenderer
	{
		private int radius = 0;

		protected override void OnElementChanged(ElementChangedEventArgs<BoxView> e) {
			base.OnElementChanged(e);

			radius = (Element as CircleBoxView).Radius;
		}

		protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == CircleBoxView.RadiusProperty.PropertyName) {
				radius = (Element as CircleBoxView).Radius;
				Invalidate();
			}
		}

		public override void Draw(Android.Graphics.Canvas canvas) {
			if (radius == 0) {
				base.Draw(canvas);
			}
			else {
				using (var paint = new Paint()) {

					paint.AntiAlias = true;
					paint.Color = Element.Color.ToAndroid();
					paint.SetMaskFilter(null);
					var rectangle = new RectF(0, 0, Width, Height);

					double scale = (int)Context.Resources.DisplayMetrics.DensityDpi / 160d;
					int realRadius = (int)(radius * scale);
					canvas.DrawRoundRect(rectangle, realRadius, realRadius, paint);
				}
			}
		}

	}
}

