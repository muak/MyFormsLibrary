using System;
using Xamarin.Forms;

namespace MyFormsLibrary.CustomRenderers
{
    public class TimePickerCell:CellBase
    {
        public TimePickerCell()
        {
        }

        public static BindableProperty TimeProperty =
            BindableProperty.Create(
                nameof(Time),
                typeof(TimeSpan),
                typeof(TimePickerCell),
                default(TimeSpan),
                defaultBindingMode: BindingMode.TwoWay
            );

        public TimeSpan Time {
            get { return (TimeSpan)GetValue(TimeProperty); }
            set { SetValue(TimeProperty, value); }
        }

        public static BindableProperty FormatProperty =
            BindableProperty.Create(
                nameof(Format),
                typeof(string),
                typeof(TimePickerCell),
                "t",
                defaultBindingMode: BindingMode.OneWay
            );

        public string Format {
            get { return (string)GetValue(FormatProperty); }
            set { SetValue(FormatProperty, value); }
        }

        public static BindableProperty TitleProperty =
            BindableProperty.Create(
                nameof(Title),
                typeof(string),
                typeof(TimePickerCell),
                default(string),
                defaultBindingMode: BindingMode.OneWay
            );

        public string Title {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
    }
}
