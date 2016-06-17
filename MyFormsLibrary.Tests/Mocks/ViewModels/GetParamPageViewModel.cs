using System;
using Prism.Mvvm;
using MyFormsLibrary.Navigation;
using MyFormsLibrary.Tests.Mocks.Views;
namespace MyFormsLibrary.Tests.Mocks.ViewModels
{
    public class GetParamPageViewModel:BindableBase,INavigationParameter
    {
        public object Value { get; set; }

       

        public GetParamPageViewModel(INavigationParameter param) {
            Value = param.Value;
        }
    }
}

