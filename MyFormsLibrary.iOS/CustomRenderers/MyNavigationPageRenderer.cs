using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyFormsLibrary.CustomRenderers;
using MyFormsLibrary.iOS.CustomRenderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(MyNavigationPage), typeof(MyNavigationPageRenderer))]
namespace MyFormsLibrary.iOS.CustomRenderers
{
    [Foundation.Preserve(AllMembers = true)]
    public class MyNavigationPageRenderer: NavigationRenderer
    {
        public MyNavigationPageRenderer()
        {
        }

        protected override void OnElementChanged(VisualElementChangedEventArgs e) {
            base.OnElementChanged(e);
            if (e.OldElement != null) {
                e.OldElement.PropertyChanged -= OnPropertyChanged;
            }
            if (e.NewElement != null) {
                e.NewElement.PropertyChanged += OnPropertyChanged;
                
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

        public override void PushViewController(UIViewController viewController, bool animated)
        {            
            base.PushViewController(viewController, animated);

            var curPage = (Element as NavigationPage).CurrentPage;
            foreach (var item in curPage.ToolbarItems.OfType<MyToolbarItem>())
            {
                item.PropertyChanged += OnToolbarItemPropertyChanged;
            }
            SetToolbarItemVisibility();
            //SetLeftToolbarItem();
        }

        public override UIViewController PopViewController(bool animated)
        {           
            var curPage = (Element as NavigationPage).CurrentPage;
            foreach(var item in curPage.ToolbarItems.OfType<MyToolbarItem>())
            {
                item.PropertyChanged -= OnToolbarItemPropertyChanged;
            }

            return base.PopViewController(animated);
        }
        

        void SetToolbarItemVisibility()
        {            
            var curPage = (Element as NavigationPage).CurrentPage;

            if(!curPage.ToolbarItems.OfType<MyToolbarItem>().Any())
            {
                return;
            }

            var ctrl = ViewControllers.Last();
            if (ctrl.NavigationItem.RightBarButtonItems != null)
            {
                for (var i = 0; i < ctrl.NavigationItem.RightBarButtonItems.Length; i++)
                    ctrl.NavigationItem.RightBarButtonItems[i].Dispose();
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
            ctrl.NavigationItem.SetRightBarButtonItems(primaries == null ? new UIBarButtonItem[0] : primaries.ToArray(), false);
            ToolbarItems = secondaries == null ? new UIBarButtonItem[0] : secondaries.ToArray();

            UpdateToolBarVisible();
        }

        // Androidでの実装がめんどくさいので見送り。できないことはないが不安定要素になりかねないし
        // そこまでLeftが欲しいという場面は少ない。少ない場面ならTitleViewを使えば良い。
        void SetLeftToolbarItem()
        {
            var curPage = (Element as NavigationPage).CurrentPage;
            var leftItem = LeftToolItem.GetToolbarItem(curPage);
            if(leftItem == null)
            {
                return;
            }

            var ctrl = ViewControllers.Last();

            if (ctrl.NavigationItem.LeftBarButtonItem != null)
            {
                ctrl.NavigationItem.LeftBarButtonItem.Dispose();
            }

            ctrl.NavigationItem.SetLeftBarButtonItem(leftItem.ToUIBarButtonItem(), false);                
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
