﻿using System;
using MyFormsLibrary.iOS.Effects;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using UIKit;
using MyFormsLibrary.Effects;


[assembly: ExportEffect(typeof(HideBorderPlatformEffect), nameof(HideBorder))]

namespace MyFormsLibrary.iOS.Effects
{
	
	public class HideBorderPlatformEffect:PlatformEffect
	{
		private object oldBorderValue;
		private object oldBackgroundValue;

		protected override void OnAttached() {
			if (Control is UITextField) {
				var textfield = Control as UITextField;
				oldBorderValue = textfield.BorderStyle;
				oldBackgroundValue = textfield.BackgroundColor;
				textfield.BorderStyle = UITextBorderStyle.None;

			}

		}

		protected override void OnDetached() {
			if (Control is UITextField) {
				var textfield = Control as UITextField;
				textfield.BorderStyle = (UITextBorderStyle)oldBorderValue;
				textfield.BackgroundColor = (UIColor)oldBackgroundValue;
			}
		}

	}
}

