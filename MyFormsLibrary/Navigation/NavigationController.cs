﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Prism.Common;
using Xamarin.Forms;

namespace MyFormsLibrary.Navigation
{
	public class NavigationController : INavigationController
	{

		IUnityContainer UnityContainer;
		IApplicationProvider ApplicationProvider;
		NavigationPage PreviousTabPage;
		Dictionary<NavigationPage, Page> LastNavigationCurrent;
		Dictionary<NavigationPage, Type> NavigationRoot;
		


		public NavigationController(IUnityContainer container, IApplicationProvider applicationProvider)
		{
			UnityContainer = container;
			ApplicationProvider = applicationProvider;
			LastNavigationCurrent = new Dictionary<NavigationPage, Page>();
			NavigationRoot = new Dictionary<NavigationPage, Type>();
			
            Application.Current.ModalPopped += (sender, e) => {
                var curPage = GetCurrentPage();
                (curPage.BindingContext as INavigationAction)?.OnNavigatedBack();
                (e.Modal.BindingContext as IDisposable)?.Dispose();
            };

		}

		/// <summary>
		/// NavigationPageの生成
		/// </summary>
		/// <returns>NavigationPage</returns>
		/// <typeparam name="TNavigationPage">NavigaionPageの派生クラス</typeparam>
		/// <typeparam name="TContentPage">初期ページ</typeparam>
		public async Task<NavigationPage> CreateNavigationPage<TNavigationPage, TContentPage>()
			where TNavigationPage : NavigationPage
			where TContentPage : ContentPage
		{
			var nav = CreatePage<TNavigationPage>() as NavigationPage;

			NavigationRoot[nav] = typeof(TContentPage);

			nav.Popped += (sender, e) =>
			{
				var curPage = sender as NavigationPage;
				(curPage.CurrentPage.BindingContext as INavigationAction)?.OnNavigatedBack();
				(e.Page.BindingContext as IDisposable)?.Dispose();
			};

			

			var page = CreatePage<TContentPage>();

			await nav.PushAsync(page, false);

			return nav;
		}

		/// <summary>
		/// TabbedPageの生成
		/// </summary>
		/// <returns>TabbedPage</returns>
		/// <param name="Children">NavigationPage List</param>
		/// <typeparam name="TTabbedPage">TabbedPageの派生クラス</typeparam>
		public Page CreateTabbedPage<TTabbedPage>(IEnumerable<NavigationPage> Children)
			where TTabbedPage : TabbedPage
		{
			var parent = CreatePage<TTabbedPage>() as TabbedPage;

			foreach (var c in Children)
			{
				parent.Children.Add(c);
			}

            PreviousTabPage = parent.Children.First() as NavigationPage;
			LastNavigationCurrent[PreviousTabPage] = PreviousTabPage.CurrentPage;

            (PreviousTabPage.CurrentPage.BindingContext as ITabAction)?.OnTabChangedTo(true);

			parent.CurrentPageChanged +=  (sender, e) => {
                var nextTabPage = (sender as TabbedPage).CurrentPage as NavigationPage;

                var isFirst = true;
                if (LastNavigationCurrent.ContainsKey(nextTabPage)) {
                    isFirst = !object.ReferenceEquals(LastNavigationCurrent[nextTabPage], nextTabPage.CurrentPage);
                }

                // Raise TabChangedFrom 
                (PreviousTabPage.CurrentPage.BindingContext as ITabAction)?.OnTabChangedFrom();
		    	LastNavigationCurrent[PreviousTabPage] = PreviousTabPage.CurrentPage;

				// Raise TabChangedTo
				(nextTabPage.CurrentPage.BindingContext as ITabAction)?.OnTabChangedTo(isFirst);
				PreviousTabPage = nextTabPage;
			};

			return parent;
		}

		/// <summary>
		/// Tabの切り替え
		/// </summary>
		/// <returns></returns>
		/// <typeparam name="TNavigationPage">移動先のNavigationPage</typeparam>
		public void ChangeTab<TNavigationPage>()
		{
			var mainPage = GetMainPage() as TabbedPage;

			var target = mainPage?.Children.Where((x) => x is TNavigationPage).FirstOrDefault() as NavigationPage;
			if (target != null)
			{
				mainPage.CurrentPage = target;
			}
		}


