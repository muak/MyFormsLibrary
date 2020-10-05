using System;
using System.Threading.Tasks;
using MyFormsLibrary.CustomRenderers;
using MyFormsLibrary.iOS.CustomRenderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(MyTabbedPage), typeof(MyTabbedPageRenderer))]
namespace MyFormsLibrary.iOS.CustomRenderers
{
    public class MyTabbedPageRenderer: TabbedRenderer
    {
        public MyTabbedPageRenderer()
        {
        }

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null)
            {
                e.OldElement.PropertyChanged -= OnPropertyChanged;
            }
            if (e.NewElement != null)
            {
                e.NewElement.PropertyChanged += OnPropertyChanged;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Element.PropertyChanged -= OnPropertyChanged;
            }
            base.Dispose(disposing);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            SetUpTab();
        }

        private void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == TabbedPage.SelectedTabColorProperty.PropertyName ||
                e.PropertyName == TabbedPage.UnselectedTabColorProperty.PropertyName ||
                e.PropertyName == MyTabbedPage.SelectedTextColorProperty.PropertyName ||
                e.PropertyName == MyTabbedPage.UnSelectedTextColorProperty.PropertyName ||
                e.PropertyName == MyTabbedPage.StatusBarBackColorProperty.PropertyName ||
                e.PropertyName == MyTabbedPage.BarTextColorProperty.PropertyName ||
                e.PropertyName == MyTabbedPage.BarBackgroundColorProperty.PropertyName)
            {
                SetUpTab();
            }
        }

        void SetUpTab()
		{
			var myTabbed = Element as MyTabbedPage;

			for (var i = 0; i < Tabbed.Children.Count; i++)
			{
				Page child = Tabbed.Children[i];
				var item = myTabbed.TabItems[i];				

				if (string.IsNullOrEmpty(item.Resource)) continue;
				var vc = Platform.GetRenderer(child).ViewController;

				if (myTabbed.IsDefaultColor)
				{
					var icon = SvgToUIImage.GetUIImage(item.Resource, 30, 30);
					vc.TabBarItem.Image = icon;
				}
				else
				{
					vc.TabBarItem.Title = item.Title;
					vc.TabBarItem.SetTitleTextAttributes(
						new UITextAttributes { TextColor = myTabbed.SelectedTextColor.ToUIColor() },
						UIControlState.Selected
					);
					vc.TabBarItem.SetTitleTextAttributes(
						new UITextAttributes { TextColor = myTabbed.UnSelectedTextColor.ToUIColor() },
						UIControlState.Normal
					);

                    var selColor = myTabbed.SelectedTabColor;
                    var unColor = myTabbed.UnselectedTabColor;
					var icon1 = SvgToUIImage.GetUIImage(item.Resource, 30, 30, selColor);
					var icon2 = SvgToUIImage.GetUIImage(item.Resource, 30, 30, unColor);
					vc.TabBarItem.SetFinishedImages(icon1, icon2);
				}

				if (myTabbed.IsTextHidden)
				{
					vc.TabBarItem.Title = null;
					vc.TabBarItem.ImageInsets = new UIEdgeInsets(6, 0, -6, 0);
				}
			}

			if (!myTabbed.IsDefaultColor)
			{
				UITabBarController tabctrl = Platform.GetRenderer(Tabbed.Children[0]).ViewController.TabBarController;
				tabctrl.TabBar.TintColor = (Element as MyTabbedPage).SelectedTextColor.ToUIColor();
			}
		}
	}
}
