using MyFormsLibrary.Infrastructure;
using NGraphics;
using UIKit;

namespace MyFormsLibrary.iOS.CustomRenderers
{
	public static class SvgToUIImage
	{
		public static UIImage GetUIImage(string resource, double width, double height)
		{
			var g = SvgLoader.GetResourceAndLoadSvg(resource);
			var transform = Transform.AspectFillRect(g.ViewBox, new Rect(0, 0, width, height));
			var transGraphic = g.TransformGeometry(transform);

			var canvas = Platforms.Current.CreateImageCanvas(
				new NGraphics.Size(width, height),
				UIScreen.MainScreen.Scale);

			transGraphic.Draw(canvas);

			return canvas.GetImage().GetUIImage();
		}

		public static UIImage GetUIImage(string resource, double width, double height, Xamarin.Forms.Color color)
		{
			if (color == Xamarin.Forms.Color.Default) {
				return GetUIImage(resource, width, height);
			}

			var g = SvgLoader.GetResourceAndLoadSvg(resource);

			var transform = Transform.AspectFillRect(g.ViewBox, new Rect(0, 0, width, height));
			var transGraphic = g.TransformGeometry(transform);

			var canvas = Platforms.Current.CreateImageCanvas(
				new NGraphics.Size(width, height),
				UIScreen.MainScreen.Scale);


			var nColor = new NGraphics.Color(color.R, color.G, color.B, color.A);

			foreach (var element in transGraphic.Children) {

				ApplyColor(element, nColor);
				element.Draw(canvas);
			}

			return canvas.GetImage().GetUIImage();
		}

		private static void ApplyColor(NGraphics.Element element, NGraphics.Color color)
		{
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