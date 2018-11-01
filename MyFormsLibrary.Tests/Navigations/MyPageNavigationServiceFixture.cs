using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyFormsLibrary.Navigation;
using MyFormsLibrary.Tests.Mocks;
using MyFormsLibrary.Tests.Mocks.ViewModels;
using MyFormsLibrary.Tests.Mocks.Views;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Prism.Behaviors;
using Prism.Navigation;
using Xamarin.Forms;

namespace MyFormsLibrary.Tests.Navigations
{
    [TestFixture]
    public class MyPageNavigationServiceFixture
    {
        AppMock App;

        public MyPageNavigationServiceFixture() 
        {
            Xamarin.Forms.Mocks.MockForms.Init();
            App = new AppMock(null);
        }

        [Test]
        public void CreateContentPage() {
            var page = App.MyPageNavigationService.CreateContentPage("ContentPageNoAction");

            page.GetType().Is(typeof(ContentPageNoAction));
            page.BindingContext.GetType().Is(typeof(ContentPageNoActionViewModel));
        }

        static TestCaseData[] Src_CreateNavigationPage = new[] {
            new TestCaseData(null),new TestCaseData(new NavigationParameters{ {"Key","Value" }} ) 
        };

        [TestCaseSource("Src_CreateNavigationPage")]
        public void CreateNavigationPage(NavigationParameters param) {
            var nav = App.MyPageNavigationService;

            var page = nav.CreateNavigationPage("NavigationAlpha", nameof(PageAlpha),param);

            //NavigationPageActiveAwareBehaviorの解除に成功している
            page.Behaviors.FirstOrDefault(x => x.GetType() == typeof(NavigationPageActiveAwareBehavior)).IsNull();
            page.GetType().Is(typeof(NavigationAlpha));
            var realPage = page as NavigationAlpha;

            realPage.CurrentPage.GetType().Is(typeof(PageAlpha));
            realPage.CurrentPage.BindingContext.GetType().Is(typeof(PageAlphaViewModel));

            var vm = realPage.CurrentPage.BindingContext as PageAlphaViewModel;

            vm.DoneNavigatingTo.IsTrue();
            vm.DoneNavigatedTo.IsTrue();
            vm.DoneNavigatedFrom.IsFalse();
            // Currentにセットされた場合も発火するようになった(7.0〜)、が無理やり剥がして無効にしている
            vm.DoneOnNonActive.IsFalse();
            vm.DoneOnActive.IsFalse(); 

            if (param == null) {
                vm.Param.GetNavigationMode().Is(NavigationMode.New);
                vm.Param.Count.Is(0); //組み込み要素はノーカウント
            }

            else {
                vm.Param.GetNavigationMode().Is(NavigationMode.New);
                vm.Param.ContainsKey("Key").IsTrue();
                vm.Param["Key"].Is("Value");
                vm.Param.Count.Is(1);
            }
        }


        [Test]
        public void CreateMainPageNavigationHasTabbed() {
            var nav = App.MyPageNavigationService;
           
            var page = nav.CreateMainPageNavigationHasTabbed(
                "NavigationTop", "MainTabbedPage",
               new List<ContentPage> {
                    nav.CreateContentPage(nameof(PageAlpha)),
                    nav.CreateContentPage(nameof(PageBeta))
                },
                new List<NavigationParameters>{
                    new NavigationParameters{ {"Key","Value" }}
                }
            );

            (page.CurrentPage as IPageController).SendAppearing();

            page.GetType().Is(typeof(NavigationTop));

            var tabbedPage = (page as NavigationPage).CurrentPage;

            tabbedPage.GetType().Is(typeof(MainTabbedPage));

            var realTab = tabbedPage as MainTabbedPage;
            realTab.Children.Count.Is(2);

            var pageA = realTab.Children[0];
            var pageB = realTab.Children[1];

            pageA.GetType().Is(typeof(PageAlpha));
            pageB.GetType().Is(typeof(PageBeta));

            var vmA = pageA.BindingContext as PageAlphaViewModel;
            var vmB = pageB.BindingContext as PageBetaViewModel;

            //どちらもNavigatingTo NavigatedToが発火する
            vmA.DoneNavigatingTo.IsTrue();
            vmA.DoneNavigatedTo.IsTrue();
            vmA.DoneNavigatedFrom.IsFalse();
            vmA.DoneOnActive.IsTrue();
            vmA.DoneOnNonActive.IsFalse();
            vmA.IsActive.IsTrue();
            vmA.Param.GetNavigationMode().Is(NavigationMode.New);
            vmA.Param["Key"].Is("Value");
            vmA.Param.Count.Is(1);

            vmB.DoneNavigatingTo.IsTrue();
            vmB.DoneNavigatedTo.IsTrue();
            vmB.DoneNavigatedFrom.IsFalse();
            vmB.DoneOnActive.IsFalse();
            vmB.DoneOnNonActive.IsFalse();
            vmB.IsActive.IsFalse();
            vmB.Param.GetNavigationMode().Is(NavigationMode.New);
            vmB.Param.Count.Is(0);

        }

