using System.Reflection;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Support.Design.Widget;
using MyFormsLibrary.CustomRenderers;
using MyFormsLibrary.Droid.CustomRenderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Platform.Android.AppCompat;
using Android.Views;
using System.Runtime.Remoting.Contexts;
using Android.Support.V4.View;
using System.Linq;

[assembly: ExportRenderer(typeof(TabbedPageEx), typeof(TabbedPageExRenderer))]
namespace MyFormsLibrary.Droid.CustomRenderers
{
	public class TabbedPageExRenderer : TabbedPageRenderer,TabLayout.IOnTabSelectedListener
	{

		private TabbedPageEx tabbedEx;
		private TabLayout tabs;
		private Window window;

		protected override void OnElementChanged(ElementChangedEventArgs<TabbedPage> e) {
			base.OnElementChanged(e);

			var fieldInfo = typeof(TabbedPageRenderer).GetField("_tabLayout", BindingFlags.Instance | BindingFlags.NonPublic);
			tabs = (TabLayout)fieldInfo.GetValue(this);

			fieldInfo = typeof(TabbedPageRenderer).GetField("_viewPager", BindingFlags.Instance | BindingFlags.NonPublic);
			var viewPager = (ViewPager)fieldInfo.GetValue(this);

			viewPager.OffscreenPageLimit = 2; //非表示タブをキャッシュしておく数 タブ数3で2にすれば全ページキャッシュとなる

			window = (Context as FormsAppCompatActivity).Window;

			if (e.OldElement != null) {

			}

			if (e.NewElement != null) {
				var activity = (FormsAppCompatActivity)Context;

				tabbedEx = Element as TabbedPageEx;
				if (!tabbedEx.IsDefaultColor) {
					tabs.SetOnTabSelectedListener(this);
				}



				for (var i = 0; i < tabbedEx.TabAttributes.Count; i++) {
					var attr = tabbedEx.TabAttributes[i];

					if (i==0 && tabbedEx.Parent is NavigationPageEx) {
						var navi = tabbedEx.Parent as NavigationPageEx;
						//tabbedEx.Title = attr.Title;
						navi.BarTextColor = attr.BarTextColor;
						navi.StatusBarBackColor = attr.StatusBarBackColor;
						tabbedEx.Title = (tabbedEx.CurrentPage as Page).Title;
						tabbedEx.CurrentPage.PropertyChanged += CurrentPage_PropertyChanged;
					}

					if (string.IsNullOrEmpty(attr.Resource)) continue;

					var image = attr.Image as NGraphics.BitmapImage;
					var icon = new BitmapDrawable(Context.Resources, image.Bitmap);
					var tab = tabs.GetTabAt(i);
					tab.SetIcon(icon);

					if (!tabbedEx.IsDefaultColor || !attr.IsDefaultColor) {
						var color = tabbedEx.SelectedColor.ToAndroid();

						if (i == 0) {
							if (attr.SelectedColor != Xamarin.Forms.Color.Default) {
								color = attr.SelectedColor.ToAndroid();
							}
							tabs.SetSelectedTabIndicatorColor(color);
							if (attr.StatusBarBackColor != Xamarin.Forms.Color.Default) {
								window.SetStatusBarColor(attr.StatusBarBackColor.ToAndroid());
							}
						}
						else {
							color = tabbedEx.UnSelectedColor.ToAndroid();
							if (attr.UnSelectedColor != Xamarin.Forms.Color.Default) {
								color = attr.UnSelectedColor.ToAndroid();
							}
						}
						tab.Icon.SetColorFilter(color, PorterDuff.Mode.SrcIn);
						tabs.SetTabTextColors(tabbedEx.UnSelectedTextColor.ToAndroid(), tabbedEx.SelectedTextColor.ToAndroid());
					}

					if (tabbedEx.IsTextHidden) {
						tab.SetText("");
					}
				}




				//for (var i = 0; i < Element.Children.Count; i++) {
				//	Page child = Element.Children[i];


				//	if (child is NavigationPageEx) {
				//		var naviex = child as NavigationPageEx;

				//		if (string.IsNullOrEmpty(naviex.Resource)) continue;

				//		var image = naviex.Image as NGraphics.BitmapImage;
				//		var icon = new BitmapDrawable(Context.Resources, image.Bitmap);
				//		var tab = tabs.GetTabAt(i);
				//		tab.SetIcon(icon);

				//		if (!tabbedEx.IsDefaultColor || !naviex.IsDefaultColor) {
				//			var color = tabbedEx.SelectedColor.ToAndroid();

				//			if (i == 0) {
				//				if (naviex.SelectedColor != Xamarin.Forms.Color.Default) {
				//					color = naviex.SelectedColor.ToAndroid();
				//				}
				//				tabs.SetSelectedTabIndicatorColor(color);
				//				if (naviex.StatusBarBackColor != Xamarin.Forms.Color.Default) {
				//					window.SetStatusBarColor(naviex.StatusBarBackColor.ToAndroid());
				//				}
				//			}
				//			else {
				//				color = tabbedEx.UnSelectedColor.ToAndroid();
				//				if (naviex.UnSelectedColor != Xamarin.Forms.Color.Default) {
				//					color = naviex.UnSelectedColor.ToAndroid();
				//				}
				//			}
				//			tab.Icon.SetColorFilter(color, PorterDuff.Mode.SrcIn);
				//			tabs.SetTabTextColors(tabbedEx.UnSelectedTextColor.ToAndroid(),tabbedEx.SelectedTextColor.ToAndroid());
				//		}

				//		if (tabbedEx.IsTextHidden) {
				//			tab.SetText("");
				//		}

				//	}

				//}

			}

		}

