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
using Prism.Unity.Navigation;

namespace MyFormsLibrary.Tests.Navigations
{
    [TestFixture]
    public class DefaultNavigationServiceFixture
    {
        IUnityContainer Container;
        IApplicationProvider App;
        ILoggerFacade Log;

        public DefaultNavigationServiceFixture() {

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
            Container.RegisterTypeForNavigation<NavigationPage>();
            Container.RegisterTypeForNavigation<MyMasterDetail>();


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
        public async Task AbsoluteDestroy() {
            var nav = Container.Resolve<INavigationServiceEx>();

            var naviPage = new NavigationTop();
            App.MainPage = naviPage;

            await nav.NavigateAsync("PageAlpha/PageBeta");


            var vmA = naviPage.Navigation.NavigationStack[0].BindingContext as PageAlphaViewModel;
            var vmB = naviPage.Navigation.NavigationStack[1].BindingContext as PageBetaViewModel;

            vmA.DoneNavigatingTo.IsTrue();
            vmB.DoneNavigatingTo.IsTrue();

            await nav.NavigateAsync("/NavigationTop/PageAlpha");

            vmA.DoneDestroy.IsTrue();
            vmB.DoneDestroy.IsTrue();
            vmA.DestroyCount.Is(1);
            vmB.DestroyCount.Is(1);
        }

        [Test]
        public async Task ComplexAbsoluteDestroy() {
            var nav = Container.Resolve<INavigationServiceEx>();

            var masterd = new MyMasterDetail();

            App.MainPage = masterd;

            await nav.NavigateAsync("NavigationTop/PageAlpha/PageBeta");

            ;

            var vmA = masterd.Detail.Navigation.NavigationStack[0].BindingContext as PageAlphaViewModel;
            var vmB = masterd.Detail.Navigation.NavigationStack[1].BindingContext as PageBetaViewModel;

            vmA.DoneNavigatingTo.IsTrue();
            vmB.DoneNavigatingTo.IsTrue();

            await nav.NavigateAsync("/MyMasterDetail/NavigationTop/PageAlpha");

            vmA.DoneDestroy.IsTrue();
            vmB.DoneDestroy.IsTrue();
            vmA.DestroyCount.Is(1);
            vmB.DestroyCount.Is(1);
        }

    }
}
