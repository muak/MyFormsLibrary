using System.Collections.Generic;
using System.Reflection;
using Android.Content;
using Android.Views;
using AndroidX.AppCompat.Widget;
using Google.Android.Material.AppBar;
using MyFormsLibrary.CustomRenderers;
using MyFormsLibrary.Droid.CustomRenderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Platform.Android.AppCompat;

[assembly: ExportRenderer(typeof(CoordinatorPage), typeof(CoordinatorPageRenderer))]
namespace MyFormsLibrary.Droid.CustomRenderers
{
    public class CoordinatorPageRenderer : NavigationPageRenderer
    {
        Toolbar _toolbar;
        AppBarLayout _appBarLayout;
        IPageController PageController => Element;
        int _barHeight;

        public CoordinatorPageRenderer(Context context) : base(context) { }

        protected override void OnElementChanged(ElementChangedEventArgs<NavigationPage> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                if (_toolbar != null)
                {
                    _toolbar.RemoveFromParent();
                }
            }

            if (e.NewElement != null)
            {
                _toolbar = (Toolbar)GetChildAt(0);

                var getNavBarHeight = typeof(NavigationPageRenderer).GetMethod("ActionBarHeight", BindingFlags.Instance | BindingFlags.NonPublic);
                _barHeight = (int)getNavBarHeight.Invoke(this, new object[] { });

                _appBarLayout = (Context as FormsAppCompatActivity).FindViewById<AppBarLayout>(Resource.Id.coordinatorAppBar);

                _toolbar.RemoveFromParent();

                using (var toolbarParams = new AppBarLayout.LayoutParams(LayoutParams.MatchParent, _barHeight))
                {
                    toolbarParams.ScrollFlags = AppBarLayout.LayoutParams.ScrollFlagScroll | AppBarLayout.LayoutParams.ScrollFlagEnterAlways;
                    _appBarLayout.AddView(_toolbar, toolbarParams);
                }
            }
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            base.OnLayout(changed, l, t, r, b);

            int containerHeight = 0;
            if (!CoordinatorPage.GetIsBarScroll(Element.CurrentPage))
            {
                containerHeight = _barHeight;
            }

            PageController.ContainerArea = new Rectangle(0, 0, Context.FromPixels(r - l), Context.FromPixels(b - t - containerHeight));

            var children = new List<Android.Views.View>();
            for (var i = 0; i < ChildCount; i++)
            {
                if (GetChildAt(i) is ViewGroup viewGroup)
                {
                    if (viewGroup.ChildCount == 1 && viewGroup.GetChildAt(0) is PageRenderer page)
                    {
                        viewGroup.Layout(0, 0, r - l, b - t - containerHeight);
                    }
                }
            }
        }
    }
}
