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
        protected TableViewEx ParentElement;
        CellBase Item;

        protected override Android.Views.View GetCellCore(Xamarin.Forms.Cell item, Android.Views.View convertView, Android.Views.ViewGroup parent, Android.Content.Context context) {
            //base.GetCellCore(item, convertView, parent, context);

            Item = item as CellBase;
            ParentElement = item.Parent as TableViewEx;

            if (convertView == null) {
                ParentElement.PropertyChanged += ParentElement_PropertyChanged;
            }
            else {
                ParentElement.PropertyChanged -= ParentElement_PropertyChanged;
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
            else if (e.PropertyName == CellBase.ErrorMessageProperty.PropertyName) {
                UpdateErrorMessage();
            }
            else if (e.PropertyName == CellBase.ValueTextFontSizeProperty.PropertyName) {
                UpdateValueTextFontSize();
            }
            else if (e.PropertyName == CellBase.ValueTextColorProperty.PropertyName) {
                UpdateValueTextColor();
            }

        }

        protected void UpdateBase() {
            UpdateLabelText();
            UpdateLabelColor();
            UpdateLabelFontSize();
            UpdateIcon();
            UpdateHeight();
            UpdateValueTextColor();
            UpdateValueTextFontSize();
        }

        void ParentElement_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == TableViewEx.CellLabelColorProperty.PropertyName) {
                UpdateLabelColor();
            }
            else if (e.PropertyName == TableViewEx.CellLabelFontSizeProperty.PropertyName) {
                UpdateLabelFontSize();
            }
            else if (e.PropertyName == TableViewEx.CellValueTextColorProperty.PropertyName) {
                UpdateValueTextColor();
            }
            else if (e.PropertyName == TableViewEx.CellValueTextFontSizeProperty.PropertyName) {
                UpdateValueTextFontSize();
            }

        }

        void UpdateErrorMessage()
        {

            var msg = Item.ErrorMessage;
            if (string.IsNullOrEmpty(msg)) {
                BaseView.ErrorLabel.Visibility = Android.Views.ViewStates.Invisible;
                return;
            }

            BaseView.ErrorLabel.Text = msg;
            BaseView.ErrorLabel.Visibility = Android.Views.ViewStates.Visible;
        }

        void UpdateLabelText() {
            BaseView.TitleLabel.Text = Item.LabelText;
        }
        void UpdateLabelColor() {
            if (Item.LabelColor != Xamarin.Forms.Color.Default) {
                BaseView.TitleLabel.SetTextColor(Item.LabelColor.ToAndroid());
            }
            else if (ParentElement.CellLabelColor != Xamarin.Forms.Color.Default) {
                BaseView.TitleLabel.SetTextColor(ParentElement.CellLabelColor.ToAndroid());
            }
            BaseView.Invalidate();
        }
        void UpdateLabelFontSize() {
            if (Item.LabelFontSize > 0) {
                BaseView.TitleLabel.SetTextSize(Android.Util.ComplexUnitType.Sp, (float)Item.LabelFontSize);
            }
            else {
                BaseView.TitleLabel.SetTextSize(Android.Util.ComplexUnitType.Sp, (float)ParentElement.CellLabelFontSize);
            }
            BaseView.Invalidate();

        }

        void UpdateValueTextFontSize()
        {
            if (Item.ValueTextFontSize > 0) {
                BaseView.ValueText.SetTextSize(Android.Util.ComplexUnitType.Sp, (float)Item.ValueTextFontSize);
            }
            else {
                BaseView.ValueText.SetTextSize(Android.Util.ComplexUnitType.Sp, (float)ParentElement.CellValueTextFontSize);

            }
            BaseView.Invalidate();
        }

        void UpdateValueTextColor()
        {
            if (Item.ValueTextColor != Xamarin.Forms.Color.Default) {
                BaseView.ValueText.SetTextColor(Item.ValueTextColor.ToAndroid());
            }
            else if (ParentElement.CellValueTextColor != Xamarin.Forms.Color.Default) {
                BaseView.ValueText.SetTextColor(ParentElement.CellValueTextColor.ToAndroid());
            }
            BaseView.Invalidate();
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
