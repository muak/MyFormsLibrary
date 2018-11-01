using System;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using System.IO;

namespace MyFormsLibrary.iOS.CustomRenderers
{
    public static class ImageSourceExtensions
    {
        public static async Task<UIImage> ToUIImage(this ImageSource imageSource)
        {
            var handler = GetHandler(imageSource);

            return await handler.LoadImageAsync(imageSource);
        }

        private static IImageSourceHandler GetHandler(ImageSource source)
        {
            IImageSourceHandler returnValue = null;
            if (source is UriImageSource) {
                returnValue = new ImageLoaderSourceHandler();
            }
            else if (source is FileImageSource) {
                returnValue = new FileImageSourceHandler();
            }
            else if (source is StreamImageSource) {
                returnValue = new StreamImagesourceHandler();
            }
            return returnValue;
        }
    }
}
