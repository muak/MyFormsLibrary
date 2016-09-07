using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Xamarin.Forms;

namespace MyFormsLibrary.Navigation
{
    public class NavigationController : INavigationController
    {

        IUnityContainer UnityContainer;
        IApplicationProviderForNavi ApplicationProvider;
        Page PreviousTabPage;
  

        public NavigationController(IUnityContainer container, IApplicationProviderForNavi applicationProvider) {
            UnityContainer = container;
            ApplicationProvider = applicationProvider;

            ApplicationProvider.ModalPopped = (sender, e) => {
                var curPage = GetCurrentPage();
                (curPage.BindingContext as INavigationAction)?.OnNavigatedBack();
                (e.Modal.BindingContext as IDisposable)?.Dispose();
            };

        }

		public ContentPage CreateContentPage<TContentPage>()
			where TContentPage : ContentPage {

			return CreatePage<TContentPage>() as ContentPage;
		}

        /// <summary>
        /// NavigationPageの生成
        /// </summary>
        /// <returns>NavigationPage</returns>
        /// <typeparam name="TNavigationPage">NavigaionPageの派生クラス</typeparam>
        /// <typeparam name="TContentPage">初期ページ</typeparam>
        public async Task<NavigationPage> CreateNavigationPage<TNavigationPage, TContentPage>()
            where TNavigationPage : NavigationPage
            where TContentPage : ContentPage {
            var nav = CreatePage<TNavigationPage>() as NavigationPage;
            var page = CreatePage<TContentPage>();

			var navigationParam = UnityContainer.Resolve<INavigationParameter>();

            await nav.PushAsync(page, false);

			(page.BindingContext as INavigationAction)?.OnNavigatedTo(navigationParam);

            //素のNavigationPageなどで初期状態からルートページが存在する場合は削除する
            if (nav.Navigation.NavigationStack.Count == 2) {
                nav.Navigation.RemovePage(nav.Navigation.NavigationStack[0]);
            }

            nav.Popped += (sender, e) => {
                var curPage = sender as NavigationPage;
                (curPage.CurrentPage.BindingContext as INavigationAction)?.OnNavigatedBack();
                (e.Page.BindingContext as IDisposable)?.Dispose();
            };

            return nav;
        }

		public async Task<NavigationPage> CreateNavigationPage<TNavigationPage>(TabbedPage tabbedPage) 
			where TNavigationPage : NavigationPage {

			var nav = CreatePage<TNavigationPage>() as NavigationPage;

			await nav.PushAsync(tabbedPage, false);

			//素のNavigationPageなどで初期状態からルートページが存在する場合は削除する
			if (nav.Navigation.NavigationStack.Count == 2) {
				nav.Navigation.RemovePage(nav.Navigation.NavigationStack[0]);
			}

			nav.Popped += (sender, e) => {
				//var curPage = sender as NavigationPage;

				(GetCurrentPage()?.BindingContext as INavigationAction)?.OnNavigatedBack();
				(e.Page.BindingContext as IDisposable)?.Dispose();
			};

			return nav;
		}

        /// <summary>
        /// TabbedPageの生成
        /// </summary>
        /// <returns>TabbedPage</returns>
        /// <param name="Children">NavigationPage List</param>
        /// <typeparam name="TTabbedPage">TabbedPageの派生クラス</typeparam>
        public TabbedPage CreateTabbedPage<TTabbedPage>(IEnumerable<NavigationPage> Children)
            where TTabbedPage : TabbedPage {
            var parent = CreatePage<TTabbedPage>() as TabbedPage;

            foreach (var c in Children) {
                parent.Children.Add(c);
            }

            PreviousTabPage = parent.Children.First();
			var preNav = PreviousTabPage as NavigationPage;

            (preNav.CurrentPage.BindingContext as ITabAction)?.OnTabChangedTo();

            parent.CurrentPageChanged += (sender, e) => {
                var nextTabPage = (sender as TabbedPage).CurrentPage as NavigationPage;
				var preTab = PreviousTabPage as NavigationPage;
                
				// Raise TabChangedFrom 
				(preTab.CurrentPage.BindingContext as ITabAction)?.OnTabChangedFrom();

				// Raise TabChangedTo
				(nextTabPage.CurrentPage.BindingContext as ITabAction)?.OnTabChangedTo();
				PreviousTabPage = nextTabPage;
            };

            return parent;
        }

		public TabbedPage CreateTabbedPage<TTabbedPage>(IEnumerable<ContentPage> Children)
			where TTabbedPage : TabbedPage {
			var parent = CreatePage<TTabbedPage>() as TabbedPage;

			var navigationParam = UnityContainer.Resolve<INavigationParameter>();

			foreach (var c in Children) {
				parent.Children.Add(c);
				(c.BindingContext as INavigationAction)?.OnNavigatedTo(navigationParam);
			}

			PreviousTabPage = parent.Children.First();

			(PreviousTabPage.BindingContext as ITabAction)?.OnTabChangedTo();

			parent.CurrentPageChanged += (sender, e) => {
				var nextTabPage = (sender as TabbedPage).CurrentPage as ContentPage;


			   // Raise TabChangedFrom 
			   (PreviousTabPage.BindingContext as ITabAction)?.OnTabChangedFrom();

				// Raise TabChangedTo
				(nextTabPage.BindingContext as ITabAction)?.OnTabChangedTo();
				PreviousTabPage = nextTabPage;
			};

			return parent;
		}

