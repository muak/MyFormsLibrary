using System;
using MyFormsLibrary.Services;
namespace MyFormsLibrary.Droid.Services
{
    public class ImageManipulator:IImageManipulator
    {
        public ImageManipulator()
        {
        }

        public IManipulatableImage CreateImage(byte[] imageArray)
        {
            return new ManipulatableImage(imageArray);
        }
    }
}
