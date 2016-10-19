using System;
using System.IO;
using System.Linq;
using System.Reflection;
using NGraphics;
using MyFormsLibrary.DependencyServices;
using MyFormsLibrary.Infrastructure;
using Xamarin.Forms;
namespace MyFormsLibrary.CustomRenderers
{
	public class NavigationPageEx:NavigationPage
	{
		public static BindableProperty ResourceProperty =
			BindableProperty.Create(nameof(Resource), typeof(string), typeof(NavigationPageEx), null,
				defaultBindingMode: Xamarin.Forms.BindingMode.OneWay
			);

		public string Resource {
			get { return (string)GetValue(ResourceProperty); }
			set {SetValue(ResourceProperty, value);}
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


		private IImage _Image;
		public IImage Image {
			get {
				if (_Image == null) {
					var g = SvgLoader.GetResourceAndLoadSvg(Resource);
					var sv = DependencyService.Get<ISvgService>();
					Device.OnPlatform(
						iOS: () => {
							if (this.Parent is TabbedPageEx) {
								var tab = this.Parent as TabbedPageEx;
								if (tab.IsDefaultColor && this.IsDefaultColor) {
									_Image = sv.GetCanvas(g, 30, 30);
								}
								else {
									_Image = sv.GetCanvas(g, 30, 30,
			                      		SelectedColor == Xamarin.Forms.Color.Default ?  tab.SelectedColor : SelectedColor);
									UnSelectedImage = sv.GetCanvas(g, 30, 30,
	                               		UnSelectedColor == Xamarin.Forms.Color.Default ? tab.UnSelectedColor : UnSelectedColor);
								}
							}

						},
						Android: () => { _Image = sv.GetCanvas(g, 24, 24); }
					);
				}

				return _Image;
			}
		}

		public IImage UnSelectedImage { get; private set;}



	}
}

