using System.Threading.Tasks;
using Xamarin.Forms;

namespace MyFormsLibrary.Navigation
{
	public interface INavigationController
	{
		void ChangeTab<TNavigationPage>();

		Task NavigateAsync<TContentPage>(object param = null,bool? useModalNavigation = default(bool?), bool animated = true)
			where TContentPage : ContentPage;
        
        Task NavigateAsync<TNavigationPage, TContentPage>
            (object param = null, bool? useModalNavigation = default(bool?), bool animated = true)
            where TNavigationPage : NavigationPage where TContentPage : ContentPage;
        
		Task GoBackAsync(bool animated = true);
	}
}

