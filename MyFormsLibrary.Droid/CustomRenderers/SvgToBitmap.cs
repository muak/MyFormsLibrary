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

        public static Bitmap GetBitmap(string resource, double width, double height, Xamarin.Forms.Color color)
        {
            if (color == Xamarin.Forms.Color.Default) {
                return GetBitmap(resource, width, height);
            }

            var g = SvgLoader.GetResourceAndLoadSvg(resource);

            double scale = (int)Android.App.Application.Context.Resources.DisplayMetrics.DensityDpi / 160d;

            var transform = Transform.AspectFillRect(g.ViewBox, new NGraphics.Rect(0, 0, width, height));
            var transGraphic = g.TransformGeometry(transform);

            var canvas = Platforms.Current.CreateImageCanvas(new NGraphics.Size(width, height), scale);

            var nColor = new NGraphics.Color(color.R, color.G, color.B, color.A);

            foreach (var element in transGraphic.Children) {

                ApplyColor(element, nColor);
                element.Draw(canvas);
            }

            return (canvas.GetImage() as NGraphics.BitmapImage)?.Bitmap;
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

