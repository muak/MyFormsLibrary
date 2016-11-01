using System;
using Android.Content;
using Android.OS;
using Android.Text;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Java.Lang;
using MyFormsLibrary.CustomRenderers;
using MyFormsLibrary.Droid.CustomRenderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;


[assembly: ExportRenderer(typeof(EntryCellAlt), typeof(EntryCellAltRenderer))]
namespace MyFormsLibrary.Droid.CustomRenderers
{
    public class EntryCellAltRenderer:CellBaseRenderer
    {
        EntryCellAlt EntryCell;
        EntryCellAltView CellView;

        protected override Android.Views.View GetCellCore(Cell item, Android.Views.View convertView, Android.Views.ViewGroup parent, Android.Content.Context context) {
            base.GetCellCore(item, convertView, parent, context);

            EntryCell = item as EntryCellAlt;

            CellView = convertView as EntryCellAltView;
            if (CellView == null) {
                CellView = new EntryCellAltView(context, item);

            }
            else {
                CellView.TextChanged = null;
                CellView.EditingCompleted = null;

            }

            BaseView = CellView;

            UpdateBase();
            UpdateText();
            UpdateTextColor();
            UpdateTextFontSize();
            UpdateKeyboard();

            CellView.TextChanged = OnTextChanged;
            CellView.EditingCompleted = OnEditingCompleted;


            return CellView;
        }

        protected override void OnCellPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            base.OnCellPropertyChanged(sender, e);

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

        void OnTextChanged(string text) {
            EntryCell.Text = text;
        }

        void OnEditingCompleted() {
            var entryCell = (IEntryCellController)Cell;
            entryCell.SendCompleted();
        }

        void UpdateText() {
            CellView.EditText.Text = EntryCell.Text;
        }
        void UpdateTextColor() {
            if (EntryCell.TextColor != Color.Default) {
                CellView.EditText.SetTextColor(EntryCell.TextColor.ToAndroid());
            }
        }
        void UpdateTextFontSize() {
            if (EntryCell.TextFontSize < 0) {
                CellView.EditText.SetTextSize(Android.Util.ComplexUnitType.Sp,(float)EntryCell.LabelFontSize);
            }
            else {
                CellView.EditText.SetTextSize(Android.Util.ComplexUnitType.Sp,(float)EntryCell.TextFontSize);
            }
           
        }
        void UpdateKeyboard() {
            CellView.EditText.InputType = EntryCell.Keyboard.ToInputType();
        }

    }

    public class EntryCellAltView:CellBaseView,ITextWatcher,global::Android.Views.View.IOnTouchListener
            ,global::Android.Views.View.IOnFocusChangeListener,TextView.IOnEditorActionListener
    {
        public EntryCellEditText EditText { get; set; }



        public EntryCellAltView(Context context, Cell cell) : base(context,cell) {

            EditText = new EntryCellEditText(context);
            EditText.Focusable = true;
            EditText.ImeOptions = ImeAction.Done;
            EditText.SetOnEditorActionListener(this);
            EditText.AddTextChangedListener(this);
            EditText.OnFocusChangeListener = this;
            EditText.SetOnTouchListener(this);
            EditText.SetSingleLine(true);
            EditText.Gravity = GravityFlags.Right;
           
            var textParams = new LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent) {
                Width = 0,
                Weight = 1,
                Gravity = GravityFlags.FillHorizontal | GravityFlags.CenterVertical
            };
            using (textParams) {
                AddView(EditText, textParams);
            }

        }

        public Action EditingCompleted { get; set; }
        public Action<bool> FocusChanged { get; set; }
        public Action<string> TextChanged { get; set; }

     
        bool TextView.IOnEditorActionListener.OnEditorAction(TextView v, ImeAction actionId, KeyEvent e) {
            if (actionId == ImeAction.Done) {
                OnKeyboardDoneButtonPressed(EditText, EventArgs.Empty);
                EditText.ClearFocus();
                HideKeyboard(v);
            }

            // Fire Completed and dismiss keyboard for hardware / physical keyboards
            if (actionId == ImeAction.ImeNull && e.KeyCode == Keycode.Enter) {
                OnKeyboardDoneButtonPressed(EditText, EventArgs.Empty);
                EditText.ClearFocus();
                HideKeyboard(v);
            }

            return true;
        }

        void HideKeyboard(Android.Views.View inputView) {
            using (var inputMethodManager = (InputMethodManager)Forms.Context.GetSystemService(Context.InputMethodService)) {
               
                IBinder windowToken = inputView.WindowToken;
                if (windowToken != null)
                    inputMethodManager.HideSoftInputFromWindow(windowToken, HideSoftInputFlags.None);
            }
        }

        void ITextWatcher.AfterTextChanged(IEditable s) {
        }

        void ITextWatcher.BeforeTextChanged(ICharSequence s, int start, int count, int after) {
        }

        void ITextWatcher.OnTextChanged(ICharSequence s, int start, int before, int count) {
            Action<string> changed = TextChanged;
            if (changed != null)
                changed(s != null ? s.ToString() : null);
        }

        void IOnFocusChangeListener.OnFocusChange(Android.Views.View v, bool hasFocus) {
            Action<bool> focusChanged = FocusChanged;
            if (focusChanged != null)
                focusChanged(hasFocus);
        }

        void OnKeyboardDoneButtonPressed(object sender, EventArgs e) {
            // TODO Clear focus
            Action editingCompleted = EditingCompleted;
            if (editingCompleted != null)
                editingCompleted();
        }

        public bool OnTouch(Android.Views.View v, MotionEvent e) {
            //EditTextがフォーカスを受けられるようにする
            var p = Parent as LinearLayout;
            p.DescendantFocusability = DescendantFocusability.AfterDescendants;
            return false;
        }
    }
}
