using System;
using Prism.Mvvm;
using Xamarin.Forms;
using MyFormsLibrary.Navigation;
using System.Net.Mime;
namespace MyFormsLibrary.Tests.Mocks.Views
{
    public class ContentPageNonViewNonAction : ContentPage
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

    public class ContentPageNoAction : ContentPage
    {
        public ContentPageNoAction() {
            ViewModelLocator.SetAutowireViewModel(this, true);
        }
    }

    public class ContentPageAllAction : ContentPage
    {
        public ContentPageAllAction() {
            ViewModelLocator.SetAutowireViewModel(this, true);
        }
    }

    public class PageAlpha : ContentPageAllAction
    {
        public PageAlpha() {
            ViewModelLocator.SetAutowireViewModel(this, true);
        }
    }

    public class PageBeta : ContentPageAllAction
    {
        public PageBeta() {
            ViewModelLocator.SetAutowireViewModel(this, true);
        }
    }


}

