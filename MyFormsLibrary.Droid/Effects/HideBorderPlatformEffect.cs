using System;
using Android.Widget;
using MyFormsLibrary.Droid.Effects;
using MyFormsLibrary.Effects;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Graphics.Drawables;

[assembly: ExportEffect(typeof(HideBorderPlatformEffect), nameof(HideBorder))]
namespace MyFormsLibrary.Droid.Effects
{
	public class HideBorderPlatformEffect : PlatformEffect
	{

		protected override void OnAttached() {
			var textview = Control as TextView;
			if (textview == null) {
				return;
			}
            textview.Background.Alpha = 0;

		}

		protected override void OnDetached() {
			
		}

	}
}
