using System;
using Prism.Unity.Navigation;
using Microsoft.Practices.Unity;
using Prism.Common;
using Prism.Logging;
using Xamarin.Forms;
using Prism.Navigation;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace MyFormsLibrary.Navigation
{
    public class MyPageNavigationService:UnityPageNavigationService,INavigationServiceEx
    {
        public IUnityContainer Container { get;private set; }
        IApplicationProvider _app;

        internal static NavigationParameters ParameterProxy { get; set;}

        public MyPageNavigationService(IUnityContainer container, IApplicationProvider applicationProvider, ILoggerFacade logger) 
            :base(container,applicationProvider,logger)
        {
            _app = applicationProvider;
            Container = container;
        }

        public Page MainPage { 
            get {
                return _app.MainPage;
            } 
        }

        public TabbedPage CreateMainPageTabbedHasNavigation(string tabbedName, IEnumerable<NavigationPage> children) {
            var tabbedPage = CreatePageFromSegment(tabbedName) as TabbedPage;

            tabbedPage.Behaviors.Add(new TabbedPageOverNavigationPageActiveAwareBehavior());

            foreach (var c in children) {
                tabbedPage.Children.Add(c);
            }

            return tabbedPage;
        }

        public async Task<NavigationPage> CreateMainPageNavigationHasTabbed(string naviName, string tabbedName, IEnumerable<ContentPage> children) {
            var tabbedPage = CreatePageFromSegment(tabbedName) as TabbedPage;


            foreach (var c in children) {
                PageUtilities.OnNavigatingTo(c, new NavigationParameters());
                tabbedPage.Children.Add(c);
                PageUtilities.OnNavigatedTo(c, new NavigationParameters());
            }

            var naviPage = CreatePageFromSegment(naviName) as NavigationPage;

            PageUtilities.OnNavigatingTo(tabbedPage, new NavigationParameters());

            await naviPage.PushAsync(tabbedPage,false);

            PageUtilities.OnNavigatedTo(tabbedPage, new NavigationParameters());

            //tabbedPage.Behaviors.Add(new TabbedPageCurrentPageOnNavigatedToBehavior());
            naviPage.Behaviors.Add(new NavigationPageOverTabbedPageCurrentBehavior());
            return naviPage;
        }

        public async Task<NavigationPage> CreateNavigationPage(string navName,string pageName,NavigationParameters parameters=null){

            var naviPage = CreatePageFromSegment(navName) as NavigationPage;

            var contentPage = CreatePageFromSegment(pageName);

            if (parameters == null) {
                parameters = new NavigationParameters();
            }

            PageUtilities.OnNavigatingTo(contentPage,parameters);

            await naviPage.PushAsync(contentPage, false);

            PageUtilities.OnNavigatedTo(contentPage,parameters);

            return naviPage;
        }

        public ContentPage CreateContentPage(string pageName) {
            var contentPage = CreatePageFromSegment(pageName);

            return contentPage as ContentPage;
        }


        public override async Task<bool> GoBackAsync(NavigationParameters parameters = null, bool? useModalNavigation = default(bool?), bool animated = true)
        {
            //GoBackAsyncの時にTabbedPageのCurrentPageにParameterProxyを通して渡す。実行はBehaviorで行う
            MyPageNavigationService.ParameterProxy = parameters;
            return await base.GoBackAsync(parameters, useModalNavigation, animated);
        }

        public async Task NavigateAsync<T>(object myParam = null, bool animated = true, NavigationParameters originalParam = null) where T : ContentPage
        {
            var param = this.Container.Resolve<INavigationParameter>();
            param.Value = myParam;

            if (originalParam == null) {
                originalParam = new NavigationParameters();
            }
            await base.NavigateAsync(typeof(T).Name, originalParam, (bool?)false, animated);
        }

        public async Task NavigateModalAsync<T>(object myParam = null, bool animated = true, NavigationParameters originalParam = null) where T : ContentPage
        {
            var param = this.Container.Resolve<INavigationParameter>();
            param.Value = myParam;

            if (originalParam == null) {
                originalParam = new NavigationParameters();
            }
            await base.NavigateAsync(typeof(T).Name, originalParam, (bool?)true, animated);
        }

        public async Task NavigateModalAsync<Tnavi, Tpage>(object myParam = null, bool animated = true, NavigationParameters originalParam = null)
            where Tnavi : NavigationPage
            where Tpage : ContentPage
        {
            var param = this.Container.Resolve<INavigationParameter>();
            param.Value = myParam;

            if (originalParam == null) {
                originalParam = new NavigationParameters();
            }
            await base.NavigateAsync(typeof(Tnavi).Name + "/" + typeof(Tpage).Name, originalParam, (bool?)true, animated);
        }

        public async Task GoBackModalAsync(bool animated = true)
        {
            await this.GoBackAsync(null, true, animated);
        }

        public bool ChangeTab<T>() where T : Page
        {
            var mainPage = this.MainPage;
            if (mainPage == null) {
                return false;
            }

            TabbedPage tabbed = null;
            if (mainPage is TabbedPage) {
                tabbed = mainPage as TabbedPage;
                return SearchTargetTab(tabbed, typeof(T));
            }
            else if (mainPage is NavigationPage && (mainPage as NavigationPage)?.CurrentPage is TabbedPage) {
                tabbed = (mainPage as NavigationPage).CurrentPage as TabbedPage;
                return SearchTargetTab(tabbed, typeof(T));
            }
            else {
                return false;
            }

        }

        bool SearchTargetTab(TabbedPage tabbed, Type target)
        {

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
