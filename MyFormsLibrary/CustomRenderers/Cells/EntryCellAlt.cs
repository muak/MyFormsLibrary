using System;
using Xamarin.Forms;

namespace MyFormsLibrary.CustomRenderers
{
    public class EntryCellAlt:CellBase,IEntryCellController
    {
        public EntryCellAlt() {
        }

        public static BindableProperty TextProperty =
            BindableProperty.Create(
                nameof(Text),
                typeof(string),
                typeof(EntryCellAlt),
                default(string),
                defaultBindingMode: BindingMode.OneWay
            );

        public string Text {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static BindableProperty TextColorProperty =
            BindableProperty.Create(
                nameof(TextColor),
                typeof(Color),
                typeof(EntryCellAlt),
                default(Color),
                defaultBindingMode: BindingMode.OneWay
            );

        public Color TextColor {
            get { return (Color)GetValue(TextColorProperty); }
            set { SetValue(TextColorProperty, value); }
        }

        public static BindableProperty TextFontSizeProperty =
            BindableProperty.Create(
                nameof(TextFontSize),
                typeof(double),
                typeof(EntryCellAlt),
                -1.0d,
                defaultBindingMode: BindingMode.OneWay
            );

        [TypeConverter(typeof(FontSizeConverter))]
        public double TextFontSize {
            get { return (double)GetValue(TextFontSizeProperty); }
            set { SetValue(TextFontSizeProperty, value); }
        }

        public static BindableProperty KeyboardProperty =
            BindableProperty.Create(
                nameof(Keyboard),
                typeof(Keyboard),
                typeof(EntryCellAlt),
                Keyboard.Default,
                defaultBindingMode: BindingMode.OneWay
            );

        public Keyboard Keyboard {
            get { return (Keyboard)GetValue(KeyboardProperty); }
            set { SetValue(KeyboardProperty, value); }
        }

        public event EventHandler Completed;
        public void SendCompleted() {
           EventHandler handler = Completed;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        public static BindableProperty PlaceholderProperty =
            BindableProperty.Create(
                nameof(Placeholder),
                typeof(string),
                typeof(EntryCellAlt),
                default(string),
                defaultBindingMode: BindingMode.OneWay
            );

        public string Placeholder {
            get { return (string)GetValue(PlaceholderProperty); }
            set { SetValue(PlaceholderProperty, value); }
        }
    }
}
