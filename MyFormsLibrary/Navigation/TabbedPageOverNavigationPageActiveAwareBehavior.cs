using System;
using Prism;
using Prism.Behaviors;
using Prism.Common;
using Xamarin.Forms;
using System.Linq;
namespace MyFormsLibrary.Navigation
{
    /// <summary>
    /// TabbePage/NavigationPage/ContentPageの構成の時にTabChange時にContentPage側で
    /// ActiveAwareが発動しない問題のFixBehavior
    /// </summary>
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
                SetIsActive(_lastNavigationCurrent,true);
                return;
            }

            SetIsActive(_lastNavigationCurrent,false);

            _lastNavigationCurrent = (AssociatedObject.CurrentPage as NavigationPage)?.CurrentPage;

            SetIsActive(_lastNavigationCurrent,true);
        }

        void SetIsActive(Page view, bool isActive) {
            PageUtilities.InvokeViewAndViewModelAction<IActiveAware>(view, activeAware => activeAware.IsActive = isActive);
        }
    }
}
