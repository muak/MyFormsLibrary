using System;
using Xamarin.Forms;
using Prism.Navigation;
using Microsoft.Practices.Unity;
using System.Collections.Generic;
using System.Threading.Tasks;
using Prism.Unity.Navigation;

namespace MyFormsLibrary.Navigation
{
    public static class NavigationServiceExtentions
    {

        public static async Task NavigateAsync<T>(this INavigationService nav,object myParam=null,bool animated=true, NavigationParameters originalParam = null) where T:ContentPage {
            var myNavi = nav as MyPageNavigationService;

            var param = myNavi.Container.Resolve<INavigationParameter>();
            param.Value = myParam;

            if (originalParam == null) {
                originalParam = new NavigationParameters();
            }
            await nav.NavigateAsync(typeof(T).Name,originalParam,(bool?)false,animated);
        }

        public static async Task NavigateModalAsync<T>(this INavigationService nav,object myParam=null, bool animated = true, NavigationParameters originalParam = null) where T : ContentPage {
            var myNavi = nav as MyPageNavigationService;
            var param = myNavi.Container.Resolve<INavigationParameter>();
            param.Value = myParam;

            if (originalParam == null) {
                originalParam = new NavigationParameters();
            }
            await nav.NavigateAsync(typeof(T).Name, originalParam, (bool?)true, animated);
        }

        public static bool ChangeTab<T>(this INavigationService nav) where T:Page {

            var mainPage = (nav as MyPageNavigationService)?.MainPage;
            if (mainPage == null) {
                return false;
            }

            TabbedPage tabbed = null;
            if (mainPage is TabbedPage) {
                tabbed = mainPage as TabbedPage;
                return SearchTargetTab(tabbed,typeof(T));
            }
            else if (mainPage is NavigationPage && (mainPage as NavigationPage)?.CurrentPage is TabbedPage) {
                tabbed = (mainPage as NavigationPage).CurrentPage as TabbedPage;
                return SearchTargetTab(tabbed, typeof(T));
            }
            else {
                return false;
            }

        }

        static bool SearchTargetTab(TabbedPage tabbed,Type target) {
            
            foreach (var child in tabbed.Children) {
                if (child.GetType() == target) {
                    tabbed.CurrentPage = child;
                    return true;
                }
                var nav = (child as NavigationPage);
                if (nav == null) {
                    continue;
                }

                if (nav.CurrentPage.GetType() == target) {
                    tabbed.CurrentPage = child;
                    return true;
                }

            }

            return false;
        }

    }
}
