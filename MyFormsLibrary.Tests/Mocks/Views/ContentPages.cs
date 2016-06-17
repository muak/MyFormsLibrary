using System;
using Prism.Mvvm;
using Xamarin.Forms;
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
}

