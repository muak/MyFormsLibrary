using System;
using Android.Content;
using Android.Text;
using Android.Views;
using Android.Widget;
using MyFormsLibrary.CustomRenderers;
using MyFormsLibrary.Droid.CustomRenderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(LabelCell), typeof(LabelCellRenderer))]
namespace MyFormsLibrary.Droid.CustomRenderers
{
    public class LabelCellRenderer:CellBaseRenderer    {

        LabelCell LabelCell;
        CommandCellView CellView;

        protected override Android.Views.View GetCellCore(Xamarin.Forms.Cell item, Android.Views.View convertView, Android.Views.ViewGroup parent, Android.Content.Context context) {
            base.GetCellCore(item, convertView, parent, context);

            LabelCell = item as LabelCell;

            CellView = convertView as CommandCellView;
            if (CellView == null) {
                CellView = new CommandCellView(context, item);

            }
            else {
                ParentElement.PropertyChanged -= ParentElement_PropertyChanged;
            }

            BaseView = CellView;

            ParentElement.PropertyChanged += ParentElement_PropertyChanged;
           
            UpdateBase();
            UpdateValueText();
            UpdateValueTextFontSize();
            UpdateValueTextColor();


            return CellView;

        }

        protected override void OnCellPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            base.OnCellPropertyChanged(sender, e);

            if (e.PropertyName == LabelCell.ValueTextProperty.PropertyName) {
                UpdateValueText();
            }
            else if (e.PropertyName == LabelCell.ValueTextFontSizeProperty.PropertyName) {
                UpdateValueTextFontSize();
            }
            else if (e.PropertyName == LabelCell.ValueTextColorProperty.PropertyName) {
                UpdateValueTextColor();
            }

        }

        void ParentElement_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == TableViewEx.CellValueTextColorProperty.PropertyName) {
                UpdateValueTextColor();
            }
            else if (e.PropertyName == TableViewEx.CellValueTextFontSizeProperty.PropertyName) {
                UpdateValueTextFontSize();
            }
        }

        void UpdateValueText() {
            CellView.TextView.Text = LabelCell.ValueText;
        }
        void UpdateValueTextFontSize() {
            if (LabelCell.ValueTextFontSize > 0) {
                CellView.TextView.SetTextSize(Android.Util.ComplexUnitType.Sp, (float)LabelCell.ValueTextFontSize);
            }
            else {
                CellView.TextView.SetTextSize(Android.Util.ComplexUnitType.Sp, (float)ParentElement.CellValueTextFontSize);

            }
            CellView.Invalidate();
        }
        void UpdateValueTextColor() {
            if (LabelCell.ValueTextColor != Xamarin.Forms.Color.Default) {
                CellView.TextView.SetTextColor(LabelCell.ValueTextColor.ToAndroid());
            }
            else if (ParentElement.CellValueTextColor != Xamarin.Forms.Color.Default) {
                CellView.TextView.SetTextColor(ParentElement.CellValueTextColor.ToAndroid());
            }
            CellView.Invalidate();
        }
    }
    public class CommandCellView : CellBaseView
    {
        public Action Execute { get; set; }
        public TextView TextView { get; set; }

        public CommandCellView(Context context, Cell cell) : base(context, cell) {

            TextView = new TextView(context);
            TextView.SetSingleLine(true);
            TextView.Ellipsize = TextUtils.TruncateAt.End;
            TextView.Gravity = GravityFlags.Right;

            var textParams = new LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent) {
                Width = 0,
                Weight = 1,
                Gravity = GravityFlags.FillHorizontal | GravityFlags.CenterVertical
            };
            using (textParams) {
                AddView(TextView, textParams);
            }

        }
    }
}
