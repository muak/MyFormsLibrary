using System;
using Xamarin.Forms.Platform.Android;
using Android.Widget;
using Xamarin.Forms;
using MyFormsLibrary.Droid.Effects;
using MyFormsLibrary.Effects;

[assembly: ExportEffect(typeof(LineHeightPlatformEffect), nameof(LineHeight))]
namespace MyFormsLibrary.Droid.Effects
{
	public class LineHeightPlatformEffect:PlatformEffect
	{
		private TextView textView;

		protected override void OnAttached() {
			textView = Control as TextView;

			Update();
		}

		protected override void OnDetached() {
			textView = null;
		}

		void Update() {
			var lineHeight =  (float)LineHeight.GetHeight(Element);
			var fontSize = (Element as Label).FontSize;
			textView.SetLineSpacing(1f, lineHeight / (float)fontSize);
		}

		protected override void OnElementPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e) {
			base.OnElementPropertyChanged(e);

			if (e.PropertyName == LineHeight.HeightProperty.PropertyName) {
				Update();
			}

			else if (e.PropertyName == Label.FontSizeProperty.PropertyName){
				Update();
			}
		}
	}
}

