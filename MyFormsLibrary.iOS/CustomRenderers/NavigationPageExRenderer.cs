using System.Linq;
using NGraphics;
using MyFormsLibrary.CustomRenderers;
using MyFormsLibrary.iOS.CustomRenderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Foundation;
using System.Collections.Generic;
using Xamarin.Forms.Internals;

[assembly: ExportRenderer(typeof(NavigationPageEx), typeof(NavigationPageExRenderer))]
namespace MyFormsLibrary.iOS.CustomRenderers
{
	public class NavigationPageExRenderer : NavigationRenderer
	{
		
		protected override void OnElementChanged(VisualElementChangedEventArgs e) {
			base.OnElementChanged(e);

			if (e.NewElement != null) {
				
			}
		}


		public override void PushViewController(UIViewController viewController, bool animated) {
			base.PushViewController(viewController, animated);

			SetIcons();
		}

		protected override void Dispose(bool disposing) {
			var formsItems = (Element as NavigationPageEx).CurrentPage
														  .ToolbarItems
														  .Where(x => x.Order != ToolbarItemOrder.Secondary)
														  .OrderByDescending(x => x.Priority);

			foreach (var item in formsItems) {
				var itemEx = item as ToolbarItemEx;
				if (itemEx == null) continue;
				itemEx.PropertyChanged -= ItemEx_PropertyChanged;
			}

			base.Dispose(disposing);
		}

		void SetIcons() {
			

			var formsItems = (Element as NavigationPageEx).CurrentPage
														  .ToolbarItems
														  .Where(x => x.Order != ToolbarItemOrder.Secondary)
														  .OrderByDescending(x => x.Priority);

			var ctrl = ViewControllers.Last();
			var nativeItems = ctrl.NavigationItem.RightBarButtonItems;

			var ncnt = -1;
			foreach (var item in formsItems) {
				ncnt++;
				var itemEx = item as ToolbarItemEx;
				if (itemEx == null) continue;

				itemEx.PropertyChanged -= ItemEx_PropertyChanged;
				itemEx.PropertyChanged += ItemEx_PropertyChanged;

				if (!itemEx.IsVisible) {
					nativeItems[ncnt].Image = null;
					nativeItems[ncnt].Title = null;
					continue;
				}
				if (string.IsNullOrEmpty(itemEx.Resource)) continue;


				nativeItems[ncnt].Image = itemEx.Image.GetUIImage();
				nativeItems[ncnt].Title = null;
				nativeItems[ncnt].Style = UIBarButtonItemStyle.Plain;
				nativeItems[ncnt].Enabled = itemEx.IsEnabledEx;
			}

		}


		void ItemEx_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
			if (e.PropertyName == ToolbarItemEx.IsEnabledExProperty.PropertyName ||
			   e.PropertyName == ToolbarItemEx.IsVisibleProperty.PropertyName) {
				SetIcons();
			}
		}
	}
}

