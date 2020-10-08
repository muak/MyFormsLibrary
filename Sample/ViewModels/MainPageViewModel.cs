using System;
using Prism.Mvvm;
using Prism.Navigation;
using Reactive.Bindings;

namespace Sample.ViewModels
{
    public class MainPageViewModel:BindableBase
    {
        public ReactiveCommand GoNextCommand { get; } = new ReactiveCommand();

        public MainPageViewModel(INavigationService navigationService)
        {
            GoNextCommand.Subscribe(_ => {
                navigationService.NavigateAsync("SecondPage");
            });
        }
    }
}
