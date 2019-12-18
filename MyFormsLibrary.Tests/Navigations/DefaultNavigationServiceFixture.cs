using System.Threading.Tasks;
using Moq;
using MyFormsLibrary.Navigation;
using MyFormsLibrary.Tests.Mocks;
using MyFormsLibrary.Tests.Mocks.ViewModels;
using MyFormsLibrary.Tests.Mocks.Views;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Prism.Common;
using Prism.Logging;
using Prism.Mvvm;
using Prism.Navigation;
using Xamarin.Forms;
using Prism.Navigation.TabbedPages;

namespace MyFormsLibrary.Tests.Navigations
{
    [TestFixture]
    public class DefaultNavigationServiceFixture
    {
        AppMock App;

        public DefaultNavigationServiceFixture() 
        {
            Xamarin.Forms.Mocks.MockForms.Init();
            App = new AppMock(null);    
        }

        [Test]
        public async Task AbsoluteDestroy() {
            var nav = App.DefNavigationService;
            var naviPage = new NavigationTop();

            await nav.NavigateAsync("/NavigationTop/PageAlpha/PageBeta");

            var stack = App.MainPage.Navigation.NavigationStack;
            var vmA = stack[0].BindingContext as PageAlphaViewModel;
            var vmB = stack[1].BindingContext as PageBetaViewModel;

            vmA.DoneInitialize.IsTrue();
            vmB.DoneInitialize.IsTrue();

            await nav.NavigateAsync("/NavigationTop/PageAlpha");

            vmA.DoneDestroy.IsTrue();
            vmB.DoneDestroy.IsTrue();
            vmA.DestroyCount.Is(1);
            vmB.DestroyCount.Is(1);
        }

        [Test]
        public async Task ComplexAbsoluteDestroy() {
            var nav = App.DefNavigationService;

            await nav.NavigateAsync("/MyMasterDetail/NavigationTop/PageAlpha/PageBeta");

            var masterDetail = App.MainPage as MyMasterDetail;
            var master = masterDetail.Master as NextPage;
            var detail = masterDetail.Detail as NavigationTop;
            var stack = detail.Navigation.NavigationStack;
            var vmA = stack[0].BindingContext as PageAlphaViewModel;
            var vmB = stack[1].BindingContext as PageBetaViewModel;

            vmA.DoneInitialize.IsTrue();
            vmB.DoneInitialize.IsTrue();

            await nav.NavigateAsync("/MyMasterDetail/NavigationTop/PageAlpha");

            vmA.DoneDestroy.IsTrue();
            vmB.DoneDestroy.IsTrue();
            vmA.DestroyCount.Is(1);
            vmB.DestroyCount.Is(1);
            masterDetail.DoneDestroy.IsTrue();
            master.DoneDestroy.IsTrue();
            detail.DoneDestroy.IsTrue();
        }

        [Test]
        public async Task NavigationHasTabbedTest()
        {
            var nav = App.DefNavigationService;

            await nav.NavigateAsync("/NavigationTop/MainTabbedPage?createTab=PageAlpha&createTab=PageBeta");

            var tabs = App.MainPage.Navigation.NavigationStack[0] as TabbedPage;

            var vmA = tabs.Children[0].BindingContext as PageAlphaViewModel;
            var vmB = tabs.Children[1].BindingContext as PageBetaViewModel;

            tabs.CurrentPage.GetType().Is(typeof(PageAlpha));

            vmA.DoneInitialize.IsTrue(); //Navigatingは全てのTabで発火する
            vmA.DoneNavigatedTo.IsTrue(); //NavigatedはカレントTabのみで発火する
            vmA.DoneNavigatedFrom.IsFalse();
            vmA.DoneOnActive.IsTrue();
            vmA.DoneOnNonActive.IsTrue();

            vmB.DoneInitialize.IsTrue();  //全てのTabで発火するのでOK
            vmB.DoneNavigatedTo.IsFalse();  //カレントじゃないのでFalse
            vmB.DoneNavigatedFrom.IsFalse(); 
            vmB.DoneOnActive.IsFalse();
            vmB.DoneOnNonActive.IsFalse();

            vmA.AllClear();
            vmB.AllClear();

            // TODO: タブ移動をテストで実現できない
            ////PageBetaタブに移動
            //await nav.SelectTabAsync("PageBeta");

            //tabs.CurrentPage.GetType().Is(typeof(PageBeta));

            //vmA.DoneOnActive.IsFalse();
            //vmA.DoneOnNonActive.IsTrue();
            //vmA.DoneInitialize.IsTrue(); //タブ移動でも走ってしまう ゴミ仕様
            //vmA.DoneNavigatedFrom.IsTrue();
            //vmA.DoneNavigatedTo.IsFalse();

            //vmB.DoneOnActive.IsTrue();
            //vmB.DoneOnNonActive.IsFalse();
            //vmB.DoneInitialize.IsTrue(); //タブ移動でも走る
            //vmB.DoneNavigatedFrom.IsFalse();
            //vmB.DoneNavigatedTo.IsTrue();
        }

        [Test]
        public async Task TabbedHasNavigationTest()
        {
            var nav = App.DefNavigationService;

            await nav.NavigateAsync("/MainTabbedPage?createTab=NavigationAlpha|PageAlpha&createTab=NavigationBeta|PageBeta");

            var tabs = App.MainPage as TabbedPage;

            tabs.CurrentPage.GetType().Is(typeof(NavigationAlpha));

            var vmA = (tabs.Children[0] as NavigationPage).CurrentPage.BindingContext as PageAlphaViewModel;
            var vmB = (tabs.Children[1] as NavigationPage).CurrentPage.BindingContext as PageBetaViewModel;

            vmA.DoneInitialize.IsTrue(); //Navigatingは全てのTabで発火する
            vmA.DoneNavigatedTo.IsTrue(); //NavigatedはカレントTabのみで発火する
            vmA.DoneNavigatedFrom.IsFalse();
            vmA.DoneOnActive.IsTrue();
            vmA.DoneOnNonActive.IsTrue();

            vmB.DoneInitialize.IsTrue();  //全てのTabで発火するのでOK
            vmB.DoneNavigatedTo.IsFalse();  //カレントじゃないのでFalse
            vmB.DoneNavigatedFrom.IsFalse();
            vmB.DoneOnActive.IsTrue(); //NavigationのCurrentPageの変更で反応してしまうのでTrue ゴミ仕様
            vmB.DoneOnNonActive.IsFalse();
        }

    }
}
