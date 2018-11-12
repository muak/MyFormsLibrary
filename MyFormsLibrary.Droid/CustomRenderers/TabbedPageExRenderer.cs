using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Contexts;
using Android.Content;
using Android.Graphics.Drawables;
using Android.Support.Design.Internal;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using MyFormsLibrary.CustomRenderers;
using MyFormsLibrary.Droid.CustomRenderers;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Platform.Android.AppCompat;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using XF = Xamarin.Forms;

[assembly: XF.ExportRenderer(typeof(TabbedPageEx), typeof(TabbedPageExRenderer))]
namespace MyFormsLibrary.Droid.CustomRenderers
{
    public class TabbedPageExRenderer : TabbedPageRenderer, TabLayout.IOnTabSelectedListener, BottomNavigationView.IOnNavigationItemSelectedListener
    {
		TabbedPageEx _tabbedEx;
		TabLayout _tabs;
        BottomNavigationView _bottomNavigationView;
		Window _window;

        bool IsBottomTabPlacement => (Element != null) && Element.OnThisPlatform().GetToolbarPlacement() == ToolbarPlacement.Bottom;

        public TabbedPageExRenderer(Android.Content.Context context) : base(context) { }

        protected override void OnElementChanged(ElementChangedEventArgs<XF.TabbedPage> e)
		{
			base.OnElementChanged(e);

			var fieldInfo = typeof(TabbedPageRenderer).GetField("_tabLayout", BindingFlags.Instance | BindingFlags.NonPublic);
			_tabs = (TabLayout)fieldInfo.GetValue(this);

            fieldInfo = typeof(TabbedPageRenderer).GetField("_bottomNavigationView", BindingFlags.Instance | BindingFlags.NonPublic);
            _bottomNavigationView = (BottomNavigationView)fieldInfo.GetValue(this);

            var teardownPage = typeof(TabbedPageRenderer).GetMethod("TeardownPage", BindingFlags.Instance | BindingFlags.NonPublic);

			_window = (Context as FormsAppCompatActivity).Window;

			if (e.OldElement != null) {

			}

			if (e.NewElement != null) {

				_tabbedEx = Element as TabbedPageEx;

				if (!_tabbedEx.IsDefaultColor && !IsBottomTabPlacement) {
                    //OnTabSelectedListenerを上書きする
                    _tabs.AddOnTabSelectedListener(this);
				}

                if(IsBottomTabPlacement){
                    _bottomNavigationView.SetOnNavigationItemSelectedListener(this);
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
						if (!attr.BarTextColor.IsDefault) {
							navi.BarTextColor = attr.BarTextColor;
						}
						navi.StatusBarBackColor = _tabbedEx.StatusBarBackColor;
						if (!attr.StatusBarBackColor.IsDefault) {
							navi.StatusBarBackColor = attr.StatusBarBackColor;
						}
                        var curPage = (_tabbedEx.CurrentPage as XF.Page);
                        _tabbedEx.Title = curPage.Title;

                        var titleView = XF.NavigationPage.GetTitleView(curPage);

                        if(titleView != null)
                        {
                            XF.NavigationPage.SetTitleView(_tabbedEx, titleView);
                            titleView.BindingContext = curPage.BindingContext;
                        }

						_tabbedEx.CurrentPage.PropertyChanged += CurrentPage_PropertyChanged;

						var renderer = Platform.GetRenderer(navi) as NavigationPageExRenderer;
						renderer.UpdateMenu();
					}


					if (string.IsNullOrEmpty(attr.Resource)) continue;

					var bitmap = SvgToBitmap.GetBitmap(attr.Resource, 24, 24);
					var icon = new BitmapDrawable(Context.Resources, bitmap);


                    if(IsBottomTabPlacement){
                        var menuItem = _bottomNavigationView.Menu.GetItem(i);
                        menuItem.SetIcon(icon);
                        menuItem.SetTitle(attr.Title);
                        continue;
                    }

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

                if (IsBottomTabPlacement) 
                {
                    Element.OnThisPlatform().SetBarItemColor(_tabbedEx.UnSelectedColor);
                    Element.OnThisPlatform().SetBarSelectedItemColor(_tabbedEx.SelectedColor);
                    Element.OnThisPlatform().SetIsSmoothScrollEnabled(false);
                    Element.OnThisPlatform().SetIsSwipePagingEnabled(false);
                    _bottomNavigationView.SetShiftMode(false, false,_tabbedEx.SelectedTextColor.ToAndroid(),_tabbedEx.UnSelectedTextColor.ToAndroid());
                }
            }
		}

