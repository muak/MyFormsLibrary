using System;
using Xamarin.Forms;
using Prism.Navigation;

namespace MyFormsLibrary.Tests.Mocks.Views
{
    public class MainTabbedPage : TabbedPage
    {
    }

    public class MyMasterDetail : MasterDetailPage, IDestructible
    {
        public void Destroy() {
            
        }
    }

    public class NavigationAlpha : NavigationPage { }

    public class NavigationBeta : NavigationPage { }
    public class NavigationGamma : NavigationPage { }

    public class NavigationTop : NavigationPage, IDestructible
    {
        public void Destroy() {
            
        }
    }

}