		/// <summary>
		/// ページ遷移
		/// </summary>
		/// <returns></returns>
		/// <param name="param">次のページに渡すパラメータ</param>
		/// <param name="useModalNavigation">モーダルかどうか</param>
		/// <param name="animated">アニメーション</param>
		/// <typeparam name="TContentPage">遷移先ページ</typeparam>
		public async Task NavigateAsync<TContentPage>
			(object param = null, bool? useModalNavigation = default(bool?), bool animated = true)
			where TContentPage : ContentPage
		{
			if (param != null)
			{
				var navigationParam = UnityContainer.Resolve<INavigationParameter>();
				navigationParam.Value = param;
			}

			var curPage = GetCurrentNavigationPage();

			var newPage = CreatePage<TContentPage>();
			(GetCurrentPage()?.BindingContext as INavigationAction)?.OnNavigatedFoward();
			if (UseModalNavigation(useModalNavigation))
			{
				await curPage.Navigation.PushModalAsync(newPage, animated);
			}
			else
			{
				await curPage.Navigation.PushAsync(newPage, animated);
			}


		}

		/// <summary>
		/// ページ遷移
		/// </summary>
		/// <returns></returns>
		/// <param name="param"></param>
		/// <param name="useModalNavigation"></param>
		/// <param name="animated"></param>
		/// <typeparam name="TNavigationPage">移動先タブのNavigationPage</typeparam>
		/// <typeparam name="TContentPage">移動先ページ</typeparam>
		public async Task NavigateAsync<TNavigationPage, TContentPage>
			(object param = null, bool? useModalNavigation = default(bool?), bool animated = true)
			where TNavigationPage : NavigationPage where TContentPage : ContentPage
		{
			ChangeTab<TNavigationPage>();

			var curPage = GetCurrentPage();
			if (!(curPage is TContentPage))
			{
				await NavigateAsync<TContentPage>(param, useModalNavigation, animated);
			}
		}


		/// <summary>
		/// 1つ前のページに戻る
		/// </summary>
		/// <returns></returns>
		/// <param name="animated">Animated.</param>
		public async Task GoBackAsync(bool animated = true)
		{
			var curPage = GetCurrentNavigationPage();

			if (curPage.Navigation.ModalStack.Count > 0)
			{
				await curPage.Navigation.PopModalAsync(animated);
			}
			else if (curPage.Navigation.NavigationStack.Count > 1)
			{
				await curPage.Navigation.PopAsync(animated);
			}
		}

		public async Task GoBackRootAsync(bool animated = true)
		{
			var mainPage = GetMainPage();

			var stack = mainPage.Navigation.NavigationStack;
			if (stack.Count > 2)
			{
				var removePages = stack.Skip(1).Take(stack.Count - 1);
				foreach (var p in removePages)
				{
					(p as IDisposable)?.Dispose();
					mainPage.Navigation.RemovePage(p);
				}
			}
			await mainPage.Navigation.PopToRootAsync(animated);
		}


		bool UseModalNavigation(bool? useModalNavigationDefault)
		{
			if (useModalNavigationDefault.HasValue)
				return useModalNavigationDefault.Value;
			else
				return false;
		}


		Page GetMainPage()
		{
			return ApplicationProvider.MainPage;
		}

		NavigationPage GetCurrentNavigationPage()
		{
			var main = GetMainPage();
			if (main is TabbedPage)
			{
				var tab = main as TabbedPage;
				if (tab.CurrentPage is NavigationPage)
				{
					return tab.CurrentPage as NavigationPage;
				}
				return null;
			}
			else if (main is NavigationPage)
			{
				return main as NavigationPage;
			}

			return null;
		}

		Page GetCurrentPage()
		{
			var NaviPage = GetCurrentNavigationPage();
			if (NaviPage.Navigation.ModalStack.Count > 0)
			{
				return NaviPage.Navigation.ModalStack.Last();
			}

			return NaviPage.CurrentPage;
		}

		Page CreatePage<T>()
		{
			return UnityContainer.Resolve<T>() as Page;
		}
		Page CreatePage(Type type)
		{
			return UnityContainer.Resolve(type) as Page;
		}
	}
}

