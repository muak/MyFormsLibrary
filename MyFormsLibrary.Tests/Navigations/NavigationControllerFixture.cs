using System;
using NUnit.Framework;
using MyFormsLibrary.Navigation;
using MyFormsLibrary.Tests.Mocks.Views;
using Microsoft.Practices.Unity;
using System.ComponentModel;
using Prism.Common;
using Xamarin.Forms;
using MyFormsLibrary.Tests.Mocks;
using System.Threading.Tasks;
using System.Runtime.InteropServices.ComTypes;
using System.Collections;
using System.Collections.Generic;
using Prism.Navigation;
using Prism.Unity;
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
            var cnt = page.Navigation.NavigationStack.Count;
            await nav.NavigateAsync<GetParamPage>(param);

            page.CurrentPage.GetType().Is(typeof(GetParamPage));
            page.Navigation.NavigationStack.Count.Is(cnt+1);
            page.Navigation.NavigationStack[cnt-1].IsSameReferenceAs(prePage);
            page.Navigation.ModalStack.Count.Is(0);

            (page.CurrentPage.BindingContext as INavigationParameter).Value.Is(param);

            await nav.GoBackAsync();

            page.CurrentPage.GetType().Is(typeof(ContentPage));
            page.Navigation.NavigationStack.Count.Is(cnt);
            page.Navigation.NavigationStack.Last().IsSameReferenceAs(prePage);
            page.Navigation.ModalStack.Count.Is(0);
        }

        public async Task NavigateAsync_AllAction() {
            var nav = new NavigationController(Container, IApplicationProvider);
            var page = await nav.CreateNavigationPage<NavigationPage, ContentPage>();
            IApplicationProvider.MainPage = page;
        }
    }
}

