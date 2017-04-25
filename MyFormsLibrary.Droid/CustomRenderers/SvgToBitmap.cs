using Android.Graphics;
using MyFormsLibrary.Infrastructure;
using NGraphics;

namespace MyFormsLibrary.Droid.CustomRenderers
{
	public static class SvgToBitmap
	{
		public static Bitmap GetBitmap(string resource, double width, double height)
		{
			var g = SvgLoader.GetResourceAndLoadSvg(resource);

			double scale = (int)Android.App.Application.Context.Resources.DisplayMetrics.DensityDpi / 160d;

			var transform = Transform.AspectFillRect(g.ViewBox, new NGraphics.Rect(0, 0, width, height));
			var transGraphic = g.TransformGeometry(transform);

			var canvas = Platforms.Current.CreateImageCanvas(new NGraphics.Size(width, height), scale);

			transGraphic.Draw(canvas);

			return (canvas.GetImage() as NGraphics.BitmapImage)?.Bitmap;
		}

	}
}

