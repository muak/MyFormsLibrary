using System;
using System.IO;
using System.Linq;
using System.Reflection;
using NGraphics;
using Xamarin.Forms;
using MyFormsLibrary.Infrastructure;
using MyFormsLibrary.DependencyServices;

namespace MyFormsLibrary.CustomRenderers
{
	public class ToolbarItemEx:ToolbarItem
	{

		public static BindableProperty ResourceProperty =
			BindableProperty.Create(nameof(Resource), typeof(string), typeof(ToolbarItemEx), null,
				defaultBindingMode: Xamarin.Forms.BindingMode.OneWay
			);


		public string Resource {
			get { return (string)GetValue(ResourceProperty); }
			set {SetValue(ResourceProperty, value);}
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

		public static BindableProperty IsEnabledExProperty =
			BindableProperty.Create(
				nameof(IsEnabledEx),
				typeof(bool),
				typeof(ToolbarItemEx),
				true,
				defaultBindingMode: BindingMode.OneWay
			);

		public bool IsEnabledEx {
			get { return (bool)GetValue(IsEnabledExProperty); }
			set { SetValue(IsEnabledExProperty, value); }
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

		private IImage _Image;
		public IImage Image {
			get {
				if (_Image == null) {
					var g = SvgLoader.GetResourceAndLoadSvg(Resource);
					var sv = DependencyService.Get<ISvgService>();
					Device.OnPlatform(
						iOS: () => { _Image = sv.GetCanvas(g, 20, 20); },
						Android: () => { _Image = sv.GetCanvas(g, 24, 24); }
					);
				}

				return _Image;
			}
		}

	}
}

