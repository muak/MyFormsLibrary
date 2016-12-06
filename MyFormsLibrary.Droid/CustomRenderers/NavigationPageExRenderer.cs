using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using MyFormsLibrary.CustomRenderers;
using MyFormsLibrary.Droid.CustomRenderers;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Platform.Android.AppCompat;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Android.Support.V7.Graphics.Drawable;

[assembly: ExportRenderer(typeof(NavigationPageEx), typeof(NavigationPageExRenderer))]
namespace MyFormsLibrary.Droid.CustomRenderers
{
	public class NavigationPageExRenderer:NavigationPageRenderer
	{
		private ToolbarTracker toolbarTracker;
		private Toolbar toolbar;


		public Android.Graphics.Color ForeColor { get {
				return (Element as NavigationPage).BarTextColor.ToAndroid();	
		} }
		public bool IsForeColorDefault { get {
				return (Element as NavigationPage).BarTextColor == Xamarin.Forms.Color.Default;
		} }

		protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<NavigationPage> e) {
			base.OnElementChanged(e);


			if (e.OldElement != null) {
				toolbarTracker.CollectionChanged -= toolbarCollectionChanged;
				Element.Pushed -= pagePusshed;
				Element.Popped -= pagePopped;
			}

			if (e.NewElement != null) {
				var fieldInfo = typeof(NavigationPageRenderer).GetField("_toolbarTracker", BindingFlags.Instance | BindingFlags.NonPublic);
				toolbarTracker = (ToolbarTracker)fieldInfo.GetValue(this);

				fieldInfo = typeof(NavigationPageRenderer).GetField("_toolbar", BindingFlags.Instance | BindingFlags.NonPublic);
				toolbar = (Toolbar)fieldInfo.GetValue(this);

				toolbarTracker.CollectionChanged += toolbarCollectionChanged;



				toolbar.SetTitleTextColor(ForeColor);
				toolbar.SetSubtitleTextColor(ForeColor);
				Element.Pushed += pagePusshed;
				Element.Popped += pagePopped;


				UpdateMenu();

				if (Element.Parent == null) {
					var statusBarColor = (Element as NavigationPageEx).StatusBarBackColor;
					if (statusBarColor != Xamarin.Forms.Color.Default) {
						(Context as FormsAppCompatActivity).Window.SetStatusBarColor(statusBarColor.ToAndroid());
					}
				}


			}
		}


		protected override void Dispose(bool disposing) {
			
			var methodInfo = typeof(NavigationPageRenderer).GetMethod("HandleToolbarItemPropertyChanged", BindingFlags.Instance | BindingFlags.NonPublic);

			Action<ToolbarItem> removeEvent = (item) => {
		
				Delegate eventMethod = methodInfo.CreateDelegate(typeof(PropertyChangedEventHandler),this);
				item.GetType().GetEvent("PropertyChanged").RemoveEventHandler(item, eventMethod);
			};

			foreach (var item in toolbarTracker.ToolbarItems) {
				removeEvent(item);
				item.PropertyChanged -= HandleToolbarItemPropertyChanged;
			}
			toolbarTracker.CollectionChanged -= toolbarCollectionChanged;

			Element.Pushed -= pagePusshed;
			Element.Popped -= pagePopped;


			toolbar = null;
			toolbarTracker = null;

			base.Dispose(disposing);

		}

		protected override void OnAttachedToWindow() {
			base.OnAttachedToWindow();

			//OffscreenPageLimitを超えてDisposeされてから再度生成された場合にBackボタンが初期化
			//されるので再度上書きする。OffscreenPageLimitを超えない設定なら不要

			//if (toolbar.NavigationIcon != null) {
			//	Device.BeginInvokeOnMainThread(async() => {
			//		await Task.Delay(500);
			//		toolbar.NavigationIcon.SetColorFilter(foreColor, PorterDuff.Mode.SrcIn);
			//	});
			//}
		}

		async void pagePusshed(object sender, EventArgs e) {

			var limit = 100;
			await Task.Delay(250);
			while (toolbar.NavigationIcon == null) {
				if (limit == 0) break;
				await Task.Delay(250);
				limit--;
			}
			toolbar.NavigationIcon.SetColorFilter(ForeColor, PorterDuff.Mode.SrcIn);
		}

		void pagePopped(object sender, NavigationEventArgs e) {
			Task.Run(async () => {
				await Task.Delay(50);
				toolbar.NavigationIcon.SetColorFilter(ForeColor, PorterDuff.Mode.SrcIn);
			});
		}

		void toolbarCollectionChanged(object sender, EventArgs e) {
			UpdateMenu();
		}

		void HandleToolbarItemPropertyChanged(object sender, PropertyChangedEventArgs e) {
			if (e.PropertyName == "IsEnabled" || 
			    e.PropertyName == MenuItem.TextProperty.PropertyName || 
			    e.PropertyName == MenuItem.IconProperty.PropertyName ||
				e.PropertyName == ToolbarItemEx.IsVisibleProperty.PropertyName ||
				 e.PropertyName == ToolbarItemEx.IsEnabledExProperty.PropertyName){
				UpdateMenu();
			}

		}



		public void UpdateMenu() {
			foreach (ToolbarItem item in toolbarTracker.ToolbarItems)
				item.PropertyChanged -= HandleToolbarItemPropertyChanged;
			
			IMenu menu = toolbar.Menu;


			for (var i = 0; i < toolbarTracker.ToolbarItems.Count(); i++) {
				var item = toolbarTracker.ToolbarItems.ElementAt(i);
				item.PropertyChanged += HandleToolbarItemPropertyChanged;
				var menuItem = menu.GetItem(i);


				if (item.Order != ToolbarItemOrder.Secondary) {
					if (item is ToolbarItemEx) {
						var itemEx = item as ToolbarItemEx;

						if (!itemEx.IsVisible) {
							menuItem.SetVisible(false);
							continue;
						}

						menuItem.SetVisible(true);
						menuItem.SetEnabled(itemEx.IsEnabledEx);

						if (!string.IsNullOrEmpty(itemEx.Resource)) {
							
							var image = itemEx.Image as NGraphics.BitmapImage;
							var icon = new BitmapDrawable(Context.Resources, image.Bitmap);
							if (icon != null) {
								menuItem.SetIcon(icon);
								if (!IsForeColorDefault && itemEx.IsEnabledEx) {
									menuItem.Icon.SetColorFilter(ForeColor, PorterDuff.Mode.SrcIn);
								}
								else if (!itemEx.IsEnabledEx) {
									menuItem.Icon.SetColorFilter(Android.Graphics.Color.Rgb(242,242,242), PorterDuff.Mode.SrcIn);
								}
							}
						}

					}
				}

			}

		}

	}
}