        [Test]
        public async Task NavigationHasTabbedGoNextGoBack() {
            var nav = App.MyPageNavigationService;

            var pageA = nav.CreateContentPage(nameof(PageAlpha));
            var pageB = nav.CreateContentPage(nameof(PageBeta));

            var page = nav.CreateMainPageNavigationHasTabbed(
                "NavigationTop", "MainTabbedPage",
               new List<ContentPage> {
                    pageA,
                    pageB
                },
                new List<NavigationParameters>{
                    new NavigationParameters{ {"Key1","Value1" }},
                    new NavigationParameters{ {"Key2","Value2" }}
                }
            );
            App.MainPage = page;

            var tabbed = page.CurrentPage as IPageController;
            tabbed.SendAppearing();

            var vmA = pageA.BindingContext as PageAlphaViewModel;
            var vmB = pageB.BindingContext as PageBetaViewModel;

            vmA.AllClear();
            vmB.AllClear();

            var curNavi = vmA.NavigationService;
            await curNavi.NavigateAsync(nameof(NextPage));

            tabbed.SendDisappearing();

            vmA.DoneNavigatingTo.IsFalse();
            vmA.DoneNavigatedTo.IsFalse();
            vmA.DoneNavigatedFrom.IsTrue(); //カレントページのみ発火
            vmA.IsActive.IsFalse();
            vmA.DoneOnActive.IsFalse();
            vmA.DoneOnNonActive.IsTrue(); //ページ遷移で隠れても非アクティブにする

            vmB.DoneNavigatingTo.IsFalse();
            vmB.DoneNavigatedTo.IsFalse();
            vmB.DoneNavigatedFrom.IsFalse(); //カレントページでないので発火しない
            vmB.DoneOnActive.IsFalse();
            vmB.DoneOnNonActive.IsFalse();
            vmB.IsActive.IsFalse();

            var nextVM = page.CurrentPage.BindingContext as NextPageViewModel;

            nextVM.DoneNavigatingTo.IsTrue();
            nextVM.DoneNavigatedTo.IsTrue();
            nextVM.DoneNavigatedFrom.IsFalse();
            nextVM.DoneOnActive.IsTrue();
            nextVM.DoneOnNonActive.IsFalse();

            vmA.AllClear();
            vmB.AllClear();
            nextVM.AllClear();

            var ret = await nextVM.NavigationService.GoBackAsync();
            ret.IsTrue();
            tabbed.SendAppearing();

            vmA.DoneNavigatingTo.IsTrue(); //戻った時も呼ばれるのは仕様
            vmA.DoneNavigatedTo.IsTrue();
            vmA.DoneNavigatedFrom.IsFalse();
            vmA.DoneOnActive.IsTrue();  //戻ったタイミングでカレントタブはアクティブが発火
            vmA.DoneOnNonActive.IsFalse();
            vmA.IsActive.IsTrue();

            vmB.DoneNavigatingTo.IsFalse(); //非表示タブは発火しない
            vmB.DoneNavigatedTo.IsFalse();
            vmB.DoneNavigatedFrom.IsFalse();
            vmB.DoneOnActive.IsFalse();
            vmB.DoneOnNonActive.IsFalse();
            vmB.IsActive.IsFalse();

            nextVM.DoneNavigatingTo.IsFalse();
            nextVM.DoneNavigatedTo.IsFalse();
            nextVM.DoneNavigatedFrom.IsTrue();
            nextVM.DoneOnActive.IsFalse();
            nextVM.DoneOnNonActive.IsTrue();

        }

