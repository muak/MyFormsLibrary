using System;
using MyFormsLibrary.CustomRenderers;
using MyFormsLibrary.Droid.CustomRenderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ViewCellAlt), typeof(ViewCellAltRenderer))]
namespace MyFormsLibrary.Droid.CustomRenderers
{

    public class ViewCellAltRenderer : ViewCellRenderer
    {
        protected override Android.Views.View GetCellCore(Xamarin.Forms.Cell item, Android.Views.View convertView, Android.Views.ViewGroup parent, Android.Content.Context context) {
            if (convertView != null) {
                convertView.LongClick -= OnLongClick;
            }
            var view = base.GetCellCore(item, convertView, parent, context);

            view.LongClick += OnLongClick;

            return view;
        }

        void OnLongClick(object sender, Android.Views.View.LongClickEventArgs e) {
            e.Handled = false;
        }
    }
}
