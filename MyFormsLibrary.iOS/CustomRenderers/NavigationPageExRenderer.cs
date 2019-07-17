using System.Collections.Generic;
using System.Linq;
using MyFormsLibrary.CustomRenderers;
using MyFormsLibrary.iOS.CustomRenderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using CoreGraphics;

[assembly: ExportRenderer(typeof(NavigationPageEx), typeof(NavigationPageExRenderer))]
namespace MyFormsLibrary.iOS.CustomRenderers
{
    public class NavigationPageExRenderer : NavigationRenderer
    {
        ConcurrentDictionary<Page, List<UIBarButtonItem>> _itemsCache = new ConcurrentDictionary<Page, List<UIBarButtonItem>>();

        public NavigationPageExRenderer()
        {

        }

        public override void PushViewController(UIViewController viewController, bool animated)
        {
            base.PushViewController(viewController, animated);

            SetIcons();
        }

        public override UIViewController PopViewController(bool animated)
        {
            return base.PopViewController(animated);
        }

        public override UIViewController[] PopToViewController(UIViewController viewController, bool animated)
        {
            return base.PopToViewController(viewController, animated);
        }

        protected async override Task<bool> OnPopViewAsync(Page page, bool animated)
        {
            _itemsCache.TryRemove((Element as NavigationPageEx).CurrentPage, out var ret);

            var baseRet = await base.OnPopViewAsync(page, animated);

            // https://forums.xamarin.com/discussion/88042/back-button-ios-white-page-crash
            // http://sandreichuk.blogspot.jp/2017/09/xamarinios-xamarinforms-back-navigation.html
            // ページA遷移→普通にnavigationのbackボタンで戻る→ページA遷移→コードからPopAsync→空白ページ100%発生
            // この問題の対策
            // というか2.5.1.444934で解消されていた件… とりあえずコメントアウトして同様の現象が発生したら解除することにする。

            //var formsCount = page.Navigation.NavigationStack.Count;
            //var nativeCount = ViewControllers.Count();

            //if(formsCount == nativeCount){
            //    await Task.Delay(100);
            //    PopToViewController(ViewControllers[ViewControllers.Count() - 2], false);
            //}

            return baseRet; 
        }

        protected override void Dispose(bool disposing)
        {
            var formsItems = (Element as NavigationPageEx)?.CurrentPage
                                                          .ToolbarItems
                                                          .Where(x => x.Order != ToolbarItemOrder.Secondary)
                                                          .OrderByDescending(x => x.Priority);

            foreach (var item in formsItems) {
                var itemEx = item as ToolbarItemEx;
                if (itemEx == null) continue;
                itemEx.PropertyChanged -= ItemEx_PropertyChanged;
            }

            var ctrl = ViewControllers.Last();
            if (ctrl.NavigationItem.LeftBarButtonItems != null)
            {
                foreach (var item in ctrl.NavigationItem.LeftBarButtonItems)
                {
                    item.Dispose();
                }
            }

            _itemsCache.Clear();
            _itemsCache = null;

            base.Dispose(disposing);
        }

        void SetIcons()
        {
            var curPage = (Element as NavigationPageEx).CurrentPage;
            var formsItems = curPage.ToolbarItems
                                    .Where(x => x.Order != ToolbarItemOrder.Secondary)
                                    .OrderByDescending(x => x.Priority);

            if (!formsItems.Any()){
                return;
            }

            var ctrl = ViewControllers.Last();

            if (ctrl.NavigationItem.RightBarButtonItems == null)
            {
                return;
            }

            var nativeItems = _itemsCache.GetOrAdd(curPage, ctrl.NavigationItem.RightBarButtonItems.ToList());

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

                if(!nativeItems.Any())
                {
                    continue;
                }

                UpdateIcon(itemEx, nativeItems[ncnt]);

                if (!itemEx.IsVisible) {
                    continue;
                }

                rightItems.Add(nativeItems[ncnt]);
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

            var formsRightItems = formsItems.OfType<ToolbarItemEx>().Where(x => !x.IsLeftIcon && x.IsVisible).ToList();
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
            if (e.PropertyName == ToolbarItemEx.IsEnabledProperty.PropertyName) {
                RefreshIcons();
            }
            else if(e.PropertyName == ToolbarItemEx.IsVisibleProperty.PropertyName){
                SetIcons();
            }
        }
    }
}

