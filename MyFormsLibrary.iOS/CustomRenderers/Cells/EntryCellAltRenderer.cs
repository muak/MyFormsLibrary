using System;
using System.Drawing;
using MyFormsLibrary.CustomRenderers;
using MyFormsLibrary.iOS.CustomRenderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(EntryCellAlt), typeof(EntryCellAltRenderer))]
namespace MyFormsLibrary.iOS.CustomRenderers
{
    public class EntryCellAltRenderer:CellBaseRenderer
    {
        EntryCellAlt EntryCell;
        EntryCellAltView TableViewCell;

        public override UIKit.UITableViewCell GetCell(Xamarin.Forms.Cell item, UIKit.UITableViewCell reusableCell, UIKit.UITableView tv) {
            base.GetCell(item, reusableCell, tv);

            base.GetCell(item, reusableCell, tv);

            EntryCell = (EntryCellAlt)item;

            TableViewCell = reusableCell as EntryCellAltView;
            if (TableViewCell == null) {
                TableViewCell = new EntryCellAltView(item.GetType().FullName);
               
            }
            else {
                item.PropertyChanged -= Item_PropertyChanged;
                TableViewCell.TextFieldTextChanged -= TableViewCell_TextFieldTextChanged;
            }

            Cell = TableViewCell;
            TableViewCell.Cell = item;

            item.PropertyChanged += Item_PropertyChanged;
            TableViewCell.TextFieldTextChanged += TableViewCell_TextFieldTextChanged;

            WireUpForceUpdateSizeRequested(item, TableViewCell, tv);

            UpdateBackground(TableViewCell, EntryCell);
            UpdateBase();

            UpdateText();
            UpdateTextColor();
            UpdateTextFontSize();
            UpdateKeyboard();

            return TableViewCell;
        }

        void Item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == EntryCellAlt.TextProperty.PropertyName) {
                UpdateText();
            }
            else if (e.PropertyName == EntryCellAlt.TextColorProperty.PropertyName) {
                UpdateTextColor();
            }
            else if (e.PropertyName == EntryCellAlt.TextFontSizeProperty.PropertyName) {
                UpdateTextFontSize();
            }
            else if (e.PropertyName == EntryCellAlt.KeyboardProperty.PropertyName) {
                UpdateKeyboard();
            }
        }

        void TableViewCell_TextFieldTextChanged(object sender, EventArgs e) {
            EntryCell.Text = TableViewCell.TextField.Text;
        }

        void UpdateText() {
            TableViewCell.TextField.Text = EntryCell.Text;
        }
        void UpdateTextColor() {
            if (EntryCell.TextColor != Color.Default) {
                TableViewCell.TextField.TextColor = EntryCell.TextColor.ToUIColor();
            }
        }
        void UpdateTextFontSize() {
            if (EntryCell.TextFontSize < 0) {
                TableViewCell.TextField.Font = TableViewCell.TextField.Font.WithSize((nfloat)EntryCell.LabelFontSize);
            }
            else {
                TableViewCell.TextField.Font =
                                 TableViewCell.TextField.Font.WithSize((nfloat)EntryCell.TextFontSize);
            }
        }
        void UpdateKeyboard() {
            TableViewCell.TextField.ApplyKeyboard(EntryCell.Keyboard);
        }

      
    }

    public class EntryCellAltView : CellTableViewCell
    {
        public UITextField TextField { get; }

        public EntryCellAltView(string cellName) : base(UITableViewCellStyle.Value1,cellName) {
            TextField = new UITextField(new RectangleF(0, 0, 100, 30)) { BorderStyle = UITextBorderStyle.None };
            TextField.TextAlignment = UITextAlignment.Right;

            TextField.EditingChanged += TextFieldOnEditingChanged;
            TextField.ShouldReturn = OnShouldReturn;

            ContentView.AddSubview(TextField);
        }

        public override void LayoutSubviews() {
            base.LayoutSubviews();

            // simple algorithm to generally line up entries
            var start = (float)Math.Round(Math.Max(Frame.Width * 0.3, TextLabel.Frame.Right + 10));

            var width = ImageView.Image == null ? (float)TextLabel.Frame.Left : (float)ImageView.Frame.Left;
            TextField.Frame = new RectangleF(start, ((float)Frame.Height - 30) / 2, (float)Frame.Width - width - start, 30);
            // Centers TextField Content  (iOS6)
            TextField.VerticalAlignment = UIControlContentVerticalAlignment.Center;
        }

        public event EventHandler TextFieldTextChanged;

        bool OnShouldReturn(UITextField view) {
            TextField.ResignFirstResponder();
            return true;
        }

        void TextFieldOnEditingChanged(object sender, EventArgs eventArgs) {
            var handler = TextFieldTextChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
    }
}