        [Test]
        public void CreateMainPageTabbedHasNavigation() {
            var nav = App.MyPageNavigationService;

            var naviA = nav.CreateNavigationPage(nameof(NavigationAlpha), nameof(PageAlpha));
            var naviB = nav.CreateNavigationPage(nameof(NavigationBeta), nameof(PageBeta));

            var tabbed = nav.CreateMainPageTabbedHasNavigation(nameof(MainTabbedPage),new List<NavigationPage>{ 
                naviA,naviB});


            naviA.GetType().Is(typeof(NavigationAlpha));
            naviB.GetType().Is(typeof(NavigationBeta));
            naviA.CurrentPage.GetType().Is(typeof(PageAlpha));
            naviB.CurrentPage.GetType().Is(typeof(PageBeta));
            tabbed.GetType().Is(typeof(MainTabbedPage));
            tabbed.Children.Count.Is(2);

            (tabbed as IPageController).SendAppearing();

            var vmA = naviA.CurrentPage.BindingContext as PageAlphaViewModel;
            var vmB = naviB.CurrentPage.BindingContext as PageBetaViewModel;

            vmA.DoneNavigatingTo.IsTrue();
            vmA.DoneNavigatedTo.IsTrue();
            vmA.DoneNavigatedFrom.IsFalse();
            vmA.DoneOnActive.IsTrue();
            vmA.DoneOnNonActive.IsFalse();
            vmA.IsActive.IsTrue();

            vmB.DoneNavigatingTo.IsTrue();
            vmB.DoneNavigatedTo.IsTrue();
            vmB.DoneNavigatedFrom.IsFalse();
            vmB.DoneOnActive.IsFalse();
            vmB.DoneOnNonActive.IsFalse();
            vmB.IsActive.IsFalse();

        }

        /// <summary>
        /// Tabbed->Navi->ConentPageパターン
        /// 次ページ遷移で次ページのIsActiveがtrueになる
        /// 遷移しても遷移元のIsActiveはtrueのまま
        /// あくまでも現在のタブの状態を入れるようにする
        /// 戻る時は次ページのActiveは何も変更しない
        /// 遷移元に戻ってきた場合はActive化する
        /// </summary>
        [Test]
        public async Task TabbedHasNavigationGoNextGoBack() {
            var nav = App.MyPageNavigationService;

            var naviA = nav.CreateNavigationPage(nameof(NavigationAlpha), nameof(PageAlpha));
            var naviB = nav.CreateNavigationPage(nameof(NavigationBeta), nameof(PageBeta));

            var tabbed = nav.CreateMainPageTabbedHasNavigation(nameof(MainTabbedPage), new List<NavigationPage>{
                naviA,naviB});

            App.MainPage = tabbed;

            (tabbed as IPageController).SendAppearing();

            var vmA = naviA.CurrentPage.BindingContext as PageAlphaViewModel;
            var vmB = naviB.CurrentPage.BindingContext as PageBetaViewModel;

            vmA.AllClear();
            vmB.AllClear();

            var curNavi = vmA.NavigationService;
            await curNavi.NavigateAsync(nameof(NextPage));

            vmA.DoneNavigatingTo.IsFalse();
            vmA.DoneNavigatedTo.IsFalse();
            vmA.DoneNavigatedFrom.IsTrue();
            vmA.IsActive.IsFalse();
            vmA.DoneOnActive.IsFalse();
            vmA.DoneOnNonActive.IsTrue();
            vmA.OnNonActiveCount.Is(1);

            vmB.DoneNavigatingTo.IsFalse();
            vmB.DoneNavigatedTo.IsFalse();
            vmB.DoneNavigatedFrom.IsFalse();
            vmB.DoneOnActive.IsFalse();
            vmB.DoneOnNonActive.IsFalse();
            vmB.IsActive.IsFalse();

            var nextVM = naviA.CurrentPage.BindingContext as NextPageViewModel;

            nextVM.DoneNavigatingTo.IsTrue();
            nextVM.DoneNavigatedTo.IsTrue();
            nextVM.DoneNavigatedFrom.IsFalse();
            nextVM.IsActive.IsTrue();
            nextVM.DoneOnActive.IsTrue();
            nextVM.DoneOnNonActive.IsFalse();

            vmA.AllClear();
            vmB.AllClear();
            nextVM.AllClear();

            var ret = await nextVM.NavigationService.GoBackAsync();
            ret.IsTrue();

            vmA.DoneNavigatingTo.IsTrue();
            vmA.DoneNavigatedTo.IsTrue();
            vmA.DoneNavigatedFrom.IsFalse();
            vmA.DoneOnActive.IsTrue();
            vmA.DoneOnNonActive.IsFalse();
            vmA.IsActive.IsTrue();

            vmB.DoneNavigatingTo.IsFalse();
            vmB.DoneNavigatedTo.IsFalse();
            vmB.DoneNavigatedFrom.IsFalse();
            vmB.DoneOnActive.IsFalse();
            vmB.DoneOnNonActive.IsFalse();
            vmB.IsActive.IsFalse();

            nextVM.DoneNavigatingTo.IsFalse();
            nextVM.DoneNavigatedTo.IsFalse();
            nextVM.DoneNavigatedFrom.IsTrue();
            nextVM.IsActive.IsFalse();
            nextVM.DoneOnActive.IsFalse();
            nextVM.DoneOnNonActive.IsTrue();
            nextVM.OnNonActiveCount.Is(1);
        }

