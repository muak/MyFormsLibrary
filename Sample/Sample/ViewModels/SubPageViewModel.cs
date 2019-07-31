using System;
using System.Collections.ObjectModel;
using Reactive.Bindings;
using System.Threading.Tasks;
using Xamarin.Forms;
namespace Sample.ViewModels
{
    public class SubPageViewModel
    {
        public ObservableCollection<string> ItemsSource { get; } = new ObservableCollection<string>();
        public ReactiveCommand LoadMoreCommand { get; } = new ReactiveCommand();
        public ReactivePropertySlim<bool> IsLoading { get; } = new ReactivePropertySlim<bool>();
        public ReactivePropertySlim<double> LoadingHeight { get; } = new ReactivePropertySlim<double>();

        public SubPageViewModel()
        {
            for(var i = 0; i < 100; i++)
            {
                ItemsSource.Add($"Item{i + 1}");
            }

            LoadMoreCommand.Subscribe(async _ => {
                Device.BeginInvokeOnMainThread(() => {
                    IsLoading.Value = true;
                });
                await Task.Run(async () => {
                    for (var i = 100; i < 150; i++)
                    {
                        await Task.Delay(250);
                        ItemsSource.Add($"AddItem{i + 1}");
                    }
                });
                Device.BeginInvokeOnMainThread(() => {
                    IsLoading.Value = false;
                });
            });
        }
    }
}
