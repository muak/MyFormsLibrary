using System;
using Prism.Navigation;
using Prism;
using MyFormsLibrary.Navigation;
namespace MyFormsLibrary.Tests.Mocks.ViewModels
{
    public class PageAlphaViewModel:ContentPageAllActionViewModel,INavigationAware,IActiveAware
    {
        public INavigationParameter MyParam { get; set; }

        public PageAlphaViewModel(INavigationParameter param) {
            MyParam = param;
        }

        private bool _IsActive;
        public bool IsActive {
            get {
                return _IsActive;
            }

            set {
                _IsActive = value;
                if (value) {
                    OnActive();
                }
                else {
                    OnNonActive();
                }
            }
        }

        void OnActive() {
            DoneOnActive = true;
        }
        void OnNonActive() {
            DoneOnNonActive = true;
        }

        public event EventHandler IsActiveChanged;

        public void OnNavigatedFrom(NavigationParameters parameters) {
            DoneNavigatedFrom = true;
        }

        public void OnNavigatedTo(NavigationParameters parameters) {
            DoneNavigatedTo = true;
        }

        public void OnNavigatingTo(NavigationParameters parameters) {
            Param = parameters;
            DoneNavigatingTo = true;
        }
    }
}
