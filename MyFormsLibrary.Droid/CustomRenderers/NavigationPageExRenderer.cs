﻿using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Support.V7.Widget;
using Android.Text;
using Android.Views;
using MyFormsLibrary.CustomRenderers;
using MyFormsLibrary.Droid.CustomRenderers;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Platform.Android.AppCompat;
using Android.Text.Style;

[assembly: ExportRenderer(typeof(NavigationPageEx), typeof(NavigationPageExRenderer))]
namespace MyFormsLibrary.Droid.CustomRenderers
{
	public class NavigationPageExRenderer : NavigationPageRenderer
	{
		private ToolbarTracker _toolbarTracker;
		private Toolbar _toolbar;
		private NavigationIconBack _navigationBackListener;
		private NavigationIconClickListener _navigationCustomListener;

		public Android.Graphics.Color ForeColor {
			get {
				return (Element as NavigationPage).BarTextColor.ToAndroid();
			}
		}
		public bool IsForeColorDefault {
			get {
				return (Element as NavigationPage).BarTextColor == Xamarin.Forms.Color.Default;
			}
		}

		protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<NavigationPage> e)
		{
			base.OnElementChanged(e);


			if (e.OldElement != null) {
				_toolbarTracker.CollectionChanged -= toolbarCollectionChanged;
				Element.Pushed -= pagePusshed;
				//Element.Popped -= pagePopped;
			}

			if (e.NewElement != null) {
				var fieldInfo = typeof(NavigationPageRenderer).GetField("_toolbarTracker", BindingFlags.Instance | BindingFlags.NonPublic);
				_toolbarTracker = (ToolbarTracker)fieldInfo.GetValue(this);

				fieldInfo = typeof(NavigationPageRenderer).GetField("_toolbar", BindingFlags.Instance | BindingFlags.NonPublic);
				_toolbar = (Toolbar)fieldInfo.GetValue(this);

				_toolbarTracker.CollectionChanged += toolbarCollectionChanged;

				_toolbar.SetTitleTextColor(ForeColor);
				_toolbar.SetSubtitleTextColor(ForeColor);
				Element.Pushed += pagePusshed;
				//Element.Popped += pagePopped;

				_navigationBackListener = new NavigationIconBack(Element,ArrowUpdate);

				UpdateMenu();

				if (Element.Parent is Xamarin.Forms.Application) {
					var statusBarColor = (Element as NavigationPageEx).StatusBarBackColor;
					if (statusBarColor != Xamarin.Forms.Color.Default) {
						(Context as FormsAppCompatActivity).Window.SetStatusBarColor(statusBarColor.ToAndroid());
					}
				}
			}
		}

		protected override void Dispose(bool disposing)
		{
			_toolbarTracker.CollectionChanged -= toolbarCollectionChanged;

			Element.Pushed -= pagePusshed;
			//Element.Popped -= pagePopped;

			_toolbar.SetNavigationOnClickListener(null);
			_navigationBackListener?.Dispose();
			_navigationCustomListener?.Dispose();

			_toolbar = null;
			_toolbarTracker = null;

			base.Dispose(disposing);

		}

		void pagePusshed(object sender, EventArgs e)
		{
			ArrowUpdate();
		}

		void ArrowUpdate(){
			Device.StartTimer(TimeSpan.FromMilliseconds(50), () => {
				if (_toolbar.NavigationIcon != null) {
					_toolbar.NavigationIcon.SetColorFilter(ForeColor, PorterDuff.Mode.SrcIn);
					return false;
				}
				return true;
			});
		}

		void toolbarCollectionChanged(object sender, EventArgs e)
		{
			UpdateMenu();
		}

