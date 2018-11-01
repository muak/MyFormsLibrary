using Xamarin.Forms;
namespace MyFormsLibrary.CustomRenderers
{
	public class NavigationPageEx : NavigationPage
	{
		public static BindableProperty ResourceProperty =
			BindableProperty.Create(nameof(Resource), typeof(string), typeof(NavigationPageEx), null,
									defaultBindingMode: Xamarin.Forms.BindingMode.OneWay
								   );

		public string Resource {
			get { return (string)GetValue(ResourceProperty); }
			set { SetValue(ResourceProperty, value); }
		}

		public static BindableProperty SelectedColorProperty =
			BindableProperty.Create(nameof(SelectedColor), typeof(Xamarin.Forms.Color), typeof(NavigationPageEx), Xamarin.Forms.Color.Default,
									defaultBindingMode: Xamarin.Forms.BindingMode.OneWay
								   );

		public Xamarin.Forms.Color SelectedColor {
			get { return (Xamarin.Forms.Color)GetValue(SelectedColorProperty); }
			set { SetValue(SelectedColorProperty, value); }
		}

		public static BindableProperty UnSelectedColorProperty =
			BindableProperty.Create(nameof(UnSelectedColor), typeof(Xamarin.Forms.Color), typeof(NavigationPageEx), Xamarin.Forms.Color.Default,
									defaultBindingMode: Xamarin.Forms.BindingMode.OneWay
								   );

		public Xamarin.Forms.Color UnSelectedColor {
			get { return (Xamarin.Forms.Color)GetValue(UnSelectedColorProperty); }
			set { SetValue(UnSelectedColorProperty, value); }
		}

		public static BindableProperty ForeColorProperty =
			BindableProperty.Create(nameof(ForeColor), typeof(Xamarin.Forms.Color), typeof(NavigationPageEx), Xamarin.Forms.Color.Default,
									defaultBindingMode: Xamarin.Forms.BindingMode.OneWay
								   );

		public Xamarin.Forms.Color ForeColor {
			get { return (Xamarin.Forms.Color)GetValue(ForeColorProperty); }
			set { SetValue(ForeColorProperty, value); }
		}

		/// <summary>
		/// Android Only
		/// </summary>
		public static BindableProperty StatusBarBackColorProperty =
			BindableProperty.Create(nameof(StatusBarBackColor), typeof(Xamarin.Forms.Color), typeof(NavigationPageEx), Xamarin.Forms.Color.Default,
									defaultBindingMode: Xamarin.Forms.BindingMode.OneWay
								   );

		public Xamarin.Forms.Color StatusBarBackColor {
			get { return (Xamarin.Forms.Color)GetValue(StatusBarBackColorProperty); }
			set { SetValue(StatusBarBackColorProperty, value); }
		}

		public bool IsDefaultColor {
			get {
				return (SelectedColor == Xamarin.Forms.Color.Default && UnSelectedColor == Xamarin.Forms.Color.Default);
			}
		}

		public NavigationPageEx() { }
		public NavigationPageEx(Page root) : base(root) { }

	}
}