        [Test]
        public async Task TabbedHasNavigationActiveTest() {
            var nav = App.MyPageNavigationService;

            var naviA = nav.CreateNavigationPage(nameof(NavigationAlpha), nameof(PageAlpha));
            var naviB = nav.CreateNavigationPage(nameof(NavigationBeta), nameof(PageBeta));

            var tabbed = nav.CreateMainPageTabbedHasNavigation(nameof(MainTabbedPage), new List<NavigationPage>{
                naviA,naviB});

            App.MainPage = tabbed;

            (tabbed as IPageController).SendAppearing();

            var vmA = naviA.CurrentPage.BindingContext as PageAlphaViewModel;
            var vmB = naviB.CurrentPage.BindingContext as PageBetaViewModel;

            vmA.AllClear();
            vmB.AllClear();

            var curNavi = vmA.NavigationService;
            await curNavi.NavigateAsync(nameof(NextPage));

            vmA.DoneNavigatingTo.IsFalse();
            vmA.DoneNavigatedTo.IsFalse();
            vmA.DoneNavigatedFrom.IsTrue();
            vmA.IsActive.IsFalse();
            vmA.DoneOnActive.IsFalse();
            vmA.DoneOnNonActive.IsTrue();
            vmA.OnNonActiveCount.Is(1);


            vmB.DoneNavigatingTo.IsFalse();
            vmB.DoneNavigatedTo.IsFalse();
            vmB.DoneNavigatedFrom.IsFalse();
            vmB.DoneOnActive.IsFalse();
            vmB.DoneOnNonActive.IsFalse();
            vmB.IsActive.IsFalse();

            var nextVM = naviA.CurrentPage.BindingContext as NextPageViewModel;

            nextVM.DoneNavigatingTo.IsTrue();
            nextVM.DoneNavigatedTo.IsTrue();
            nextVM.DoneNavigatedFrom.IsFalse();
            nextVM.IsActive.IsTrue();
            nextVM.DoneOnActive.IsTrue();
            nextVM.DoneOnNonActive.IsFalse();

            vmA.AllClear();
            vmB.AllClear();
            nextVM.AllClear();

            //タブBに移動
            nextVM.NavigationService.ChangeTab<PageBeta>();

            //スタック奥のページは変化なし
            vmA.IsActive.IsFalse();
            vmA.DoneOnActive.IsFalse();
            vmA.DoneOnNonActive.IsFalse();
            //スタック手前のページは非アクティブが発火
            nextVM.IsActive.IsFalse();
            nextVM.DoneOnActive.IsFalse();
            nextVM.DoneOnNonActive.IsTrue();
            nextVM.OnNonActiveCount.Is(1);
            //スタック手前のページはアクティブが発火
            vmB.IsActive.IsTrue();
            vmB.DoneOnActive.IsTrue();
            vmB.DoneOnNonActive.IsFalse();
            vmB.OnActiveCount.Is(1);

            vmA.AllClear();
            vmB.AllClear();
            nextVM.AllClear();

            //タブAに戻る
            vmB.NavigationService.ChangeTab<NextPage>();

            //スタック奥のページは変化なし
            vmA.IsActive.IsFalse();
            vmA.DoneOnActive.IsFalse();
            vmA.DoneOnNonActive.IsFalse();
            //スタック手前のページはアクティブが発火
            nextVM.IsActive.IsTrue();
            nextVM.DoneOnActive.IsTrue();
            nextVM.DoneOnNonActive.IsFalse();
            nextVM.OnActiveCount.Is(1);
            //スタック手前のページは非アクティブが発火
            vmB.IsActive.IsFalse();
            vmB.DoneOnActive.IsFalse();
            vmB.DoneOnNonActive.IsTrue();
            vmB.OnNonActiveCount.Is(1);

            vmA.AllClear();
            vmB.AllClear();
            nextVM.AllClear();

            //戻る
            var ret = await nextVM.NavigationService.GoBackAsync();
            ret.IsTrue();

            vmA.IsActive.IsTrue();
            vmA.DoneOnActive.IsTrue();
            vmA.DoneOnNonActive.IsFalse();
            vmA.OnActiveCount.Is(1);

            //Bは無関係なので変化なし
            vmB.IsActive.IsFalse();
            vmB.DoneOnActive.IsFalse();
            vmB.DoneOnNonActive.IsFalse();

            vmA.AllClear();
            vmB.AllClear();

            //await vmA.NavigationService.NavigateModal<NextPage>();

            //nextVM = naviA.Navigation.ModalStack.Last().BindingContext as NextPageViewModel;

            //vmA.IsActive.IsTrue();
            //vmA.DoneOnActive.IsFalse();
            //vmA.DoneOnNonActive.IsFalse();

            //await nextVM.NavigationService.GoBackModalAsync();

            //vmA.IsActive.IsTrue();
            //vmA.DoneOnActive.IsTrue();
        }

