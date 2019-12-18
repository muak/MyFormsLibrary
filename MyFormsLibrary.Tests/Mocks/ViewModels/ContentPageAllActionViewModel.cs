using System;
using Prism.Mvvm;
using Prism.Navigation;
using Prism;
using MyFormsLibrary.Navigation;

namespace MyFormsLibrary.Tests.Mocks.ViewModels
{
    public class ContentPageAllActionViewModel:BindableBase
    {
        public INavigationParameters Param { get; set; }
        public bool DoneInitialize { get; set; } = false;
        public bool DoneNavigatedTo { get; set; } = false;
        public bool DoneNavigatedFrom { get; set; } = false;
        public bool DoneOnActive { get; set; } = false;
        public bool DoneOnNonActive { get; set; } = false;
        public bool DoneDestroy { get; set; } = false;

        public int NavigatingCount { get; set; } = 0;
        public int NavigatedToCount { get; set; } = 0;
        public int NavigatedFromCount { get; set; } = 0;
        public int OnActiveCount { get; set; } = 0;
        public int OnNonActiveCount { get; set; } = 0;
        public int DestroyCount { get; set; } = 0;

        public INavigationServiceEx NavigationService { get; set; }
  

        public ContentPageAllActionViewModel() {
            
        }
        public void AllClear() {
            DoneOnActive = false;
            DoneOnNonActive = false;
            DoneNavigatedTo = false;
            DoneInitialize = false;
            DoneNavigatedFrom = false;
            Param = null;
            OnActiveCount = 0;
            OnNonActiveCount = 0;
            NavigatingCount = 0;
            NavigatedToCount = 0;
            NavigatedFromCount = 0;
            DoneDestroy = false;
            DestroyCount = 0;
        }
    }
}
