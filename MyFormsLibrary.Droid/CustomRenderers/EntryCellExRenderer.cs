using System;
using System.Reflection;
using MyFormsLibrary.CustomRenderers;
using MyFormsLibrary.Droid.CustomRenderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Widget;


[assembly: ExportRenderer(typeof(EntryCellEx), typeof(EntryCellExRenderer))]
namespace MyFormsLibrary.Droid.CustomRenderers
{
    public class EntryCellExRenderer:EntryCellRenderer
    {
        EntryCellView EntryCell;
        TextView Label;
        Cell Item;

        protected override Android.Views.View GetCellCore(Xamarin.Forms.Cell item, Android.Views.View convertView, Android.Views.ViewGroup parent, Android.Content.Context context) {
            


            var view =  base.GetCellCore(item, convertView, parent, context);

            var fieldInfo = typeof(EntryCellView).GetField("_label", BindingFlags.Instance | BindingFlags.NonPublic);
            Label = (TextView)fieldInfo.GetValue(view);

            EntryCell = view as EntryCellView;
            Item = item;

            UpdateTextColor();
            Label.SetTextSize(Android.Util.ComplexUnitType.Sp, 18.0f);
            return view;
        }

        protected override void OnCellPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            base.OnCellPropertyChanged(sender, e);

            if (e.PropertyName == EntryCellEx.TextColorProperty.PropertyName) {
                UpdateTextColor();
            }
        }

        void UpdateTextColor() {
            var color = (Item as EntryCellEx).TextColor;
            if (color != Color.Default) {
               EntryCell.EditText.SetTextColor(color.ToAndroid());
            }
        }
    }
}