        [Test]
        public async Task NavigateAsync_ForViewType() {
            var Inav = App.NavigationServiceEx;

            var naviPage = new NavigationAlpha();
            App.MainPage = naviPage;

            await Inav.Navigate<PageAlpha>();

            var vm1 = naviPage.CurrentPage.BindingContext as PageAlphaViewModel;

            naviPage.CurrentPage.GetType().Is(typeof(PageAlpha));
            naviPage.Navigation.NavigationStack.Count.Is(1);

            vm1.DoneNavigatingTo.IsTrue();
            vm1.DoneNavigatedTo.IsTrue();
            vm1.DoneNavigatedFrom.IsFalse();
            vm1.NavigatingCount.Is(1);
            vm1.NavigatedToCount.Is(1);

            var param = new PageBetaParameters {
                Name = "hoge",
                Age = 20
            };
            await vm1.NavigationService.Navigate<PageBeta>(param);

            naviPage.CurrentPage.GetType().Is(typeof(PageBeta));
            naviPage.Navigation.NavigationStack.Count.Is(2);

            var vm2 = naviPage.CurrentPage.BindingContext as PageBetaViewModel;

            vm1.DoneNavigatedFrom.IsTrue();
            vm1.NavigatedFromCount.Is(1);
            vm2.DoneNavigatingTo.IsTrue();
            vm2.DoneNavigatedTo.IsTrue();
            vm2.DoneNavigatedFrom.IsFalse();
            vm2.NavigatingCount.Is(1);
            vm2.NavigatedToCount.Is(1);
            vm2.Param.To<PageBetaParameters>().IsStructuralEqual(param);
        }

