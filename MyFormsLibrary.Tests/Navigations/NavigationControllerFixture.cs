using NUnit.Framework;
using MyFormsLibrary.Navigation;
using MyFormsLibrary.Tests.Mocks.Views;
using Microsoft.Practices.Unity;
using Xamarin.Forms;
using MyFormsLibrary.Tests.Mocks;
using System.Threading.Tasks;
using System.Collections.Generic;
using Prism.Mvvm;
using MyFormsLibrary.Tests.Mocks.ViewModels;
using System.Linq;

namespace MyFormsLibrary.Tests.Navigations
{
    [TestFixture]
    public class NavigationControllerFixture
    {

        UnityContainer Container;
        IApplicationProviderForNavi IApplicationProvider;

        public NavigationControllerFixture() {
            
            Container = new UnityContainer();
           

            Container.RegisterType<IApplicationProviderForNavi, ApplicationProviderForNaviMock>(null,new ContainerControlledLifetimeManager());
            Container.RegisterType<INavigationController, NavigationController>(null, new ContainerControlledLifetimeManager());
            Container.RegisterType<INavigationParameter, NavigationParameter>(null, new ContainerControlledLifetimeManager());
           
            IApplicationProvider = Container.Resolve<IApplicationProviderForNavi>();
            ViewModelLocationProvider.SetDefaultViewModelFactory((type) => Container.Resolve(type));    //超重要
        }

        static TestCaseData[] Src_CreateNavigationPage_Exec = new[]{ new TestCaseData(new NavigationAlpha(),new ContentPageNonViewNonAction())};

        [TestCaseSource("Src_CreateNavigationPage_Exec")]
        public async Task CreateNavigationPage_Exec<T1,T2>(T1 value1,T2 value2) where T1:NavigationPage where T2:ContentPage{
            var nav = new NavigationController(Container, IApplicationProvider);

            var page = await nav.CreateNavigationPage<T1,T2>();

            page.GetType().Is(typeof(T1));
            page.CurrentPage.GetType().Is(typeof(T2));
            page.Navigation.NavigationStack.Count.Is(1);
        }

        static TestCaseData[] Src_CreateTabbedPage_Exec = new[] { new TestCaseData(new MainTabbedPage()),new TestCaseData(new TabbedPage()) };

        [TestCaseSource("Src_CreateTabbedPage_Exec")]
        public async Task CreateTabbedPage_Exec<T1>(T1 value) where T1:TabbedPage{
            var nav = new NavigationController(Container, IApplicationProvider);
            var list = new List<NavigationPage> {
                await nav.CreateNavigationPage<NavigationPage,ContentPage>(),
                await nav.CreateNavigationPage<NavigationPage,ContentPage>(),
                await nav.CreateNavigationPage<NavigationPage,ContentPage>(),
            };
            var tab = nav.CreateTabbedPage<T1>(list);

            tab.GetType().Is(typeof(T1));
            tab.Children.Is(list);
            foreach (var l in list) {
                l.Navigation.NavigationStack.Count.Is(1);
            }
        }

        async Task PageSet_NoAction(NavigationController navi) {
            var list = new List<NavigationPage> {
                await navi.CreateNavigationPage<NavigationPage,ContentPage>(),
                await navi.CreateNavigationPage<NavigationPage,ContentPage>(),
                await navi.CreateNavigationPage<NavigationPage,ContentPage>(),
            };
            var tab = navi.CreateTabbedPage<MainTabbedPage>(list);

            IApplicationProvider.MainPage = tab;
        }


        static TestCaseData[] Src_Navigate_Base_SendParam = new[] { new TestCaseData(1),new TestCaseData("abc"),new TestCaseData(new ContentPage { Title = "xyz" }),new TestCaseData(null) };

