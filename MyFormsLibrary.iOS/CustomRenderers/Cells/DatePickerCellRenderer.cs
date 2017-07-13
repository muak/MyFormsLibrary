using System;
using CoreGraphics;
using Foundation;
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
                _tableViewCell = new DatePickerCellView(item,item.GetType().FullName);
            }
            else {
                item.PropertyChanged -= Item_PropertyChanged;
            }

            Cell = _tableViewCell;
            _tableViewCell.Cell = item;

            item.PropertyChanged += Item_PropertyChanged;

            WireUpForceUpdateSizeRequested(item, _tableViewCell, tv);

            UpdateBackground(_tableViewCell, _datePikcerCell);
            UpdateBase();

            _tableViewCell.UpdateDate();
            _tableViewCell.UpdateMaximumDate();
            _tableViewCell.UpdateMinimumDate();

            return _tableViewCell;
        }

        void Item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == DatePickerCell.DateProperty.PropertyName) {
                _tableViewCell.UpdateDate();
            }
            else if (e.PropertyName == DatePickerCell.MaximumDateProperty.PropertyName) {
                _tableViewCell.UpdateMaximumDate();
            }
            else if (e.PropertyName == DatePickerCell.MinimumDateProperty.PropertyName) {
                _tableViewCell.UpdateMinimumDate();
            }
        }
    }

    public class DatePickerCellView : CellBaseView
    {
        public UIDatePicker Picker { get; set; }

        NSDate _preSelectedDate;
        DatePickerCell _datePickerCell => Cell as DatePickerCell;
        NoCaretField _entry;

        public DatePickerCellView(Cell cell,string cellName) : base( cellName)
        {
            Cell = cell;
            _entry = new NoCaretField();
            _entry.BorderStyle = UITextBorderStyle.None;
            _entry.BackgroundColor = UIColor.Clear;
            ContentView.AddSubview(_entry);
            ContentView.SendSubviewToBack(_entry);

            Picker = new UIDatePicker { Mode = UIDatePickerMode.Date, TimeZone = new Foundation.NSTimeZone("UTC") };

            var width = UIScreen.MainScreen.Bounds.Width;
            var toolbar = new UIToolbar(new CGRect(0, 0, width, 44)) { BarStyle = UIBarStyle.Default, Translucent = true };

            var cancelButton = new UIBarButtonItem(UIBarButtonSystemItem.Cancel, (o, e) => {
                _entry.ResignFirstResponder();
                Picker.Date = _preSelectedDate;
            });

            var spacer = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace);
            var doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done, (o, a) => {
                _entry.ResignFirstResponder();
                _datePickerCell.Date = Picker.Date.ToDateTime().Date;
                DetailTextLabel.Text = _datePickerCell.Date.ToString(_datePickerCell.Format);
                _preSelectedDate = Picker.Date;
            });

            if(!string.IsNullOrEmpty(_datePickerCell.TodayText)){
                var labelButton = new UIBarButtonItem(_datePickerCell.TodayText, UIBarButtonItemStyle.Plain, (sender, e) => {
                    SetToday();
                });
                var fixspacer = new UIBarButtonItem(UIBarButtonSystemItem.FixedSpace) { Width = 20 };
                toolbar.SetItems(new[] { cancelButton, spacer, labelButton, fixspacer, doneButton }, false);
            }
            else{
                toolbar.SetItems(new[] {cancelButton, spacer, doneButton }, false);
            }

            _entry.InputView = Picker;
            _entry.InputAccessoryView = toolbar;
        
        }

        public void UpdateDate()
        {
            Picker.SetDate(_datePickerCell.Date.ToNSDate(),true);
            DetailTextLabel.Text = _datePickerCell.Date.ToString(_datePickerCell.Format);
            _preSelectedDate = _datePickerCell.Date.ToNSDate();
        }

        public void UpdateMaximumDate()
        {
            Picker.MaximumDate = _datePickerCell.MaximumDate.ToNSDate();
        }

        public void UpdateMinimumDate()
        {
            Picker.MinimumDate = _datePickerCell.MinimumDate.ToNSDate();
        }

        void SetToday()
        {
            if(Picker.MinimumDate.ToDateTime() <= DateTime.Today && Picker.MaximumDate.ToDateTime() >= DateTime.Today){
                Picker.SetDate(DateTime.Today.ToNSDate(), true);
            }
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            _entry.Frame = new CGRect(0, 0, Frame.Width, Frame.Height);
        }

    }
}
