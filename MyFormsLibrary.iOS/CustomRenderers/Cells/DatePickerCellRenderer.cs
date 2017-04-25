using System;
using CoreGraphics;
using MyFormsLibrary.CustomRenderers;
using MyFormsLibrary.iOS.CustomRenderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(DatePickerCell), typeof(DatePickerCellRenderer))]
namespace MyFormsLibrary.iOS.CustomRenderers
{
    public class DatePickerCellRenderer:CellBaseRenderer
    {
        DatePickerCell _datePikcerCell;
        DatePickerCellView _tableViewCell;
        string _format;

        public override UITableViewCell GetCell(Cell item, UITableViewCell reusableCell, UITableView tv)
        {
            base.GetCell(item, reusableCell, tv);

            _datePikcerCell = (DatePickerCell)item;

            _tableViewCell = reusableCell as DatePickerCellView;
            if (_tableViewCell == null) {
                _tableViewCell = new DatePickerCellView(item.GetType().FullName);
            }
            else {
                item.PropertyChanged -= Item_PropertyChanged;
                _tableViewCell.Picker.ValueChanged -= Picker_ValueChanged;
            }

            Cell = _tableViewCell;
            _tableViewCell.Cell = item;

            item.PropertyChanged += Item_PropertyChanged;
            _tableViewCell.Picker.ValueChanged += Picker_ValueChanged;

            WireUpForceUpdateSizeRequested(item, _tableViewCell, tv);

            UpdateBackground(_tableViewCell, _datePikcerCell);
            UpdateBase();

            UpdateFormat();
            UpdateMaximumDate();
            UpdateMinimumDate();


            return _tableViewCell;
        }

        void Item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == DatePickerCell.DateProperty.PropertyName) {
                UpdateDate();
            }
            else if (e.PropertyName == DatePickerCell.FormatProperty.PropertyName) {
                UpdateFormat();
            }
            else if (e.PropertyName == DatePickerCell.MaximumDateProperty.PropertyName) {
                UpdateMaximumDate();
            }
            else if (e.PropertyName == DatePickerCell.MinimumDateProperty.PropertyName) {
                UpdateMinimumDate();
            }

        }

        void UpdateFormat()
        {
            _format = _datePikcerCell.Format;
            UpdateDate();
        }

        void UpdateDate()
        {
            _tableViewCell.Picker.SetDate(_datePikcerCell.Date.ToNSDate(),true);
            _tableViewCell.DetailTextLabel.Text = _datePikcerCell.Date.ToString(_format);
        }

        void UpdateMaximumDate()
        {
            _tableViewCell.Picker.MaximumDate = _datePikcerCell.MaximumDate.ToNSDate();
        }

        void UpdateMinimumDate()
        {
            _tableViewCell.Picker.MinimumDate = _datePikcerCell.MinimumDate.ToNSDate();
        }

        void Picker_ValueChanged(object sender, EventArgs e)
        {
            _datePikcerCell.SetValue(DatePickerCell.DateProperty, _tableViewCell.Picker.Date.ToDateTime().Date);
            _tableViewCell.DetailTextLabel.Text = _datePikcerCell.Date.ToString(_format);
        }
    }

    public class DatePickerCellView : CellBaseView
    {
        public UIDatePicker Picker { get; set; }
        private NoCaretField _entry;

        public DatePickerCellView(string cellName) : base( cellName)
        {
            
            _entry = new NoCaretField();
            _entry.BorderStyle = UITextBorderStyle.None;
            _entry.BackgroundColor = UIColor.Clear;
            ContentView.AddSubview(_entry);
            ContentView.SendSubviewToBack(_entry);

            Picker = new UIDatePicker { Mode = UIDatePickerMode.Date, TimeZone = new Foundation.NSTimeZone("UTC") };

            var width = UIScreen.MainScreen.Bounds.Width;
            var toolbar = new UIToolbar(new CGRect(0, 0, width, 44)) { BarStyle = UIBarStyle.Default, Translucent = true };
            var spacer = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace);
            var doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done, (o, a) => {
                _entry.ResignFirstResponder();
            });

            toolbar.SetItems(new[] { spacer, doneButton }, false);

            _entry.InputView = Picker;
            _entry.InputAccessoryView = toolbar;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            _entry.Frame = new CGRect(0, 0, Frame.Width, Frame.Height);
        }

    }
}
