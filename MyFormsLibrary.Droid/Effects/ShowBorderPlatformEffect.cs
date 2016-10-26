using System;
using Xamarin.Forms.Platform.Android;
using Android.Views;
using Android.Graphics.Drawables;
using Android.Graphics;
using MyFormsLibrary.Effects;
using MyFormsLibrary.Droid.Effects;
using Xamarin.Forms;

[assembly: ExportEffect(typeof(ShowBorderPlatformEffect), nameof(ShowBorder))]
namespace MyFormsLibrary.Droid.Effects
{
    public class ShowBorderPlatformEffect:PlatformEffect
    {
        private Android.Views.View View;
        private GradientDrawable Border;
        private Android.Graphics.Color Color;
        private int Width;

        protected override void OnAttached() {
            View = Control ?? Container;

            Border = new GradientDrawable();
            Border.SetColor(Android.Graphics.Color.Transparent);

            UpdateColor();
            UpdateWidth();
            UpdateBorder();
        }

        protected override void OnDetached() {
            Border.Dispose();
            Border = null;
            View = null;
        }

        protected override void OnElementPropertyChanged(System.ComponentModel.PropertyChangedEventArgs args) {
            base.OnElementPropertyChanged(args);

            if (args.PropertyName == ShowBorder.ColorProperty.PropertyName) {
                UpdateColor();
                UpdateBorder();
            }
            else if (args.PropertyName == ShowBorder.WidthProperty.PropertyName) {
                UpdateWidth();
                UpdateBorder();
            }
        }

        void UpdateColor() {
            Color = ShowBorder.GetColor(Element).ToAndroid();
        }
        void UpdateWidth() {
            double scale = (int)Android.App.Application.Context.Resources.DisplayMetrics.DensityDpi / 160d;
            Width = (int)(ShowBorder.GetWidth(Element) * scale);
        }
        void UpdateBorder() {
            Border.SetStroke(Width, Color);
            View.SetPadding(Width,Width,Width,Width);
            View.SetBackground(Border);

        }
    }
}
