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
    public class MyPageNavigationService:UnityPageNavigationService
    {
        public IUnityContainer Container { get;private set; }
        IApplicationProvider _app;



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
                tabbedPage.Children.Add(c);
            }

            var naviPage = CreatePageFromSegment(naviName) as NavigationPage;

            PageUtilities.OnNavigatingTo(tabbedPage, new NavigationParameters());

            await naviPage.PushAsync(tabbedPage,false);

            PageUtilities.OnNavigatedTo(tabbedPage, new NavigationParameters());

            tabbedPage.Behaviors.Add(new TabbedPageCurrentPageOnNavigatedToBehavior());
          
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
    }
}
