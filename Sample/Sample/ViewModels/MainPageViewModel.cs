using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Services;

namespace Sample.ViewModels
{
	public class MainPageViewModel : BindableBase, INavigationAware
	{
        public ReactiveProperty<bool> ToolVisible { get; } = new ReactiveProperty<bool>(true);
        public ReactiveCommand TestCommand { get; } = new ReactiveCommand();
        public ReactiveCommand Test2Command { get; } = new ReactiveCommand();

        public MainPageViewModel(IPageDialogService pageDialog)
		{
            TestCommand.Subscribe(x =>
            {
                ToolVisible.Value = !ToolVisible.Value;
            });
            Test2Command.Subscribe(async x =>
            {
                await pageDialog.DisplayAlertAsync("", "Fire", "OK");
            });
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

