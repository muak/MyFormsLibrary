using System;
using MyFormsLibrary.Droid.DependencyServices;
using Xamarin.Forms;
using MyFormsLibrary.DependencyServices;
using NGraphics;


[assembly: Dependency(typeof(SvgService))]
namespace MyFormsLibrary.Droid.DependencyServices
{
	public class SvgService:ISvgService
	{
		
		public IImage GetCanvas(Graphic g, double width, double height) {
			double scale = (int)Android.App.Application.Context.Resources.DisplayMetrics.DensityDpi / 160d;

			var transform = Transform.AspectFillRect(g.ViewBox, new NGraphics.Rect(0, 0,width,height));
			var transGraphic = g.TransformGeometry(transform);

			var canvas = Platforms.Current.CreateImageCanvas(new NGraphics.Size(width,height), scale);

			transGraphic.Draw(canvas);

			return canvas.GetImage();
		}

		public IImage GetCanvas(Graphic g, double width, double height, Xamarin.Forms.Color color) {
            if (color == Xamarin.Forms.Color.Default) {
                return GetCanvas(g, width, height);
            }

            double scale = (int)Android.App.Application.Context.Resources.DisplayMetrics.DensityDpi / 160d;

            var transform = Transform.AspectFillRect(g.ViewBox, new NGraphics.Rect(0, 0, width, height));
            var transGraphic = g.TransformGeometry(transform);

            var canvas = Platforms.Current.CreateImageCanvas(new NGraphics.Size(width, height), scale);

            var nColor = new NGraphics.Color(color.R, color.G, color.B, color.A);

            foreach (var element in transGraphic.Children) {

                ApplyColor(element, nColor);
                element.Draw(canvas);
            }

            return canvas.GetImage();
		}

        private void ApplyColor(NGraphics.Element element, NGraphics.Color color) {
            var children = (element as Group)?.Children;
            if (children != null) {
                foreach (var child in children) {
                    ApplyColor(child, color);
                }
            }

            if (element?.Pen != null) {
                element.Pen = new Pen(color, element.Pen.Width);
            }

            if (element?.Brush != null) {
                element.Brush = new SolidBrush(color);
            }
        }
	}
}

