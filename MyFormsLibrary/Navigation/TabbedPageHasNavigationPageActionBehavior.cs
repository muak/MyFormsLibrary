using System;
using Prism;
using Prism.Behaviors;
using Prism.Common;
using Xamarin.Forms;

namespace MyFormsLibrary.Navigation
{
    /// <summary>
    /// TabbedPage/NavigationPage/ContentPageの構成で
    /// ContentPageから次ページへ遷移したときにActiveAwareの状態を引き継ぐためのBehavior
    /// </summary>
    public class TabbedPageHasNavigationPageActionBehavior: BehaviorBase<NavigationPage>
    {
        protected override void OnAttachedTo(NavigationPage bindable) {
            base.OnAttachedTo(bindable);
            bindable.Pushed += Bindable_Pushed;
        }

        protected override void OnDetachingFrom(NavigationPage bindable) {
            base.OnDetachingFrom(bindable);
            bindable.Pushed -= Bindable_Pushed;
        }

        void Bindable_Pushed(object sender, NavigationEventArgs e) {
            SetIsActive(AssociatedObject.CurrentPage,true);
        }

        void SetIsActive(object view, bool isActive) {
            PageUtilities.InvokeViewAndViewModelAction<IActiveAware>(view, activeAware => activeAware.IsActive = isActive);
        }
    }
}
