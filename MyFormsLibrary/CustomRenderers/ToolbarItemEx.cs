using Xamarin.Forms;

namespace MyFormsLibrary.CustomRenderers
{
	public class ToolbarItemEx : ToolbarItem
	{

		public static BindableProperty ResourceProperty =
			BindableProperty.Create(nameof(Resource), typeof(string), typeof(ToolbarItemEx), null,
									defaultBindingMode: Xamarin.Forms.BindingMode.OneWay
								   );


		public string Resource {
			get { return (string)GetValue(ResourceProperty); }
			set { SetValue(ResourceProperty, value); }
		}

		public static BindableProperty IsVisibleProperty =
			BindableProperty.Create(
				nameof(IsVisible),
				typeof(bool),
				typeof(ToolbarItemEx),
				true,
				defaultBindingMode: BindingMode.OneWay
			);

		public bool IsVisible {
			get { return (bool)GetValue(IsVisibleProperty); }
			set { SetValue(IsVisibleProperty, value); }
		}

		public static BindableProperty IsEnabledProperty =
			BindableProperty.Create(
				nameof(IsEnabled),
				typeof(bool),
				typeof(ToolbarItemEx),
				true,
				defaultBindingMode: BindingMode.OneWay
			);

		public bool IsEnabled {
			get { return (bool)GetValue(IsEnabledProperty); }
			set { SetValue(IsEnabledProperty, value); }
		}

		public static BindableProperty IsLeftIconProperty =
			BindableProperty.Create(
				nameof(IsLeftIcon),
				typeof(bool),
				typeof(ToolbarItemEx),
				default(bool),
				defaultBindingMode: BindingMode.OneWay
			);

		public bool IsLeftIcon {
			get { return (bool)GetValue(IsLeftIconProperty); }
			set { SetValue(IsLeftIconProperty, value); }
		}

	}
}

