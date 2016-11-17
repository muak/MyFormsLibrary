using System;
using Android.App;
using Android.Views;
using Android.Widget;
using MyFormsLibrary.Droid.Effects;
using MyFormsLibrary.Effects;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using AGraphics = Android.Graphics;
using ARelativeLayout = Android.Widget.RelativeLayout;


[assembly: ExportEffect(typeof(AddErrorMessagePlatformEffect), nameof(AddErrorMessage))]
namespace MyFormsLibrary.Droid.Effects
{
    public class AddErrorMessagePlatformEffect:PlatformEffect
    {
        private TextView textView;
        private ARelativeLayout relative;

        protected override void OnAttached() {
            relative = new ARelativeLayout(Container.Context);

            using (var lparam = new ARelativeLayout.LayoutParams(-1, -1)) {
                lparam.AddRule(LayoutRules.AlignRight);

                textView = new TextView(Container.Context);
                textView.SetTextColor(new AGraphics.Color(255, 0, 0, 200));
                textView.SetBackgroundColor(new AGraphics.Color(255, 255, 255, 128));
                textView.TextSize = 10f;
                textView.Gravity = GravityFlags.Right | GravityFlags.Top;
                textView.Visibility = ViewStates.Invisible;
                textView.SetPadding(8, 0, 8, 0);

                relative.AddView(textView, lparam);
            }

            var textParams = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent) {
                Width = 0,
                Weight = 1,
                Gravity = GravityFlags.FillHorizontal | GravityFlags.CenterVertical
            };
            using (textParams) {
                Container.AddView(relative, textParams);
            }

            var listener = new ContainerOnLayoutChangeListener(relative,textView);
            Control.AddOnLayoutChangeListener(listener);

            UpdateText();

        }

        protected override void OnDetached() {
            textView.Dispose();
            relative.Dispose();
        }

        private void UpdateText() {
            var msg = AddErrorMessage.GetErrorMessage(Element);
            if (string.IsNullOrEmpty(msg)) {
                textView.Visibility = ViewStates.Invisible;
                return;
            }

            textView.Text = msg;
            textView.Visibility = ViewStates.Visible;
        }

        protected override void OnElementPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e) {
            base.OnElementPropertyChanged(e);

            if (e.PropertyName == AddErrorMessage.ErrorMessageProperty.PropertyName) {
                UpdateText();
            }
           
        }

        internal class ContainerOnLayoutChangeListener : Java.Lang.Object, Android.Views.View.IOnLayoutChangeListener
        {
            private Android.Widget.RelativeLayout _layout;
            private TextView _textView;

            public ContainerOnLayoutChangeListener(Android.Widget.RelativeLayout layout,TextView textView) {
                _layout = layout;
                _textView = textView;
            }

            //ContainerにAddViewした子要素のサイズを確定する必要があるため
            //ControlのOnLayoutChangeのタイミングでセットする
            public void OnLayoutChange(Android.Views.View v, int left, int top, int right, int bottom, int oldLeft, int oldTop, int oldRight, int oldBottom) {
                _layout.Right = v.Right;
                _layout.Bottom = v.Bottom;
                _textView.Right = v.Right;
                _textView.Bottom = v.Bottom;
            }
        }
    }


}
