using MyFormsLibrary.Navigation;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Collections.ObjectModel;

namespace Sample.ViewModels
{
	public class MainPageViewModel : BindableBase, INavigationAware
	{
        public ReactivePropertySlim<bool> TitleVisible { get; } = new ReactivePropertySlim<bool>(true);
        public ReadOnlyReactivePropertySlim<bool> SearchVisible { get; set; }
        public ReactiveCommand GoCommand { get; } = new ReactiveCommand();
        public ObservableCollection<string> ItemsSource { get; } = new ObservableCollection<string>();

        public MainPageViewModel(INavigationServiceEx navigationService)
		{
            SearchVisible = TitleVisible.Select(x => !x).ToReadOnlyReactivePropertySlim();

            RaisePropertyChanged(nameof(SearchVisible));
            GoCommand.Subscribe(async _ => 
            {
                await navigationService.NavigateAsync("SubPage");
            });

            for (var i = 0; i < 100;i++)
            {
                ItemsSource.Add($"Name{i}");
            }
		}

		public void OnNavigatedFrom(NavigationParameters parameters)
		{

		}

		public void OnNavigatedTo(NavigationParameters parameters)
		{
		
		}

		public void OnNavigatingTo(NavigationParameters parameters)
		{

		}
	}
}

