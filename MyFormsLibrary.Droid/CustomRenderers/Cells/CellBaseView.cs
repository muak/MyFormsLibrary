using System;
using Android.Content;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Text;
using Android.Util;

namespace MyFormsLibrary.Droid.CustomRenderers
{
    public class CellBaseView:LinearLayout,INativeElementView
    {
        public const double DefaultMinHeight = 55;

        public TextView TitleLabel { get; set; }
        public ImageView ImageView { get; set; }

        Context  _Context;
        Cell _Cell;

        public CellBaseView(Context context,Cell cell):base(context) {
            _Context = context;
            _Cell = cell;
           
            SetMinimumWidth((int)context.ToPixels(50));
            SetMinimumHeight((int)context.ToPixels(85));
            Orientation = Orientation.Horizontal;


            var padding = (int)context.ToPixels(8);
            SetPadding((int)context.ToPixels(15), padding, (int)context.ToPixels(15), padding);

            //this.LayoutParameters = new LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.MatchParent);

            TitleLabel = new TextView(context);
            TitleLabel.SetSingleLine(true);
            TitleLabel.Ellipsize = TextUtils.TruncateAt.End;
            //TitleLabel.SetTextSize(ComplexUnitType.Sp, 14f);

            var textParams = new LayoutParams(ViewGroup.LayoutParams.WrapContent,ViewGroup.LayoutParams.WrapContent) {
                Gravity = GravityFlags.Left | GravityFlags.CenterVertical
            };
            using (textParams) {
                AddView(TitleLabel,textParams);
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
