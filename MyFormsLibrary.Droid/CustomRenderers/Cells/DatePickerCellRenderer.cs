using System;
using MyFormsLibrary.CustomRenderers;
using MyFormsLibrary.Droid.CustomRenderers;
using Xamarin.Forms;
using Android.Content;
using Android.App;

[assembly: ExportRenderer(typeof(DatePickerCell), typeof(DatePickerCellRenderer))]
namespace MyFormsLibrary.Droid.CustomRenderers
{
    public class DatePickerCellRenderer : CellBaseRenderer
    {
        DatePickerCell _datePickerCell;
        DatePickerCellView _cellView;
        DatePickerDialog _dialog;
        Context _context;
        string _format;

        protected override Android.Views.View GetCellCore(Xamarin.Forms.Cell item, Android.Views.View convertView, Android.Views.ViewGroup parent, Android.Content.Context context)
        {
            base.GetCellCore(item, convertView, parent, context);

            _context = context;
            _datePickerCell = item as DatePickerCell;

            _cellView = convertView as DatePickerCellView;
            if (_cellView == null) {
                _cellView = new DatePickerCellView(context, item);

            }
            else {
                _cellView.Click -= _cellView_Click;
            }

            BaseView = _cellView;

            _cellView.Click += _cellView_Click;


            UpdateBase();

            UpdateFormat();
            UpdateMaximumDate();
            UpdateMinimumDate();

            return _cellView;
        }

        protected override void OnCellPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnCellPropertyChanged(sender, e);

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

        void _cellView_Click(object sender, EventArgs e)
        {
            CreateDatePickerDialog(_datePickerCell.Date.Year, _datePickerCell.Date.Month - 1, _datePickerCell.Date.Day);

            UpdateMinimumDate();
            UpdateMaximumDate();

            _dialog.CancelEvent += OnCancelButtonClicked;

            _dialog.Show();
        }

        void CreateDatePickerDialog(int year, int month, int day)
        {

            _dialog = new DatePickerDialog(_context, (o, e) => {
                _datePickerCell.Date = e.Date;
                _cellView.ClearFocus();
                _dialog.CancelEvent -= OnCancelButtonClicked;

                _dialog = null;
            }, year, month, day);
        }

        void UpdateFormat()
        {
            _format = _datePickerCell.Format;
            UpdateDate();
        }

        void UpdateDate()
        {
            _cellView.ValueText.Text = _datePickerCell.Date.ToString(_format);
        }

        void UpdateMaximumDate()
        {
            if (_dialog != null) {
                _dialog.DatePicker.MaxDate = (long)_datePickerCell.MaximumDate.ToUniversalTime().Subtract(DateTime.MinValue.AddYears(1969)).TotalMilliseconds;
            }
        }

        void UpdateMinimumDate()
        {
            if (_dialog != null) {
                _dialog.DatePicker.MinDate = (long)_datePickerCell.MinimumDate.ToUniversalTime().Subtract(DateTime.MinValue.AddYears(1969)).TotalMilliseconds;
            }
        }

        void OnCancelButtonClicked(object sender, EventArgs e)
        {
            _cellView.ClearFocus();
        }
    }

    public class DatePickerCellView : CellBaseView
    {

        public DatePickerCellView(Context context, Cell cell) : base(context, cell)
        {
            ValueText.Visibility = Android.Views.ViewStates.Visible;
        }
    }
}