        [TestCaseSource("Src_Navigate_Base_SendParam")]
        public async Task Navigate_Base_SendParam(object param){
            var nav = new NavigationController(Container, IApplicationProvider);
            var page = await nav.CreateNavigationPage<NavigationPage,ContentPage>();
            IApplicationProvider.MainPage = page;
            var prePage = page.CurrentPage;
            page.Navigation.NavigationStack.Count.Is(1);
            await nav.PushAsync<GetParamPage>(param);

            page.CurrentPage.GetType().Is(typeof(GetParamPage));
            page.Navigation.NavigationStack.Count.Is(2);
            page.Navigation.NavigationStack[0].IsSameReferenceAs(prePage);
            page.Navigation.ModalStack.Count.Is(0);

            (page.CurrentPage.BindingContext as INavigationParameter).Value.Is(param);

            await nav.GoBackAsync();

            page.CurrentPage.GetType().Is(typeof(ContentPage));
            page.Navigation.NavigationStack.Count.Is(1);
            page.Navigation.NavigationStack.Last().IsSameReferenceAs(prePage);
            page.Navigation.ModalStack.Count.Is(0);
        }

        [TestCaseSource("Src_Navigate_Base_SendParam")]
        public async Task Navigate_AllAction(object param) {
            var nav = new NavigationController(Container, IApplicationProvider);
            var page = await nav.CreateNavigationPage<NavigationPage, AllActionPage>();
            IApplicationProvider.MainPage = page;
            var prePage = page.CurrentPage;
            (prePage.BindingContext as AllActionPageViewModel).DoneFoward.IsFalse();
            await nav.PushAsync<AllActionPage>(param);

            (prePage.BindingContext as AllActionPageViewModel).DoneFoward.IsTrue();
                       
            page.Navigation.NavigationStack[0].IsSameReferenceAs(prePage);
            page.Navigation.ModalStack.Count.Is(0);

            (page.CurrentPage.BindingContext as INavigationParameter).Value.Is(param);

            var poppedPage = page.CurrentPage;
            (poppedPage.BindingContext as AllActionPageViewModel).DoneDispose.IsFalse();
            (prePage.BindingContext as AllActionPageViewModel).DoneBack.IsFalse();
          
            await nav.GoBackAsync();

            (poppedPage.BindingContext as AllActionPageViewModel).DoneDispose.IsTrue();
            (prePage.BindingContext as AllActionPageViewModel).DoneBack.IsTrue();

            page.Navigation.NavigationStack.Last().IsSameReferenceAs(prePage);
            page.Navigation.ModalStack.Count.Is(0);
        }

        [Test]
        public async Task Navigation_OnModal() {
            var nav = new NavigationController(Container, IApplicationProvider);
            var page = await nav.CreateNavigationPage<NavigationPage, AllActionPage>();
            IApplicationProvider.MainPage = page;
            var curPage = page.CurrentPage;

            await nav.PushModalAsync<AllActionPage>();

            (page.CurrentPage.BindingContext as AllActionPageViewModel).DoneFoward = false;
            page.Navigation.ModalStack.Count.Is(1);

            var modalPrePage = page.Navigation.ModalStack[0];

            await nav.PushAsync<AllActionPage>();

            (page.CurrentPage.BindingContext as AllActionPageViewModel).DoneFoward.IsFalse();
            (modalPrePage.BindingContext as AllActionPageViewModel).DoneFoward.IsFalse();
            page.Navigation.ModalStack.Count.Is(1);
            page.CurrentPage.IsSameReferenceAs(curPage);
            page.Navigation.ModalStack[0].IsSameReferenceAs(modalPrePage);

        }

