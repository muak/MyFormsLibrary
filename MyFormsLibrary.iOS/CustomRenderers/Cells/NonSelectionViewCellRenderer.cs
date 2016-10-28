using System;
using CoreGraphics;
using MyFormsLibrary.CustomRenderers;
using MyFormsLibrary.iOS.CustomRenderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(NonSelectionViewCell), typeof(NonSelectionViewCellRenderer))]
namespace MyFormsLibrary.iOS.CustomRenderers
{
	public class NonSelectionViewCellRenderer:ViewCellRenderer
	{
		private UITapGestureRecognizer tapGesture;
		private ContentPage page;
		private UIView overlay;
		private UITableViewCell cell;


		public override UITableViewCell GetCell(Cell item, UITableViewCell reusableCell, UITableView tv) {
			var viewCell = item as NonSelectionViewCell;

            var list = Platform.GetRenderer(item.Parent as NonSelectionListView) as NonSelectionListViewRenderer;

			cell = reusableCell;
			if (cell != null) {
				cell.RemoveGestureRecognizer(tapGesture);
				tapGesture.Dispose();
			}

			cell = base.GetCell(item, reusableCell, tv);

            if (viewCell.Command != null) {
                
                var elm = viewCell.Parent;
                while (elm != null) {
                    if (elm is ContentPage) {
                        break;
                    }
                    elm = elm.Parent;
                }

                page = elm as ContentPage;



                tapGesture = new UITapGestureRecognizer((obj) => {
                    cell.RemoveGestureRecognizer(tapGesture);

                    overlay = new UIView();
                    overlay.Frame = new CGRect(0, 0, cell.Bounds.Width, cell.Bounds.Height);
                    overlay.BackgroundColor = UIColor.FromHSBA(0, 0, 0.3f, 0.3f);

                    cell.AddSubview(overlay);
                    cell.BringSubviewToFront(overlay);
    				page.Appearing += Page_Appearing;

    				viewCell.Command?.Execute(viewCell.CommandParameter ?? viewCell);
    			});


                cell.AddGestureRecognizer(tapGesture);
            }

            list.ItemCleanUp.Add(this);

			return cell;
		}

		public void CleanUp() {
            if (tapGesture != null) {
                cell.RemoveGestureRecognizer(tapGesture);
                tapGesture.Dispose();
            }
			cell = null;
			tapGesture = null;
			page = null;
			if (overlay != null) {
				overlay.Dispose();
				overlay = null;
			}
		}


		async void Page_Appearing(object sender, EventArgs e) {

			await UIView.AnimateAsync(0.2f, () => {
						overlay.Alpha = 0;
			});
			overlay.RemoveFromSuperview();
			overlay.Dispose();
			overlay = null;
			cell.AddGestureRecognizer(tapGesture);
			page.Appearing -= Page_Appearing;
		}


	}
}

