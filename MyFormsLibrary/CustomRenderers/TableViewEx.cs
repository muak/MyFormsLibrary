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

        public static BindableProperty SectionTitleColorProperty =
            BindableProperty.Create(
                nameof(SectionTitleColor),
                typeof(Color),
                typeof(TableViewEx),
                default(Color),
                defaultBindingMode: BindingMode.OneWay
            );

        public Color SectionTitleColor {
            get { return (Color)GetValue(SectionTitleColorProperty); }
            set { SetValue(SectionTitleColorProperty, value); }
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
      
    }
}
