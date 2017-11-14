using System;
using System.Threading.Tasks;
using Prism.Navigation;
using Xamarin.Forms;

namespace MyFormsLibrary.Navigation
{
    public interface INavigationServiceEx:INavigationService
    {
        Task NavigateAsync<T>(ParametersBase parameters = null, bool animated = true) where T : ContentPage;
        Task NavigateModalAsync<T>(ParametersBase parameters = null, bool animated = true) where T : ContentPage;

        Task NavigateAsync<T>(object myParam = null, bool animated = true, NavigationParameters originalParam = null) where T : ContentPage;
        Task NavigateModalAsync<T>(object myParam = null, bool animated = true, NavigationParameters originalParam = null) where T : ContentPage;
        Task NavigateModalAsync<Tnavi, Tpage>(object myParam = null, bool animated = true, NavigationParameters originalParam = null) where Tnavi : NavigationPage where Tpage : ContentPage;
        Task GoBackModalAsync(bool animated = true);
        bool ChangeTab<T>() where T : Page;
    }
}
