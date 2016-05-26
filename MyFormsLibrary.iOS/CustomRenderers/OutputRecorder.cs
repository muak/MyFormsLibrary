using System;
using AVFoundation;
using CoreGraphics;
using CoreMedia;
using CoreVideo;
using MyFormsLibrary.CustomRenderers;
using UIKit;
using Xamarin.Forms;

namespace MyFormsLibrary.iOS.CustomRenderers
{

    public class OutputRecorder : AVCaptureVideoDataOutputSampleBufferDelegate
	{

		public CameraPreview CameraPreview { get; set;}

		private UIImage GetImageFromSampleBuffer (CMSampleBuffer sampleBuffer)
		{

			// Get a pixel buffer from the sample buffer
			using (var pixelBuffer = sampleBuffer.GetImageBuffer () as CVPixelBuffer) {
				// Lock the base address

				pixelBuffer.Lock (CVPixelBufferLock.None);

				// Prepare to decode buffer
				var flags = CGBitmapFlags.PremultipliedFirst | CGBitmapFlags.ByteOrder32Little;

				// Decode buffer - Create a new colorspace
				using (var cs = CGColorSpace.CreateDeviceRGB ()) {

					// Create new context from buffer
					using (var context = new CGBitmapContext (pixelBuffer.BaseAddress,
											 pixelBuffer.Width,
											 pixelBuffer.Height,
											 8,
											 pixelBuffer.BytesPerRow,
											 cs,
											 (CGImageAlphaInfo)flags)) {

						// Get the image from the context
						using (var cgImage = context.ToImage ()) {
							
							// Unlock and return image
							pixelBuffer.Unlock (CVPixelBufferLock.None);
							return UIImage.FromImage (cgImage);
						}
					}
				}
			}
		}

		#region Override Methods
		/// <Docs>The capture output on which the frame was captured.</Docs>
		/// <param name="connection">The connection on which the video frame was received.</param>
		/// <remarks>Unless you need to keep the buffer for longer, you must call
		///  Dispose() on the sampleBuffer before returning. The system
		///  has a limited pool of video frames, and once it runs out of
		///  those buffers, the system will stop calling this method
		///  until the buffers are released.</remarks>
		/// <summary>
		/// Dids the output sample buffer.
		/// </summary>
		/// <param name="captureOutput">Capture output.</param>
		/// <param name="sampleBuffer">Sample buffer.</param>
		public override void DidOutputSampleBuffer (
			AVCaptureOutput captureOutput, 
			CMSampleBuffer sampleBuffer, 
			AVCaptureConnection connection)
		{
			
			// Trap all errors
			try {
				// Grab an image from the buffer
				var image = GetImageFromSampleBuffer (sampleBuffer);

				var cg = image.CGImage;
				var prov = cg.DataProvider.CopyData ();
				var scale = 1;
				var width = (int)cg.Width;
				var height = (int)cg.Height;
				int x = width / 2;
				int y = height / 2;
				int BytesPerPixel = (int)cg.BitsPerPixel / 8;
				int addr = (width * (int)(y * scale) + (int)(x * scale)) * BytesPerPixel;

				var b = prov [addr];
				var g = prov [addr + 1];
				var r = prov [addr + 2];
				var a = prov [addr + 3];


				Device.BeginInvokeOnMainThread (()=>{
					try{
						CameraPreview.OnColorChanged (Color.FromRgb (r, g, b));	
					}catch(Exception e){
						Console.WriteLine ("BeginInvokeOnMainThread failed =>"+e.Message);
					}
					finally{
						cg.Dispose ();
						prov.Dispose ();
						sampleBuffer.Dispose ();
						GC.Collect ();
					}
				});


			} catch (Exception e) {
				// Report error
				Console.WriteLine ("Error sampling buffer: {0}", e.Message);
			}
		}
		#endregion
	}
}