        bool BottomNavigationView.IOnNavigationItemSelectedListener.OnNavigationItemSelected(IMenuItem item)
        {
            _tabbedEx.CurrentPage.PropertyChanged -= CurrentPage_PropertyChanged;

            if (!base.OnNavigationItemSelected(item))
                return false;

            _tabbedEx.Children[item.Order].PropertyChanged += CurrentPage_PropertyChanged;

            var selectedPage = _tabbedEx.Children[item.Order];
            _tabbedEx.Title = selectedPage.Title;

            var titleView = XF.NavigationPage.GetTitleView(selectedPage);
            if (titleView != null) {
                XF.NavigationPage.SetTitleView(_tabbedEx, titleView);
                titleView.BindingContext = selectedPage.BindingContext;
            }

            return true;
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
            var selectedPage = _tabbedEx.Children[selectedIndex];
            _tabbedEx.Title = selectedPage.Title;

            var titleView = XF.NavigationPage.GetTitleView(selectedPage);

            if (titleView != null)
            {
                XF.NavigationPage.SetTitleView(_tabbedEx, titleView);
                titleView.BindingContext = selectedPage.BindingContext;
            }

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
			if (e.PropertyName == XF.Page.TitleProperty.PropertyName) 
            {
				_tabbedEx.Title = (sender as XF.Page).Title;
			}
            else if(e.PropertyName == XF.NavigationPage.TitleViewProperty.PropertyName) 
            {
                var titleView = XF.NavigationPage.GetTitleView(sender as XF.Page);
                if (titleView != null) {
                    XF.NavigationPage.SetTitleView(_tabbedEx, titleView);
                    titleView.BindingContext = (sender as XF.Page).BindingContext;
                }
            }
		}


    }

    public static class BottomNavigationHelpers
    {
        public static void SetShiftMode(this BottomNavigationView bottomNavigationView, bool enableShiftMode, bool enableItemShiftMode,Android.Graphics.Color? selectedTextColor = null,Android.Graphics.Color? unselectedTextColor = null) {
            try {
                var menuView = bottomNavigationView.GetChildAt(0) as BottomNavigationMenuView;
                if (menuView == null) {
                    System.Diagnostics.Debug.WriteLine("Unable to find BottomNavigationMenuView");
                    return;
                }

                SetField(menuView.Class, menuView, "mShiftingMode", false);

                for (int i = 0; i < menuView.ChildCount; i++) {
                    var item = menuView.GetChildAt(i) as BottomNavigationItemView;
                    if (item == null)
                        continue;

                    item.SetShiftingMode(enableItemShiftMode);
                    item.SetChecked(item.ItemData.IsChecked);
                    SetField(item.Class, item, "mShiftAmount", 0);
                    SetField(item.Class, item, "mScaleUpFactor", 1);
                    SetField(item.Class, item, "mScaleDownFactor", 1);

                    var mLargeLabel = GetField<TextView>(item.Class, item, "mLargeLabel");
                    var mSmallLabel = GetField<TextView>(item.Class, item, "mSmallLabel");

                    mLargeLabel.SetTextSize(Android.Util.ComplexUnitType.Px, mSmallLabel.TextSize);
                    if(selectedTextColor != null)
                    {
                        mLargeLabel.SetTextColor(selectedTextColor.Value);
                    }
                    if(unselectedTextColor != null)
                    {
                        mSmallLabel.SetTextColor(unselectedTextColor.Value);
                    }
                }

                menuView.UpdateMenuView();
            }
            catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine($"Unable to set shift mode: {ex}");
            }
        }

        static T GetField<T>(Java.Lang.Class targetClass, Java.Lang.Object instance, string fieldName) where T : Java.Lang.Object
        {
            try {
                var field = targetClass.GetDeclaredField(fieldName);
                field.Accessible = true;
                var value = field.Get(instance);
                field.Accessible = false;
                field.Dispose();

                return value as T;
            }
            catch {
                return null;
            }

        }

        static void SetField(Java.Lang.Class targetClass, Java.Lang.Object instance, string fieldName, Java.Lang.Object value) {
            try {
                var field = targetClass.GetDeclaredField(fieldName);
                field.Accessible = true;
                field.Set(instance, value);
                field.Accessible = false;
                field.Dispose();
            }
            catch {
                return;
            }
        }
    }


}

