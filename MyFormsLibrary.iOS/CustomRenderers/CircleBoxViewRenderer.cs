using System;
using Xamarin.Forms.Platform.iOS;
using MyFormsLibrary.CustomRenderers;
using UIKit;
using CoreGraphics;
using Xamarin.Forms;
using MyFormsLibrary.iOS.CustomRenderers;

[assembly: ExportRenderer(typeof(CircleBoxView), typeof(CircleBoxViewRenderer))]
namespace MyFormsLibrary.iOS.CustomRenderers
{
	public class CircleBoxViewRenderer:BoxRenderer
	{
		private int radius = 0;


		protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.BoxView> e) {
			base.OnElementChanged(e);

			if (e.NewElement != null) {
				radius = (Element as CircleBoxView).Radius;
			}
		}

		protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == CircleBoxView.RadiusProperty.PropertyName) {
				radius = (Element as CircleBoxView).Radius;
				SetNeedsDisplay();
			}

		}

		public override void Draw(CoreGraphics.CGRect rect) {
			if (radius == 0) {
				base.Draw(rect);
			}
			else {        
				using (var context = UIGraphics.GetCurrentContext()) {   

					context.SetFillColor(Element.Color.ToCGColor());    
					context.AddPath(CGPath.FromRoundedRect(rect, radius, radius));
					context.DrawPath(CGPathDrawingMode.Fill);
				}
			}
		}
	}
}

