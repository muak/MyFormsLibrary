using System;
using Prism.Common;
using Prism.Logging;
using Xamarin.Forms;
using Prism.Navigation;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Prism.Behaviors;
using Prism.Ioc;

namespace MyFormsLibrary.Navigation
{
    public class MyPageNavigationService:PageNavigationService,INavigationServiceEx
    {
        public IContainerExtension Container { get;private set; }
        IApplicationProvider _app;
        public const string MyNavigationServiceName = "MyPageNavigationService";
        public const string NavigationModeKey = "__NavigationMode";
        internal static NavigationParameters ParameterProxy { get; set;}

        public MyPageNavigationService(IContainerExtension container, IApplicationProvider applicationProvider,IPageBehaviorFactory pageBehaviorFactory, ILoggerFacade logger) 
            :base(container,applicationProvider,pageBehaviorFactory,logger)
        {
            _app = applicationProvider;
            Container = container;
        }

        public Page MainPage { 
            get {
                return _app.MainPage;
            } 
        }

        public void SetAutowireViewModelOnPage(Page page) {
            var vmlResult = Prism.Mvvm.ViewModelLocator.GetAutowireViewModel(page);
            if (vmlResult == null)
                Prism.Mvvm.ViewModelLocator.SetAutowireViewModel(page, true);
        }

        public TabbedPage CreateMainPageTabbedHasNavigation(string tabbedName, IEnumerable<NavigationPage> children) {
            var tabbedPage = CreatePage(tabbedName) as TabbedPage; 

            SetAutowireViewModelOnPage(tabbedPage);

            //tabbedPage.Behaviors.Add(new TabbedPageOverNavigationPageActiveAwareBehavior());

            foreach (var c in children) {
                tabbedPage.Children.Add(c);
                c.Behaviors.Add(new TabbedPageHasNavigationPageActionBehavior());
            }

            //子を追加し終わってからBehaviorを適用しないとActiveAwareが余分に呼ばれる
            _pageBehaviorFactory.ApplyPageBehaviors(tabbedPage);

            return tabbedPage;
        }

        public NavigationPage CreateMainPageNavigationHasTabbed(string naviName, string tabbedName, IList<ContentPage> children,IList<NavigationParameters> parameters) {
            var tabbedPage = CreatePage(tabbedName) as TabbedPage;
            SetAutowireViewModelOnPage(tabbedPage);

            if(parameters == null){
                parameters = new List<NavigationParameters>();
            }

            foreach(var p in parameters){
                p.AddInternalParameter(NavigationModeKey, NavigationMode.New);
            }

            for (var i = parameters.Count - 1; i < children.Count;i++){
                var p = new NavigationParameters();
                p.AddInternalParameter(NavigationModeKey, NavigationMode.New);
                parameters.Add(p);
            }

            for (var i = 0; i < children.Count;i++) {
                var c = children[i];
				PageUtilities.OnNavigatingTo(c, parameters[i]);
                tabbedPage.Children.Add(c);
				PageUtilities.OnNavigatedTo(c, parameters[i]);
            }

            //子を追加し終わってからBehaviorを適用しないとActiveAwareが余分に呼ばれる
            _pageBehaviorFactory.ApplyPageBehaviors(tabbedPage);

            var naviPage = CreatePageFromSegment(naviName) as NavigationPage;
            //naviPage.Behaviors.Remove(naviPage.Behaviors.FirstOrDefault(x => x.GetType() == typeof(NavigationPageActiveAwareBehavior)));

            PageUtilities.OnNavigatingTo(tabbedPage, new NavigationParameters{{NavigationModeKey,NavigationMode.New}});

            naviPage.PushAsync(tabbedPage,false).Wait();

            PageUtilities.OnNavigatedTo(tabbedPage, new NavigationParameters{{NavigationModeKey,NavigationMode.New}});

            //naviPage.Behaviors.Add(new NavigationPageOverTabbedPageCurrentBehavior()); //7.0以降これ使わなくても問題なくなった
            return naviPage;
        }

        public NavigationPage CreateNavigationPage(string navName,string pageName,NavigationParameters parameters=null){

            var naviPage = CreatePageFromSegment(navName) as NavigationPage;
            naviPage.Behaviors.Remove(naviPage.Behaviors.FirstOrDefault(x => x.GetType() == typeof(NavigationPageActiveAwareBehavior)));

            var contentPage = CreatePageFromSegment(pageName);

            if (parameters == null) {
                parameters = new NavigationParameters();
            }
            parameters.AddInternalParameter(NavigationModeKey,NavigationMode.New);

            PageUtilities.OnNavigatingTo(contentPage,parameters);

            naviPage.PushAsync(contentPage, false).Wait();

            PageUtilities.OnNavigatedTo(contentPage,parameters);

            return naviPage;
        }

        public ContentPage CreateContentPage(string pageName) {
            var contentPage = CreatePageFromSegment(pageName);

            return contentPage as ContentPage;
        }

        public Task<bool> GoBackAsync(NavigationParameters parameters, bool? useModalNavigation, bool animated = true) {
            //GoBackAsyncの時にTabbedPageのCurrentPageにParameterProxyを通して渡す。実行はBehaviorで行う
            MyPageNavigationService.ParameterProxy = parameters;
            return GoBackInternal(parameters, useModalNavigation, animated);
        }

        public Task NavigateAsync(string name, NavigationParameters parameters, bool? useModalNavigation, bool animated = true) {
            return NavigateInternal(name, parameters, useModalNavigation, animated);
        }

        public async Task Navigate<T>(ParametersBase parameters = null, bool animated = true) where T : ContentPage {

            var prismParam = parameters?.ToNavigationParameters();
            if (prismParam == null) {
                prismParam = new NavigationParameters();
            }
            await NavigateAsync(typeof(T).Name, prismParam, (bool?)false, animated);
        }

        public async Task NavigateModal<T>(ParametersBase parameters = null, bool animated = true) where T : ContentPage {
            var prismParam = parameters?.ToNavigationParameters();
            if (prismParam == null) {
                prismParam = new NavigationParameters();
            }
            await NavigateAsync(typeof(T).Name, prismParam, (bool?)true, animated);
        }

        public async Task NavigateModal<Tnavi, Tpage>(ParametersBase parameters = null, bool animated = true)
            where Tnavi : NavigationPage
            where Tpage : ContentPage
        {
            var prismParam = parameters?.ToNavigationParameters();
            if (prismParam == null) {
                prismParam = new NavigationParameters();
            }
            await NavigateAsync(typeof(Tnavi).Name + "/" + typeof(Tpage).Name, prismParam, (bool?)true, animated);
        }

        public async Task GoBackModalAsync(bool animated = true)
        {
            await GoBackAsync(null, true, animated);
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
