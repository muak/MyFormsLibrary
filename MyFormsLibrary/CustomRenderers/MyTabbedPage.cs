using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace MyFormsLibrary.CustomRenderers
{
    public class MyTabbedPage:TabbedPage
    {		

		public static BindableProperty SelectedTextColorProperty =
			BindableProperty.Create(nameof(SelectedTextColor), typeof(Xamarin.Forms.Color), typeof(MyTabbedPage), Xamarin.Forms.Color.Default,
									defaultBindingMode: Xamarin.Forms.BindingMode.OneWay
								   );

		public Xamarin.Forms.Color SelectedTextColor
		{
			get { return (Xamarin.Forms.Color)GetValue(SelectedTextColorProperty); }
			set { SetValue(SelectedTextColorProperty, value); }
		}

		public static BindableProperty UnSelectedTextColorProperty =
			BindableProperty.Create(nameof(UnSelectedTextColor), typeof(Xamarin.Forms.Color), typeof(MyTabbedPage), Xamarin.Forms.Color.Default,
									defaultBindingMode: Xamarin.Forms.BindingMode.OneWay
								   );

		public Xamarin.Forms.Color UnSelectedTextColor
		{
			get { return (Xamarin.Forms.Color)GetValue(UnSelectedTextColorProperty); }
			set { SetValue(UnSelectedTextColorProperty, value); }
		}

		public static BindableProperty StatusBarBackColorProperty =
			BindableProperty.Create(
				nameof(StatusBarBackColor),
				typeof(Color),
				typeof(MyTabbedPage),
				default(Color),
				defaultBindingMode: BindingMode.OneWay
			);

		public Color StatusBarBackColor
		{
			get { return (Color)GetValue(StatusBarBackColorProperty); }
			set { SetValue(StatusBarBackColorProperty, value); }
		}

		public static BindableProperty BottomTabFontSizeProperty =
			BindableProperty.Create(
				nameof(BottomTabFontSize),
				typeof(double),
				typeof(MyTabbedPage),
				-1d,
				defaultBindingMode: BindingMode.OneWay
			);

		public double BottomTabFontSize
		{
			get { return (double)GetValue(BottomTabFontSizeProperty); }
			set { SetValue(BottomTabFontSizeProperty, value); }
		}

		public bool IsDefaultColor
		{
			get
			{
				return (SelectedTabColor == Color.Default && UnselectedTabColor == Color.Default);
			}
		}

		public static BindableProperty IsTextHiddenProperty =
			BindableProperty.Create(nameof(IsTextHidden), typeof(bool), typeof(MyTabbedPage), false,
									defaultBindingMode: Xamarin.Forms.BindingMode.OneWay
								   );

		public bool IsTextHidden
		{
			get { return (bool)GetValue(IsTextHiddenProperty); }
			set { SetValue(IsTextHiddenProperty, value); }
		}

		public static BindableProperty TabBorderColorProperty = BindableProperty.Create(
			nameof(TabBorderColor),
			typeof(Color),
			typeof(MyTabbedPage),
			default(Color),
			defaultBindingMode: BindingMode.OneWay
		);

		public Color TabBorderColor
		{
			get { return (Color)GetValue(TabBorderColorProperty); }
			set { SetValue(TabBorderColorProperty, value); }
		}

		public static BindableProperty TabItemsProperty =
			BindableProperty.Create(
				nameof(TabItems),
				typeof(IList<TabItem>),
				typeof(MyTabbedPage),
				null,
				defaultBindingMode: BindingMode.OneWay
			);

		public IList<TabItem> TabItems
		{
			get { return (IList<TabItem>)GetValue(TabItemsProperty); }
			set { SetValue(TabItemsProperty, value); }
		}
	}
}
