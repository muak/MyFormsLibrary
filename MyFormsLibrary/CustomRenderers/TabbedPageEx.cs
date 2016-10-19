using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using MyFormsLibrary.DependencyServices;
using MyFormsLibrary.Infrastructure;
using Xamarin.Forms;

namespace MyFormsLibrary.CustomRenderers
{
	public class TabbedPageEx:TabbedPage
	{
		
		public static BindableProperty ResourceProperty =
			BindableProperty.Create(nameof(Resource), typeof(string), typeof(TabbedPageEx), null,
				defaultBindingMode: Xamarin.Forms.BindingMode.OneWay
			);


		public string Resource {
			get { return (string)GetValue(ResourceProperty); }
			set {SetValue(ResourceProperty, value);}
		}

		public static BindableProperty SelectedColorProperty =
			BindableProperty.Create(nameof(SelectedColor), typeof(Xamarin.Forms.Color), typeof(TabbedPageEx), Xamarin.Forms.Color.Default,
				defaultBindingMode: Xamarin.Forms.BindingMode.OneWay
			);

		public Xamarin.Forms.Color SelectedColor {
			get { return (Xamarin.Forms.Color)GetValue(SelectedColorProperty); }
			set { SetValue(SelectedColorProperty, value); }
		}

		public static BindableProperty UnSelectedColorProperty =
			BindableProperty.Create(nameof(UnSelectedColor), typeof(Xamarin.Forms.Color), typeof(TabbedPageEx), Xamarin.Forms.Color.Default,
				defaultBindingMode: Xamarin.Forms.BindingMode.OneWay
			);

		public Xamarin.Forms.Color UnSelectedColor {
			get { return (Xamarin.Forms.Color)GetValue(UnSelectedColorProperty); }
			set { SetValue(UnSelectedColorProperty, value); }
		}

		public static BindableProperty SelectedTextColorProperty =
			BindableProperty.Create(nameof(SelectedTextColor), typeof(Xamarin.Forms.Color), typeof(TabbedPageEx), Xamarin.Forms.Color.Default,
				defaultBindingMode: Xamarin.Forms.BindingMode.OneWay
			);

		public Xamarin.Forms.Color SelectedTextColor {
			get { return (Xamarin.Forms.Color)GetValue(SelectedTextColorProperty); }
			set { SetValue(SelectedTextColorProperty, value); }
		}

		public static BindableProperty UnSelectedTextColorProperty =
			BindableProperty.Create(nameof(UnSelectedTextColor), typeof(Xamarin.Forms.Color), typeof(TabbedPageEx), Xamarin.Forms.Color.Default,
				defaultBindingMode: Xamarin.Forms.BindingMode.OneWay
			);

		public Xamarin.Forms.Color UnSelectedTextColor {
			get { return (Xamarin.Forms.Color)GetValue(UnSelectedTextColorProperty); }
			set { SetValue(UnSelectedTextColorProperty, value); }
		}



		public bool IsDefaultColor { 
			get {
				return (SelectedColor == Color.Default && UnSelectedColor == Color.Default);
			} 
		}

		public static BindableProperty IsTextHiddenProperty =
			BindableProperty.Create(nameof(IsTextHidden), typeof(bool), typeof(TabbedPageEx), false,
				defaultBindingMode: Xamarin.Forms.BindingMode.OneWay
			);

		public bool IsTextHidden {
			get { return (bool)GetValue(IsTextHiddenProperty); }
			set { SetValue(IsTextHiddenProperty, value); }
		}

		public static BindableProperty TabAttributesProperty =
			BindableProperty.Create(
				nameof(TabAttributes),
				typeof(IList<TabAttribute>),
				typeof(TabbedPageEx),
				null,
				defaultBindingMode: BindingMode.OneWay
			);

		public IList<TabAttribute> TabAttributes {
			get { return (IList<TabAttribute>)GetValue(TabAttributesProperty); }
			set { SetValue(TabAttributesProperty, value); }
		}



		public class TabAttribute
		{
			public string Title { get; set; }
			public string Resource { get; set; }
			public Color SelectedColor { get; set; }
			public Color UnSelectedColor { get; set; }
			public Color BarTextColor { get; set; }
			public Color StatusBarBackColor { get; set; }
			public TabbedPageEx Parent { get; set; }

			public bool IsDefaultColor {
				get {
					return (SelectedColor == Color.Default && UnSelectedColor == Color.Default);
				}
			}

			private NGraphics.IImage _Image;
			public NGraphics.IImage Image {
				get {
					if (_Image == null) {
						var g = SvgLoader.GetResourceAndLoadSvg(Resource);
						var sv = DependencyService.Get<ISvgService>();
						Device.OnPlatform(
							iOS: () => {
								
									var tab = this.Parent as TabbedPageEx;
									if (tab.IsDefaultColor && this.IsDefaultColor) {
										_Image = sv.GetCanvas(g, 30, 30);
									}
									else {
										_Image = sv.GetCanvas(g, 30, 30,
											  SelectedColor == Xamarin.Forms.Color.Default ? tab.SelectedColor : SelectedColor);
										UnSelectedImage = sv.GetCanvas(g, 30, 30,
											   UnSelectedColor == Xamarin.Forms.Color.Default ? tab.UnSelectedColor : UnSelectedColor);
									}
								

							},
							Android: () => { _Image = sv.GetCanvas(g, 24, 24); }
						);
					}

					return _Image;
				}
			}

			public NGraphics.IImage UnSelectedImage { get; private set; }
		}
	}
}

