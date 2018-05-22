using System.Reflection;
using System.Runtime.Remoting.Contexts;
using Android.Content;
using Android.Graphics.Drawables;
using Android.Support.Design.Widget;
using Android.Views;
using MyFormsLibrary.CustomRenderers;
using MyFormsLibrary.Droid.CustomRenderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Platform.Android.AppCompat;

[assembly: ExportRenderer(typeof(TabbedPageEx), typeof(TabbedPageExRenderer))]
namespace MyFormsLibrary.Droid.CustomRenderers
{
	public class TabbedPageExRenderer : TabbedPageRenderer, TabLayout.IOnTabSelectedListener
	{
		private TabbedPageEx _tabbedEx;
		private TabLayout _tabs;
		private Window _window;

        public TabbedPageExRenderer(Android.Content.Context context) : base(context) { }

        protected override void OnElementChanged(ElementChangedEventArgs<TabbedPage> e)
		{
			base.OnElementChanged(e);

			var fieldInfo = typeof(TabbedPageRenderer).GetField("_tabLayout", BindingFlags.Instance | BindingFlags.NonPublic);
			_tabs = (TabLayout)fieldInfo.GetValue(this);

			var teardownPage = typeof(TabbedPageRenderer).GetMethod("TeardownPage", BindingFlags.Instance | BindingFlags.NonPublic);

			_window = (Context as FormsAppCompatActivity).Window;

			if (e.OldElement != null) {

			}

			if (e.NewElement != null) {

				_tabbedEx = Element as TabbedPageEx;
				if (!_tabbedEx.IsDefaultColor) {
                    //OnTabSelectedListenerを上書きする
                    _tabs.AddOnTabSelectedListener(this);
					//_tabs.SetOnTabSelectedListener(this);
				}

				// https://github.com/xamarin/Xamarin.Forms/blob/master/Xamarin.Forms.Platform.Android/AppCompat/TabbedPageRenderer.cs#L297
				// OnPagePropertyChangedでいらんことしてるので即TeardownPageを呼び出して解除する
				// （TabのTextを子ページのTitleと連動させているが、TabのTextはTabAttributeで設定するようにしているので不要）
				foreach (var page in Element.Children) {
					teardownPage.Invoke(this, new object[] { page });
				}

				for (var i = 0; i < _tabbedEx.TabAttributes.Count; i++) {
					var attr = _tabbedEx.TabAttributes[i];

					if (i == 0 && _tabbedEx.Parent is NavigationPageEx) {
						var navi = _tabbedEx.Parent as NavigationPageEx;
						navi.BarTextColor = _tabbedEx.BarTextColor;
						if (attr.BarTextColor != Xamarin.Forms.Color.Default) {
							navi.BarTextColor = attr.BarTextColor;
						}
						navi.StatusBarBackColor = _tabbedEx.StatusBarBackColor;
						if (attr.StatusBarBackColor != Xamarin.Forms.Color.Default) {
							navi.StatusBarBackColor = attr.StatusBarBackColor;
						}
						_tabbedEx.Title = (_tabbedEx.CurrentPage as Page).Title;
						_tabbedEx.CurrentPage.PropertyChanged += CurrentPage_PropertyChanged;

						var renderer = Platform.GetRenderer(navi) as NavigationPageExRenderer;
						renderer.UpdateMenu();
					}


					if (string.IsNullOrEmpty(attr.Resource)) continue;

					var bitmap = SvgToBitmap.GetBitmap(attr.Resource, 24, 24);
					var icon = new BitmapDrawable(Context.Resources, bitmap);
					var tab = _tabs.GetTabAt(i);
					tab.SetIcon(icon);

					if (!_tabbedEx.IsDefaultColor || !attr.IsDefaultColor) {
						var color = _tabbedEx.SelectedColor.ToAndroid();

						if (i == 0) {
							if (attr.SelectedColor != Xamarin.Forms.Color.Default) {
								color = attr.SelectedColor.ToAndroid();
							}
							_tabs.SetSelectedTabIndicatorColor(color);
							if (_tabbedEx.StatusBarBackColor != Xamarin.Forms.Color.Default) {
								_window.SetStatusBarColor(_tabbedEx.StatusBarBackColor.ToAndroid());
							}
							else if (attr.StatusBarBackColor != Xamarin.Forms.Color.Default) {
								_window.SetStatusBarColor(attr.StatusBarBackColor.ToAndroid());
							}
						}
						else {
							color = _tabbedEx.UnSelectedColor.ToAndroid();
							if (attr.UnSelectedColor != Xamarin.Forms.Color.Default) {
								color = attr.UnSelectedColor.ToAndroid();
							}
						}
						tab.Icon.SetTint(color);
						_tabs.SetTabTextColors(_tabbedEx.UnSelectedTextColor.ToAndroid(), _tabbedEx.SelectedTextColor.ToAndroid());
					}

					if (_tabbedEx.IsTextHidden) {
						tab.SetText("");
					}
				}
			}
		}

