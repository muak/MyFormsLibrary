using System;
using System.Linq;
using AVFoundation;
using CoreFoundation;
using CoreGraphics;
using CoreMedia;
using CoreVideo;
using Foundation;
using MyFormsLibrary.CustomRenderers;
using MyFormsLibrary.iOS.CustomRenderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CameraPreview), typeof(CameraPreviewRenderer))]
namespace MyFormsLibrary.iOS.CustomRenderers
{
    public class CameraPreviewRenderer :ViewRenderer<CameraPreview,UICameraPreview>
	{
		UICameraPreview uiCameraPreview;

		protected override void OnElementChanged (ElementChangedEventArgs<CameraPreview> e)
		{
			base.OnElementChanged (e);


			if (Control == null) {
				uiCameraPreview = new UICameraPreview (e.NewElement);
				SetNativeControl (uiCameraPreview);
			}
			if (e.OldElement != null) {
				// Unsubscribe
				//uiCameraPreview.Tapped -= OnCameraPreviewTapped;
			}
			if (e.NewElement != null) {
				// Subscribe
				//uiCameraPreview.Tapped += OnCameraPreviewTapped;
			}
		}

		void OnCameraPreviewTapped (object sender, EventArgs e)
		{
			if (uiCameraPreview.IsPreviewing) {
				uiCameraPreview.CaptureSession.StopRunning ();
				uiCameraPreview.IsPreviewing = false;

			} else {
				uiCameraPreview.CaptureSession.StartRunning ();
				uiCameraPreview.IsPreviewing = true;
			}
		}

		protected override void Dispose (bool disposing)
		{
			if (disposing) {
				Control.CaptureSession.Dispose ();
				Control.Dispose ();
			}
			base.Dispose (disposing);
		}
	}

	public class UICameraPreview : UIView
	{
		AVCaptureVideoPreviewLayer previewLayer;
		CameraOptions cameraOptions;
		CameraPreview cameraPreview;

		public event EventHandler<EventArgs> Tapped;

		public AVCaptureSession CaptureSession { get; private set; }

		public AVCaptureVideoDataOutput Output{ get; private set;}
		public OutputRecorder Recorder { get; set;}
		public DispatchQueue Queue { get; set;}
		public AVCaptureDevice MainDevice { get; private set;}


		public float MaxZoom { get; private set;}
		public float MinZoom { get; } = 1.0f;

		public bool IsPreviewing { get; set; }

		public UICameraPreview (CameraPreview camera)
		{
            
			cameraPreview = camera;
			cameraOptions = camera.Camera;
			IsPreviewing = false;
            SetPinchGesture();
			Initialize ();
		}

        private void SetPinchGesture(){
            nfloat lastscale = 1.0f;
            var pinch = new UIPinchGestureRecognizer((e)=>{
                if (e.State == UIGestureRecognizerState.Changed)
                {
                    NSError device_error;
                    MainDevice.LockForConfiguration(out device_error);
                    if (device_error != null)
                    {
                        Console.WriteLine($"Error: {device_error.LocalizedDescription}");
                        MainDevice.UnlockForConfiguration();
                        return;
                    }
                    var scale = e.Scale + (1 - lastscale);
                    var zoom = MainDevice.VideoZoomFactor * scale;
                    if (zoom > MaxZoom) zoom = MaxZoom;
                    if (zoom < MinZoom) zoom = MinZoom;
                    MainDevice.VideoZoomFactor = zoom;
                    MainDevice.UnlockForConfiguration();
                    lastscale = e.Scale;
                }
                else if (e.State == UIGestureRecognizerState.Ended)
                {
                    lastscale = 1.0f;
                }
            });
            this.AddGestureRecognizer(pinch);
        }

		public override void Draw (CGRect rect)
		{
			base.Draw (rect);
			previewLayer.Frame = rect;
		}

		public override void TouchesBegan (NSSet touches, UIEvent evt)
		{
			base.TouchesBegan (touches, evt);
			OnTapped ();
		}

		protected virtual void OnTapped ()
		{
			var eventHandler = Tapped;
			if (eventHandler != null) {
				eventHandler (this, new EventArgs ());
			}
		}



		void Initialize ()
		{
            
			CaptureSession = new AVCaptureSession ();
			previewLayer = new AVCaptureVideoPreviewLayer (CaptureSession) {
				Frame = Bounds,
				VideoGravity = AVLayerVideoGravity.ResizeAspectFill
			};
			Layer.AddSublayer (previewLayer);

            //デバイス設定
			var videoDevices = AVCaptureDevice.DevicesWithMediaType (AVMediaType.Video);
			var cameraPosition = (cameraOptions == CameraOptions.Front) ? AVCaptureDevicePosition.Front : AVCaptureDevicePosition.Back;
			MainDevice = videoDevices.FirstOrDefault (d => d.Position == cameraPosition);
			NSError device_error;
			MainDevice.LockForConfiguration (out device_error);
			if(device_error != null){
				Console.WriteLine ($"Error: {device_error.LocalizedDescription}");
				MainDevice.UnlockForConfiguration ();
				return;
			}
            //フレームレート設定
			MainDevice.ActiveVideoMinFrameDuration = new CMTime (1, 24);
			MainDevice.UnlockForConfiguration ();
			if (MainDevice == null) {
				return;
			}

			//max zoom
			MaxZoom = (float)Math.Min (MainDevice.ActiveFormat.VideoMaxZoomFactor, 6);

            //入力設定
			NSError error;
			var input = new AVCaptureDeviceInput (MainDevice, out error);
			CaptureSession.AddInput (input);

            //出力設定
			Output = new AVCaptureVideoDataOutput ();

			Queue= new DispatchQueue ("myQueue");
			Output.AlwaysDiscardsLateVideoFrames = true;
			Recorder = new OutputRecorder ();
			Recorder.CameraPreview = cameraPreview;
			Output.SetSampleBufferDelegate(Recorder,Queue);
			var vSettings = new AVVideoSettingsUncompressed ();
			vSettings.PixelFormatType = CVPixelFormatType.CV32BGRA;
			Output.WeakVideoSettings = vSettings.Dictionary;

			CaptureSession.AddOutput (Output);
			CaptureSession.StartRunning ();
			IsPreviewing = false;


		}
	}
}

