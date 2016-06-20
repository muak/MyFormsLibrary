using System;
using Prism.Mvvm;
using Xamarin.Forms;
using MyFormsLibrary.Navigation;
namespace MyFormsLibrary.Tests.Mocks.Views
{
    public class ContentPageNonViewNonAction:ContentPage
    {
        public ContentPageNonViewNonAction() {
            ViewModelLocator.SetAutowireViewModel(this, true);
        }
    }

    public class GetParamPage : ContentPage
    {
        public GetParamPage() {
            ViewModelLocator.SetAutowireViewModel(this, true);
        }
    }

    public class AllActionPage : ContentPage
    {
        

        public AllActionPage() {
            ViewModelLocator.SetAutowireViewModel(this, true);
        }


    }


}

