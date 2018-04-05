using System;
using Reactive.Bindings;
using Prism.Navigation;
namespace Sample.ViewModels
{
    public class MenuPageViewModel
    {
        public ReactiveCommand GoToCommand { get; } = new ReactiveCommand();

        public MenuPageViewModel(INavigationService navigationService)
        {
            GoToCommand.Subscribe(async _ =>
            {
                await navigationService.NavigateAsync("MainPage");
            });
        }
    }
}
