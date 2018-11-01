using System;
using Xamarin.Forms;
using NControl.Droid;
using MyFormsLibrary.Droid.CustomRenderers;
using NControl.Abstractions;
using System.Reflection;
using MyFormsLibrary.NControls;

[assembly: ExportRenderer(typeof(SvgView), typeof(SvgViewFixRenderer))]
namespace MyFormsLibrary.Droid.CustomRenderers
{
	public class SvgViewFixRenderer:NControlViewRenderer
	{
		private NControlView _currentElement;

		protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<NControl.Abstractions.NControlView> e) {
			base.OnElementChanged(e);

			if(e.OldElement != null) {
				_currentElement = null;
			}

			if (e.NewElement != null) {
				_currentElement = e.NewElement;
			}
		}



		protected override void Dispose(bool disposing) {
			if ((disposing) && (_currentElement != null)) {
				var handleInvalidate = typeof(NControlViewRenderer).GetMethod("HandleInvalidate", BindingFlags.Instance | BindingFlags.NonPublic);
				var onGetPlatformHandler = typeof(NControlViewRenderer).GetMethod("OnGetPlatformHandler", BindingFlags.Instance | BindingFlags.NonPublic);

				Action<NControlView> removeEvent = (item) => {

					Delegate eventMethod = handleInvalidate.CreateDelegate(typeof(EventHandler), this);
					item.GetType().GetEvent("OnInvalidate").RemoveEventHandler(item, eventMethod);

					eventMethod = onGetPlatformHandler.CreateDelegate(typeof(NControlView.GetPlatformDelegate), this);
					item.GetType().GetEvent("OnGetPlatform").RemoveEventHandler(item, eventMethod);
				};

				removeEvent(_currentElement);

				_currentElement = null;
			}

			base.Dispose(disposing);
		}
	}
}

