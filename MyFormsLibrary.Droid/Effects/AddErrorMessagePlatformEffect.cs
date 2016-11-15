using System;
using Android.Views;
using Android.Widget;
using MyFormsLibrary.Droid.Effects;
using MyFormsLibrary.Effects;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using AGraphics = Android.Graphics;

[assembly: ExportEffect(typeof(AddErrorMessagePlatformEffect), nameof(AddErrorMessage))]
namespace MyFormsLibrary.Droid.Effects
{
    public class AddErrorMessagePlatformEffect:PlatformEffect
    {
        private TextView textView;

        protected override void OnAttached() {
            textView = new TextView(Container.Context);
            textView.SetTextColor(new AGraphics.Color(255, 0, 0, 200));
            textView.SetBackgroundColor(new AGraphics.Color(255, 255, 255, 128));
            textView.TextSize = 10f;
            textView.Gravity = GravityFlags.Left | GravityFlags.CenterVertical;
            textView.LayoutParameters = new ViewGroup.LayoutParams(-1, -1);
            textView.Visibility = ViewStates.Invisible;
            textView.SetPadding(8, 0, 8, 0);
            Container.AddView(textView);

            UpdateText();

        }

        protected override void OnDetached() {
            textView.Dispose();
        }


        private void UpdateText() {
            var msg = AddErrorMessage.GetErrorMessage(Element);
            if (string.IsNullOrEmpty(msg)) {
                textView.Visibility = ViewStates.Invisible;
                return;
            }

            textView.Text = msg;

            var textpaint = textView.Paint;
            var rect = new Android.Graphics.Rect();
            textpaint.GetTextBounds(msg, 0, msg.Length, rect);
            var addwidth = (int)(rect.Width() / msg.Length) + textView.PaddingLeft + textView.PaddingRight;
            var addHeight = rect.Height() / 2;
            var left = Control.Right - rect.Width() - addwidth > 0 ? Control.Right - rect.Width() - addwidth : 0;
            var height = (rect.Width() / Control.Right + 1) * rect.Height() + addHeight;

            textView.Top = 0;
            textView.Left = left;
            textView.Right = Control.Right;
            textView.Bottom = height;

            textView.Visibility = ViewStates.Visible;
        }

        protected override void OnElementPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e) {
            base.OnElementPropertyChanged(e);

            if (e.PropertyName == AddErrorMessage.ErrorMessageProperty.PropertyName) {
                UpdateText();
            }

        }
    }
}
