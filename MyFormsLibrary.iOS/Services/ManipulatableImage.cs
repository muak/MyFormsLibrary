using System;
using MyFormsLibrary.Services;
using UIKit;
using CoreGraphics;
using Foundation;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Linq;
using System.Collections.Generic;

namespace MyFormsLibrary.iOS.Services
{
    public class ImageManipulator : IImageManipulator
    {
        public ImageManipulator()
        {

        }
        public IManipulatableImage CreateImage(byte[] imageArray)
        {
            return new ManipulatableImage(imageArray);
        }
    }

    public class ManipulatableImage : IManipulatableImage
    {
        public int Height { get; private set; }
        public int Width { get; private set; }

        private UIImage _image = null;

        public ManipulatableImage(byte[] bin)
        {
            _image = new UIImage(NSData.FromArray(bin));
            UpdateSize();
        }


        public IManipulatableImage Crop(int x, int y, int width, int height)
        {
            var rect = new CGRect(x, y, width, height);
            var crop = _image.CGImage.WithImageInRect(rect);
            _image.Dispose();
            _image = new UIImage(crop);

            UpdateSize();

            return this;
        }

        void UpdateSize()
        {
            Width = (int)_image.CGImage.Width;
            Height = (int)_image.CGImage.Height;
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

            var rect = new CGRect(0, 0, w, h);

            UIGraphics.BeginImageContext(rect.Size);

            _image.Draw(rect);

            var newImage = UIGraphics.GetImageFromCurrentImageContext();

            UIGraphics.EndImageContext();

            _image.Dispose();
            _image = newImage;
            newImage = null;

            UpdateSize();

            return this;
        }

        CGPoint CalculateRotatePoint(CGPoint p, double radian)
        {
            return new CGPoint(
                p.X * Math.Cos(radian) - p.Y * Math.Sin(radian),
                p.X * Math.Sin(radian) + p.Y * Math.Cos(radian)
            );
        }

        public IManipulatableImage Rotate(float degree)
        {
            // reference https://ruigomes.me/blog/how-to-rotate-an-uiimage-using-swift/
            //           https://oshiete.goo.ne.jp/qa/6592961.html

            var radian = -degree * Math.PI / 180f;

            //calculate canvas size
            var x = Width / 2f;
            var y = Height / 2f;

            var points = new List<CGPoint> { new CGPoint(x, y), new CGPoint(x, -y), new CGPoint(-x, -y), new CGPoint(-x, y) };

            for (var i = 0; i < points.Count; i++) {
                points[i] = CalculateRotatePoint(points[i], radian);
            }

            var canvasSize = new CGSize(
                 points.Max(p => p.X) - points.Min(p => p.X),
                 points.Max(p => p.Y) - points.Min(p => p.Y)
            );


            UIGraphics.BeginImageContext(canvasSize);

            var context = UIGraphics.GetCurrentContext();
            context.TranslateCTM(canvasSize.Width / 2.0f, canvasSize.Height / 2.0f);
            context.ScaleCTM(1.0f, -1.0f);
            context.RotateCTM((float)radian);
            context.DrawImage(new CGRect(-_image.Size.Width / 2, -_image.Size.Height / 2, _image.Size.Width, _image.Size.Height), _image.CGImage);

            var rotated = UIGraphics.GetImageFromCurrentImageContext();

            UIGraphics.EndImageContext();

            _image.Dispose();
            _image = rotated;
            rotated = null;
            points = null;

            return this;
        }



        public byte[] ToJpeg(float quality = 80)
        {
            using (var data = _image.AsJPEG(quality)) {
                return data.ToArray();
            }
        }

        public byte[] ToPng()
        {
            using (var data = _image.AsPNG()) {
                return data.ToArray();
            }
        }

        public int[] ToArgbPixels()
        {
            using (var data = _image.CGImage.DataProvider.CopyData()) {
                var bytesPerPixel = _image.CGImage.BitsPerPixel / 8;

                var pixels = new int[Width * Height];
                var idx = 0;
                for (var i = 0; i < Height; i++) {
                    for (var j = 0; j < Width; j++) {
                        var addr = (i * Width + j) * bytesPerPixel;
                        pixels[idx++] =
                            (data[addr + 3] << 24) |
                            (data[addr + 2] << 16) |
                            (data[addr + 1] << 8) |
                            data[addr];
                    }
                }

                return pixels;
            }
        }

        public async Task<int[]> ToArgbPixelsAsync()
        {
            int[] pixiels = null;

            await Task.Run(() => {
                pixiels = ToArgbPixels();
                return pixiels;
            });

            return pixiels;
        }




        public void Dispose()
        {
            _image.Dispose();
            _image = null;
        }
    }
}