        [Test]
        public async Task Navigation_TabChange() {
            var nav = new NavigationController(Container, IApplicationProvider);
            var list = new List<NavigationPage> {
                await nav.CreateNavigationPage<NavigationAlpha,AllActionPage>(),
                await nav.CreateNavigationPage<NavigationBeta,AllActionPage>(),
            };
            var tab = nav.CreateTabbedPage<MainTabbedPage>(list);
            IApplicationProvider.MainPage = tab;

            var tab1 = list[0].CurrentPage.BindingContext as AllActionPageViewModel;
            var tab2 = list[1].CurrentPage.BindingContext as AllActionPageViewModel;

            await nav.PushAsync<NavigationBeta,AllActionPage>();

            tab1.DoneTabFrom.IsTrue();
            tab2.DoneTabTo.IsTrue();
            tab2.DoneFoward.IsTrue();

            tab.CurrentPage.Navigation.NavigationStack.Count.Is(2);

            var curPage = (tab.CurrentPage as NavigationPage).CurrentPage.BindingContext as AllActionPageViewModel;

            curPage.DoneTabFrom.IsFalse();
            curPage.DoneTabTo.IsFalse();
            curPage.DoneBack.IsFalse();
            curPage.DoneFoward.IsFalse();

            await nav.PushAsync<NavigationPage, AllActionPage>();

            tab.CurrentPage.GetType().Is(typeof(NavigationBeta));


        }

        [TestCaseSource("Src_Navigate_Base_SendParam")]
        public async Task NavigationModal_Base_SendParam(object param) {
            var nav = new NavigationController(Container, IApplicationProvider);
            var page = await nav.CreateNavigationPage<NavigationPage, ContentPage>();
            IApplicationProvider.MainPage = page;
            var prePage = page.CurrentPage;
            page.Navigation.NavigationStack.Count.Is(1);
            page.Navigation.ModalStack.Count.Is(0);
            await nav.PushModalAsync<GetParamPage>(param);

            var curPage = page.Navigation.ModalStack[0];
            curPage.GetType().Is(typeof(GetParamPage));
            page.Navigation.ModalStack.Count.Is(1);
            page.Navigation.NavigationStack[0].IsSameReferenceAs(prePage);
            page.Navigation.NavigationStack.Count.Is(1);

            (curPage.BindingContext as INavigationParameter).Value.Is(param);

         
            await nav.GoBackAsync();
            IApplicationProvider.ModalPopped.Invoke(page,new ModalPoppedEventArgs(curPage)); // raise ModalPopped Manual

            page.CurrentPage.GetType().Is(typeof(ContentPage));
            page.Navigation.NavigationStack.Count.Is(1);
            page.Navigation.NavigationStack.Last().IsSameReferenceAs(prePage);
            page.Navigation.ModalStack.Count.Is(0);
        }

        [TestCaseSource("Src_Navigate_Base_SendParam")]
        public async Task NavigationModal_AllAction(object param) {
            var nav = new NavigationController(Container, IApplicationProvider);
            var page = await nav.CreateNavigationPage<NavigationPage, AllActionPage>();
            IApplicationProvider.MainPage = page;
            var prePage = page.CurrentPage;
            page.Navigation.NavigationStack.Count.Is(1);
            page.Navigation.ModalStack.Count.Is(0);

            (prePage.BindingContext as AllActionPageViewModel).DoneFoward.IsFalse();
            await nav.PushModalAsync<AllActionPage>(param, true);
            (prePage.BindingContext as AllActionPageViewModel).DoneFoward.IsTrue();

            var curPage = page.Navigation.ModalStack[0];
            page.Navigation.ModalStack.Count.Is(1);
            page.Navigation.NavigationStack[0].IsSameReferenceAs(prePage);
            page.Navigation.NavigationStack.Count.Is(1);

            (curPage.BindingContext as INavigationParameter).Value.Is(param);


            (curPage.BindingContext as AllActionPageViewModel).DoneDispose.IsFalse();
            (curPage.BindingContext as AllActionPageViewModel).DoneBack.IsFalse();

            await nav.GoBackAsync();
            IApplicationProvider.ModalPopped.Invoke(page, new ModalPoppedEventArgs(curPage)); // raise ModalPopped Manual

            (curPage.BindingContext as AllActionPageViewModel).DoneDispose.IsTrue();
            (prePage.BindingContext as AllActionPageViewModel).DoneBack.IsTrue();

            page.Navigation.NavigationStack.Count.Is(1);
            page.Navigation.NavigationStack.Last().IsSameReferenceAs(prePage);
            page.Navigation.ModalStack.Count.Is(0);
        }

