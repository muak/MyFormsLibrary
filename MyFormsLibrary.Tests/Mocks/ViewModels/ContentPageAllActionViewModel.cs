using System;
using Prism.Mvvm;
using Prism.Navigation;
using Prism;
namespace MyFormsLibrary.Tests.Mocks.ViewModels
{
    public class ContentPageAllActionViewModel:BindableBase
    {
        public NavigationParameters Param { get; set; }
        public bool DoneNavigatingTo { get; set; } = false;
        public bool DoneNavigatedTo { get; set; } = false;
        public bool DoneNavigatedFrom { get; set; } = false;
        public bool DoneOnActive { get; set; } = false;
        public bool DoneOnNonActive { get; set; } = false;  

        public INavigationService NavigationService { get; set; }
  

        public ContentPageAllActionViewModel() {
            
        }




        public void AllClear() {
            DoneOnActive = false;
            DoneOnNonActive = false;
            DoneNavigatedTo = false;
            DoneNavigatingTo = false;
            DoneNavigatedFrom = false;
            Param = null;
        }
    }
}
