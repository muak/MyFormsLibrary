using System;
using Prism.Common;
using Xamarin.Forms;

namespace MyFormsLibrary.Tests.Mocks
{
    public class ApplicationProviderMock:IApplicationProvider
    {
        public Page MainPage { get; set; }
    }
}
