using System;
using Xamarin.Forms;
namespace MyFormsLibrary.Navigation
{
    public class ModalPage:ContentPage
    {
        private INavigationController NaviCtrl;

        public ModalPage(INavigationController navi) {
            NaviCtrl = navi;
        }

        protected override bool OnBackButtonPressed() {
            NaviCtrl.GoBackAsync();
            return true;
        }
    }
}

