using System;
using MyFormsLibrary.CustomRenderers;
using Prism.DryIoc;
using Prism.Ioc;
using Sample.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Sample
{
    public partial class App : PrismApplication
    {
        public App()
        {
            InitializeComponent();            
        }

        protected override void OnInitialized()
        {
            NavigationService.NavigateAsync("/MyNavi/MainPage");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NavigationPageEx>();
            containerRegistry.RegisterForNavigation<MyNavi>();
            containerRegistry.RegisterForNavigation<MainPage>();
            containerRegistry.RegisterForNavigation<SecondPage>();
        }
    }
}