		void TabLayout.IOnTabSelectedListener.OnTabReselected(TabLayout.Tab tab)
		{

		}
		void TabLayout.IOnTabSelectedListener.OnTabSelected(TabLayout.Tab tab)
		{
			if (_tabbedEx == null)
				return;

			int selectedIndex = tab.Position;


			var attr = _tabbedEx.TabAttributes[selectedIndex];
			if (attr == null) return;

			var color = _tabbedEx.SelectedColor.ToAndroid();
			if (attr.SelectedColor != Xamarin.Forms.Color.Default) {
				color = attr.SelectedColor.ToAndroid();
			}

			tab.Icon.SetTint(color);
			_tabs.SetSelectedTabIndicatorColor(color);

			if (_tabbedEx.StatusBarBackColor != Xamarin.Forms.Color.Default) {
				_window.SetStatusBarColor(_tabbedEx.StatusBarBackColor.ToAndroid());
			}
			else if (attr.StatusBarBackColor != Xamarin.Forms.Color.Default) {
				_window.SetStatusBarColor(attr.StatusBarBackColor.ToAndroid());
			}

			if (_tabbedEx.Parent is NavigationPageEx) {
				var navi = _tabbedEx.Parent as NavigationPageEx;
				navi.BarTextColor = _tabbedEx.BarTextColor;
				if (attr.BarTextColor != Xamarin.Forms.Color.Default) {
					navi.BarTextColor = attr.BarTextColor;
				}
				navi.StatusBarBackColor = _tabbedEx.StatusBarBackColor;
				if (attr.StatusBarBackColor != Xamarin.Forms.Color.Default) {
					navi.StatusBarBackColor = attr.StatusBarBackColor;
				}
			}

			_tabbedEx.Title = _tabbedEx.Children[selectedIndex].Title;
			_tabbedEx.Children[selectedIndex].PropertyChanged += CurrentPage_PropertyChanged;

			if (Element.Children.Count > selectedIndex && selectedIndex >= 0) {
				Element.CurrentPage = Element.Children[selectedIndex];
			}

		}

		void TabLayout.IOnTabSelectedListener.OnTabUnselected(TabLayout.Tab tab)
		{
			if (_tabbedEx == null) return;

			int selectedIndex = tab.Position;

			var attr = _tabbedEx.TabAttributes[selectedIndex];
			if (attr == null) return;

			var color = _tabbedEx.UnSelectedColor.ToAndroid();
			if (attr.UnSelectedColor != Xamarin.Forms.Color.Default) {
				color = attr.UnSelectedColor.ToAndroid();
			}

			tab.Icon.SetTint(color);

			_tabbedEx.Children[selectedIndex].PropertyChanged -= CurrentPage_PropertyChanged;
		}

		void CurrentPage_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == Page.TitleProperty.PropertyName) {
				_tabbedEx.Title = (sender as Page).Title;
			}
		}
	}
}