        [Test]
        public async Task NavigateModalAsync_ForViewtype() {
            var Inav = App.NavigationServiceEx;

            var naviPage = new NavigationAlpha();
            App.MainPage = naviPage;

            await Inav.Navigate<PageAlpha>();

            var vm1 = naviPage.CurrentPage.BindingContext as PageAlphaViewModel;

            naviPage.CurrentPage.GetType().Is(typeof(PageAlpha));
            naviPage.Navigation.NavigationStack.Count.Is(1);

            vm1.DoneNavigatingTo.IsTrue();
            vm1.DoneNavigatedTo.IsTrue();
            vm1.DoneNavigatedFrom.IsFalse();
            vm1.NavigatingCount.Is(1);
            vm1.NavigatedToCount.Is(1);

            var param = new PageBetaParameters {
                Name = "hoge",
                Age = 20
            };
            await vm1.NavigationService.NavigateModal<PageBeta>(param);
            naviPage.Navigation.ModalStack.Count.Is(1);
           
            var vm2 = naviPage.Navigation.ModalStack[0].BindingContext as PageBetaViewModel;

            vm1.DoneNavigatedFrom.IsTrue();
            vm1.NavigatedFromCount.Is(1);
            vm2.DoneNavigatingTo.IsTrue();
            vm2.DoneNavigatedTo.IsTrue();
            vm2.DoneNavigatedFrom.IsFalse();
            vm2.NavigatingCount.Is(1);
            vm2.NavigatedToCount.Is(1);
            vm2.Param.To<PageBetaParameters>().IsStructuralEqual(param);
        }

        /// <summary>
        /// Prism.Forms 7.0 においてselectedTabパラメータでTab移動が可能になっているが
        /// 相変わらずの文字列指定で非常に使いにくい上に、selectedTabを発行しただけで
        /// OnNavigatingToが走ったりと良いことが無いので、独自実装の方の使用を続ける
        /// </summary>
        [Test]
        public void ChangeTab_NaviTabbed() {
            var nav = App.MyPageNavigationService;

            var pageA = nav.CreateContentPage(nameof(PageAlpha));
            var pageB = nav.CreateContentPage(nameof(PageBeta));

            var page = nav.CreateMainPageNavigationHasTabbed(
                "NavigationTop", "MainTabbedPage",
               new List<ContentPage> {
                    pageA,
                    pageB
                },
                new List<NavigationParameters>()
            );

            App.MainPage = page;

            var tabbed = page.CurrentPage as IPageController;
            tabbed.SendAppearing();

            var vmA = pageA.BindingContext as PageAlphaViewModel;
            var vmB = pageB.BindingContext as PageBetaViewModel;

            vmA.IsActive.IsTrue();
            vmA.DoneOnActive.IsTrue();
            vmA.DoneOnNonActive.IsFalse();
            vmA.OnActiveCount.Is(1);
            vmB.IsActive.IsFalse();
            vmB.DoneOnActive.IsFalse();
            vmB.DoneOnNonActive.IsFalse();

            vmA.AllClear();
            vmB.AllClear();

            var ret = vmA.NavigationService.ChangeTab<PageAlpha>();
            ret.IsTrue();
            vmA.IsActive.IsTrue();
            vmA.DoneOnActive.IsFalse();
            vmA.DoneOnNonActive.IsFalse();
            vmB.IsActive.IsFalse();
            vmB.DoneOnActive.IsFalse();
            vmB.DoneOnNonActive.IsFalse();

            vmA.AllClear();
            vmB.AllClear();

            ret = vmA.NavigationService.ChangeTab<PageBeta>();

            ret.IsTrue();

            vmA.IsActive.IsFalse();
            vmA.DoneOnNonActive.IsTrue();
            vmA.OnActiveCount.Is(0);
            vmA.OnNonActiveCount.Is(1);
            vmB.IsActive.IsTrue();
            vmB.DoneOnActive.IsTrue();
            vmB.OnActiveCount.Is(1);
            vmB.OnNonActiveCount.Is(0);

            vmA.AllClear();
            vmB.AllClear();

            ret = vmB.NavigationService.ChangeTab<NavigationPage>();
            ret.IsFalse();
        }

