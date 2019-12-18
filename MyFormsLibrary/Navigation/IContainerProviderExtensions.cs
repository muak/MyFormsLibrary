using System;
using MyFormsLibrary.Navigation;
using Prism.Common;
using Unity.Resolution;
using Xamarin.Forms;
using Prism.Unity;
using Unity;

namespace Prism.Ioc
{
    public static class IContainerProviderExtensions
    {
        public static INavigationServiceEx CreateNavigationService(this IContainerProvider container, Page page) {
            var navigationService = container.Resolve<INavigationServiceEx>(MyPageNavigationService.MyNavigationServiceName);
            ((IPageAware)navigationService).Page = page;
            return navigationService;
        }

        public static object ResolveViewModelForView(this IContainerProvider container,object view, Type viewModelType) {
            ParameterOverride paramOverride = null;

            if (view is Xamarin.Forms.Page page) {
                paramOverride = new ParameterOverride(PrismApplicationBase.NavigationServiceParameterName, container.CreateNavigationService(page));
            }

            return container.GetContainer().Resolve(viewModelType,paramOverride);
        }
    }
}