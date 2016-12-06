﻿using System;
using Xamarin.Forms;

namespace MyFormsLibrary.CustomRenderers
{
    public class DatePickerCell:CellBase
    {
        public DatePickerCell()
        {
        }

        public static BindableProperty DateProperty =
            BindableProperty.Create(
                nameof(Date),
                typeof(DateTime),
                typeof(DatePickerCell),
                default(DateTime),
                defaultBindingMode: BindingMode.TwoWay
            );

        public DateTime Date {
            get { return (DateTime)GetValue(DateProperty); }
            set { SetValue(DateProperty, value); }
        }

        public static BindableProperty MaximunDateProperty =
            BindableProperty.Create(
                nameof(MaximunDate),
                typeof(DateTime),
                typeof(DatePickerCell),
                new DateTime(2100, 12, 31),
                defaultBindingMode: BindingMode.OneWay
            );

        public DateTime MaximunDate {
            get { return (DateTime)GetValue(MaximunDateProperty); }
            set { SetValue(MaximunDateProperty, value); }
        }

        public static BindableProperty MinimumDateProperty =
            BindableProperty.Create(
                nameof(MinimumDate),
                typeof(DateTime),
                typeof(DatePickerCell),
                new DateTime(1900, 1, 1),
                defaultBindingMode: BindingMode.OneWay
            );

        public DateTime MinimumDate {
            get { return (DateTime)GetValue(MinimumDateProperty); }
            set { SetValue(MinimumDateProperty, value); }
        }

        public static BindableProperty FormatProperty =
            BindableProperty.Create(
                nameof(Format),
                typeof(string),
                typeof(DatePickerCell),
                "d",
                defaultBindingMode: BindingMode.OneWay
            );

        public string Format {
            get { return (string)GetValue(FormatProperty); }
            set { SetValue(FormatProperty, value); }
        }

        public static BindableProperty AccentColorProperty =
            BindableProperty.Create(
                nameof(AccentColor),
                typeof(Color),
                typeof(DatePickerCell),
                default(Color),
                defaultBindingMode: BindingMode.OneWay
            );

        public Color AccentColor {
            get { return (Color)GetValue(AccentColorProperty); }
            set { SetValue(AccentColorProperty, value); }
        }
    }
}
