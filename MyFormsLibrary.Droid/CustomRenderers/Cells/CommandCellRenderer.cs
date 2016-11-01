﻿using System;
using MyFormsLibrary.CustomRenderers;
using MyFormsLibrary.Droid.CustomRenderers;
using Xamarin.Forms;
using Android.Content;
using Android.Widget;
using Android.Text;
using Android.Util;
using Android.Views;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CommandCell), typeof(CommandCellRenderer))]
namespace MyFormsLibrary.Droid.CustomRenderers
{
    public class CommandCellRenderer:CellBaseRenderer
    {
        CommandCell CommandCell;
        CommandCellView CellView;

        protected override Android.Views.View GetCellCore(Cell item, Android.Views.View convertView, Android.Views.ViewGroup parent, Android.Content.Context context) {
            base.GetCellCore(item, convertView, parent, context);

            CommandCell = item as CommandCell;

            CellView = convertView as CommandCellView;
            if (CellView == null) {
                CellView = new CommandCellView(context, item);

            }
            else {
                CellView.Execute = null;
            }

            BaseView = CellView;

            CellView.Execute = () => CommandCell.Command?.Execute(CommandCell.CommandParameter);

            UpdateBase();
            UpdateValueText();
            UpdateValueTextFontSize();
            UpdateValueTextColor();


            return CellView;
        }


        protected override void OnCellPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            base.OnCellPropertyChanged(sender, e);

            if (e.PropertyName == CommandCell.ValueTextProperty.PropertyName) {
                UpdateValueText();
            }
            else if (e.PropertyName == CommandCell.ValueTextFontSizeProperty.PropertyName) {
                UpdateValueTextFontSize();
            }
            else if (e.PropertyName == CommandCell.ValueTextColorProperty.PropertyName) {
                UpdateValueTextColor();
            }

        }

        void UpdateValueText() {
            CellView.TextView.Text = CommandCell.ValueText;
        }
        void UpdateValueTextFontSize() {
            if (CommandCell.ValueTextFontSize < 0) {
                CellView.TextView.SetTextSize(ComplexUnitType.Sp,(float)CommandCell.LabelFontSize);
            }
            else {
                CellView.TextView.SetTextSize(ComplexUnitType.Sp,(float)CommandCell.ValueTextFontSize);
            }
        }
        void UpdateValueTextColor() {
            if (CommandCell.ValueTextColor != Xamarin.Forms.Color.Default) {
                CellView.TextView.SetTextColor(CommandCell.ValueTextColor.ToAndroid());
            }
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
            TextView.SetTextSize(ComplexUnitType.Sp, 14f);
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