		void HandleToolbarItemPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "IsEnabled" ||
				e.PropertyName == MenuItem.TextProperty.PropertyName ||
				e.PropertyName == MenuItem.IconProperty.PropertyName ||
				e.PropertyName == ToolbarItemEx.IsVisibleProperty.PropertyName ||
				e.PropertyName == ToolbarItemEx.IsEnabledProperty.PropertyName) {
				UpdateMenu();
			}
		}

		public void UpdateMenu()
		{
			if (NavigationPage.GetHasBackButton(Element.CurrentPage)) {
				_toolbar.SetNavigationOnClickListener(_navigationBackListener);
			}
			else {
				_toolbar.SetNavigationOnClickListener(null);
			}

			foreach (ToolbarItem item in _toolbarTracker.ToolbarItems) {
				item.PropertyChanged -= HandleToolbarItemPropertyChanged;
			}

			IMenu menu = _toolbar.Menu;

			for (var i = 0; i < _toolbarTracker.ToolbarItems.Count(); i++) {

				var item = _toolbarTracker.ToolbarItems.ElementAt(i);
				item.PropertyChanged += HandleToolbarItemPropertyChanged;
				var menuItem = menu.GetItem(i);

				if (item.Order == ToolbarItemOrder.Secondary) {
					continue;
				}

				var itemEx = item as ToolbarItemEx;
				if (itemEx == null) {
					continue;
				}

				if (!itemEx.IsVisible) {
					menuItem.SetVisible(false);
					continue;
				}

				//左側のアイコン設定。NavigationIconを使うためBackButtonとの併用は不可
				//BackButtonよりも優先される。
				//複数の左アイコンが指定されても最後の一つだけが有効となる
				//今の所modalのみで使う方が良い
				if (itemEx.IsLeftIcon) {
					UpdateLeftIcon(itemEx, menuItem);
					continue;
				}

				UpdateRightIcon(itemEx, menuItem);
			}

		}

		void UpdateLeftIcon(ToolbarItemEx itemEx, IMenuItem menuItem)
		{
			if (string.IsNullOrEmpty(itemEx.Resource)) {
				return;
			}
			var image = SvgToBitmap.GetBitmap(itemEx.Resource, 24, 24);
			var icon = new BitmapDrawable(Context.Resources, image);

			if (!IsForeColorDefault && itemEx.IsEnabled) {
				icon.SetTint(ForeColor);
			}
			else if (!itemEx.IsEnabled) {
				icon.SetTint(ForeColor);
				icon.SetAlpha(80);
			}

			//戻った時にこうしとかないと表示されない。BeginInvokeOnMainThreadだと失敗することがある。時間も250は必要。
			Device.StartTimer(TimeSpan.FromMilliseconds(250), () => {
				_toolbar.NavigationIcon = icon;
				return false;
			});
			_navigationCustomListener?.Dispose();
			_navigationCustomListener = new NavigationIconClickListener(itemEx.Command, itemEx.CommandParameter);
			_toolbar.SetNavigationOnClickListener(_navigationCustomListener);
			menuItem.SetVisible(false);
		}

		void UpdateRightIcon(ToolbarItemEx itemEx, IMenuItem menuItem)
		{
			menuItem.SetVisible(true);
			menuItem.SetEnabled(itemEx.IsEnabled);

			if (string.IsNullOrEmpty(itemEx.Resource)) {
				return;
			}

			var image = SvgToBitmap.GetBitmap(itemEx.Resource, 24, 24);
			var icon = new BitmapDrawable(Context.Resources, image);
			menuItem.SetIcon(icon);

			if (!IsForeColorDefault && itemEx.IsEnabled) {
				menuItem.Icon.SetTint(ForeColor);
			}
			else if (!itemEx.IsEnabled) {
				menuItem.Icon.SetTint(ForeColor);
				menuItem.Icon.SetAlpha(80);
			}
		}

		class NavigationIconClickListener : Java.Lang.Object, IOnClickListener
		{
			private ICommand _command;
			private object _commandParameter;

			public NavigationIconClickListener(ICommand command, object commandParameter)
			{
				_command = command;
				_commandParameter = commandParameter;
			}
			public void OnClick(Android.Views.View v)
			{
				_command?.Execute(_commandParameter);
			}
		}

		class NavigationIconBack : Java.Lang.Object, IOnClickListener
		{
			private NavigationPage _element;
			private Action _popped;
			public NavigationIconBack(NavigationPage element,Action popped)
			{
				_element = element;
				_popped = popped;
			}

			public void OnClick(Android.Views.View v)
			{
				_element.PopAsync(false);
				_popped?.Invoke();
			}
		}
	}
}