		void TabLayout.IOnTabSelectedListener.OnTabReselected(TabLayout.Tab tab) {

		}
		void TabLayout.IOnTabSelectedListener.OnTabSelected(TabLayout.Tab tab) {
			if (Element == null)
				return;

			int selectedIndex = tab.Position;


			var attr = tabbedEx.TabAttributes[selectedIndex];
			if (attr == null) return;

			var color = tabbedEx.SelectedColor.ToAndroid();
			if (attr.SelectedColor != Xamarin.Forms.Color.Default) {
				color = attr.SelectedColor.ToAndroid();
			}

			tab.Icon.SetColorFilter(color, PorterDuff.Mode.SrcIn);
			tabs.SetSelectedTabIndicatorColor(color);
			if (attr.StatusBarBackColor != Xamarin.Forms.Color.Default) {
				window.SetStatusBarColor(attr.StatusBarBackColor.ToAndroid());
			}

			if (tabbedEx.Parent is NavigationPageEx) {
				var navi = tabbedEx.Parent as NavigationPageEx;
				navi.BarTextColor = attr.BarTextColor;
				navi.StatusBarBackColor = attr.StatusBarBackColor;
			}
			tabbedEx.Title = tabbedEx.Children[selectedIndex].Title;
			tabbedEx.Children[selectedIndex].PropertyChanged += CurrentPage_PropertyChanged;

			if (Element.Children.Count > selectedIndex && selectedIndex >= 0) {
				Element.CurrentPage = Element.Children[selectedIndex];
			}


		}

		void TabLayout.IOnTabSelectedListener.OnTabUnselected(TabLayout.Tab tab) {
			int selectedIndex = tab.Position;

			var attr = tabbedEx.TabAttributes[selectedIndex];
			if (attr == null) return;

			var color = tabbedEx.UnSelectedColor.ToAndroid();
			if (attr.UnSelectedColor != Xamarin.Forms.Color.Default) {
				color = attr.UnSelectedColor.ToAndroid();
			}

			tab.Icon.SetColorFilter(color, PorterDuff.Mode.SrcIn);

			tabbedEx.Children[selectedIndex].PropertyChanged -= CurrentPage_PropertyChanged;
		}

		void CurrentPage_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
			if (e.PropertyName == Page.TitleProperty.PropertyName) {
				tabbedEx.Title = (sender as Page).Title;
			}
		}
	}
}

