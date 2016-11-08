using System;
using Xamarin.Forms.Platform.iOS;
using MyFormsLibrary.Effects;
using UIKit;
using Xamarin.Forms;
using MyFormsLibrary.iOS.Effects;
using CoreGraphics;

[assembly: ExportEffect(typeof(ShowBorderPlatformEffect), nameof(ShowBorder))]
namespace MyFormsLibrary.iOS.Effects
{
    public class ShowBorderPlatformEffect:PlatformEffect
    {
        private UIView View;

        protected override void OnAttached() {
            View = Control ?? Container;
            

            UpdateColor();
            UpdateWidth();
        }

        protected override void OnDetached() {
            View = null;
        }

        protected override void OnElementPropertyChanged(System.ComponentModel.PropertyChangedEventArgs args) {
            base.OnElementPropertyChanged(args);

            if (args.PropertyName == ShowBorder.ColorProperty.PropertyName) {
                UpdateColor();
            }
            else if (args.PropertyName == ShowBorder.WidthProperty.PropertyName) {
                UpdateWidth();
            }
        }

        void UpdateColor() {
            var color = ShowBorder.GetColor(Element);
            View.Layer.BorderColor = color.ToCGColor();
        }
        void UpdateWidth() {
            var width = ShowBorder.GetWidth(Element);
            View.Layer.BorderWidth = width;
        }
    }
}
