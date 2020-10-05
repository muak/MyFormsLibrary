using System;
using Xamarin.Forms.Platform.Android;
using Android.Content;
using Xamarin.Forms;
using MyFormsLibrary.CustomRenderers;
using MyFormsLibrary.Droid.CustomRenderers;
using AndroidX.Core.View;

[assembly: ExportRenderer(typeof(NestedListView), typeof(NestedListViewRenderer))]
namespace MyFormsLibrary.Droid.CustomRenderers
{
    public class NestedListViewRenderer:ListViewRenderer
    {
        public NestedListViewRenderer(Context context):base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        {
            base.OnElementChanged(e);

            if(e.NewElement != null)
            {
                ViewCompat.SetNestedScrollingEnabled(Control, true);
            }
        }
    }
}
