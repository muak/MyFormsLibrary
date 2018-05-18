using System;
using Prism;
using Prism.Behaviors;
using Prism.Common;
using Xamarin.Forms;
using System.Linq;
namespace MyFormsLibrary.Navigation
{
    /// <summary>
    /// TabbePage/NavigationPage/ContentPageの構成の時にContentPage側で
    /// ActiveAwareが発動しない問題のFixBehavior
    /// 7.0でスタックの最後のページのみ発火するようになったが、それ以外は同期しないので
    /// 最終を除いた残りのスタックも処理するようにする
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
            foreach (var p in view.Navigation.NavigationStack.Reverse().Skip(1)) {
                PageUtilities.InvokeViewAndViewModelAction<IActiveAware>(p, activeAware => activeAware.IsActive = isActive);
            }

        }
    }
}
