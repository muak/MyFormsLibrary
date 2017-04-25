using System;
using CoreGraphics;
using Foundation;
using MyFormsLibrary.CustomRenderers;
using MyFormsLibrary.iOS.CustomRenderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(TimePickerCell), typeof(TimePickerCellRenderer))]
namespace MyFormsLibrary.iOS.CustomRenderers
{
    public class TimePickerCellRenderer:CellBaseRenderer
    {
        TimePickerCell _timePikcerCell;
        TimePickerCellView _tableViewCell;

        public override UITableViewCell GetCell(Cell item, UITableViewCell reusableCell, UITableView tv)
        {
            base.GetCell(item, reusableCell, tv);

            _timePikcerCell = (TimePickerCell)item;

            _tableViewCell = reusableCell as TimePickerCellView;
            if (_tableViewCell == null) {
                _tableViewCell = new TimePickerCellView(item.GetType().FullName);
            }
            else {
                _tableViewCell.Done -= _tableViewCell_Done;
                _tableViewCell.Canceled -= _tableViewCell_Canceled;
            }

            Cell = _tableViewCell;
            _tableViewCell.Cell = item;

            _tableViewCell.Done += _tableViewCell_Done;
            _tableViewCell.Canceled += _tableViewCell_Canceled;

            item.PropertyChanged += Item_PropertyChanged; ;

            WireUpForceUpdateSizeRequested(item, _tableViewCell, tv);

            UpdateBackground(_tableViewCell, _timePikcerCell);
            UpdateBase();

            UpdateTime();
            UpdateTitle();

            return _tableViewCell;
        }

        void Item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == TimePickerCell.TimeProperty.PropertyName) {
                UpdateTime();
            }
            else if (e.PropertyName == TimePickerCell.TitleProperty.PropertyName) {
                UpdateTitle();
            }
        }

        void _tableViewCell_Canceled(object sender, EventArgs e)
        {
            _tableViewCell.Picker.Date = _tableViewCell.PreSelectedDate;
        }

        void _tableViewCell_Done(object sender, EventArgs e)
        {
            _timePikcerCell.Time = _tableViewCell.Picker.Date.ToDateTime() - new DateTime(1, 1, 1);
            _tableViewCell.DetailTextLabel.Text = DateTime.Today.Add(_timePikcerCell.Time).ToString(_timePikcerCell.Format);
            _tableViewCell.PreSelectedDate = _tableViewCell.Picker.Date;
        }

        void UpdateTime()
        {
            _tableViewCell.Picker.Date = new DateTime(1, 1, 1).Add(_timePikcerCell.Time).ToNSDate();
            _tableViewCell.DetailTextLabel.Text = DateTime.Today.Add(_timePikcerCell.Time).ToString(_timePikcerCell.Format);
            _tableViewCell.PreSelectedDate = _tableViewCell.Picker.Date;
        }


        void UpdateTitle()
        {
            _tableViewCell.Title.Text = _timePikcerCell.Title;
            _tableViewCell.Title.SizeToFit();
        }

    }

    public class TimePickerCellView : CellBaseView
    {
        public UILabel Title { get; private set; }
        public UIDatePicker Picker { get; private set; }
        public NSDate PreSelectedDate { get; set; }

        public event EventHandler Canceled;
        public event EventHandler Done;

        private NoCaretField _entry;

        public TimePickerCellView(string cellName) : base(cellName)
        {
            _entry = new NoCaretField();
            _entry.BorderStyle = UITextBorderStyle.None;
            _entry.BackgroundColor = UIColor.Clear;
            ContentView.AddSubview(_entry);
            ContentView.SendSubviewToBack(_entry);

            Picker = new UIDatePicker { Mode = UIDatePickerMode.Time, TimeZone = new NSTimeZone("UTC") };

            Title = new UILabel();

            var width = UIScreen.MainScreen.Bounds.Width;
            var toolbar = new UIToolbar(new CGRect(0, 0, (float)width, 44)) { BarStyle = UIBarStyle.Default, Translucent = true };
            var cancelButton = new UIBarButtonItem(UIBarButtonSystemItem.Cancel, (o, e) => {
                _entry.ResignFirstResponder();
                Picker.Date = PreSelectedDate;
                Canceled?.Invoke(this, EventArgs.Empty);
            });

            var labelButton = new UIBarButtonItem(Title);
            var spacer = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace);
            var doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done, (o, a) => {
                _entry.ResignFirstResponder();
                Done?.Invoke(this, EventArgs.Empty);
            });

            toolbar.SetItems(new[] { cancelButton, spacer, labelButton, spacer, doneButton }, false);

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
