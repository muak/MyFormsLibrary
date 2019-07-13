using System;
using System.Threading.Tasks;
using Prism.Navigation;
using Xamarin.Forms;

namespace MyFormsLibrary.Navigation
{
    public interface INavigationServiceEx:INavigationService
    {
        Task Navigate<T>(ParametersBase parameters = null, bool animated = true) where T : ContentPage;
        Task NavigateModal<T>(ParametersBase parameters = null, bool animated = true) where T : ContentPage;
        Task NavigateModal<Tnavi, Tpage>(ParametersBase parameters = null, bool animated = true) where Tnavi : NavigationPage where Tpage : ContentPage;

        Task AnywhereNavigate<T>(NavigationParameters parameters = null, bool animated = true) where T : ContentPage;

        Task<bool> GoBackAsync(NavigationParameters parameters, bool? useModalNavigation, bool animated = true);
        Task NavigateAsync(string name, NavigationParameters parameters, bool? useModalNavigation, bool animated = true);

        Task GoBackModalAsync(bool animated = true);
        bool ChangeTab<T>() where T : Page;
    }
}
