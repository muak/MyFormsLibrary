using System;
using Prism.Navigation;
using Prism;
using MyFormsLibrary.Navigation;
namespace MyFormsLibrary.Tests.Mocks.ViewModels
{
    public class PageAlphaViewModel:ContentPageAllActionViewModel,INavigationAware,IActiveAware,IDestructible
    {
        public INavigationParameter MyParam { get; set; }

        public PageAlphaViewModel(INavigationServiceEx navigationService, INavigationParameter param) {
            NavigationService = navigationService;
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
            OnActiveCount++;
        }
        void OnNonActive() {
            DoneOnNonActive = true;
            OnNonActiveCount++;
        }

        public event EventHandler IsActiveChanged;

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

        public void Destroy() {
            DoneDestroy = true;
            DestroyCount++;
        }
    }
}
