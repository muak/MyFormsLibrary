using System.Threading.Tasks;
using Xamarin.Forms;

namespace MyFormsLibrary.Navigation
{
    public interface INavigationController
    {
        bool ChangeTab<TPage>() where TPage:Page;

        Task PushAsync<TContentPage>(object param = null, bool animated = true)
            where TContentPage : ContentPage;

        Task PushAsync<TNavigationPage, TContentPage>(object param = null, bool animated = true)
            where TNavigationPage : NavigationPage where TContentPage : ContentPage;

        Task PushModalAsync<TContentPage>(object param = null, bool animated = true)
            where TContentPage : ContentPage;


        Task GoBackAsync(bool animated = true);
    }
}

