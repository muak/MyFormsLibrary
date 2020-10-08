using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyFormsLibrary.CustomRenderers;
using MyFormsLibrary.iOS.CustomRenderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(MyNavigationPage), typeof(MyNavigationPageRenderer))]
namespace MyFormsLibrary.iOS.CustomRenderers
{
    [Foundation.Preserve(AllMembers = true)]
    public class MyNavigationPageRenderer: NavigationRenderer
    {
        public MyNavigationPageRenderer()
        {
            System.Diagnostics.Debug.WriteLine("In renderer");
        }

        protected override void OnElementChanged(VisualElementChangedEventArgs e) {
            base.OnElementChanged(e);
            if (e.OldElement != null) {
                e.OldElement.PropertyChanged -= OnPropertyChanged;
            }
            if (e.NewElement != null) {
                e.NewElement.PropertyChanged += OnPropertyChanged;
                (e.NewElement as NavigationPage).BarBackgroundColor = Xamarin.Forms.Color.Red;
            }            
        }

        private void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == NavigationPage.BarBackgroundColorProperty.PropertyName) {
                SetNeedsStatusBarAppearanceUpdate();
                PreferredStatusBarStyle();
                SetNeedsStatusBarAppearanceUpdate();
            }
        }

        public override UIStatusBarStyle PreferredStatusBarStyle() {
            var navigationPage = Element as NavigationPage;
            return navigationPage.BarBackgroundColor.Luminosity >= 0.5 ? UIStatusBarStyle.DarkContent : UIStatusBarStyle.LightContent;
        }

        UIViewController _currentVC;

        public override void PushViewController(UIViewController viewController, bool animated)
        {
            base.PushViewController(viewController, animated);
            _currentVC = viewController;
            SetToolbarItemVisibility();
        }

        public override UIViewController PopViewController(bool animated)
        {
            if(ViewControllers.Last() == _currentVC)
            {
                ;
            }
            return base.PopViewController(animated);
        }

        protected override Task<bool> OnPopViewAsync(Page page, bool animated)
        {
            if (ViewControllers.Last() == _currentVC)
            {
                ;
            }
            return base.OnPopViewAsync(page, animated);
        }

        void SetToolbarItemVisibility()
        {            
            var curPage = (Element as NavigationPage).CurrentPage;

            if(!curPage.ToolbarItems.OfType<MyToolbarItem>().Any())
            {
                return;
            }

            if (NavigationItem.RightBarButtonItems != null)
            {
                for (var i = 0; i < NavigationItem.RightBarButtonItems.Length; i++)
                    NavigationItem.RightBarButtonItems[i].Dispose();
            }
            if (ToolbarItems != null)
            {
                for (var i = 0; i < ToolbarItems.Length; i++)
                    ToolbarItems[i].Dispose();
            }

            List<UIBarButtonItem> primaries = null;
            List<UIBarButtonItem> secondaries = null;
            
            foreach (var item in curPage.ToolbarItems)
            {
                item.PropertyChanged += OnToolbarItemPropertyChanged;

                if(item is MyToolbarItem myItem)
                {
                    if (!myItem.IsVisible)
                        continue;
                }

                if (item.Order == ToolbarItemOrder.Secondary)
                    (secondaries = secondaries ?? new List<UIBarButtonItem>()).Add(item.ToUIBarButtonItem(true));
                else
                    (primaries = primaries ?? new List<UIBarButtonItem>()).Add(item.ToUIBarButtonItem());
            }

            if (primaries != null)
                primaries.Reverse();
            NavigationItem.SetRightBarButtonItems(primaries == null ? new UIBarButtonItem[0] : primaries.ToArray(), false);
            ToolbarItems = secondaries == null ? new UIBarButtonItem[0] : secondaries.ToArray();

            UpdateToolBarVisible();
        }

        void UpdateToolBarVisible()
        {
            if(!(View is UIToolbar secondaryToolbar))
            {
                return;
            }

            if (secondaryToolbar == null)
                return;
            if (TopViewController != null && TopViewController.ToolbarItems != null && TopViewController.ToolbarItems.Any())
            {
                secondaryToolbar.Hidden = false;
                secondaryToolbar.Items = TopViewController.ToolbarItems;
            }
            else
            {
                secondaryToolbar.Hidden = true;
                //secondaryToolbar.Items = null;
            }

            TopViewController?.NavigationItem?.TitleView?.SizeToFit();
            TopViewController?.NavigationItem?.TitleView?.LayoutSubviews();
        }

        private void OnToolbarItemPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == MyToolbarItem.IsVisibleProperty.PropertyName)
            {
                SetToolbarItemVisibility();
            }
        }
    }
}
