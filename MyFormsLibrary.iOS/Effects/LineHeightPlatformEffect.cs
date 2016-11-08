using System;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;
using MyFormsLibrary.iOS.Effects;
using MyFormsLibrary.Effects;
using UIKit;
using Foundation;

[assembly: ExportEffect(typeof(LineHeightPlatformEffect), nameof(LineHeight))]
namespace MyFormsLibrary.iOS.Effects
{
	public class LineHeightPlatformEffect:PlatformEffect
	{
		private UILabel label;

		protected override void OnAttached() {
			label = Control as UILabel;

			Update();
		}

		protected override void OnDetached() {
			label = null;
		}

		void Update() {
			var text = (Element as Label).Text;
			if (text == null)
				return;
			var lineHeight = (float)LineHeight.GetHeight(Element);
			var fontSize = (float)(Element as Label).FontSize;
			var pStyle = new NSMutableParagraphStyle() { LineSpacing = lineHeight - fontSize};
			var attrString = new NSMutableAttributedString(text);

			attrString.AddAttribute(UIStringAttributeKey.ParagraphStyle,
			                        pStyle,
			                        new NSRange(0, attrString.Length));

			label.AttributedText = attrString;
		}

		protected override void OnElementPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e) {
			base.OnElementPropertyChanged(e);

			if (e.PropertyName == LineHeight.HeightProperty.PropertyName) {
				Update();
			}

			else if (e.PropertyName == Label.FontSizeProperty.PropertyName) {
				Update();
			}
			else if (e.PropertyName == Label.TextProperty.PropertyName) {
				Update();
			}
		}
	}
}

