using System;
using Prism.Navigation;
using Prism;
using MyFormsLibrary.Navigation;
using Prism.AppModel;
using System.Threading.Tasks;

namespace MyFormsLibrary.Tests.Mocks.ViewModels
{
    public class PageAlphaViewModel:ContentPageAllActionViewModel,INavigationAware,IInitializeAsync,IActiveAware,IDestructible
    {
        public PageAlphaViewModel(INavigationServiceEx navigationService) {
            NavigationService = navigationService;
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
            OnActiveCount++;
        }
        void OnNonActive() {
            DoneOnNonActive = true;
            OnNonActiveCount++;
        }

        public event EventHandler IsActiveChanged;

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
