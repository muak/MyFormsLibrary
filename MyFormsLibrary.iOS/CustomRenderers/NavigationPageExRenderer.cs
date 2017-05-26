using System.Collections.Generic;
using System.Linq;
using MyFormsLibrary.CustomRenderers;
using MyFormsLibrary.iOS.CustomRenderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(NavigationPageEx), typeof(NavigationPageExRenderer))]
namespace MyFormsLibrary.iOS.CustomRenderers
{
    public class NavigationPageExRenderer : NavigationRenderer
    {

        public override void PushViewController(UIViewController viewController, bool animated)
        {
            base.PushViewController(viewController, animated);

            SetIcons();
        }

        protected override void Dispose(bool disposing)
        {
            var formsItems = (Element as NavigationPageEx).CurrentPage
                                                          .ToolbarItems
                                                          .Where(x => x.Order != ToolbarItemOrder.Secondary)
                                                          .OrderByDescending(x => x.Priority);

            foreach (var item in formsItems) {
                var itemEx = item as ToolbarItemEx;
                if (itemEx == null) continue;
                itemEx.PropertyChanged -= ItemEx_PropertyChanged;
            }

            var ctrl = ViewControllers.Last();
            foreach (var item in ctrl.NavigationItem.LeftBarButtonItems) {
                item.Dispose();
            }

            base.Dispose(disposing);
        }

        void SetIcons()
        {
            var formsItems = (Element as NavigationPageEx).CurrentPage
                                                          .ToolbarItems
                                                          .Where(x => x.Order != ToolbarItemOrder.Secondary)
                                                          .OrderByDescending(x => x.Priority);

            var ctrl = ViewControllers.Last();
            var nativeItems = ctrl.NavigationItem.RightBarButtonItems;

            var rightItems = new List<UIBarButtonItem>();
            var leftItems = new List<UIBarButtonItem>();

            var ncnt = -1;
            foreach (var item in formsItems) {
                ncnt++;
                var itemEx = item as ToolbarItemEx;
                if (itemEx == null) continue;

                if (itemEx.IsLeftIcon) {
                    leftItems.Add(nativeItems[ncnt]);
                    UpdateIcon(itemEx, nativeItems[ncnt]);
                    continue;
                }

                rightItems.Add(nativeItems[ncnt]);
                UpdateIcon(itemEx, nativeItems[ncnt]);
            }

            ctrl.NavigationItem.SetRightBarButtonItems(rightItems.ToArray(), false);
            ctrl.NavigationItem.SetLeftBarButtonItems(leftItems.ToArray(), false);
        }

        void RefreshIcons()
        {
            var formsItems = (Element as NavigationPageEx).CurrentPage
                                                          .ToolbarItems
                                                          .Where(x => x.Order != ToolbarItemOrder.Secondary)
                                                          .OrderByDescending(x => x.Priority);

            var formsRightItems = formsItems.OfType<ToolbarItemEx>().Where(x => !x.IsLeftIcon).ToList();
            var formsLeftItems = formsItems.OfType<ToolbarItemEx>().Where(x => x.IsLeftIcon).ToList();

            var ctrl = ViewControllers.Last();
            var nativeRightItems = ctrl.NavigationItem.RightBarButtonItems;
            var nativeLeftItems = ctrl.NavigationItem.LeftBarButtonItems;

            for (var i = 0; i < formsRightItems.Count(); i++) {
                var itemEx = formsRightItems[i];
                UpdateIcon(itemEx, nativeRightItems[i]);
            }

            for (var i = 0; i < formsLeftItems.Count(); i++) {
                var itemEx = formsLeftItems[i];
                UpdateIcon(itemEx, nativeLeftItems[i]);
            }
        }


        void UpdateIcon(ToolbarItemEx itemEx, UIBarButtonItem nativeItem)
        {
            itemEx.PropertyChanged -= ItemEx_PropertyChanged;
            itemEx.PropertyChanged += ItemEx_PropertyChanged;

            if (!itemEx.IsVisible) {
                nativeItem.Image = null;
                nativeItem.Title = null;
                return;
            }
            if (string.IsNullOrEmpty(itemEx.Resource)) return;

            nativeItem.Image = SvgToUIImage.GetUIImage(itemEx.Resource, 20, 20);
            nativeItem.Title = null;
            nativeItem.Style = UIBarButtonItemStyle.Plain;
            nativeItem.Enabled = itemEx.IsEnabled;
        }

        void ItemEx_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == ToolbarItemEx.IsEnabledProperty.PropertyName ||
                e.PropertyName == ToolbarItemEx.IsVisibleProperty.PropertyName) {
                RefreshIcons();
            }
        }
    }
}

