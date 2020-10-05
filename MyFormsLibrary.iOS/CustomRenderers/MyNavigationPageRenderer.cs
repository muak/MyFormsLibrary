using System;
using System.Threading.Tasks;
using MyFormsLibrary.CustomRenderers;
using MyFormsLibrary.iOS.CustomRenderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(MyNavigationPage), typeof(MyNavigationPageRenderer))]
namespace MyFormsLibrary.iOS.CustomRenderers
{
    public class MyNavigationPageRenderer: NavigationRenderer
    {
        protected override void OnElementChanged(VisualElementChangedEventArgs e) {
            base.OnElementChanged(e);
            if (e.OldElement != null) {
                e.OldElement.PropertyChanged -= OnPropertyChanged;
            }
            if (e.NewElement != null) {
                e.NewElement.PropertyChanged += OnPropertyChanged;
            }            
        }

        private void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == NavigationPage.BarBackgroundColorProperty.PropertyName) {
                SetNeedsStatusBarAppearanceUpdate();
                PreferredStatusBarStyle();
                SetNeedsStatusBarAppearanceUpdate();
            }
        }

        public override UIStatusBarStyle PreferredStatusBarStyle() {
            var navigationPage = Element as NavigationPage;
            return navigationPage.BarBackgroundColor.Luminosity >= 0.5 ? UIStatusBarStyle.DarkContent : UIStatusBarStyle.LightContent;
        }  
    }
}
