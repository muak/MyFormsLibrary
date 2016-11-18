using System;
using Xamarin.Forms;
namespace MyFormsLibrary.CustomRenderers
{
    public class TableViewEx:TableView
    {
        
        public static BindableProperty SeparatorColorProperty =
            BindableProperty.Create(
                nameof(SeparatorColor),
                typeof(Color),
                typeof(TableViewEx),
                default(Color),
                defaultBindingMode: BindingMode.OneWay
            );

        public Color SeparatorColor {
            get { return (Color)GetValue(SeparatorColorProperty); }
            set { SetValue(SeparatorColorProperty, value); }
        }

        public static BindableProperty HeaderTextColorProperty =
            BindableProperty.Create(
                nameof(HeaderTextColor),
                typeof(Color),
                typeof(TableViewEx),
                default(Color),
                defaultBindingMode: BindingMode.OneWay
            );

        public Color HeaderTextColor {
            get { return (Color)GetValue(HeaderTextColorProperty); }
            set { SetValue(HeaderTextColorProperty, value); }
        }

        public static BindableProperty CellLabelColorProperty =
            BindableProperty.Create(
                nameof(CellLabelColor),
                typeof(Color),
                typeof(TableViewEx),
                default(Color),
                defaultBindingMode: BindingMode.OneWay
            );

        public Color CellLabelColor {
            get { return (Color)GetValue(CellLabelColorProperty); }
            set { SetValue(CellLabelColorProperty, value); }
        }


        public static BindableProperty CellLabelFontSizeProperty =
            BindableProperty.Create(
                nameof(CellLabelFontSize),
                typeof(double),
                typeof(TableViewEx),
                -1.0,
                defaultBindingMode: BindingMode.OneWay,
                defaultValueCreator: bindable => Device.GetNamedSize(NamedSize.Default, (TableViewEx)bindable)
            );

        [TypeConverter(typeof(FontSizeConverter))]
        public double CellLabelFontSize {
            get { return (double)GetValue(CellLabelFontSizeProperty); }
            set { SetValue(CellLabelFontSizeProperty, value); }
        }

        public static BindableProperty CellValueTextColorProperty =
            BindableProperty.Create(
                nameof(CellValueTextColor),
                typeof(Color),
                typeof(TableViewEx),
                default(Color),
                defaultBindingMode: BindingMode.OneWay
            );

        public Color CellValueTextColor {
            get { return (Color)GetValue(CellValueTextColorProperty); }
            set { SetValue(CellValueTextColorProperty, value); }
        }

        public static BindableProperty CellValueTextFontSizeProperty =
            BindableProperty.Create(
                nameof(CellValueTextFontSize),
                typeof(double),
                typeof(TableViewEx),
                -1.0,
                defaultBindingMode: BindingMode.OneWay
            );

        [TypeConverter(typeof(FontSizeConverter))]
        public double CellValueTextFontSize {
            get { return (double)GetValue(CellValueTextFontSizeProperty); }
            set { SetValue(CellValueTextFontSizeProperty, value); }
        }

        public static BindableProperty HeaderFontSizeProperty =
            BindableProperty.Create(
                nameof(HeaderFontSize),
                typeof(double),
                typeof(TableViewEx),
                -1.0,
                defaultBindingMode: BindingMode.OneWay,
                defaultValueCreator: bindable => Device.GetNamedSize(NamedSize.Small, (TableViewEx)bindable)
            );

        [TypeConverter(typeof(FontSizeConverter))]
        public double HeaderFontSize {
            get { return (double)GetValue(HeaderFontSizeProperty); }
            set { SetValue(HeaderFontSizeProperty, value); }
        }


        public static BindableProperty HeaderTextVerticalAlignProperty =
            BindableProperty.Create(
                nameof(HeaderTextVerticalAlign),
                typeof(LayoutAlignment),
                typeof(TableViewEx),
                LayoutAlignment.End,
                defaultBindingMode: BindingMode.OneWay
            );

        public LayoutAlignment HeaderTextVerticalAlign {
            get { return (LayoutAlignment)GetValue(HeaderTextVerticalAlignProperty); }
            set { SetValue(HeaderTextVerticalAlignProperty, value); }
        }

        public static BindableProperty HeaderBackgroundColorProperty =
            BindableProperty.Create(
                nameof(HeaderBackgroundColor),
                typeof(Color),
                typeof(TableViewEx),
                default(Color),
                defaultBindingMode: BindingMode.OneWay
            );

        public Color HeaderBackgroundColor {
            get { return (Color)GetValue(HeaderBackgroundColorProperty); }
            set { SetValue(HeaderBackgroundColorProperty, value); }
        }

        public static BindableProperty HeaderHeightProperty =
            BindableProperty.Create(
                nameof(HeaderHeight),
                typeof(float),
                typeof(TableViewEx),
                44f,
                defaultBindingMode: BindingMode.OneWay
            );

        public float HeaderHeight {
            get { return (float)GetValue(HeaderHeightProperty); }
            set { SetValue(HeaderHeightProperty, value); }
        }

        public static BindableProperty CellBackgroundColorProperty =
            BindableProperty.Create(
                nameof(CellBackgroundColor),
                typeof(Color),
                typeof(TableViewEx),
                default(Color),
                defaultBindingMode: BindingMode.OneWay
            );

        public Color CellBackgroundColor {
            get { return (Color)GetValue(CellBackgroundColorProperty); }
            set { SetValue(CellBackgroundColorProperty, value); }
        }

        //Android Only
        public static BindableProperty ShowSectionTopBottomBorderProperty =
            BindableProperty.Create(
                nameof(ShowSectionTopBottomBorder),
                typeof(bool),
                typeof(TableViewEx),
                true,
                defaultBindingMode: BindingMode.OneWay
            );

        public bool ShowSectionTopBottomBorder {
            get { return (bool)GetValue(ShowSectionTopBottomBorderProperty); }
            set { SetValue(ShowSectionTopBottomBorderProperty, value); }
        }
      
    }
}
