using System.Collections;
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
using Prism.Unity.Navigation;
using Xamarin.Forms;
using System.Linq;

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
            App = new ApplicationProviderMock();

            Container.RegisterInstance(Log);
            Container.RegisterInstance(App, new ContainerControlledLifetimeManager());
            Container.RegisterType<INavigationServiceEx, MyPageNavigationService>();
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
            Container.RegisterTypeForNavigation<NextPage>();

            ViewModelLocationProvider.SetDefaultViewModelFactory((view, type) => {
                ParameterOverrides overrides = null;

                var page = view as Page;
                if (page != null) {
                    var navService = Container.Resolve<INavigationServiceEx>();
                    ((IPageAware)navService).Page = page;

                    overrides = new ParameterOverrides
                    {
                        { "navigationService", navService }
                    };
                }

                return Container.Resolve(type, overrides);
            });

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
        public void CreateNavigationPage(NavigationParameters param) {
            var nav = new MyPageNavigationService(Container, App, Log);

            var page = nav.CreateNavigationPage("NavigationAlpha", nameof(PageAlpha),param);

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
                vm.Param.GetNavigationMode().Is(NavigationMode.New);
                vm.Param.Count.Is(1);
            }
            else {
                vm.Param.GetNavigationMode().Is(NavigationMode.New);
                vm.Param.ContainsKey("Key").IsTrue();
                vm.Param["Key"].Is("Value");
                vm.Param.Count.Is(2);
            }
        }


        [Test]
        public void CreateMainPageNavigationHasTabbed() {
            var nav = new MyPageNavigationService(Container, App, Log);
           
            var page = nav.CreateMainPageNavigationHasTabbed(
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

            //どちらもNavigatingTo NavigatedToが発火する
            vmA.DoneNavigatingTo.IsTrue();
            vmA.DoneNavigatedTo.IsTrue();
            vmA.DoneNavigatedFrom.IsFalse();
            vmA.DoneOnActive.IsTrue();
            vmA.DoneOnNonActive.IsFalse();
            vmA.IsActive.IsTrue();

            vmB.DoneNavigatingTo.IsTrue();
            vmB.DoneNavigatedTo.IsTrue();
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

            var page = nav.CreateMainPageNavigationHasTabbed(
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

            var curNavi = vmA.NavigationService;
            await curNavi.NavigateAsync(nameof(NextPage));
            var asfassde = pageA.Navigation.NavigationStack;
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

            var nextVM = page.CurrentPage.BindingContext as NextPageViewModel;

            nextVM.DoneNavigatingTo.IsTrue();
            nextVM.DoneNavigatedTo.IsTrue();
            nextVM.DoneNavigatedFrom.IsFalse();
            nextVM.DoneOnActive.IsFalse();
            nextVM.DoneOnNonActive.IsFalse();

            vmA.AllClear();
            vmB.AllClear();
            nextVM.AllClear();

            var ret = await nextVM.NavigationService.GoBackAsync();
            ret.IsTrue();
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

            nextVM.DoneNavigatingTo.IsFalse();
            nextVM.DoneNavigatedTo.IsFalse();
            nextVM.DoneNavigatedFrom.IsTrue();
            nextVM.DoneOnActive.IsFalse();
            nextVM.DoneOnNonActive.IsFalse();

        }

        [Test]
        public async Task CreateMainPageTabbedHasNavigation() {
            var nav = new MyPageNavigationService(Container, App, Log);

            var naviA = nav.CreateNavigationPage(nameof(NavigationAlpha), nameof(PageAlpha));
            var naviB = nav.CreateNavigationPage(nameof(NavigationBeta), nameof(PageBeta));

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
            vmA.DoneOnNonActive.IsFalse();
            vmA.IsActive.IsTrue();

            vmB.DoneNavigatingTo.IsTrue();
            vmB.DoneNavigatedTo.IsTrue();
            vmB.DoneNavigatedFrom.IsFalse();
            vmB.DoneOnActive.IsFalse();
            vmB.DoneOnNonActive.IsFalse();
            vmB.IsActive.IsFalse();

        }

        /// <summary>
        /// Tabbed->Navi->ConentPageパターン
        /// 次ページ遷移で次ページのIsActiveがtrueになる
        /// 遷移しても遷移元おIsActiveはtrueのまま
        /// あくまでも現在のタブの状態を入れるようにする
        /// 戻る時は次ページのActiveは何も変更しない
        /// </summary>
        [Test]
        public async Task TabbedHasNavigationGoNextGoBack() {
            var nav = new MyPageNavigationService(Container, App, Log);

            var naviA = nav.CreateNavigationPage(nameof(NavigationAlpha), nameof(PageAlpha));
            var naviB = nav.CreateNavigationPage(nameof(NavigationBeta), nameof(PageBeta));

            var tabbed = nav.CreateMainPageTabbedHasNavigation(nameof(MainTabbedPage), new List<NavigationPage>{
                naviA,naviB});

            App.MainPage = tabbed;

            (tabbed as IPageController).SendAppearing();

            var vmA = naviA.CurrentPage.BindingContext as PageAlphaViewModel;
            var vmB = naviB.CurrentPage.BindingContext as PageBetaViewModel;

            vmA.AllClear();
            vmB.AllClear();

            var curNavi = vmA.NavigationService;
            await curNavi.NavigateAsync(nameof(NextPage));

            vmA.DoneNavigatingTo.IsFalse();
            vmA.DoneNavigatedTo.IsFalse();
            vmA.DoneNavigatedFrom.IsTrue();
            vmA.IsActive.IsTrue();
            vmA.DoneOnActive.IsFalse();
            vmA.DoneOnNonActive.IsFalse();


            vmB.DoneNavigatingTo.IsFalse();
            vmB.DoneNavigatedTo.IsFalse();
            vmB.DoneNavigatedFrom.IsFalse();
            vmB.DoneOnActive.IsFalse();
            vmB.DoneOnNonActive.IsFalse();
            vmB.IsActive.IsFalse();

            var nextVM = naviA.CurrentPage.BindingContext as NextPageViewModel;

            nextVM.DoneNavigatingTo.IsTrue();
            nextVM.DoneNavigatedTo.IsTrue();
            nextVM.DoneNavigatedFrom.IsFalse();
            nextVM.IsActive.IsTrue();
            nextVM.DoneOnActive.IsTrue();
            nextVM.DoneOnNonActive.IsFalse();

            vmA.AllClear();
            vmB.AllClear();
            nextVM.AllClear();

            var ret = await nextVM.NavigationService.GoBackAsync();
            ret.IsTrue();

            vmA.DoneNavigatingTo.IsTrue();
            vmA.DoneNavigatedTo.IsTrue();
            vmA.DoneNavigatedFrom.IsFalse();
            vmA.DoneOnActive.IsFalse();
            vmA.DoneOnNonActive.IsFalse();
            vmA.IsActive.IsTrue();

            vmB.DoneNavigatingTo.IsFalse();
            vmB.DoneNavigatedTo.IsFalse();
            vmB.DoneNavigatedFrom.IsFalse();
            vmB.DoneOnActive.IsFalse();
            vmB.DoneOnNonActive.IsFalse();
            vmB.IsActive.IsFalse();

            nextVM.DoneNavigatingTo.IsFalse();
            nextVM.DoneNavigatedTo.IsFalse();
            nextVM.DoneNavigatedFrom.IsTrue();
            nextVM.IsActive.IsTrue();
            nextVM.DoneOnActive.IsFalse();
            nextVM.DoneOnNonActive.IsFalse();
        }

        [Test]
        public async Task NavigateAsync_ForViewType() {
            var Inav = new MyPageNavigationService(Container, App, Log) as INavigationServiceEx;

            var naviPage = new NavigationAlpha();
            App.MainPage = naviPage;


            await Inav.NavigateAsync(nameof(PageAlpha));

            var vm1 = naviPage.CurrentPage.BindingContext as PageAlphaViewModel;

            naviPage.CurrentPage.GetType().Is(typeof(PageAlpha));
            naviPage.Navigation.NavigationStack.Count.Is(1);

            vm1.DoneNavigatingTo.IsTrue();
            vm1.DoneNavigatedTo.IsTrue();
            vm1.DoneNavigatedFrom.IsFalse();
            vm1.NavigatingCount.Is(1);
            vm1.NavigatedToCount.Is(1);

            var param = "Parameter";

            //((IPageAware)Inav).Page = naviPage.CurrentPage;
            await vm1.NavigationService.NavigateAsync<PageBeta>(param);

            vm1.MyParam.Value.Is("Parameter");
            naviPage.CurrentPage.GetType().Is(typeof(PageBeta));
            naviPage.Navigation.NavigationStack.Count.Is(2);

            var vm2 = naviPage.CurrentPage.BindingContext as PageBetaViewModel;

            vm1.DoneNavigatedFrom.IsTrue();
            vm1.NavigatedFromCount.Is(1);
            vm2.DoneNavigatingTo.IsTrue();
            vm2.DoneNavigatedTo.IsTrue();
            vm2.DoneNavigatedFrom.IsFalse();
            vm2.NavigatingCount.Is(1);
            vm2.NavigatedToCount.Is(1);
        }

        [Test]
        public async Task NavigateModalAsync_ForViewtype() {
            var Inav = new MyPageNavigationService(Container, App, Log) as INavigationService;

            var naviPage = new NavigationAlpha();
            App.MainPage = naviPage;


            await Inav.NavigateAsync(nameof(PageAlpha));

            var vm1 = naviPage.CurrentPage.BindingContext as PageAlphaViewModel;

            naviPage.CurrentPage.GetType().Is(typeof(PageAlpha));
            naviPage.Navigation.NavigationStack.Count.Is(1);

            vm1.DoneNavigatingTo.IsTrue();
            vm1.DoneNavigatedTo.IsTrue();
            vm1.DoneNavigatedFrom.IsFalse();
            vm1.NavigatingCount.Is(1);
            vm1.NavigatedToCount.Is(1);
           
            await vm1.NavigationService.NavigateModalAsync<PageBeta>();
            naviPage.Navigation.ModalStack.Count.Is(1);
            (naviPage.Navigation.ModalStack[0].BindingContext as PageBetaViewModel).MyParam.Value.IsNull();

            var vm2 = naviPage.Navigation.ModalStack[0].BindingContext as PageBetaViewModel;

            vm1.DoneNavigatedFrom.IsTrue();
            vm1.NavigatedFromCount.Is(1);
            vm2.DoneNavigatingTo.IsTrue();
            vm2.DoneNavigatedTo.IsTrue();
            vm2.DoneNavigatedFrom.IsFalse();
            vm2.NavigatingCount.Is(1);
            vm2.NavigatedToCount.Is(1);
        }

        [Test]
        public async Task ChangeTab_NaviTabbed() {
            var nav = new MyPageNavigationService(Container, App, Log);

            var pageA = nav.CreateContentPage(nameof(PageAlpha));
            var pageB = nav.CreateContentPage(nameof(PageBeta));

            var page = nav.CreateMainPageNavigationHasTabbed(
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

            vmA.IsActive.IsTrue();
            vmA.DoneOnActive.IsTrue();
            vmA.DoneOnNonActive.IsFalse();
            vmA.OnActiveCount.Is(1);
            vmB.IsActive.IsFalse();
            vmB.DoneOnActive.IsFalse();
            vmB.DoneOnNonActive.IsFalse();

            vmA.AllClear();
            vmB.AllClear();

            var ret = vmA.NavigationService.ChangeTab<PageAlpha>();
            ret.IsTrue();
            vmA.IsActive.IsTrue();
            vmA.DoneOnActive.IsFalse();
            vmA.DoneOnNonActive.IsFalse();
            vmB.IsActive.IsFalse();
            vmB.DoneOnActive.IsFalse();
            vmB.DoneOnNonActive.IsFalse();

            vmA.AllClear();
            vmB.AllClear();

            ret = vmA.NavigationService.ChangeTab<PageBeta>();

            ret.IsTrue();

            vmA.IsActive.IsFalse();
            vmA.DoneOnNonActive.IsTrue();
            vmA.OnActiveCount.Is(0);
            vmA.OnNonActiveCount.Is(1);
            vmB.IsActive.IsTrue();
            vmB.DoneOnActive.IsTrue();
            vmB.OnActiveCount.Is(1);
            vmB.OnNonActiveCount.Is(0);

            vmA.AllClear();
            vmB.AllClear();

            ret = vmB.NavigationService.ChangeTab<NavigationPage>();
            ret.IsFalse();
        }

        [Test]
        public async Task ChangeTab_TabbedNavi() {
            var nav = new MyPageNavigationService(Container, App, Log);

            var naviA = nav.CreateNavigationPage(nameof(NavigationAlpha), nameof(PageAlpha));
            var naviB = nav.CreateNavigationPage(nameof(NavigationBeta), nameof(PageBeta));

            var tabbed = nav.CreateMainPageTabbedHasNavigation(nameof(MainTabbedPage), new List<NavigationPage>{
                naviA,naviB});

            App.MainPage = tabbed;
            (tabbed as IPageController).SendAppearing();


            var vmA = naviA.CurrentPage.BindingContext as PageAlphaViewModel;
            var vmB = naviB.CurrentPage.BindingContext as PageBetaViewModel;

            vmA.IsActive.IsTrue();
            vmA.DoneOnActive.IsTrue();
            vmA.DoneOnNonActive.IsFalse();
            vmA.OnActiveCount.Is(1);
            vmB.IsActive.IsFalse();
            vmB.DoneOnActive.IsFalse();
            vmB.DoneOnNonActive.IsFalse();

            vmA.AllClear();
            vmB.AllClear();

            var ret = vmA.NavigationService.ChangeTab<NavigationBeta>();

            ret.IsTrue();
            vmA.IsActive.IsFalse();
            vmA.DoneOnActive.IsFalse();
            vmA.DoneOnNonActive.IsTrue();
            vmA.OnNonActiveCount.Is(1);
            vmB.IsActive.IsTrue();
            vmB.DoneOnActive.IsTrue();
            vmB.DoneOnNonActive.IsFalse();
            vmB.OnActiveCount.Is(1);

            vmA.AllClear();
            vmB.AllClear();

            ret = vmB.NavigationService.ChangeTab<PageAlpha>();
            ret.IsTrue();

            vmA.IsActive.IsTrue();
            vmA.DoneOnActive.IsTrue();
            vmA.DoneOnNonActive.IsFalse();
            vmA.OnActiveCount.Is(1);
            vmB.IsActive.IsFalse();
            vmB.DoneOnActive.IsFalse();
            vmB.DoneOnNonActive.IsTrue();
            vmB.OnNonActiveCount.Is(1);

            vmA.AllClear();
            vmB.AllClear();

            await vmA.NavigationService.NavigateAsync<NextPage>();

            var nextVM = naviA.CurrentPage.BindingContext as NextPageViewModel;

            vmA.IsActive.IsTrue();
            vmA.DoneOnActive.IsFalse();
            vmA.DoneOnNonActive.IsFalse();
            vmB.IsActive.IsFalse();
            vmB.DoneOnActive.IsFalse();
            vmB.DoneOnNonActive.IsFalse();

            nextVM.IsActive.IsTrue();
            nextVM.DoneOnActive.IsTrue();
            nextVM.DoneOnNonActive.IsFalse();
            nextVM.OnActiveCount.Is(1);

            vmA.AllClear();
            vmB.AllClear();
            nextVM.AllClear();

            ret = nextVM.NavigationService.ChangeTab<PageBeta>();
            ret.IsTrue();

            vmA.IsActive.IsFalse();
            vmA.DoneOnActive.IsFalse();
            vmA.DoneOnNonActive.IsTrue();
            vmA.OnNonActiveCount.Is(1);
            vmB.IsActive.IsTrue();
            vmB.DoneOnActive.IsTrue();
            vmB.DoneOnNonActive.IsFalse();
            vmB.OnActiveCount.Is(1);

            nextVM.IsActive.IsFalse();
            nextVM.DoneOnActive.IsFalse();
            nextVM.DoneOnNonActive.IsTrue();
            nextVM.OnNonActiveCount.Is(1);

            vmA.AllClear();
            vmB.AllClear();
            nextVM.AllClear();

            ret = vmB.NavigationService.ChangeTab<ContentPage>();
            ret.IsFalse();
        }
    }
}
