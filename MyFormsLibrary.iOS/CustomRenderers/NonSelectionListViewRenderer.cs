using System;
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
        private IDisposable _offsetObserver;
        private bool _isReachedBottom;
        NonSelectionListView MyListView => Element as NonSelectionListView;

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.ListView> e) {

            base.OnElementChanged(e);

            if(e.OldElement != null)
            {
                _offsetObserver?.Dispose();
                _offsetObserver = null;
            }
           
            if (e.NewElement != null)
            {
				 //Control.Bounces = false;
				 Control.AllowsSelection = false;
                ItemCleanUp = new List<NonSelectionViewCellRenderer>();

                _offsetObserver = Control.AddObserver("contentOffset",
                             Foundation.NSKeyValueObservingOptions.New, OffsetChanged);

                var myListVIew = e.NewElement as NonSelectionListView;
                myListVIew.SetLoadMoreCompletionAction = (isEnd) => {
                    _isReachedBottom = isEnd;
                };
            }
		}

        private void OffsetChanged(Foundation.NSObservedChange obj)
        {
            if (_isReachedBottom || MyListView.LoadMoreCommand == null || Control.ContentSize.Height < Control.Bounds.Height)
            {
                return;
            }

            var scrollView = Control as UIScrollView;
            if (scrollView.ContentSize.Height <= scrollView.ContentOffset.Y + scrollView.Bounds.Height)
            {
                _isReachedBottom = true;
                MyListView?.LoadMoreCommand?.Execute(null);
            }

        }

        protected override void Dispose(bool disposing) 
        {
           if(disposing)
            {
                _offsetObserver.Dispose();
                _offsetObserver = null;
                foreach (var d in ItemCleanUp)
                {
                    d.CleanUp();
                }
                ItemCleanUp.Clear();
                ItemCleanUp = null;
            }
			

			base.Dispose(disposing);
		}
    }
}

