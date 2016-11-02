using System;
using MyFormsLibrary.CustomRenderers;
using MyFormsLibrary.Droid.CustomRenderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Graphics.Drawables;

[assembly: ExportRenderer(typeof(TableViewEx), typeof(TableViewExRenderer))]
namespace MyFormsLibrary.Droid.CustomRenderers
{
    public class TableViewExRenderer:TableViewRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<TableView> e) {
            base.OnElementChanged(e);

            if (e.NewElement != null) {
                UpdateSeparator();

            }
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == TableViewEx.SeparatorColorProperty.PropertyName) {
                UpdateSeparator();
                Control.InvalidateViews();
            }
            else if (e.PropertyName == TableViewEx.HeaderTextColorProperty.PropertyName) {
                Control.InvalidateViews();
            }
        }

        protected override TableViewModelRenderer GetModelRenderer(Android.Widget.ListView listView, TableView view) {
            return new TableViewModelExRenderer(Context, listView, view);
        }

        void UpdateSeparator() {
            var color = (Element as TableViewEx).SeparatorColor;
            if (color != Color.Default) {
                //Control.Divider.SetColorFilter(color.ToAndroid(), Android.Graphics.PorterDuff.Mode.SrcIn);
                Control.Divider = new ColorDrawable(color.ToAndroid());
                Control.DividerHeight = 1;
                //Control.ScrollingCacheEnabled = false;
            }

        }
    }
}
