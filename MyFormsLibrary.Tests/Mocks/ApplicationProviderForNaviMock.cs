using System;
using MyFormsLibrary.Navigation;
using Xamarin.Forms;

namespace MyFormsLibrary.Tests.Mocks
{
    public class ApplicationProviderForNaviMock : IApplicationProviderForNavi
    {


        public ApplicationProviderForNaviMock() {

        }

        public Page MainPage { get; set; }

        public Action<object, ModalPoppedEventArgs> ModalPopped { get; set; }
    }
}

