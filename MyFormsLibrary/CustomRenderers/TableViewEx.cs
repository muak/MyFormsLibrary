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
    }
}
