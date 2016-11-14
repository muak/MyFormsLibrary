using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Moq;
using MyFormsLibrary.Navigation;
using MyFormsLibrary.Tests.Mocks;
using MyFormsLibrary.Tests.Mocks.ViewModels;
using MyFormsLibrary.Tests.Mocks.Views;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Prism.Common;
using Prism.Logging;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Unity;
using Xamarin.Forms;

namespace MyFormsLibrary.Tests.Navigations
{
    [TestFixture]
    public class NavigationServiceExtensionsFixture
    {
        IUnityContainer Container;
        IApplicationProvider App;
        ILoggerFacade Log;

        public NavigationServiceExtensionsFixture() {
            Container = new UnityContainer();


            Log = new Mock<ILoggerFacade>().Object;

            Container.RegisterType<IApplicationProvider, ApplicationProviderMock>(null, new ContainerControlledLifetimeManager());
            Container.RegisterType<INavigationService, MyPageNavigationService>("MyPageNavigationService");
            Container.RegisterType<INavigationParameter, NavigationParameter>(null, new ContainerControlledLifetimeManager());

            Container.RegisterTypeForNavigation<ContentPageNoAction>();
            Container.RegisterTypeForNavigation<ContentPageAllAction>();
            Container.RegisterTypeForNavigation<PageAlpha>();
            Container.RegisterTypeForNavigation<PageBeta>();
            Container.RegisterTypeForNavigation<MainTabbedPage>();
            Container.RegisterTypeForNavigation<NavigationAlpha>();
            Container.RegisterTypeForNavigation<NavigationBeta>();
            Container.RegisterTypeForNavigation<NavigationGamma>();
            Container.RegisterTypeForNavigation<NavigationTop>();

            App = Container.Resolve<IApplicationProvider>();

            ViewModelLocationProvider.SetDefaultViewModelFactory((type) => Container.Resolve(type));
        }

        [Test]
        public async Task NavigateAsync_ForViewType() {
            var Inav = new MyPageNavigationService(Container, App, Log) as INavigationService;

            var naviPage = new NavigationAlpha();
            App.MainPage = naviPage;


            await Inav.NavigateAsync(nameof(PageAlpha));

            naviPage.CurrentPage.GetType().Is(typeof(PageAlpha));
            naviPage.Navigation.NavigationStack.Count.Is(1);

            var param = "Parameter";

            ((IPageAware)Inav).Page = naviPage.CurrentPage;
            await Inav.NavigateAsync<PageBeta>(param);

            (naviPage.CurrentPage.BindingContext as PageBetaViewModel).MyParam.Value.Is("Parameter");
            naviPage.CurrentPage.GetType().Is(typeof(PageBeta));
            naviPage.Navigation.NavigationStack.Count.Is(2);
        }

        [Test]
        public async Task NavigateModalAsync_ForViewtype() {
            var Inav = new MyPageNavigationService(Container, App, Log) as INavigationService;

            var naviPage = new NavigationAlpha();
            App.MainPage = naviPage;


            await Inav.NavigateAsync(nameof(PageAlpha));

            naviPage.CurrentPage.GetType().Is(typeof(PageAlpha));
            naviPage.Navigation.NavigationStack.Count.Is(1);

            ((IPageAware)Inav).Page = naviPage.CurrentPage;
            await Inav.NavigateModalAsync<PageBeta>();
            naviPage.Navigation.ModalStack.Count.Is(1);
            (naviPage.Navigation.ModalStack[0].BindingContext as PageBetaViewModel).MyParam.Value.IsNull();
        }

        [Test]
        public async Task ChangeTab_NaviTabbed() {
            var nav = new MyPageNavigationService(Container, App, Log);

            var pageA = nav.CreateContentPage(nameof(PageAlpha));
            var pageB = nav.CreateContentPage(nameof(PageBeta));

            var page = await nav.CreateMainPageNavigationHasTabbed(
                "NavigationTop", "MainTabbedPage",
               new List<ContentPage> {
                    pageA,
                    pageB
                }
            );

            App.MainPage = page;

            var tabbed = page.CurrentPage as IPageController;
            tabbed.SendAppearing();

            var vmA = pageA.BindingContext as PageAlphaViewModel;
            var vmB = pageB.BindingContext as PageBetaViewModel;

            vmA.AllClear();
            vmB.AllClear();

            var ret = nav.ChangeTab<PageAlpha>();
            ret.IsTrue();
            vmA.IsActive.IsTrue();
            vmA.DoneOnActive.IsFalse();
            vmA.DoneOnNonActive.IsFalse();
            vmB.IsActive.IsFalse();
            vmB.DoneOnActive.IsFalse();
            vmB.DoneOnNonActive.IsFalse();

            vmA.AllClear();
            vmB.AllClear();

            ret = nav.ChangeTab<PageBeta>();

            ret.IsTrue();

            vmA.IsActive.IsFalse();
            vmA.DoneOnNonActive.IsTrue();
            vmB.IsActive.IsTrue();
            vmB.DoneOnActive.IsTrue();

            vmA.AllClear();
            vmB.AllClear();

            ret = nav.ChangeTab<NavigationPage>();
            ret.IsFalse();
        }

        [Test]
        public async Task ChangeTab_TabbedNavi() {
            var nav = new MyPageNavigationService(Container, App, Log);

            var naviA = await nav.CreateNavigationPage(nameof(NavigationAlpha), nameof(PageAlpha));
            var naviB = await nav.CreateNavigationPage(nameof(NavigationBeta), nameof(PageBeta));

            var tabbed = nav.CreateMainPageTabbedHasNavigation(nameof(MainTabbedPage), new List<NavigationPage>{
                naviA,naviB});

            App.MainPage = tabbed;
            (tabbed as IPageController).SendAppearing();


            var vmA = naviA.CurrentPage.BindingContext as PageAlphaViewModel;
            var vmB = naviB.CurrentPage.BindingContext as PageBetaViewModel;

            vmA.AllClear();
            vmB.AllClear();

            var ret = nav.ChangeTab<NavigationBeta>();

            ret.IsTrue();
            vmA.IsActive.IsFalse();
            vmA.DoneOnNonActive.IsTrue();
            vmB.IsActive.IsTrue();
            vmB.DoneOnActive.IsTrue();

            vmA.AllClear();
            vmB.AllClear();

            ret = nav.ChangeTab<PageAlpha>();
            ret.IsTrue();
            vmA.IsActive.IsTrue();
            vmB.IsActive.IsFalse();

            ret = nav.ChangeTab<ContentPage>();
            ret.IsFalse();
        }
    
    }
}
