using System;
using MyFormsLibrary.CustomRenderers;
using MyFormsLibrary.iOS.CustomRenderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using UIKit;
using NGraphics;
using System.ComponentModel;
using Xamarin.Forms.Internals;
using System.Runtime.Remoting.Contexts;

[assembly: ExportRenderer(typeof(TabbedPageEx), typeof(TabbedPageExRenderer))]
namespace MyFormsLibrary.iOS.CustomRenderers
{
	public class TabbedPageExRenderer:TabbedRenderer
	{
		protected override void OnElementChanged(VisualElementChangedEventArgs e) {
			base.OnElementChanged(e);

			if (e.NewElement != null) {

			}
		}

		public override void ViewWillAppear(bool animated) {
			base.ViewWillAppear(animated);

			var tabbedEx = Element as TabbedPageEx;

			UITabBarController tabctrl = Platform.GetRenderer(Tabbed.Children[0]).ViewController.TabBarController;

			for (var i = 0; i < Tabbed.Children.Count; i++) {
				Page child = Tabbed.Children[i];
				var attr = tabbedEx.TabAttributes[i];

				if (child is NavigationPageEx) {
					var naviEx = child as NavigationPageEx;

                    naviEx.BarTextColor = tabbedEx.BarTextColor;
                    if (tabbedEx.BarTextColor != Xamarin.Forms.Color.Default) {
                        naviEx.BarTextColor = attr.BarTextColor;
                    }
					

					if (string.IsNullOrEmpty(attr.Resource)) continue;

					var vc = Platform.GetRenderer(naviEx).ViewController;


					if (tabbedEx.IsDefaultColor && attr.IsDefaultColor) {
						vc.TabBarItem.Image = attr.Image.GetUIImage();
					}
					else {
						vc.TabBarItem.Title = attr.Title;
						vc.TabBarItem.SetTitleTextAttributes(
							new UITextAttributes { TextColor = tabbedEx.SelectedTextColor.ToUIColor() },
							UIControlState.Selected
						);
						vc.TabBarItem.SetTitleTextAttributes(
							new UITextAttributes { TextColor = tabbedEx.UnSelectedTextColor.ToUIColor()},
							UIControlState.Normal
						);
						vc.TabBarItem.SetFinishedImages(attr.Image.GetUIImage(), attr.UnSelectedImage.GetUIImage());
					}


					if (tabbedEx.IsTextHidden) {
						vc.TabBarItem.Title = null;
						vc.TabBarItem.ImageInsets = new UIEdgeInsets(6, 0, -6, 0);
					}

				}

			}

			if (!tabbedEx.IsDefaultColor) {
				tabctrl.TabBar.TintColor = (Element as TabbedPageEx).SelectedTextColor.ToUIColor();
			}
		}


	}
}

