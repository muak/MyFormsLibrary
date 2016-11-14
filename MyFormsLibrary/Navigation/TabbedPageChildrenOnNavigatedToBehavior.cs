using System;
using MyFormsLibrary.Behaviors;
using Prism.Common;
using Prism.Navigation;
using Xamarin.Forms;

namespace MyFormsLibrary.Navigation
{
    public class TabbedPageCurrentPageOnNavigatedToBehavior:BehaviorBase<TabbedPage>
    {
        protected override void OnAttachedTo(TabbedPage bindable) {
            base.OnAttachedTo(bindable);
            bindable.Appearing += Bindable_Appearing;
        }

        protected override void OnDetachingFrom(TabbedPage bindable) {
            base.OnDetachingFrom(bindable);
            bindable.Appearing -= Bindable_Appearing;
        }

        void Bindable_Appearing(object sender, EventArgs e) {
            var tabbedPage = (sender as TabbedPage);

            var param = new NavigationParameters();

            PageUtilities.OnNavigatingTo(tabbedPage.CurrentPage, param);
            PageUtilities.OnNavigatedTo(tabbedPage.CurrentPage,param);
        }
    }
}