        [Test]
        public async Task ChangeTab_TabbedNavi() {
            var nav = App.MyPageNavigationService;

            var naviA = nav.CreateNavigationPage(nameof(NavigationAlpha), nameof(PageAlpha));
            var naviB = nav.CreateNavigationPage(nameof(NavigationBeta), nameof(PageBeta));

            var tabbed = nav.CreateMainPageTabbedHasNavigation(nameof(MainTabbedPage), new List<NavigationPage>{
                naviA,naviB});

            App.MainPage = tabbed;
            (tabbed as IPageController).SendAppearing();


            var vmA = naviA.CurrentPage.BindingContext as PageAlphaViewModel;
            var vmB = naviB.CurrentPage.BindingContext as PageBetaViewModel;

            vmA.IsActive.IsTrue();
            vmA.DoneOnActive.IsTrue();
            vmA.DoneOnNonActive.IsFalse();
            vmA.OnActiveCount.Is(1);
            vmB.IsActive.IsFalse();
            vmB.DoneOnActive.IsFalse();
            vmB.DoneOnNonActive.IsFalse();

            vmA.AllClear();
            vmB.AllClear();

            var ret = vmA.NavigationService.ChangeTab<NavigationBeta>();

            ret.IsTrue();
            vmA.IsActive.IsFalse();
            vmA.DoneOnActive.IsFalse();
            vmA.DoneOnNonActive.IsTrue();
            vmA.OnNonActiveCount.Is(1);
            vmB.IsActive.IsTrue();
            vmB.DoneOnActive.IsTrue();
            vmB.DoneOnNonActive.IsFalse();
            vmB.OnActiveCount.Is(1);

            vmA.AllClear();
            vmB.AllClear();

            ret = vmB.NavigationService.ChangeTab<PageAlpha>();
            ret.IsTrue();

            vmA.IsActive.IsTrue();
            vmA.DoneOnActive.IsTrue();
            vmA.DoneOnNonActive.IsFalse();
            vmA.OnActiveCount.Is(1);
            vmB.IsActive.IsFalse();
            vmB.DoneOnActive.IsFalse();
            vmB.DoneOnNonActive.IsTrue();
            vmB.OnNonActiveCount.Is(1);

            vmA.AllClear();
            vmB.AllClear();

            await vmA.NavigationService.Navigate<NextPage>();

            var nextVM = naviA.CurrentPage.BindingContext as NextPageViewModel;

            vmA.IsActive.IsFalse();
            vmA.DoneOnActive.IsFalse();
            vmA.DoneOnNonActive.IsTrue();
            vmA.OnNonActiveCount.Is(1);
            vmB.IsActive.IsFalse();
            vmB.DoneOnActive.IsFalse();
            vmB.DoneOnNonActive.IsFalse();

            nextVM.IsActive.IsTrue();
            nextVM.DoneOnActive.IsTrue();
            nextVM.DoneOnNonActive.IsFalse();
            nextVM.OnActiveCount.Is(1);

            vmA.AllClear();
            vmB.AllClear();
            nextVM.AllClear();

            ret = nextVM.NavigationService.ChangeTab<PageBeta>();
            ret.IsTrue();

            //スタックの奥のページは何も影響がない
            vmA.IsActive.IsFalse();
            vmA.DoneOnActive.IsFalse();
            vmA.DoneOnNonActive.IsFalse();
            vmA.OnNonActiveCount.Is(0);
            //スタック最後のページなのでアクティブが発火する
            vmB.IsActive.IsTrue();
            vmB.DoneOnActive.IsTrue();
            vmB.DoneOnNonActive.IsFalse();
            vmB.OnActiveCount.Is(1);

            nextVM.IsActive.IsFalse();
            nextVM.DoneOnActive.IsFalse();
            nextVM.DoneOnNonActive.IsTrue();
            nextVM.OnNonActiveCount.Is(1);

            vmA.AllClear();
            vmB.AllClear();
            nextVM.AllClear();

            ret = vmB.NavigationService.ChangeTab<ContentPage>();
            ret.IsFalse();
        }
    }
}
