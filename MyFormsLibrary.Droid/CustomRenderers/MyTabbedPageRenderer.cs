using System;
using System.Reflection;
using Android.Content;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using Google.Android.Material.BottomNavigation;
using MyFormsLibrary.CustomRenderers;
using MyFormsLibrary.Droid.CustomRenderers;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Platform.Android.AppCompat;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using XF = Xamarin.Forms;

[assembly: XF.ExportRenderer(typeof(MyTabbedPage), typeof(MyTabbedPageRenderer))]
namespace MyFormsLibrary.Droid.CustomRenderers
{
	[Android.Runtime.Preserve(AllMembers = true)]
    public class MyTabbedPageRenderer : TabbedPageRenderer, BottomNavigationView.IOnNavigationItemSelectedListener
    {
		MyTabbedPage _myTabbed;
		BottomNavigationView _bottomNavigationView;
		Window _window;

		public MyTabbedPageRenderer(Context context):base(context)
        {
        }

		protected override void OnElementChanged(ElementChangedEventArgs<XF.TabbedPage> e)
		{
			base.OnElementChanged(e);

			var fieldInfo = typeof(TabbedPageRenderer).GetField("_tabLayout", BindingFlags.Instance | BindingFlags.NonPublic);

			fieldInfo = typeof(TabbedPageRenderer).GetField("_bottomNavigationView", BindingFlags.Instance | BindingFlags.NonPublic);
			_bottomNavigationView = (BottomNavigationView)fieldInfo.GetValue(this);

			var teardownPage = typeof(TabbedPageRenderer).GetMethod("TeardownPage", BindingFlags.Instance | BindingFlags.NonPublic);

			_window = (Context as FormsAppCompatActivity).Window;


			if (e.NewElement != null)
			{
				_myTabbed = Element as MyTabbedPage;

				_bottomNavigationView.SetOnNavigationItemSelectedListener(this);

				var layout = _bottomNavigationView.Parent as RelativeLayout;
				var border = new View(Context);
				border.SetBackgroundColor(_myTabbed.TabBorderColor.ToAndroid());
				border.Alpha = 0.6f;
				using (var param = new RelativeLayout.LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent)
				{
					Height = (int)Context.ToPixels(0.5)
				})
				{
					param.AddRule(LayoutRules.Above, _bottomNavigationView.Id);
					layout.AddView(border, param);
				}				

				// https://github.com/xamarin/Xamarin.Forms/blob/master/Xamarin.Forms.Platform.Android/AppCompat/TabbedPageRenderer.cs#L297
				// OnPagePropertyChangedでいらんことしてるので即TeardownPageを呼び出して解除する
				// （TabのTextを子ページのTitleと連動させているが、TabのTextはTabAttributeで設定するようにしているので不要）
				foreach (var page in Element.Children)
				{
					teardownPage.Invoke(this, new object[] { page });
				}

				for (var i = 0; i < _myTabbed.TabItems.Count; i++)
				{
					var item = _myTabbed.TabItems[i];

					if (i == 0 && _myTabbed.Parent is MyNavigationPage)
					{
						var navi = _myTabbed.Parent as MyNavigationPage;
						navi.BarTextColor = _myTabbed.BarTextColor;
						navi.StatusBarBackColor = _myTabbed.StatusBarBackColor;
						
						var curPage = (_myTabbed.CurrentPage as XF.Page);
						_myTabbed.Title = curPage.Title;
						ReplaceTitleView(curPage);

						_myTabbed.CurrentPage.PropertyChanged += CurrentPage_PropertyChanged;						
					}

					if (string.IsNullOrEmpty(item.Resource)) continue;

					var bitmap = SvgToBitmap.GetBitmap(item.Resource, 24, 24);
					var icon = new BitmapDrawable(Context.Resources, bitmap);

					var menuItem = _bottomNavigationView.Menu.GetItem(i);
					menuItem.SetIcon(icon);
					menuItem.SetTitle(item.Title);
					if (i == 0 && _myTabbed.StatusBarBackColor != Xamarin.Forms.Color.Default)
					{
						_window.AddFlags(Android.Views.WindowManagerFlags.DrawsSystemBarBackgrounds);
						_window.ClearFlags(Android.Views.WindowManagerFlags.TranslucentStatus);
						_window.SetStatusBarColor(_myTabbed.StatusBarBackColor.ToAndroid());
					}
				}
				
				Element.OnThisPlatform().SetIsSmoothScrollEnabled(false);
				Element.OnThisPlatform().SetIsSwipePagingEnabled(false);
				_bottomNavigationView.SetShiftMode(false, false, _myTabbed);
				
			}
		}

