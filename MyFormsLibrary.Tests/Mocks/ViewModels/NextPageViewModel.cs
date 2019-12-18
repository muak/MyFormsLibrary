using System;
using Prism.Navigation;
using MyFormsLibrary.Navigation;
using Prism.Common;
using Prism;
using System.Threading.Tasks;

namespace MyFormsLibrary.Tests.Mocks.ViewModels
{
    public class NextPageViewModel:ContentPageAllActionViewModel,IInitializeAsync,INavigationAware,IActiveAware,IDestructible
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

        public void OnNavigatedFrom(INavigationParameters parameters) {
            DoneNavigatedFrom = true;
            NavigatedFromCount++;
        }

        public void OnNavigatedTo(INavigationParameters parameters) {
            DoneNavigatedTo = true;
            NavigatedToCount++;
        }

        public void Destroy() {
            DoneDestroy = true;
            DestroyCount++;
        }

        public async Task InitializeAsync(INavigationParameters parameters)
        {
            Param = parameters;
            DoneInitialize = true;
            NavigatingCount++;
        }
    }
}
