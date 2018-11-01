using System;
using System.Threading.Tasks;
using Android.Graphics;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Content;

namespace MyFormsLibrary.Droid.CustomRenderers
{
    //
    // Sources cited from https://forums.xamarin.com/discussion/comment/174834/#Comment_174834
    //
    public static class ImageSourceExtensions
    {
        public static async Task<Bitmap> ToBitmap(this ImageSource imageSource,Context context)
        {
            var handler = GetHandler(imageSource);

            return await handler.LoadImageAsync(imageSource, context);
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
