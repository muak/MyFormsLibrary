using System;
using Prism;
using Prism.Behaviors;
using Prism.Common;
using Xamarin.Forms;

namespace MyFormsLibrary.Navigation
{
    /// <summary>
    /// TabbedPage/NavigationPage/ContentPageの構成で
    /// ContentPageから次ページへ遷移したときにActiveAwareの状態を引き継ぎ
    /// 次ページから戻ってきた時に元のページをActive化するためBehavior
    /// 
    /// 本家
    /// https://github.com/PrismLibrary/Prism/blob/7.0.0-Forms-SR1/Source/Xamarin/Prism.Forms/Behaviors/NavigationPageActiveAwareBehavior.cs
    /// こちらは使わず、ページ遷移時に遷移元ページを非アクティブにするロジックだけを使う
    /// </summary>
    public class TabbedPageHasNavigationPageActionBehavior: BehaviorBase<NavigationPage>
    {
        protected override void OnAttachedTo(NavigationPage bindable) {
            base.OnAttachedTo(bindable);
            bindable.Pushed += Bindable_Pushed;
            bindable.Popped += Bindable_Popped;
            bindable.PropertyChanging += NavigationPage_PropertyChanging;
        }

        protected override void OnDetachingFrom(NavigationPage bindable) {
            base.OnDetachingFrom(bindable);
            bindable.Pushed -= Bindable_Pushed;
            bindable.Popped -= Bindable_Popped;
            bindable.PropertyChanging -= NavigationPage_PropertyChanging;
        }

        void Bindable_Pushed(object sender, NavigationEventArgs e) {
            SetIsActive(AssociatedObject.CurrentPage,true);
        }

        void Bindable_Popped(object sender, NavigationEventArgs e) {
            SetIsActive(AssociatedObject.CurrentPage, true);
        }

        void NavigationPage_PropertyChanging(object sender, PropertyChangingEventArgs e) {
            if (e.PropertyName == "CurrentPage") {
                SetIsActive(AssociatedObject.CurrentPage, false);
            }
        }


        void SetIsActive(object view, bool isActive) {
            PageUtilities.InvokeViewAndViewModelAction<IActiveAware>(view, activeAware => activeAware.IsActive = isActive);
        }
    }
}
