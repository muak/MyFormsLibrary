using System;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Reactive.Bindings;

namespace Sample.ViewModels
{
    public class MainPageViewModel:BindableBase
    {
        public ReactiveCommand GoNextCommand { get; } = new ReactiveCommand();        
        public ReactiveCommand MenuCommand { get; }
        public ReactiveCommand MenuCommand2 { get; } = new ReactiveCommand();
        public ReactiveCommand LeftCommand { get; } = new ReactiveCommand();
        public ReactivePropertySlim<bool> Visible1 { get; } = new ReactivePropertySlim<bool>();
        public ReactivePropertySlim<bool> Visible2 { get; } = new ReactivePropertySlim<bool>();

        public MainPageViewModel(INavigationService navigationService,IPageDialogService pageDialog)
        {
            MenuCommand = new ReactivePropertySlim<bool>(false).ToReactiveCommand();

            MenuCommand2.Subscribe(_ => {
                pageDialog.DisplayAlertAsync("", "Tap", "OK");
            });

            GoNextCommand.Subscribe(_ => {
                navigationService.NavigateAsync("SecondPage");
            });

            LeftCommand.Subscribe(_ => {
                pageDialog.DisplayAlertAsync("","Left Tap","OK");
            });
        }
    }
}
