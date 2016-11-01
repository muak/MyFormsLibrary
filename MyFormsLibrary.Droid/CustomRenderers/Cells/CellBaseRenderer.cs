using System;
using Xamarin.Forms.Platform.Android;
using MyFormsLibrary.CustomRenderers;
using Android.Test;
using Android.Graphics.Drawables;

namespace MyFormsLibrary.Droid.CustomRenderers
{
    public class CellBaseRenderer:CellRenderer
    {

        protected CellBaseView BaseView { get; set; }
        CellBase Item;

        protected override Android.Views.View GetCellCore(Xamarin.Forms.Cell item, Android.Views.View convertView, Android.Views.ViewGroup parent, Android.Content.Context context) {
            //base.GetCellCore(item, convertView, parent, context);

            Item = item as CellBase;


            if (convertView == null) {
               
            }
            else {
               
            }

            return convertView;
        }

       protected override void OnCellPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            base.OnCellPropertyChanged(sender, e);

            if (e.PropertyName == CellBase.LabelTextProperty.PropertyName) {
                UpdateLabelText();
            }
            else if (e.PropertyName == CellBase.LabelColorProperty.PropertyName) {
                UpdateLabelColor();
            }
            else if (e.PropertyName == CellBase.LabelFontSizeProperty.PropertyName) {
                UpdateLabelFontSize();
            }
            else if (e.PropertyName == "RenderHeight") {
                UpdateHeight();
            }

        }

        protected void UpdateBase() {
            UpdateLabelText();
            UpdateLabelColor();
            UpdateLabelFontSize();
            UpdateIcon();
            UpdateHeight();
        }

        void UpdateLabelText() {
            BaseView.TitleLabel.Text = Item.LabelText;
        }
        void UpdateLabelColor() {
            if (Item.LabelColor != Xamarin.Forms.Color.Default) {
                BaseView.TitleLabel.SetTextColor(Item.LabelColor.ToAndroid());
            }
        }
        void UpdateLabelFontSize() {
            BaseView.TitleLabel.SetTextSize(Android.Util.ComplexUnitType.Sp, (float)Item.LabelFontSize);
        }
        void UpdateIcon() {
            var image = Item.Image as NGraphics.BitmapImage;
            if (image != null) {
                if (BaseView.ImageView == null) {
                    BaseView.AddImageView();
                }
                if (BaseView.ImageView.Drawable != null) {
                    BaseView.ImageView.Drawable.Dispose();
                }
                BaseView.ImageView.SetImageBitmap(image.Bitmap);
            }
        }
        protected void UpdateHeight() {
            BaseView.SetRenderHeight(Cell.RenderHeight);
        }



    }
}
