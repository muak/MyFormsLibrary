using System;
using System.Collections.Generic;
using System.Windows.Input;
using Android.Views;
using Android.Widget;
using MyFormsLibrary.CustomRenderers;
using MyFormsLibrary.Droid.CustomRenderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using AListView = Android.Widget.ListView;
using AView = Android.Views.View;
using ListView = Xamarin.Forms.ListView;
using MyFormsLibrary.CustomRenderers.Cells;
using System.Reflection;
using Android.Content;

[assembly: ExportRenderer(typeof(NonSelectionListView), typeof(NonSelectionListViewRenderer))]
namespace MyFormsLibrary.Droid.CustomRenderers
{
	public class NonSelectionListViewRenderer : ListViewRenderer, 
		AdapterView.IOnItemLongClickListener,AdapterView.IOnItemClickListener
	{
		public List<NonSelectionViewCellRenderer> ItemCleanUp { get; set; }

		private AListView nativeListView;
		private string contextMenuTitle;
		private IList<MenuItem> contextActions;
		private List<IDisposable> menuDisposable;

        public NonSelectionListViewRenderer(Context context):base(context){}

		protected override void OnElementChanged(ElementChangedEventArgs<ListView> e) {
			base.OnElementChanged(e);


			if (e.NewElement != null) {

				nativeListView = Control;

                              

				//ロングタップ、通常タップを上書き
				nativeListView.OnItemClickListener = this;
				nativeListView.OnItemLongClickListener = this;

				menuDisposable = new List<IDisposable>();
				ItemCleanUp = new List<NonSelectionViewCellRenderer>();
				var main = (FormsAppCompatActivity)Context;
				main.RegisterForContextMenu(nativeListView);
			}

		}

		protected override void Dispose(bool disposing) {
			
			nativeListView.OnItemClickListener = null;
			nativeListView.OnItemLongClickListener = null;

			var main = (FormsAppCompatActivity)Context;
			foreach (var d in menuDisposable) {
				d.Dispose();
			}
			menuDisposable.Clear();
			menuDisposable = null;
			main.UnregisterForContextMenu(nativeListView);

			foreach (var d in ItemCleanUp) {
				d.CleanUp();
			}
			ItemCleanUp.Clear();
			ItemCleanUp = null;

			base.Dispose(disposing);
		}


		protected override void OnCreateContextMenu(IContextMenu menu) {
			base.OnCreateContextMenu(menu);

			foreach (var d in menuDisposable) {
				d.Dispose();
			}
			menuDisposable.Clear();

			if (!string.IsNullOrEmpty(contextMenuTitle)) {
				menu.SetHeaderTitle(contextMenuTitle);
			}

			for (var i = contextActions.Count-1; i >= 0; i--) {
				var action = contextActions[i];
				var menuItem = menu.Add(Android.Views.Menu.None, i, Android.Views.Menu.None, action.Text);

				var clicked = new MenuItemClickListener(action,menuItem);
				menuItem.SetOnMenuItemClickListener(clicked);
				menuDisposable.Add(clicked);
			}

		}

		public bool OnItemLongClick(AdapterView parent, Android.Views.View view, int position, long id) {

			var viewCell =  (view as INativeElementView)?.Element as ViewCell;

			if (viewCell == null) return true;

            contextMenuTitle = (viewCell as IContextMenuCell)?.ContextMenuTitle;
			contextActions = viewCell.ContextActions;

			ShowContextMenu();

			return true;
		}

		public void OnItemClick(AdapterView parent, AView view, int position, long id) {
			//ItemClick Select 無効化
		}

		class MenuItemClickListener : Java.Lang.Object, IMenuItemOnMenuItemClickListener
		{
			private ICommand command;
			private object commandParameter;
			private IMenuItem owner;

			public MenuItemClickListener(MenuItem menu,IMenuItem imenu) {
				command = menu.Command;
				commandParameter = menu.CommandParameter;
				owner = imenu;
			}

			public bool OnMenuItemClick(IMenuItem item) {
				command?.Execute(commandParameter ?? item);
				return true;
			}

			protected override void Dispose(bool disposing) {
				base.Dispose(disposing);
				owner.SetOnMenuItemClickListener(null);
				owner.Dispose();
			}
		}

	}
}