        [TestCaseSource("Src_Navigate_Base_SendParam")]
        public async Task NavigationModal_OnModal(object param) {
            var nav = new NavigationController(Container, IApplicationProvider);
            var page = await nav.CreateNavigationPage<NavigationPage, AllActionPage>();
            IApplicationProvider.MainPage = page;
            var curPage = page.CurrentPage.BindingContext as AllActionPageViewModel;
           
            await nav.PushModalAsync<AllActionPage>();

            curPage.DoneFoward.Is(true);
            curPage.DoneFoward = false;
            page.Navigation.ModalStack.Count.Is(1);

            await nav.PushModalAsync<AllActionPage>(param);

            var prePage = page.Navigation.ModalStack[0].BindingContext as AllActionPageViewModel;

            page.Navigation.ModalStack.Count.Is(2);
            page.Navigation.NavigationStack.Count.Is(1);
            prePage.DoneFoward.IsTrue();
            curPage.DoneFoward.IsFalse();
            (page.Navigation.ModalStack[1].BindingContext as INavigationParameter).Value.Is(param);

        }

        [Test]
        public async Task TabChange(){
            var nav = new NavigationController(Container, IApplicationProvider);
            var list = new List<NavigationPage> {
                await nav.CreateNavigationPage<NavigationAlpha,AllActionPage>(),
                await nav.CreateNavigationPage<NavigationBeta,AllActionPage>(),
                await nav.CreateNavigationPage<NavigationGamma,AllActionPage>(),
            };
            var tab = nav.CreateTabbedPage<MainTabbedPage>(list);
            IApplicationProvider.MainPage = tab;

            var tab1 = list[0].CurrentPage.BindingContext as AllActionPageViewModel;
            var tab2 = list[1].CurrentPage.BindingContext as AllActionPageViewModel;
            var tab3 = list[2].CurrentPage.BindingContext as AllActionPageViewModel;

            tab1.DoneTabTo.IsTrue();
            tab1.TabChangeToParam.IsTrue();
            tab1.DoneTabFrom.IsFalse();
            tab2.DoneTabTo.IsFalse();
            tab2.DoneTabFrom.IsFalse();
            tab3.DoneTabTo.IsFalse();
            tab3.DoneTabFrom.IsFalse();

            nav.ChangeTab<NavigationAlpha>();

            tab.CurrentPage.GetType().Is(typeof(NavigationAlpha));
            tab1.DoneTabFrom.IsFalse();
            tab2.DoneTabTo.IsFalse();
            tab2.DoneTabFrom.IsFalse();
            tab3.DoneTabTo.IsFalse();
            tab3.DoneTabFrom.IsFalse();


            nav.ChangeTab<NavigationBeta>();

            tab.CurrentPage.GetType().Is(typeof(NavigationBeta));
            tab1.DoneTabFrom.IsTrue();
            tab2.DoneTabTo.IsTrue();
            tab2.DoneTabFrom.IsFalse();
            tab2.TabChangeToParam.IsTrue();
            tab3.DoneTabTo.IsFalse();
            tab3.DoneTabFrom.IsFalse();

            nav.ChangeTab<NavigationGamma>();

            tab.CurrentPage.GetType().Is(typeof(NavigationGamma));
            tab2.DoneTabFrom.IsTrue();
            tab3.DoneTabTo.IsTrue();
            tab3.DoneTabFrom.IsFalse();

            nav.ChangeTab<NavigationBeta>();

            tab.CurrentPage.GetType().Is(typeof(NavigationBeta));
            tab3.DoneTabFrom.IsTrue();
            tab2.TabChangeToParam.IsFalse();

            nav.ChangeTab<NavigationAlpha>();

            tab.CurrentPage.GetType().Is(typeof(NavigationAlpha));
            tab1.TabChangeToParam.IsFalse();


            tab1.DoneTabFrom = false;

            nav.ChangeTab<NavigationPage>();

            tab1.DoneTabFrom.IsFalse();
            tab.CurrentPage.GetType().Is(typeof(NavigationAlpha));
        }
    }
}

