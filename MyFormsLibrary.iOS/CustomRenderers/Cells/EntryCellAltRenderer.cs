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

            UITapGestureRecognizer tapGesture = null;

            EntryCell = (EntryCellAlt)item;

            TableViewCell = reusableCell as EntryCellAltView;
            if (TableViewCell == null) {
                TableViewCell = new EntryCellAltView(item.GetType().FullName);

            }
            else {
                item.PropertyChanged -= Item_PropertyChanged;
                ParentElement.PropertyChanged -= ParentElement_PropertyChanged;
                TableViewCell.TextFieldTextChanged -= OnTextFieldTextChanged;
                TableViewCell.KeyboardDoneButtonPressed -= OnKeyboardDoneButtonPressed;
                if (tapGesture != null) {
                    TableViewCell.RemoveGestureRecognizer(tapGesture);
                    tapGesture.Dispose();
                }
            }

            Cell = TableViewCell;
            TableViewCell.Cell = item;

            item.PropertyChanged += Item_PropertyChanged;
            ParentElement.PropertyChanged += ParentElement_PropertyChanged;
            TableViewCell.TextFieldTextChanged += OnTextFieldTextChanged;
            TableViewCell.KeyboardDoneButtonPressed += OnKeyboardDoneButtonPressed;

            tapGesture = new UITapGestureRecognizer((obj) => {
                TableViewCell.TextField.BecomeFirstResponder();
            });
            TableViewCell.AddGestureRecognizer(tapGesture);

            WireUpForceUpdateSizeRequested(item, TableViewCell, tv);

            UpdateBackground(TableViewCell, EntryCell);
            UpdateBase();

            UpdateText();
            UpdateTextColor();
            UpdateTextFontSize();
            UpdateKeyboard();
            UpdateErrorMessage();
            UpdatePlaceholder();
           
            return TableViewCell;
        }

        void ParentElement_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == TableViewEx.CellValueTextColorProperty.PropertyName) {
                UpdateTextColor();
            }
            else if (e.PropertyName == TableViewEx.CellValueTextFontSizeProperty.PropertyName) {
                UpdateTextFontSize();
            }

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
            else if (e.PropertyName == EntryCellAlt.ErrorMessageProperty.PropertyName) {
                UpdateErrorMessage();
            }
            else if (e.PropertyName == EntryCellAlt.PlaceholderProperty.PropertyName) {
                UpdatePlaceholder();
            }
        }

        void OnTextFieldTextChanged(object sender, EventArgs e) {
            EntryCell.Text = TableViewCell.TextField.Text;
        }

        void OnKeyboardDoneButtonPressed(object sender, EventArgs e) {
            EntryCell.SendCompleted();
        }

        void UpdateText() {
            if (TableViewCell.TextField.Text != EntryCell.Text) {
                TableViewCell.TextField.Text = EntryCell.Text;
            }
        }
        void UpdateTextColor() {
            if (EntryCell.TextColor != Color.Default) {
                TableViewCell.TextField.TextColor = EntryCell.TextColor.ToUIColor();
            }
            else if (ParentElement.CellValueTextColor != Xamarin.Forms.Color.Default) {
                TableViewCell.TextField.TextColor = ParentElement.CellValueTextColor.ToUIColor();
            }
        }
        void UpdateTextFontSize() {
            if (EntryCell.LabelFontSize > 0) {
                TableViewCell.TextField.Font = TableViewCell.TextField.Font.WithSize((nfloat)EntryCell.TextFontSize);
            }
            else {
                TableViewCell.TextField.Font = TableViewCell.TextField.Font.WithSize((nfloat)ParentElement.CellValueTextFontSize);
            }
            Cell.SetNeedsLayout();

        }
        void UpdateKeyboard() {
            TableViewCell.TextField.ApplyKeyboard(EntryCell.Keyboard);
        }


        void UpdateErrorMessage() {
            TableViewCell.ErrorLabel.Text = EntryCell.ErrorMessage;
            TableViewCell.ErrorLabel.Hidden = string.IsNullOrEmpty(EntryCell.ErrorMessage);
        }

        void UpdatePlaceholder() {
            TableViewCell.TextField.Placeholder = EntryCell.Placeholder;
        }
    }

    public class EntryCellAltView : CellTableViewCell
    {
        public UITextField TextField { get; }

        public UILabel ErrorLabel { get; private set; }
        private NSLayoutConstraint[] constraint;

        public EntryCellAltView(string cellName) : base(UITableViewCellStyle.Value1,cellName) {
            TextField = new UITextField(new RectangleF(0, 0, 100, 30)) { BorderStyle = UITextBorderStyle.None };
            TextField.TextAlignment = UITextAlignment.Right;

            TextField.EditingChanged += TextFieldOnEditingChanged;
            TextField.ShouldReturn = OnShouldReturn;

            ContentView.AddSubview(TextField);

            SetErrorMessageLabel();
        }

        private void SetErrorMessageLabel() {
            ErrorLabel = new UILabel();
            ErrorLabel.LineBreakMode = UILineBreakMode.Clip;
            ErrorLabel.Lines = 1;
            ErrorLabel.BackgroundColor = UIColor.FromWhiteAlpha(1, .5f);
            ErrorLabel.TextColor = UIColor.Red.ColorWithAlpha(0.8f);
            ErrorLabel.TintAdjustmentMode = UIViewTintAdjustmentMode.Automatic;
            ErrorLabel.AdjustsFontSizeToFitWidth = true;
            ErrorLabel.BaselineAdjustment = UIBaselineAdjustment.AlignCenters;
            ErrorLabel.TextAlignment = UITextAlignment.Right;
            ErrorLabel.AdjustsLetterSpacingToFitWidth = true;
            ErrorLabel.Font = ErrorLabel.Font.WithSize(10);

            ContentView.AddSubview(ErrorLabel);

            ErrorLabel.TranslatesAutoresizingMaskIntoConstraints = false;
            constraint = new NSLayoutConstraint[]{
                NSLayoutConstraint.Create(
                    ErrorLabel,
                    NSLayoutAttribute.Top,
                    NSLayoutRelation.Equal,
                    ContentView,
                    NSLayoutAttribute.Top,
                    1,
                    2
                ),
                NSLayoutConstraint.Create(
                    ErrorLabel,
                    NSLayoutAttribute.Right,
                    NSLayoutRelation.Equal,
                    ContentView,
                    NSLayoutAttribute.Right,
                    1,
                    -10
                ),
                NSLayoutConstraint.Create(
                    ErrorLabel,
                    NSLayoutAttribute.Height,
                    NSLayoutRelation.Equal,
                    null,
                    NSLayoutAttribute.Height,
                    1,
                    14
                ),
                NSLayoutConstraint.Create(
                    ErrorLabel,
                    NSLayoutAttribute.Width,
                    NSLayoutRelation.Equal,
                    TextField,
                    NSLayoutAttribute.Width,
                    1,
                    0
                )
            };

            ContentView.AddConstraints(constraint);
            ErrorLabel.SizeToFit();
        }

        protected override void Dispose(bool disposing) {
            TextField.EditingChanged -= TextFieldOnEditingChanged;
            TextField.ShouldReturn = null;
            TextField.Dispose();

            ContentView.RemoveConstraints(constraint);
            ErrorLabel.RemoveFromSuperview();
            ErrorLabel.Dispose();

            base.Dispose(disposing);
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

        public event EventHandler KeyboardDoneButtonPressed;

        bool OnShouldReturn(UITextField view) {
            var handler = KeyboardDoneButtonPressed;
            if (handler != null)
                handler(this, EventArgs.Empty);

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