		/// <summary>
		/// ページ遷移
		/// </summary>
		/// <returns></returns>
		/// <param name="param">次のページに渡すパラメータ</param>
		/// <param name="animated">アニメーション</param>
		/// <typeparam name="TContentPage">遷移先ページ</typeparam>
		public async Task PushAsync<TContentPage>
			(object param = null, bool animated = true)
			where TContentPage : ContentPage {
			var navigationParam = UnityContainer.Resolve<INavigationParameter>();
			navigationParam.Value = param;

			var curPage = GetCurrentNavigationPage();

			//モーダル中は何もしない
			if (curPage.Navigation.ModalStack.Count > 0) {
				return;
			}

			var newPage = CreatePage<TContentPage>();
			(GetCurrentPage()?.BindingContext as INavigationAction)?.OnNavigatedForward();

			await curPage.Navigation.PushAsync(newPage, animated);
			(newPage.BindingContext as INavigationAction)?.OnNavigatedTo(navigationParam);

		}
        /// <summary>
        /// ページ遷移（モーダル）
        /// </summary>
        /// <returns>The modal async.</returns>
        /// <param name="param">Parameter.</param>
        /// <param name="animated">Animated.</param>
        /// <typeparam name="TContentPage">The 1st type parameter.</typeparam>
        public async Task PushModalAsync<TContentPage>
            (object param = null, bool animated = true)
            where TContentPage : ContentPage {
            var navigationParam = UnityContainer.Resolve<INavigationParameter>();
            navigationParam.Value = param;

            var curPage = GetCurrentNavigationPage();

            var newPage = CreatePage<TContentPage>();
            (GetCurrentPage()?.BindingContext as INavigationAction)?.OnNavigatedForward();


            await curPage.Navigation.PushModalAsync(newPage, animated);
			(newPage.BindingContext as INavigationAction)?.OnNavigatedTo(navigationParam);

        }

        /// <summary>
        /// ページ遷移
        /// </summary>
        /// <returns></returns>
        /// <param name="param"></param>
        /// <param name="animated"></param>
        /// <typeparam name="TNavigationPage">移動先タブのNavigationPage</typeparam>
        /// <typeparam name="TContentPage">移動先ページ</typeparam>
        public async Task PushAsync<TNavigationPage, TContentPage>
            (object param = null, bool animated = true)
            where TNavigationPage : NavigationPage where TContentPage : ContentPage {

            if (!ChangeTab<TNavigationPage>()) {
                return;
            }

            await PushAsync<TContentPage>(param, animated);
        }

        /// <summary>
        /// Tabの切り替え
        /// </summary>
        /// <returns></returns>
        /// <typeparam name="TNavigationPage">移動先のNavigationPage</typeparam>
        public bool ChangeTab<TPage>() where TPage:Page {
            var mainPage = GetMainPage();
			TabbedPage tabbed = null;
			if (mainPage is TabbedPage) {
				tabbed = mainPage as TabbedPage;
			}
			else if (mainPage is NavigationPage && (mainPage as NavigationPage)?.CurrentPage is TabbedPage) {
				tabbed = (mainPage as NavigationPage).CurrentPage as TabbedPage;
			}
			else {
				return false;
			}

			if (tabbed.CurrentPage.GetType() == typeof(TPage)) {
				return false;
			}

			var target = tabbed?.Children.FirstOrDefault((x) => x.GetType() == typeof(TPage));
			if (target != null) {
				tabbed.CurrentPage = target;
				return true;
			}

			return false;            
        }




        /// <summary>
        /// 1つ前のページに戻る
        /// </summary>
        /// <returns></returns>
        /// <param name="animated">Animated.</param>
        public async Task GoBackAsync(bool animated = true) {
            var curPage = GetCurrentNavigationPage();

            if (curPage.Navigation.ModalStack.Count > 0) {
                await curPage.Navigation.PopModalAsync(animated);
            }
            else if (curPage.Navigation.NavigationStack.Count > 1) {
                await curPage.Navigation.PopAsync(animated);
            }
        }



        Page GetMainPage() {
            return ApplicationProvider.MainPage;
        }

        NavigationPage GetCurrentNavigationPage() {
            var main = GetMainPage();
            if (main is TabbedPage) {
                var tab = main as TabbedPage;
                if (tab.CurrentPage is NavigationPage) {
                    return tab.CurrentPage as NavigationPage;
                }
                return null;
            }
            else if (main is NavigationPage) {
                return main as NavigationPage;
            }

            return null;
        }

        Page GetCurrentPage() {
            var NaviPage = GetCurrentNavigationPage();
            if (NaviPage.Navigation.ModalStack.Count > 0) {
                return NaviPage.Navigation.ModalStack.Last();
            }

			if (NaviPage.CurrentPage is TabbedPage) {
				return (NaviPage.CurrentPage as TabbedPage).CurrentPage;
			}

            return NaviPage.CurrentPage;
        }

        Page CreatePage<T>() {
            return UnityContainer.Resolve<T>() as Page;
        }
        Page CreatePage(Type type) {
            return UnityContainer.Resolve(type) as Page;
        }
    }
}

