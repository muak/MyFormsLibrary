using System;
using Prism.Navigation;
using MyFormsLibrary.Navigation;
using Prism.Common;
using Prism;

namespace MyFormsLibrary.Tests.Mocks.ViewModels
{
    public class NextPageViewModel:ContentPageAllActionViewModel,INavigationAware,IActiveAware
    {
        public NextPageViewModel(INavigationServiceEx navigationService) {
            NavigationService = navigationService;
        }

        private bool _IsActive;

        public event EventHandler IsActiveChanged;

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
            OnActiveCount++;
        }
        void OnNonActive() {
            DoneOnNonActive = true;
            OnNonActiveCount++;
        }

        public void OnNavigatedFrom(NavigationParameters parameters) {
            DoneNavigatedFrom = true;
            NavigatedFromCount++;
        }

        public void OnNavigatedTo(NavigationParameters parameters) {
            DoneNavigatedTo = true;
            NavigatedToCount++;
        }

        public void OnNavigatingTo(NavigationParameters parameters) {
            Param = parameters;
            DoneNavigatingTo = true;
            NavigatingCount++;
        }

    }
}
