using System;
using Android.Content;
using MyFormsLibrary.CustomRenderers;
using MyFormsLibrary.Droid.CustomRenderers;
using Xamarin.Forms;
using Android.App;
using Android.Runtime;
using Android.Text.Format;
using Android.Widget;

[assembly: ExportRenderer(typeof(TimePickerCell), typeof(TimePickerCellRenderer))]
namespace MyFormsLibrary.Droid.CustomRenderers
{
    public class TimePickerCellRenderer:CellBaseRenderer
    {
        TimePickerCell _timePickerCell;
        TimePickerCellView _cellView;
        TimePickerDialog _dialog;
        Context _context;
        string _title;


        protected override Android.Views.View GetCellCore(Cell item, Android.Views.View convertView, Android.Views.ViewGroup parent, Android.Content.Context context)
        {
            base.GetCellCore(item, convertView, parent, context);

            _context = context;
            _timePickerCell = item as TimePickerCell;

            _cellView = convertView as TimePickerCellView;
            if (_cellView == null) {
                _cellView = new TimePickerCellView(context, item);

            }
            else {
                _cellView.Click -= _cellView_Click;
            }

            BaseView = _cellView;

            _cellView.Click += _cellView_Click;


            UpdateBase();

            UpdateTitle();
            UpdateTime();

            return _cellView;
        }

        protected override void OnCellPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnCellPropertyChanged(sender, e);

            if (e.PropertyName == TimePickerCell.TimeProperty.PropertyName) {
                UpdateTime();
            }
            else if (e.PropertyName == TimePickerCell.TitleProperty.PropertyName) {
                UpdateTitle();
            }
        }


        void UpdateTime()
        {
            _cellView.ValueText.Text = DateTime.Today.Add(_timePickerCell.Time).ToString(_timePickerCell.Format);
        }

        void UpdateTitle()
        {
            _title = _timePickerCell.Title;
        }

        void CreateDialog()
        {

            if (_dialog == null) {
                bool is24HourFormat = DateFormat.Is24HourFormat(_context);
                _dialog = new TimePickerDialog(_context, TimeSelected,_timePickerCell.Time.Hours, _timePickerCell.Time.Minutes, is24HourFormat);

                var title = new TextView(_context);

                if (!string.IsNullOrEmpty(_title)) {                   
                    title.Gravity = Android.Views.GravityFlags.Center;
                    title.SetPadding(10, 10, 10, 10);
                    title.Text = _title;
                    _dialog.SetCustomTitle(title);
                }

                _dialog.SetCanceledOnTouchOutside(true);

                _dialog.DismissEvent += (ss, ee) => {
                    title.Dispose();
                    _dialog.Dispose();
                    _dialog = null;
                };

                _dialog.Show();
            }

        }

        void TimeSelected(object sender, TimePickerDialog.TimeSetEventArgs e)
        {
            _timePickerCell.Time = new TimeSpan(e.HourOfDay, e.Minute, 0);
            _cellView.ValueText.Text = DateTime.Today.Add(_timePickerCell.Time).ToString(_timePickerCell.Format);
        }

        void _cellView_Click(object sender, EventArgs e)
        {
            CreateDialog();
        }
    }

    public class TimePickerCellView : CellBaseView
    {
        public TimePickerCellView(Context context, Cell cell) : base(context, cell)
        {
            ValueText.Visibility = Android.Views.ViewStates.Visible;
        }
    }
}
