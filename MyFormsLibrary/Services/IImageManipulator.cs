using System;
namespace MyFormsLibrary.Services
{
    public interface IImageManipulator
    {
        IManipulatableImage CreateImage(byte[] imageArray);
    }

    public interface IManipulatableImage:IDisposable
    {
        int Width { get; }
        int Height { get; }
        IManipulatableImage Resize(int width, int height);
        IManipulatableImage Crop(int x, int y, int width, int height);
        IManipulatableImage Rotate(float degree);
        byte[] ToJpeg(float quality=80);
        byte[] ToPng();
        int[] ToArgbPixels();
    }
}
