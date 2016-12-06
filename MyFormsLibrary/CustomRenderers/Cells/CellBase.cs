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
                defaultBindingMode: BindingMode.OneWay
            );

        [TypeConverter(typeof(FontSizeConverter))]
        public double LabelFontSize {
            get { return (double)GetValue(LabelFontSizeProperty); }
            set { SetValue(LabelFontSizeProperty, value); }
        }

        public static BindableProperty ValueTextColorProperty =
            BindableProperty.Create(
                nameof(ValueTextColor),
                typeof(Color),
                typeof(CellBase),
                default(Color),
                defaultBindingMode: BindingMode.OneWay
            );

        public Color ValueTextColor {
            get { return (Color)GetValue(ValueTextColorProperty); }
            set { SetValue(ValueTextColorProperty, value); }
        }

        public static BindableProperty ValueTextFontSizeProperty =
            BindableProperty.Create(
                nameof(ValueTextFontSize),
                typeof(double),
                typeof(CellBase),
                -1.0d,
                defaultBindingMode: BindingMode.OneWay
            );

        [TypeConverter(typeof(FontSizeConverter))]
        public double ValueTextFontSize {
            get { return (double)GetValue(ValueTextFontSizeProperty); }
            set { SetValue(ValueTextFontSizeProperty, value); }
        }

        public static BindableProperty ErrorMessageProperty =
            BindableProperty.Create(
                nameof(ErrorMessage),
                typeof(string),
                typeof(CellBase),
                default(string),
                defaultBindingMode: BindingMode.OneWay
            );

        public string ErrorMessage {
            get { return (string)GetValue(ErrorMessageProperty); }
            set { SetValue(ErrorMessageProperty, value); }
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
                        Android: () => { _Image = sv.GetCanvas(g, 30, 30,IconColor); }
                    );
                }

                return _Image;
            }
        }
    }
}
