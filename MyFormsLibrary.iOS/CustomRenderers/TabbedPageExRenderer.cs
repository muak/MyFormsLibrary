using MyFormsLibrary.CustomRenderers;
using MyFormsLibrary.iOS.CustomRenderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(TabbedPageEx), typeof(TabbedPageExRenderer))]
namespace MyFormsLibrary.iOS.CustomRenderers
{
	public class TabbedPageExRenderer : TabbedRenderer
	{
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
			if(e.OldElement != null)
            {
				e.OldElement.PropertyChanged -= OnPropertyChanged;
            }
			if(e.NewElement != null)
            {
				e.NewElement.PropertyChanged += OnPropertyChanged;
			}            
        }

        private void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == TabbedPageEx.SelectedColorProperty.PropertyName ||
				e.PropertyName == TabbedPageEx.UnSelectedColorProperty.PropertyName ||
				e.PropertyName == TabbedPageEx.SelectedTextColorProperty.PropertyName ||
				e.PropertyName == TabbedPageEx.UnSelectedTextColorProperty.PropertyName ||
				e.PropertyName == TabbedPageEx.StatusBarBackColorProperty.PropertyName ||
				e.PropertyName == TabbedPage.BarTextColorProperty.PropertyName ||
				e.PropertyName == TabbedPage.BarBackgroundColorProperty.PropertyName)
			{
				SetUpTab();
            }
        }

        protected override void Dispose(bool disposing)
        {
			if(disposing)
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

		void SetUpTab()
        {
			var tabbedEx = Element as TabbedPageEx;

			for (var i = 0; i < Tabbed.Children.Count; i++)
			{
				Page child = Tabbed.Children[i];
				var attr = tabbedEx.TabAttributes[i];

				if (child is NavigationPageEx)
				{
					var naviEx = child as NavigationPageEx;

					naviEx.BarTextColor = tabbedEx.BarTextColor;
					if (attr.BarTextColor != Xamarin.Forms.Color.Default)
					{
						naviEx.BarTextColor = attr.BarTextColor;
					}
				}

				if (string.IsNullOrEmpty(attr.Resource)) continue;
				var vc = Platform.GetRenderer(child).ViewController;

				if (tabbedEx.IsDefaultColor && attr.IsDefaultColor)
				{
					var icon = SvgToUIImage.GetUIImage(attr.Resource, 30, 30);
					vc.TabBarItem.Image = icon;
				}
				else
				{
					vc.TabBarItem.Title = attr.Title;
					vc.TabBarItem.SetTitleTextAttributes(
						new UITextAttributes { TextColor = tabbedEx.SelectedTextColor.ToUIColor() },
						UIControlState.Selected
					);
					vc.TabBarItem.SetTitleTextAttributes(
						new UITextAttributes { TextColor = tabbedEx.UnSelectedTextColor.ToUIColor() },
						UIControlState.Normal
					);

					var selColor = attr.SelectedColor == Xamarin.Forms.Color.Default ? tabbedEx.SelectedColor : attr.SelectedColor;
					var unColor = attr.UnSelectedColor == Xamarin.Forms.Color.Default ? tabbedEx.UnSelectedColor : attr.UnSelectedColor;
					var icon1 = SvgToUIImage.GetUIImage(attr.Resource, 30, 30, selColor);
					var icon2 = SvgToUIImage.GetUIImage(attr.Resource, 30, 30, unColor);
					vc.TabBarItem.SetFinishedImages(icon1, icon2);
				}

				if (tabbedEx.IsTextHidden)
				{
					vc.TabBarItem.Title = null;
					vc.TabBarItem.ImageInsets = new UIEdgeInsets(6, 0, -6, 0);
				}
			}

			if (!tabbedEx.IsDefaultColor)
			{
				UITabBarController tabctrl = Platform.GetRenderer(Tabbed.Children[0]).ViewController.TabBarController;
				tabctrl.TabBar.TintColor = (Element as TabbedPageEx).SelectedTextColor.ToUIColor();
			}
		}
	}
}

