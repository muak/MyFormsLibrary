using Android.Graphics;
using MyFormsLibrary.Infrastructure;
using Xamarin.Forms.Svg;

namespace MyFormsLibrary.Droid.CustomRenderers
{
    public static class SvgToBitmap
    {
        public static Bitmap GetBitmap(string resource, double width, double height)
        {
            var stream = SvgUtility.CreateImage(SvgLoader.GetResourceStream(resource), width, height, Xamarin.Forms.Color.Default).Result;
            return BitmapFactory.DecodeStream(stream);
        }

        public static Bitmap GetBitmap(string resource, double width, double height, Xamarin.Forms.Color color)
        {
            if (color == Xamarin.Forms.Color.Default) {
                return GetBitmap(resource, width, height);
            }

            var stream = SvgUtility.CreateImage(SvgLoader.GetResourceStream(resource), width, height, color).Result;
            return BitmapFactory.DecodeStream(stream);
        }
    }
}

