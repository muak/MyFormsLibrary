using System;
using MyFormsLibrary.CustomRenderers;
using MyFormsLibrary.iOS.CustomRenderers;
using Xamarin.Forms;
using UIKit;
using CoreGraphics;
using System.Windows.Input;

[assembly: ExportRenderer(typeof(NumberPickerCell), typeof(NumberPickerCellRenderer))]
namespace MyFormsLibrary.iOS.CustomRenderers
{
    public class NumberPickerCellRenderer:CellBaseRenderer
    {
        NumberPickerCell _numberPikcerCell;
        NumberPickerCellView _tableViewCell;

        public override UITableViewCell GetCell(Cell item, UITableViewCell reusableCell, UITableView tv)
        {
            base.GetCell(item, reusableCell, tv);

            _numberPikcerCell = (NumberPickerCell)item;

            _tableViewCell = reusableCell as NumberPickerCellView;
            if (_tableViewCell == null) {
                _tableViewCell = new NumberPickerCellView(item.GetType().FullName);
            }
            else {
                _tableViewCell.Model.UpdatePickerFromModel -= Model_UpdatePickerFromModel;
            }

            Cell = _tableViewCell;
            _tableViewCell.Cell = item;

            _tableViewCell.Model.UpdatePickerFromModel += Model_UpdatePickerFromModel;
            item.PropertyChanged += Item_PropertyChanged;;
           
            WireUpForceUpdateSizeRequested(item, _tableViewCell, tv);

            UpdateBackground(_tableViewCell, _numberPikcerCell);
            UpdateBase();

            UpdateNumberList();
            UpdateNumber();
            UpdateTitle();


            return _tableViewCell;
        }

        void Item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == NumberPickerCell.MinProperty.PropertyName ||
                e.PropertyName == NumberPickerCell.MaxProperty.PropertyName) {
                UpdateNumberList();
            }
            else if (e.PropertyName == NumberPickerCell.NumberProperty.PropertyName) {
                UpdateNumber();
            }
            else if (e.PropertyName == NumberPickerCell.TitleProperty.PropertyName) {
                UpdateTitle();
            }
        }

        void UpdateNumber()
        {
            _tableViewCell.Select(_numberPikcerCell.Number);
            _tableViewCell.DetailTextLabel.Text = _numberPikcerCell.Number.ToString();
        }

        void UpdateNumberList()
        {
            _tableViewCell.Model.SetNumbers(_numberPikcerCell.Min,_numberPikcerCell.Max);
        }

        void UpdateTitle()
        {
            _tableViewCell.Title.Text = _numberPikcerCell.Title;
            _tableViewCell.Title.SizeToFit();
        }

        void Model_UpdatePickerFromModel(object sender, EventArgs e)
        {
            _numberPikcerCell.SetValue(NumberPickerCell.NumberProperty, _tableViewCell.Model.SelectedItem);
            _tableViewCell.DetailTextLabel.Text = _tableViewCell.Model.SelectedItem.ToString();
        }
    }

    public class NumberPickerCellView : CellBaseView
    {
        public NumberPickerSource Model { get; private set;}
        public UILabel Title { get;private set; }

        private UIPickerView _picker;
        private NoCaretField _entry;

        private ICommand _command;

        public NumberPickerCellView(string cellName):base(cellName){
            _entry = new NoCaretField();
            _entry.BorderStyle = UITextBorderStyle.None;
            _entry.BackgroundColor = UIColor.Clear;
            ContentView.AddSubview(_entry);
            ContentView.SendSubviewToBack(_entry);

            _picker = new UIPickerView();

            Title = new UILabel();
           
            var width = UIScreen.MainScreen.Bounds.Width;
            var toolbar = new UIToolbar(new CGRect(0, 0, (float)width, 44)) { BarStyle = UIBarStyle.Default, Translucent = true };
            var cancelButton = new UIBarButtonItem(UIBarButtonSystemItem.Cancel, (o, e) => {
                _entry.ResignFirstResponder();
                Select(Model.PreSelectedItem);
            });

            var labelButton = new UIBarButtonItem(Title);
            var spacer = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace);
            var doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done, (o, a) => {
                Model.OnUpdatePickerFormModel();
                _entry.ResignFirstResponder();
                _command?.Execute(Model.SelectedItem);
            });

            toolbar.SetItems(new[] { cancelButton, spacer, labelButton, spacer, doneButton }, false);

            _entry.InputView = _picker;
            _entry.InputAccessoryView = toolbar;

            Model = new NumberPickerSource();
            _picker.Model = Model;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            _entry.Frame = new CGRect(0, 0, Frame.Width, Frame.Height);
        }

        public void Select(int number)
        {
            var idx = Model.Items.IndexOf(number);
            _picker.Select(idx,0,false);
            Model.PreSelectedItem = number;
        }
    }
}
