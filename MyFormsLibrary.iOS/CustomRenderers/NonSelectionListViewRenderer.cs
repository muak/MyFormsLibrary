using System.Collections.Generic;
using MyFormsLibrary.CustomRenderers;
using MyFormsLibrary.iOS.CustomRenderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(NonSelectionListView), typeof(NonSelectionListViewRenderer))]
namespace MyFormsLibrary.iOS.CustomRenderers
{
	public class NonSelectionListViewRenderer:ListViewRenderer,IUITableViewDelegate
	{
		public List<NonSelectionViewCellRenderer> ItemCleanUp { get; set; }

		protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.ListView> e) {
			base.OnElementChanged(e);

			if (e.NewElement != null) {

				//Control.Bounces = false;
				Control.AllowsSelection = false;
                Control.TableFooterView = new UIView();
                ItemCleanUp = new List<NonSelectionViewCellRenderer>();
			}
		}

		protected override void Dispose(bool disposing) {
			foreach (var d in ItemCleanUp) {
				d.CleanUp();
			}
			ItemCleanUp.Clear();
			ItemCleanUp = null;

			base.Dispose(disposing);
		}




	}
}

