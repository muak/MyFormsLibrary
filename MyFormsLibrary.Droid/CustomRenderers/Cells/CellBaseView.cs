using System;
using Android.Content;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Text;
using Android.Util;
using AGraphics = Android.Graphics;
using ARelativeLayout = Android.Widget.RelativeLayout;

namespace MyFormsLibrary.Droid.CustomRenderers
{
    public class CellBaseView:LinearLayout,INativeElementView
    {
        public const double DefaultMinHeight = 55;

        public TextView TitleLabel { get; set; }
        public ImageView ImageView { get; set; }
        public ARelativeLayout ContentView { get; private set;}
        public TextView ErrorLabel { get; private set; }
        public TextView ValueText { get; set; }

        Context  _Context;
        Cell _Cell;

        public CellBaseView(Context context,Cell cell):base(context) {
            _Context = context;
            _Cell = cell;
           
            SetMinimumWidth((int)context.ToPixels(50));
            SetMinimumHeight((int)context.ToPixels(85));
            Orientation = Orientation.Horizontal;


            var padding = (int)context.FromPixels(8);
         
            SetPadding(padding + (int)context.ToPixels(15), padding, (int)context.ToPixels(15), padding);

            TitleLabel = new TextView(context);
            TitleLabel.SetSingleLine(true);
            TitleLabel.Ellipsize = TextUtils.TruncateAt.End;

            var textParams = new LayoutParams(ViewGroup.LayoutParams.WrapContent,ViewGroup.LayoutParams.WrapContent) {
                Gravity = GravityFlags.Left | GravityFlags.CenterVertical
            };
            using (textParams) {
                AddView(TitleLabel,textParams);
            }

            ContentView = new ARelativeLayout(context);

            using (var valueTextParams = new ARelativeLayout.LayoutParams(-1, -1)) {
                valueTextParams.AddRule(LayoutRules.AlignRight);

                ValueText = new TextView(context);
                ValueText.SetSingleLine(true);
                ValueText.Ellipsize = TextUtils.TruncateAt.End;
                ValueText.Gravity = GravityFlags.Right | GravityFlags.CenterVertical;
                ValueText.Visibility = ViewStates.Invisible;

                ContentView.AddView(ValueText, valueTextParams);
            }

            using (var lparam = new ARelativeLayout.LayoutParams(-1, -1)) {
                lparam.AddRule(LayoutRules.AlignRight);

                ErrorLabel = new TextView(Context);
                ErrorLabel.SetTextColor(new AGraphics.Color(255, 0, 0, 200));
                ErrorLabel.SetBackgroundColor(new AGraphics.Color(255, 255, 255, 128));
                ErrorLabel.TextSize = 10f;
                ErrorLabel.Gravity = GravityFlags.Right | GravityFlags.Top;

                ErrorLabel.Visibility = ViewStates.Invisible;
                ErrorLabel.SetPadding(8, 0, 8, 0);

                ContentView.AddView(ErrorLabel, lparam);
            }

            //幅・高さ目一杯とる
            var relativeParam = new LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent) {
                Width = 0,
                Weight = 1,
                Gravity = GravityFlags.FillHorizontal | GravityFlags.FillVertical | GravityFlags.CenterVertical
            };
            using (relativeParam) {
                AddView(ContentView, relativeParam);
            }

            //SetMinimumHeight((int)context.ToPixels(DefaultMinHeight));
        }

        public Element Element {
            get {
                return _Cell;
            }
        }

        public void SetRenderHeight(double height) {
            SetMinimumHeight((int)Context.ToPixels(height == -1 ? DefaultMinHeight : height));
        }

        public void AddImageView() {
            ImageView = new ImageView(_Context);
            var imageParams = new LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.MatchParent) {
                Width = (int)_Context.ToPixels(30),
                Height = (int)_Context.ToPixels(30),
                RightMargin = (int)_Context.ToPixels(10),
                Gravity = GravityFlags.Center
            };
            using (imageParams) {
                AddView(ImageView, 0, imageParams);
            }
        }
    }
}
