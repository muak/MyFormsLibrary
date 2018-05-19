using System;
using Xamarin.Forms;
using Prism.Navigation;

namespace MyFormsLibrary.Tests.Mocks.Views
{
    public class MainTabbedPage : TabbedPage, IDestructible
    {
        public bool DoneDestroy { get; set; }
        public void Destroy() {
            DoneDestroy = true;
        }
    }

    public class MyMasterDetail : MasterDetailPage, IDestructible
    {
        public bool DoneDestroy { get; set; }
        public MyMasterDetail()
        {
            Master = new NextPage(){Title = "Hoge"};
            Detail = new NavigationTop();
        }
        public void Destroy() {
            DoneDestroy = true;
        }
    }

    public class NavigationAlpha : NavigationPage { }

    public class NavigationBeta : NavigationPage { }
    public class NavigationGamma : NavigationPage { }

    public class NavigationTop : NavigationPage, IDestructible
    {
        public bool DoneDestroy { get; set; }
        public void Destroy() {
            DoneDestroy = true;
        }
    }

}

