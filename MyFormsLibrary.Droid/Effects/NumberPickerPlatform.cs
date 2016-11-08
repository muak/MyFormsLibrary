using System;
using Android.App;
using Android.Views;
using MyFormsLibrary.Droid.Effects;
using MyFormsLibrary.Effects;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using APicker = Android.Widget.NumberPicker;

[assembly: ExportEffect(typeof(NumberPickerPlatform), nameof(NumberPicker))]
namespace MyFormsLibrary.Droid.Effects
{
    public class NumberPickerPlatform:PlatformEffect
    {
        private AlertDialog Dialog;
        private APicker Picker;
        private Android.Views.View View;
        private string Title;


        protected override void OnAttached() {

            View = Control ?? Container;

            Picker = new APicker(Container.Context);

            View.Click += (sender, e) => {
                if (Dialog == null) {
                    using (var builder = new AlertDialog.Builder(Container.Context)) {

                        builder.SetTitle(Title);

                        var cancelValue = Picker.Value;

                        Android.Widget.FrameLayout parent = new Android.Widget.FrameLayout(Container.Context);
                        parent.AddView(Picker, new Android.Widget.FrameLayout.LayoutParams(
                                ViewGroup.LayoutParams.WrapContent,
                                ViewGroup.LayoutParams.WrapContent,
                               GravityFlags.Center));
                        builder.SetView(parent);


                        builder.SetNegativeButton(global::Android.Resource.String.Cancel, (o, args) => {
                            Picker.Value = cancelValue;
                        });
                        builder.SetPositiveButton(global::Android.Resource.String.Ok, (o, args) => { 
                            NumberPicker.SetSelectedItem(Element,Picker.Value);
                        });

                        Dialog = builder.Create();
                    }
                    Dialog.SetCanceledOnTouchOutside(true);
                    Dialog.DismissEvent += (ss, ee) => {
                        Dialog.Dispose();
                        Dialog = null;
                        Picker.RemoveFromParent();
                    };

                    Dialog.Show();
                }
            };

            UpdateList();
            UpdateSelect();
            UpdateTitle();
        }

        protected override void OnDetached() {
            Picker.Dispose();
            Picker = null;
            if (Dialog != null) {
                Dialog.Dispose();
                Dialog = null;
            }
        }

        void UpdateList() {
            Picker.MinValue = NumberPicker.GetMin(Element);
            Picker.MaxValue = NumberPicker.GetMax(Element);
        }
        void UpdateSelect() {
            Picker.Value = NumberPicker.GetSelectedItem(Element);
        }
        void UpdateTitle() {
            Title = NumberPicker.GetTitle(Element);
        }

        protected override void OnElementPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e) {
            base.OnElementPropertyChanged(e);

            if (e.PropertyName == NumberPicker.MaxProperty.PropertyName) {
                UpdateList();
            }
            else if (e.PropertyName == NumberPicker.MinProperty.PropertyName) {
                UpdateList();
            }
            else if (e.PropertyName == NumberPicker.SelectedItemProperty.PropertyName) {
                UpdateSelect();
            }
            else if (e.PropertyName == NumberPicker.TitleProperty.PropertyName) {
                UpdateTitle();
            }

        }

    }
}
