using System;
using Android.Content;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Text;

namespace MyFormsLibrary.Droid.CustomRenderers.Cells
{
    public class CellBaseView:LinearLayout,INativeElementView
    {
        public const double DefaultMinHeight = 44;

        public TextView TitleLabel { get; set; }
        public ImageView ImageView { get; set; }

        Context  _Context;
        Cell _Cell;

        public CellBaseView(Context context,Cell cell):base(context) {
            _Context = context;
            _Cell = cell;

            SetMinimumWidth((int)context.ToPixels(25));
            SetMinimumHeight((int)context.ToPixels(25));
            Orientation = Orientation.Horizontal;

            var padding = (int)context.FromPixels(8);
            SetPadding(padding, padding, padding, padding);

            TitleLabel = new TextView(context);
            TitleLabel.SetSingleLine(true);
            TitleLabel.Ellipsize = TextUtils.TruncateAt.End;

            var textParams = new LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.MatchParent) {
                Gravity = GravityFlags.Start | GravityFlags.CenterVertical
            };
            using (textParams) {
                AddView(TitleLabel,textParams);
            }

            SetMinimumHeight((int)context.ToPixels(DefaultMinHeight));
        }

        public Element Element {
            get {
                return _Cell;
            }
        }

        public void AddImageView() {
            ImageView = new ImageView(_Context);
            var imageParams = new LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.MatchParent) {
                Width = (int)_Context.FromPixels(40),
                Height = (int)_Context.FromPixels(40),
                RightMargin = 0,
                Gravity = GravityFlags.Center
            };
            using (imageParams) {
                AddView(ImageView, 0, imageParams);
            }
        }
    }
}
