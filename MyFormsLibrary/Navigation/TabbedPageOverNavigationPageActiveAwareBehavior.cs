using System;
using Prism;
using Prism.Behaviors;
using Prism.Common;
using Xamarin.Forms;
namespace MyFormsLibrary.Navigation
{
    public class TabbedPageOverNavigationPageActiveAwareBehavior:BehaviorBase<TabbedPage>
    {
        private Page _lastNavigationCurrent;

        protected override void OnAttachedTo(TabbedPage bindable) {
            base.OnAttachedTo(bindable);
            bindable.CurrentPageChanged += CurrentPageChangedHandlerForNavigationPage;
        }

        protected override void OnDetachingFrom(TabbedPage bindable) {
            base.OnDetachingFrom(bindable);
            bindable.CurrentPageChanged -= CurrentPageChangedHandlerForNavigationPage; ;
        }

        void CurrentPageChangedHandlerForNavigationPage(object sender, EventArgs e) {
           
            if (_lastNavigationCurrent == null) {
                _lastNavigationCurrent = (AssociatedObject.CurrentPage as NavigationPage)?.CurrentPage;
            }

            SetIsActive(_lastNavigationCurrent,false);

            _lastNavigationCurrent = (AssociatedObject.CurrentPage as NavigationPage)?.CurrentPage;

            SetIsActive(_lastNavigationCurrent,true);
        }

        void SetIsActive(object view, bool isActive) {
            PageUtilities.InvokeViewAndViewModelAction<IActiveAware>(_lastNavigationCurrent, activeAware => activeAware.IsActive = isActive);
        }
    }
}
