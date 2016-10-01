/**
 * Copy From
 * http://qiita.com/yu_ka1984/items/5cef0fcb167febbb3e8b
 */

using System;
using CoreGraphics;
using MyFormsLibrary.DependencyServices;
using MyFormsLibrary.iOS.DependencyServices;
using UIKit;
using Xamarin.Forms;


[assembly: Dependency(typeof(LoadingMessage))]
namespace MyFormsLibrary.iOS.DependencyServices
{
	public class LoadingMessage:ILoadingMessage
	{
		private LoadingOverlay loadpop;
		private UIWindow window;
		/// <summary>ローディングを開始する</summary>
		/// <param name="message"></param>
		public void Show(string message) {
			Device.BeginInvokeOnMainThread(() => {
				window = UIApplication.SharedApplication.KeyWindow;
				var vc = window.RootViewController;
				window.SetNeedsDisplay();
				while (vc.PresentedViewController != null) {
					vc = vc.PresentedViewController;
				}

				var bounds = UIScreen.MainScreen.Bounds;
				loadpop = new LoadingOverlay(bounds, message);
				vc.Add(loadpop);
				ishow = true;
			});
		}

		/// <summary>ローディングを終了する</summary>
		public void Hide() {
			Device.BeginInvokeOnMainThread(() => {
				loadpop.Hide();
				ishow = false;
			});
		}

		/// <summary>状態</summary>
		public bool IsShow => ishow;

		private bool ishow = false;

		public void SetMessage(string message) {
			Device.BeginInvokeOnMainThread(() => {
				loadpop.loadingLabel.Text = message;
				loadpop.SetNeedsDisplay();
			});
		}

	}
	public class LoadingOverlay : UIView
	{
		// control declarations
		UIActivityIndicatorView activitySpinner;
		public UILabel loadingLabel;



		public LoadingOverlay(CGRect frame, string message) : base(frame) {
			// configurable bits
			BackgroundColor = UIColor.Black;
			Alpha = 0.75f;
			AutoresizingMask = UIViewAutoresizing.All;

			nfloat labelHeight = 60; //22;
			nfloat labelWidth = Frame.Width - 20;

			// derive the center x and y
			nfloat centerX = Frame.Width / 2;
			nfloat centerY = Frame.Height / 2;

			// create the activity spinner, center it horizontall and put it 5 points above center x
			activitySpinner = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.WhiteLarge);
			activitySpinner.Frame = new CGRect(
				centerX - (activitySpinner.Frame.Width / 2),
				centerY - activitySpinner.Frame.Height - 20,
				activitySpinner.Frame.Width,
				activitySpinner.Frame.Height);
			activitySpinner.AutoresizingMask = UIViewAutoresizing.All;
			AddSubview(activitySpinner);
			activitySpinner.StartAnimating();

			// create and configure the "Loading Data" label
			loadingLabel = new UILabel(new CGRect(
				centerX - (labelWidth / 2),
				centerY + 20,
				labelWidth,
				labelHeight
				));
			loadingLabel.BackgroundColor = UIColor.Clear;
			loadingLabel.TextColor = UIColor.White;
			loadingLabel.Text = message;
			loadingLabel.TextAlignment = UITextAlignment.Center;
			loadingLabel.AutoresizingMask = UIViewAutoresizing.All;
			loadingLabel.Lines = 0;
			loadingLabel.LineBreakMode = UILineBreakMode.WordWrap;
			AddSubview(loadingLabel);

		}

		/// <summary>
		/// Fades out the control and then removes it from the super view
		/// </summary>
		public void Hide() {
			UIView.Animate(
				0.5, // duration
				() => { Alpha = 0; },
				() => { RemoveFromSuperview(); }
			);
		}
	}
}

