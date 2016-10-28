using System;
using MyFormsLibrary.DependencyServices;
using MyFormsLibrary.Infrastructure;
using Xamarin.Forms;
namespace MyFormsLibrary.CustomRenderers
{
    public class CellBase:Cell
    {
        public CellBase() {
        }

        public static BindableProperty LabelTextProperty =
            BindableProperty.Create(
                nameof(LabelText),
                typeof(string),
                typeof(CellBase),
                default(string),
                defaultBindingMode: BindingMode.OneWay
            );

        public string LabelText {
            get { return (string)GetValue(LabelTextProperty); }
            set { SetValue(LabelTextProperty, value); }
        }

        public static BindableProperty LabelColorProperty =
            BindableProperty.Create(
                nameof(LabelColor),
                typeof(Color),
                typeof(CellBase),
                default(Color),
                defaultBindingMode: BindingMode.OneWay
            );

        public Color LabelColor {
            get { return (Color)GetValue(LabelColorProperty); }
            set { SetValue(LabelColorProperty, value); }
        }

        public static BindableProperty LabelFontSizeProperty =
            BindableProperty.Create(
                nameof(LabelFontSize),
                typeof(double),
                typeof(CellBase),
                -1.0,
                defaultBindingMode: BindingMode.OneWay,
                defaultValueCreator: bindable => Device.GetNamedSize(NamedSize.Default, (CellBase)bindable)
            );

        [TypeConverter(typeof(FontSizeConverter))]
        public double LabelFontSize {
            get { return (double)GetValue(LabelFontSizeProperty); }
            set { SetValue(LabelFontSizeProperty, value); }
        }

        public static BindableProperty IconResourceProperty =
            BindableProperty.Create(
                nameof(IconResource),
                typeof(string),
                typeof(CellBase),
                default(string),
                defaultBindingMode: BindingMode.OneWay
            );

        public string IconResource {
            get { return (string)GetValue(IconResourceProperty); }
            set { SetValue(IconResourceProperty, value); }
        }

        public static BindableProperty IconColorProperty =
            BindableProperty.Create(
                nameof(IconColor),
                typeof(Color),
                typeof(CellBase),
                default(Color),
                defaultBindingMode: BindingMode.OneWay
            );

        public Color IconColor {
            get { return (Color)GetValue(IconColorProperty); }
            set { SetValue(IconColorProperty, value); }
        }

        private NGraphics.IImage _Image;
        public NGraphics.IImage Image {
            get {
                if (string.IsNullOrEmpty(IconResource)) return null;
                if (_Image == null) {
                    var g = SvgLoader.GetResourceAndLoadSvg(IconResource);
                    var sv = DependencyService.Get<ISvgService>();
                    Device.OnPlatform(
                        iOS: () => { _Image = sv.GetCanvas(g, 25, 25,IconColor);},
                        Android: () => { _Image = sv.GetCanvas(g, 24, 24,IconColor); }
                    );
                }

                return _Image;
            }
        }
    }
}
