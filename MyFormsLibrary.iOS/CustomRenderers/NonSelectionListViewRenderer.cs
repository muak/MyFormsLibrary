using System.Collections.Generic;
using MyFormsLibrary.CustomRenderers;
using MyFormsLibrary.iOS.CustomRenderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.iOS;
using CoreAnimation;
using System;
using System.Collections.Specialized;
using System.Reflection;

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
                ItemCleanUp = new List<NonSelectionViewCellRenderer>();

                var mCollectionChanged = typeof(ListViewRenderer).GetMethod("OnCollectionChanged", BindingFlags.Instance | BindingFlags.NonPublic);
               
                Action<ITemplatedItemsList<Cell>> removeEvent = (cell) => {
                    Delegate eventMethod = mCollectionChanged.CreateDelegate(typeof(NotifyCollectionChangedEventHandler), this);
                    cell.GetType().GetEvent("CollectionChanged").RemoveEventHandler(cell, eventMethod);
                };

                var templatedItems = ((ITemplatedItemsView<Cell>)e.NewElement).TemplatedItems;
                removeEvent(templatedItems);
                templatedItems.CollectionChanged += TemplatedItems_CollectionChanged;

			}
		}

		protected override void Dispose(bool disposing) {
            if(disposing){
                var templatedItems = ((ITemplatedItemsView<Cell>)Element).TemplatedItems;
                templatedItems.CollectionChanged -= TemplatedItems_CollectionChanged;
            }
			foreach (var d in ItemCleanUp) {
				d.CleanUp();
			}
			ItemCleanUp.Clear();
			ItemCleanUp = null;

			base.Dispose(disposing);
		}


        void TemplatedItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Control.ReloadData();          
        }




    }
}

