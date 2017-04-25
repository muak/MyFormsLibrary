using System;
using MyFormsLibrary.Services;
using Android.Graphics;
using System.IO;
namespace MyFormsLibrary.Droid.Services
{
    public class ManipulatableImage : IManipulatableImage
    {

        public int Height { get; private set; }
        public int Width { get; private set; }

        private Bitmap _image;

        public ManipulatableImage(byte[] bin)
        {
            _image = BitmapFactory.DecodeByteArray(bin, 0, bin.Length);
            UpdateSize();
        }


        public IManipulatableImage Crop(int x, int y, int width, int height)
        {
  
            var croped = Bitmap.CreateBitmap(_image,x,y,width,height);
            _image.Recycle();
            _image.Dispose();
            _image = croped;
            croped = null;

            UpdateSize();

            return this;
        }

        void UpdateSize()
        {
            Width = (int)_image.Width;
            Height = (int)_image.Height;
        }

        public void Dispose()
        {
            _image.Recycle();
            _image.Dispose();
            _image = null;
        }

        public IManipulatableImage Resize(int width, int height)
        {
            int w;
            int h;
            if (width <= 0 && height <= 0) {
                return this;
            }
            else if (width <= 0) {
                h = height;
                w = (int)(height * ((double)Width / (double)Height));
            }
            else if (height <= 0) {
                w = width;
                h = (int)(width * ((double)Height / (double)Width));
            }
            else {
                w = width;
                h = height;
            }

            var resized = Bitmap.CreateScaledBitmap(_image,w,h, true);

            _image.Recycle();
            _image.Dispose();
            _image = resized;
            resized = null;

            UpdateSize();

            return this;
        }

        public IManipulatableImage Rotate(float degree)
        {
            var matrix = new Matrix();
            matrix.PostRotate(degree);
            var rotated = Bitmap.CreateBitmap(_image, 0, 0, _image.Width, _image.Height, matrix,true);

            _image.Recycle();
            _image.Dispose();
            _image = rotated;
            rotated = null;

            return this;
        }

        public int[] ToArgbPixels()
        {
            throw new NotImplementedException();
        }

        public byte[] ToJpeg(float quality = 80)
        {
            using (var ms = new MemoryStream()) {
                _image.Compress(Bitmap.CompressFormat.Jpeg, (int)quality,ms);
                return ms.ToArray();
            }
        }

        public byte[] ToPng()
        {
            using (var ms = new MemoryStream()) {
                _image.Compress(Bitmap.CompressFormat.Png, 100, ms);
                return ms.ToArray();
            }
        }
    }
}
