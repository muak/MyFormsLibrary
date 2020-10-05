using System;
using Android.Content;
using MyFormsLibrary.CustomRenderers;
using MyFormsLibrary.Droid.CustomRenderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Platform.Android.AppCompat;

[assembly: ExportRenderer(typeof(MyNavigationPage), typeof(MyNavigationPageRenderer))]
namespace MyFormsLibrary.Droid.CustomRenderers
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public class MyNavigationPageRenderer:NavigationPageRenderer
    {
        public MyNavigationPageRenderer(Context context):base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<NavigationPage> e)
        {
            base.OnElementChanged(e);
            if (e.NewElement != null)
            {
                if (Element.Parent is Xamarin.Forms.Application)
                {
                    var statusBarColor = (Element as MyNavigationPage).StatusBarBackColor;
                    if (statusBarColor != Xamarin.Forms.Color.Default)
                    {
                        var window = (Context as FormsAppCompatActivity).Window;
                        window.AddFlags(Android.Views.WindowManagerFlags.DrawsSystemBarBackgrounds);
                        window.ClearFlags(Android.Views.WindowManagerFlags.TranslucentStatus);
                        window.SetStatusBarColor(statusBarColor.ToAndroid());
                    }
                }
            }
        }
    }
}
