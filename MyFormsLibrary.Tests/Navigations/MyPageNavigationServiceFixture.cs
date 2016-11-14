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
    public class MyPageNavigationServiceFixture
    {
        IUnityContainer Container;
        IApplicationProvider App;
        ILoggerFacade Log;

        public MyPageNavigationServiceFixture() {
            
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
        public void CreateContentPage() {
            var nav = new MyPageNavigationService(Container, App, Log);

            var page = nav.CreateContentPage("ContentPageNoAction");

            page.GetType().Is(typeof(ContentPageNoAction));
            page.BindingContext.GetType().Is(typeof(ContentPageNoActionViewModel));

        }

        static TestCaseData[] Src_CreateNavigationPage = new[] {
            new TestCaseData(null),new TestCaseData(new NavigationParameters{ {"Key","Value" }} ) 
        };

        [TestCaseSource("Src_CreateNavigationPage")]
        public async Task CreateNavigationPage(NavigationParameters param) {
            var nav = new MyPageNavigationService(Container, App, Log);

            var page = await nav.CreateNavigationPage("NavigationAlpha", nameof(PageAlpha),param);

            page.GetType().Is(typeof(NavigationAlpha));
            var realPage = page as NavigationAlpha;

            realPage.CurrentPage.GetType().Is(typeof(PageAlpha));
            realPage.CurrentPage.BindingContext.GetType().Is(typeof(PageAlphaViewModel));

            var vm = realPage.CurrentPage.BindingContext as PageAlphaViewModel;

            vm.DoneNavigatingTo.IsTrue();
            vm.DoneNavigatedTo.IsTrue();
            vm.DoneNavigatedFrom.IsFalse();
            vm.DoneOnActive.IsFalse();
            vm.DoneOnNonActive.IsFalse();

            if (param == null) {
                vm.Param.Count.Is(0);
            }
            else {
                vm.Param.ContainsKey("Key").IsTrue();
                vm.Param["Key"].Is("Value");
                vm.Param.Count.Is(1);
            }
        }


        [Test]
        public async Task CreateMainPageNavigationHasTabbed() {
            var nav = new MyPageNavigationService(Container, App, Log);
           
            var page = await nav.CreateMainPageNavigationHasTabbed(
                "NavigationTop", "MainTabbedPage",
               new List<ContentPage> {
                    nav.CreateContentPage(nameof(PageAlpha)),
                    nav.CreateContentPage(nameof(PageBeta))
                }
            );

            (page.CurrentPage as IPageController).SendAppearing();

            page.GetType().Is(typeof(NavigationTop));

            var tabbedPage = (page as NavigationPage).CurrentPage;

            tabbedPage.GetType().Is(typeof(MainTabbedPage));

            var realTab = tabbedPage as MainTabbedPage;
            realTab.Children.Count.Is(2);

            var pageA = realTab.Children[0];
            var pageB = realTab.Children[1];

            pageA.GetType().Is(typeof(PageAlpha));
            pageB.GetType().Is(typeof(PageBeta));

            var vmA = pageA.BindingContext as PageAlphaViewModel;
            var vmB = pageB.BindingContext as PageBetaViewModel;

            vmA.DoneNavigatingTo.IsTrue();
            vmA.DoneNavigatedTo.IsTrue();
            vmA.DoneNavigatedFrom.IsFalse();
            vmA.DoneOnActive.IsTrue();
            vmA.DoneOnNonActive.IsTrue();
            vmA.IsActive.IsTrue();

            vmB.DoneNavigatingTo.IsFalse();
            vmB.DoneNavigatedTo.IsFalse();
            vmB.DoneNavigatedFrom.IsFalse();
            vmB.DoneOnActive.IsFalse();
            vmB.DoneOnNonActive.IsFalse();
            vmB.IsActive.IsFalse();

        }

        [Test]
        public async Task NavigationHasTabbedGoNextGoBack() {
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

            ((IPageAware)nav).Page = pageA;
            await nav.NavigateAsync(nameof(ContentPageAllAction));
            tabbed.SendDisappearing();

            vmA.DoneNavigatingTo.IsFalse();
            vmA.DoneNavigatedTo.IsFalse();
            vmA.DoneNavigatedFrom.IsTrue();
            vmA.IsActive.IsFalse();
            vmA.DoneOnActive.IsFalse();
            vmA.DoneOnNonActive.IsTrue();


            vmB.DoneNavigatingTo.IsFalse();
            vmB.DoneNavigatedTo.IsFalse();
            vmB.DoneNavigatedFrom.IsFalse();
            vmB.DoneOnActive.IsFalse();
            vmB.DoneOnNonActive.IsFalse();
            vmB.IsActive.IsFalse();

            vmA.AllClear();
            vmB.AllClear();

            ((IPageAware)nav).Page = new ContentPage();
            await nav.GoBackAsync();
            tabbed.SendAppearing();

            vmA.DoneNavigatingTo.IsTrue();
            vmA.DoneNavigatedTo.IsTrue();
            vmA.DoneNavigatedFrom.IsFalse();
            vmA.DoneOnActive.IsTrue();
            vmA.DoneOnNonActive.IsFalse();
            vmA.IsActive.IsTrue();

            vmB.DoneNavigatingTo.IsFalse();
            vmB.DoneNavigatedTo.IsFalse();
            vmB.DoneNavigatedFrom.IsFalse();
            vmB.DoneOnActive.IsFalse();
            vmB.DoneOnNonActive.IsFalse();
            vmB.IsActive.IsFalse();
        }

        [Test]
        public async Task CreateMainPageTabbedHasNavigation() {
            var nav = new MyPageNavigationService(Container, App, Log);

            var naviA = await nav.CreateNavigationPage(nameof(NavigationAlpha), nameof(PageAlpha));
            var naviB = await nav.CreateNavigationPage(nameof(NavigationBeta), nameof(PageBeta));

            var tabbed = nav.CreateMainPageTabbedHasNavigation(nameof(MainTabbedPage),new List<NavigationPage>{ 
                naviA,naviB});


            naviA.GetType().Is(typeof(NavigationAlpha));
            naviB.GetType().Is(typeof(NavigationBeta));
            naviA.CurrentPage.GetType().Is(typeof(PageAlpha));
            naviB.CurrentPage.GetType().Is(typeof(PageBeta));
            tabbed.GetType().Is(typeof(MainTabbedPage));
            tabbed.Children.Count.Is(2);

            (tabbed as IPageController).SendAppearing();

            var vmA = naviA.CurrentPage.BindingContext as PageAlphaViewModel;
            var vmB = naviB.CurrentPage.BindingContext as PageBetaViewModel;

            vmA.DoneNavigatingTo.IsTrue();
            vmA.DoneNavigatedTo.IsTrue();
            vmA.DoneNavigatedFrom.IsFalse();
            vmA.DoneOnActive.IsTrue();
            vmA.DoneOnNonActive.IsTrue();
            vmA.IsActive.IsTrue();

            vmB.DoneNavigatingTo.IsTrue();
            vmB.DoneNavigatedTo.IsTrue();
            vmB.DoneNavigatedFrom.IsFalse();
            vmB.DoneOnActive.IsFalse();
            vmB.DoneOnNonActive.IsFalse();
            vmB.IsActive.IsFalse();

        }
    }
}
