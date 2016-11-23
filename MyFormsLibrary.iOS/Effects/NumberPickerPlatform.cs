using System;
using System.Drawing;
using MyFormsLibrary.iOS.Effects;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using System.Collections.Generic;
using System.Linq;
using MyFormsLibrary.Effects;
using CoreGraphics;
using System.Windows.Input;

[assembly: ExportEffect(typeof(NumberPickerPlatform), nameof(NumberPicker))]
namespace MyFormsLibrary.iOS.Effects
{
    public class NumberPickerPlatform:PlatformEffect
    {
        private UIPickerView Picker;
        private NoCaretField Entry;
        private int Min;
        private int Max;
        private IList<int> Items;
        private UIView View;
        private NSLayoutConstraint[] Constraint;
        private UILabel Title;
        private ICommand Command;

        protected override void OnAttached() {
            View = Control ?? Container;

            Entry = new NoCaretField();
            Entry.Frame = new CGRect(0, 0, 1, 1);
            Entry.BorderStyle = UITextBorderStyle.None;
            Entry.BackgroundColor = UIColor.Clear;
            View.AddSubview(Entry);

            View.AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth;
            Entry.TranslatesAutoresizingMaskIntoConstraints = false;
            Constraint = new NSLayoutConstraint[]{
                NSLayoutConstraint.Create(
                    Entry,
                    NSLayoutAttribute.Top,
                    NSLayoutRelation.Equal,
                    View,
                    NSLayoutAttribute.Top,
                    1,
                    0
                ),
                NSLayoutConstraint.Create(
                    Entry,
                    NSLayoutAttribute.Left,
                    NSLayoutRelation.Equal,
                    View,
                    NSLayoutAttribute.Left,
                    1,
                    0
                ),
                NSLayoutConstraint.Create(
                    Entry,
                    NSLayoutAttribute.Right,
                    NSLayoutRelation.Equal,
                    View,
                    NSLayoutAttribute.Right,
                    1,
                    0
                ),
                NSLayoutConstraint.Create(
                    Entry,
                    NSLayoutAttribute.Bottom,
                    NSLayoutRelation.Equal,
                    View,
                    NSLayoutAttribute.Bottom,
                    1,
                    0
                ),
                NSLayoutConstraint.Create(
                    Entry,
                    NSLayoutAttribute.Width,
                    NSLayoutRelation.Equal,
                    View,
                    NSLayoutAttribute.Width,
                    1,
                    0
                ),
                NSLayoutConstraint.Create(
                    Entry,
                    NSLayoutAttribute.Height,
                    NSLayoutRelation.Equal,
                    View,
                    NSLayoutAttribute.Height,
                    1,
                    0
                ),
            };
            View.UserInteractionEnabled = true;
            View.AddConstraints(Constraint);
            View.SendSubviewToBack(Entry);

            Picker = new UIPickerView();

            Title = new UILabel();
            UpdateTitle();

            var width = UIScreen.MainScreen.Bounds.Width;
            var toolbar = new UIToolbar(new RectangleF(0, 0, (float)width, 44)) { BarStyle = UIBarStyle.Default, Translucent = true };
            var cancelButton = new UIBarButtonItem(UIBarButtonSystemItem.Cancel, (o, e) => {
                Entry.ResignFirstResponder();
            });

            var labelButton = new UIBarButtonItem(Title);
            var spacer = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace);
            var doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done, (o, a) => {
                var s = (PickerSource)Picker.Model;
                UpdatePickerFromModel(s);
                Entry.ResignFirstResponder();
                Command?.Execute(int.Parse(((PickerSource)Picker.Model).SelectedItem));
            });

            toolbar.SetItems(new[] {cancelButton,spacer, labelButton,spacer, doneButton }, false);

            Entry.InputView = Picker;
            Entry.InputAccessoryView = toolbar;

            Picker.Model = new PickerSource(this);

            UpdateList();
            UpdateSelect();
            UpdateCommand();
        }

        protected override void OnDetached() {
            View.RemoveConstraints(Constraint);
            Entry.RemoveFromSuperview();
            Entry.Dispose();
            Title.Dispose();
            Picker.Dispose();
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
            else if (e.PropertyName == NumberPicker.CommandProperty.PropertyName) {
                UpdateCommand();
            }

        }


        void UpdateList() {
            Max = NumberPicker.GetMax(Element);
            Min = NumberPicker.GetMin(Element);
            if (Min < 0) Min = 0;
            if (Max < 0) Max = 0;
            Items = Enumerable.Range(Min,Max-Min+1).ToList();
        }

        void UpdateSelect() {
            var a = Items.IndexOf(NumberPicker.GetSelectedItem(Element));
            ((PickerSource)Picker.Model).SelectedIndex = a;
            ((PickerSource)Picker.Model).SelectedItem = NumberPicker.GetSelectedItem(Element).ToString();
            Picker.Select(a,0,false);       }

        void UpdatePickerFromModel(PickerSource s) {
            NumberPicker.SetSelectedItem(Element,Convert.ToInt32(s.SelectedItem));
        }

        void UpdateTitle() {
            Title.Text = NumberPicker.GetTitle(Element);
            Title.SizeToFit();
        }
        void UpdateCommand() {
            Command = NumberPicker.GetCommand(Element);
        }


        class PickerSource : UIPickerViewModel
        {
            readonly NumberPickerPlatform _effect;

            public PickerSource(NumberPickerPlatform model) {
                _effect = model;
            }

            public int SelectedIndex { get; internal set; }

            public string SelectedItem { get; internal set; }

            public override nint GetComponentCount(UIPickerView picker) {
                return 1;
            }

            public override nint GetRowsInComponent(UIPickerView pickerView, nint component) {
                return _effect.Items != null ? _effect.Items.Count : 0;
            }

            public override string GetTitle(UIPickerView picker, nint row, nint component) {
                return _effect.Items[(int)row].ToString();
            }

            public override void Selected(UIPickerView picker, nint row, nint component) {
                if (_effect.Items.Count == 0) {
                    SelectedItem = null;
                    SelectedIndex = -1;
                }
                else {
                    SelectedItem = _effect.Items[(int)row].ToString();
                    SelectedIndex = (int)row;
                }
                _effect.UpdatePickerFromModel(this);
            }
        }

        internal class NoCaretField : UITextField
        {
            public NoCaretField() : base(new RectangleF()) {
            }

            public override CoreGraphics.CGRect GetCaretRectForPosition(UITextPosition position) {
                return new RectangleF();
            }

        }
    }

}
