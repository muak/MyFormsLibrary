using MyFormsLibrary.Infrastructure;
using UIKit;
using Xamarin.Forms.Svg;
using Foundation;

namespace MyFormsLibrary.iOS.CustomRenderers
{
    public static class SvgToUIImage
	{
		public static UIImage GetUIImage(string resource, double width, double height)
		{
            var stream = SvgUtility.CreateImage(SvgLoader.GetResourceStream(resource), width, height, Xamarin.Forms.Color.Default).Result;
            return UIImage.LoadFromData(NSData.FromStream(stream), UIScreen.MainScreen.Scale);
		}

		public static UIImage GetUIImage(string resource, double width, double height, Xamarin.Forms.Color color)
		{
			if (color == Xamarin.Forms.Color.Default) {
				return GetUIImage(resource, width, height);
			}

            var stream = SvgUtility.CreateImage(SvgLoader.GetResourceStream(resource), width, height, color).Result;
            return UIImage.LoadFromData(NSData.FromStream(stream), UIScreen.MainScreen.Scale);
        }
	}
}