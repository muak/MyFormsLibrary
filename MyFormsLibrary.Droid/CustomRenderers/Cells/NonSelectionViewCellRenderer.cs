using System;
using Xamarin.Forms.Platform.Android;
using MyFormsLibrary.CustomRenderers;
using Xamarin.Forms;
using Android.Views;
using Android.Widget;
using MyFormsLibrary.Droid.CustomRenderers;
using Javax.Xml.Xpath;
using System.Collections.Generic;

[assembly: ExportRenderer(typeof(NonSelectionViewCell), typeof(NonSelectionViewCellRenderer))]
namespace MyFormsLibrary.Droid.CustomRenderers
{

	public class NonSelectionViewCellRenderer:ViewCellRenderer
	{
		private NonSelectionViewCell viewCell;
		private Android.Views.ViewGroup view;
		private FrameLayout layer;

		protected override Android.Views.View GetCellCore(Xamarin.Forms.Cell item, Android.Views.View convertView, Android.Views.ViewGroup parent, Android.Content.Context context) {
			viewCell = item as NonSelectionViewCell;

			var list = Platform.GetRenderer(item.Parent as NonSelectionListView) as NonSelectionListViewRenderer;


			if (convertView != null) {
				convertView.LongClick -= View_LongClick;
				convertView.Click -= View_Click;
				convertView.Touch -= View_Touch;
			}

			view =  base.GetCellCore(item, convertView, parent, context) as ViewGroup;

            view.LongClick += View_LongClick;
            if (viewCell.Command != null) {
                layer = new FrameLayout(context);
                layer.LayoutParameters = new ViewGroup.LayoutParams(-1, -1);
                layer.SetBackgroundColor(Android.Graphics.Color.HSVToColor(100, new float[] { 0, 0, 0.2f }));


                view.Click += View_Click;
			    view.Touch += View_Touch;
            }
			list.ItemCleanUp.Add(this);

			return view;
		}

		public void CleanUp() {
            if (view != null) {
                view.LongClick -= View_LongClick;
                view.Click -= View_Click;
                view.Touch -= View_Touch;
                view = null;
            }
            if (layer != null) {
                layer.Dispose();
                layer = null;
            }
			viewCell = null;
		}

		void View_LongClick(object sender, Android.Views.View.LongClickEventArgs e) {
			e.Handled = false;
		}

		void View_Click(object sender, EventArgs e) {
			viewCell.Command?.Execute(viewCell.CommandParameter ?? viewCell);
		}

		void View_Touch(object sender, Android.Views.View.TouchEventArgs e) {
			if (e.Event.Action == MotionEventActions.Down) {
				view.AddView(layer);
				layer.Top = 0;
				layer.Left = 0;
				layer.Right = view.Right;
				layer.Bottom = view.Bottom;
				layer.BringToFront();
			}
			if (e.Event.Action == MotionEventActions.Up || e.Event.Action == MotionEventActions.Cancel) {
				view.RemoveView(layer);
			}

			e.Handled = false;
		}
	}
}

