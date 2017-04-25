using System;
using MyFormsLibrary.CustomRenderers;
using MyFormsLibrary.Droid.CustomRenderers;
using Xamarin.Forms;
using Android.Content;
using APicker = Android.Widget.NumberPicker;
using Android.App;
using Xamarin.Forms.Platform.Android;
using System.Windows.Input;
using Android.Views;
using Android.Graphics.Drawables;

[assembly: ExportRenderer(typeof(NumberPickerCell), typeof(NumberPickerCellRenderer))]
namespace MyFormsLibrary.Droid.CustomRenderers
{
    public class NumberPickerCellRenderer:CellBaseRenderer
    {
        NumberPickerCell _numberPickerCell;
        NumberPickerCellView _cellView;
        APicker _picker;
        AlertDialog _dialog;
        Context _context;
        string _title;
        ICommand _command;
        int _max;
        int _min;

       protected override Android.Views.View GetCellCore(Cell item, Android.Views.View convertView, Android.Views.ViewGroup parent, Android.Content.Context context)
        {
            base.GetCellCore(item, convertView, parent, context);

            _context = context;
            _numberPickerCell = item as NumberPickerCell;

            _cellView = convertView as NumberPickerCellView;
            if (_cellView == null) {
                _cellView = new NumberPickerCellView(context, item);

            }
            else {
                _cellView.Click -= _cellView_Click;
            }

            BaseView = _cellView;

            _cellView.Click += _cellView_Click;


            UpdateBase();

            UpdateMin();
            UpdateMax();
            UpdateTitle();
            UpdateNumber();

            return _cellView;
        }

        protected override void OnCellPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnCellPropertyChanged(sender, e);

            if (e.PropertyName == NumberPickerCell.MinProperty.PropertyName) {
                UpdateMin();
            }
            else if(e.PropertyName == NumberPickerCell.MaxProperty.PropertyName) {
                UpdateMax();
            }
            else if (e.PropertyName == NumberPickerCell.NumberProperty.PropertyName) {
                UpdateNumber();
            }
            else if (e.PropertyName == NumberPickerCell.TitleProperty.PropertyName) {
                UpdateTitle();
            }
        }

        void UpdateMin()
        {
            _min = _numberPickerCell.Min;
        }

        void UpdateMax()
        {
            _max = _numberPickerCell.Max;
        }

        void UpdateNumber()
        {
            _cellView.ValueText.Text = _numberPickerCell.Number.ToString();
        }

        void UpdateTitle()
        {
            _title = _numberPickerCell.Title;
        }

        void CreateDialog()
        {
            _picker = new APicker(_context);
            _picker.MinValue = _min;
            _picker.MaxValue = _max;
            _picker.Value = _numberPickerCell.Number;
 
            if (_dialog == null) {
                using (var builder = new AlertDialog.Builder(_context)) {

                    builder.SetTitle(_title);

                    Android.Widget.FrameLayout parent = new Android.Widget.FrameLayout(_context);
                    parent.AddView(_picker, new Android.Widget.FrameLayout.LayoutParams(
                            ViewGroup.LayoutParams.WrapContent,
                            ViewGroup.LayoutParams.WrapContent,
                           GravityFlags.Center));
                    builder.SetView(parent);


                    builder.SetNegativeButton(global::Android.Resource.String.Cancel, (o, args) => {
                        _cellView.ClearFocus();
                    });
                    builder.SetPositiveButton(global::Android.Resource.String.Ok, (o, args) => {
                        _numberPickerCell.SetValue(NumberPickerCell.NumberProperty,_picker.Value);
                        _command?.Execute(_picker.Value);
                        _cellView.ClearFocus();
                    });

                    _dialog = builder.Create();
                }
                _dialog.SetCanceledOnTouchOutside(true);
                _dialog.DismissEvent += (ss, ee) => {
                    _dialog.Dispose();
                    _dialog = null;
                    _picker.RemoveFromParent();
                    _picker.Dispose();
                    _picker = null;
                };

                _dialog.Show();
            }

        }

        void _cellView_Click(object sender, EventArgs e)
        {
            CreateDialog();
        }
    }

    public class NumberPickerCellView : CellBaseView
    {
        public NumberPickerCellView(Context context, Cell cell) : base(context, cell)
        {
            ValueText.Visibility = Android.Views.ViewStates.Visible;
        }
    }
}
