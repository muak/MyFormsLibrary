using System;
using MyFormsLibrary.Behaviors;
using Prism.Common;
using Prism.Navigation;
using Xamarin.Forms;
namespace MyFormsLibrary.Navigation
{
    public class NavigationPageOverTabbedPageCurrentBehavior:BehaviorBase<NavigationPage>
    {
        protected override void OnAttachedTo(NavigationPage bindable)
        {
            bindable.Popped += NavigationPage_Popped;
            base.OnAttachedTo(bindable);
        }

        protected override void OnDetachingFrom(NavigationPage bindable)
        {
            bindable.Popped -= NavigationPage_Popped;
            base.OnDetachingFrom(bindable);
        }

        private void NavigationPage_Popped(object sender, NavigationEventArgs e)
        {
            var parameters = MyPageNavigationService.ParameterProxy;
            MyPageNavigationService.ParameterProxy = null;

            var tabbed = AssociatedObject.CurrentPage as TabbedPage;
            if (tabbed == null) {
                return;
            }

            if (parameters == null) {
                parameters = new NavigationParameters();
            }
                
            PageUtilities.OnNavigatingTo(tabbed.CurrentPage, parameters);
            PageUtilities.OnNavigatedTo(tabbed.CurrentPage, parameters);

        }
    }
}
