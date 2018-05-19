using System;
using Prism.Ioc;
using Prism.Unity;
using Prism;
using MyFormsLibrary.Navigation;
using Prism.Mvvm;
using MyFormsLibrary.Tests.Mocks.Views;
using Prism.Navigation;

namespace MyFormsLibrary.Tests.Mocks
{
    public class AppMock:PrismApplication
    {
        public MyPageNavigationService MyPageNavigationService { get; set; }
        public INavigationServiceEx NavigationServiceEx { get; set; }
        public INavigationService DefNavigationService { get; set; }

        public AppMock(IPlatformInitializer initializer=null):base(initializer) 
        {
            DefNavigationService = NavigationService;
            var myNavi = Container.Resolve<INavigationServiceEx>(MyPageNavigationService.MyNavigationServiceName);
            MyPageNavigationService = myNavi as MyPageNavigationService;
            NavigationServiceEx = myNavi;
        }

        protected override void OnInitialized() {
            
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry) {
            containerRegistry.RegisterForNavigation<ContentPageNoAction>();
            containerRegistry.RegisterForNavigation<ContentPageAllAction>();
            containerRegistry.RegisterForNavigation<PageAlpha>();
            containerRegistry.RegisterForNavigation<PageBeta>();
            containerRegistry.RegisterForNavigation<MainTabbedPage>();
            containerRegistry.RegisterForNavigation<NavigationAlpha>();
            containerRegistry.RegisterForNavigation<NavigationBeta>();
            containerRegistry.RegisterForNavigation<NavigationGamma>();
            containerRegistry.RegisterForNavigation<NavigationTop>();
            containerRegistry.RegisterForNavigation<NextPage>();
            containerRegistry.RegisterForNavigation<MyMasterDetail>();
        }

        protected override void RegisterRequiredTypes(IContainerRegistry containerRegistry) {
            base.RegisterRequiredTypes(containerRegistry);

            containerRegistry.Register<INavigationServiceEx, MyPageNavigationService>(MyPageNavigationService.MyNavigationServiceName);
        }

        protected override void ConfigureViewModelLocator() {
            ViewModelLocationProvider.SetDefaultViewModelFactory((view, type) => {
                return Container.ResolveViewModelForView(view, type);
            });
        }
    }
}
