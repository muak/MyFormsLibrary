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
			throw new NotImplementedException();
		}
	}
}