		void ReplaceTitleView(XF.Page page)
		{
			var titleView = XF.NavigationPage.GetTitleView(page);

			if (titleView != null)
			{
				XF.NavigationPage.SetTitleView(_myTabbed, titleView);
				titleView.BindingContext = page.BindingContext;
			}
			else
			{
				XF.NavigationPage.SetTitleView(_myTabbed, null);
			}
		}

		bool BottomNavigationView.IOnNavigationItemSelectedListener.OnNavigationItemSelected(IMenuItem item)
		{
			_myTabbed.CurrentPage.PropertyChanged -= CurrentPage_PropertyChanged;

			if (!base.OnNavigationItemSelected(item))
				return false;

			_myTabbed.Children[item.Order].PropertyChanged += CurrentPage_PropertyChanged;

			var selectedPage = _myTabbed.Children[item.Order];
			_myTabbed.Title = selectedPage.Title;

			ReplaceTitleView(selectedPage);

			return true;
		}

		void CurrentPage_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == XF.Page.TitleProperty.PropertyName)
			{
				_myTabbed.Title = (sender as XF.Page).Title;
			}
			else if (e.PropertyName == XF.NavigationPage.TitleViewProperty.PropertyName)
			{
				ReplaceTitleView(sender as XF.Page);
			}
		}


	}

	// https://gist.github.com/LynoDesu/64904b6d143892cf14a60a32798a36bb
	[Android.Runtime.Preserve(AllMembers = true)]
	public static class BottomNavigationHelpers2
	{
		public static void SetShiftMode(this BottomNavigationView bottomNavigationView, bool enableShiftMode, bool enableItemShiftMode, MyTabbedPage myTabbed)
		{
			try
			{
				var menuView = bottomNavigationView.GetChildAt(0) as BottomNavigationMenuView;
				if (menuView == null)
				{
					System.Diagnostics.Debug.WriteLine("Unable to find BottomNavigationMenuView");
					return;
				}

				SetFieldVisibilityMode(menuView.Class, menuView, "labelVisibilityMode", 1);

				for (int i = 0; i < menuView.ChildCount; i++)
				{
					var item = menuView.GetChildAt(i) as BottomNavigationItemView;
					if (item == null)
						continue;

					item.SetShifting(enableItemShiftMode);
					item.SetChecked(item.ItemData.IsChecked);
					SetField(item.Class, item, "shiftAmount", 0);
					SetField(item.Class, item, "scaleUpFactor", 1);
					SetField(item.Class, item, "scaleDownFactor", 1);

					var mLargeLabel = GetField<TextView>(item.Class, item, "largeLabel");
					var mSmallLabel = GetField<TextView>(item.Class, item, "smallLabel");

					if (myTabbed.BottomTabFontSize > -1)
					{
						mSmallLabel.SetTextSize(Android.Util.ComplexUnitType.Sp, (float)myTabbed.BottomTabFontSize);
					}
					mLargeLabel.SetTextSize(Android.Util.ComplexUnitType.Px, mSmallLabel.TextSize);

					if (!myTabbed.SelectedTextColor.IsDefault)
					{
						mLargeLabel.SetTextColor(myTabbed.SelectedTextColor.ToAndroid());
					}
					if (!myTabbed.UnSelectedTextColor.IsDefault)
					{
						mSmallLabel.SetTextColor(myTabbed.UnSelectedTextColor.ToAndroid());
					}
				}

				menuView.UpdateMenuView();
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Unable to set shift mode: {ex}");
			}
		}

		static T GetField<T>(Java.Lang.Class targetClass, Java.Lang.Object instance, string fieldName) where T : Java.Lang.Object
		{
			try
			{
				var asdfdasf = targetClass.GetDeclaredFields();
				var field = targetClass.GetDeclaredField(fieldName);
				field.Accessible = true;
				var value = field.Get(instance);
				field.Accessible = false;
				field.Dispose();

				return value as T;
			}
			catch
			{
				return null;
			}

		}

		static void SetField(Java.Lang.Class targetClass, Java.Lang.Object instance, string fieldName, Java.Lang.Object value)
		{
			try
			{
				var asdfdasf = targetClass.GetDeclaredFields();
				var field = targetClass.GetDeclaredField(fieldName);
				field.Accessible = true;
				field.Set(instance, value);
				field.Accessible = false;
				field.Dispose();
			}
			catch
			{
				return;
			}
		}

		static void SetFieldVisibilityMode(Java.Lang.Class targetClass, Java.Lang.Object instance, string fieldName, int value)
		{
			try
			{
				var field = targetClass.GetDeclaredField(fieldName);
				field.Accessible = true;
				field.SetInt(instance, value);
				field.Accessible = false;
				field.Dispose();
			}
			catch
			{
				return;
			}
		}
	}

}
