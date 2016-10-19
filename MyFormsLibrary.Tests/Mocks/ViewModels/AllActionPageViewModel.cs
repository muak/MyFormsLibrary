using System;
using Prism.Mvvm;
using Xamarin.Forms;
using MyFormsLibrary.Navigation;
namespace MyFormsLibrary.Tests.Mocks.ViewModels
{
    public class AllActionPageViewModel : BindableBase, INavigationParameter, INavigationAction, ITabAction, IDisposable
    {
        public bool DoneDispose { get; set; } = false;
        public bool DoneBack { get; set; } = false;
        public bool DoneFoward { get; set; } = false;
        public bool DoneTabFrom { get; set; } = false;
        public bool DoneTabTo { get; set; } = false;
        public bool TabChangeToParam { get; set; } = false;

        public object Value { get; set; }

        public AllActionPageViewModel(INavigationParameter param) {
            Value = param.Value;
        }

        public void Dispose() {
            DoneDispose = true;
        }

        public void OnNavigatedBack() {
            DoneBack = true;
        }

        public void OnNavigatedForward() {
            DoneFoward = true;
        }

        public void OnTabChangedFrom() {
            DoneTabFrom = true;
        }

        public void OnTabChangedTo() {
            DoneTabTo = true;
            //TabChangeToParam = IsFirst;
        }


        public void OnNavigatedTo(INavigationParameter param) {
            throw new NotImplementedException();
        }

    }
}

