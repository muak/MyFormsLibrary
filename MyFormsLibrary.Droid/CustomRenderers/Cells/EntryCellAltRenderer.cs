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
using Android.Graphics.Drawables;
using AGraphics = Android.Graphics;
using ARelativeLayout = Android.Widget.RelativeLayout;

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
                ParentElement.PropertyChanged -= ParentElement_PropertyChanged;
            }

            BaseView = CellView;

            ParentElement.PropertyChanged += ParentElement_PropertyChanged;

            UpdateBase();
            UpdateText();
            UpdateTextColor();
            UpdateTextFontSize();
            UpdateKeyboard();
            UpdateErrorMessage();
            UpdatePlaceholder();

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
            else if (e.PropertyName == EntryCellAlt.ErrorMessageProperty.PropertyName) {
                UpdateErrorMessage();
            }
            else if (e.PropertyName == EntryCellAlt.PlaceholderProperty.PropertyName) {
                UpdatePlaceholder();
            }
        }

        void ParentElement_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == TableViewEx.CellValueTextColorProperty.PropertyName) {
                UpdateTextColor();
            }
            else if (e.PropertyName == TableViewEx.CellValueTextFontSizeProperty.PropertyName) {
                UpdateTextFontSize();
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
            if (CellView.EditText.Text != EntryCell.Text) {
                CellView.EditText.Text = EntryCell.Text;
            }
        }

        void UpdateTextColor() {
            if (EntryCell.TextColor != Color.Default) {
                CellView.EditText.SetTextColor(EntryCell.TextColor.ToAndroid());
            }
            else if (ParentElement.CellValueTextColor != Xamarin.Forms.Color.Default) {
                CellView.EditText.SetTextColor(ParentElement.CellValueTextColor.ToAndroid());
            }
            CellView.Invalidate();
        }

        void UpdateTextFontSize() {

            if (EntryCell.TextFontSize > 0) {
                CellView.EditText.SetTextSize(Android.Util.ComplexUnitType.Sp, (float)EntryCell.TextFontSize);
            }
            else {
                CellView.EditText.SetTextSize(Android.Util.ComplexUnitType.Sp, (float)ParentElement.CellValueTextFontSize);

            }
            CellView.Invalidate();

        }

        void UpdateKeyboard() {
            CellView.EditText.InputType = EntryCell.Keyboard.ToInputType();
        }

        void UpdateErrorMessage() {
            
            var msg = EntryCell.ErrorMessage;
            if (string.IsNullOrEmpty(msg)) {
                CellView.ErrorLabel.Visibility = ViewStates.Invisible;
                return;
            }

            CellView.ErrorLabel.Text = msg;
            CellView.ErrorLabel.Visibility = ViewStates.Visible;
        }

        void UpdatePlaceholder() {
            CellView.EditText.Hint = EntryCell.Placeholder;
            CellView.EditText.SetHintTextColor(Android.Graphics.Color.Rgb(210,210,210));
        }

    }

    public class EntryCellAltView:CellBaseView,ITextWatcher,global::Android.Views.View.IOnTouchListener,Android.Views.View.IOnClickListener
            ,global::Android.Views.View.IOnFocusChangeListener,TextView.IOnEditorActionListener
    {
        public EntryCellEditText EditText { get; set; }
        public TextView ErrorLabel { get; private set;}

        public EntryCellAltView(Context context, Cell cell) : base(context,cell) {
            var relative = new ARelativeLayout(context);

            using (var lparam = new ARelativeLayout.LayoutParams(-1, -1)){
                lparam.AddRule(LayoutRules.AlignRight);

                EditText = new EntryCellEditText(context);
                EditText.Focusable = true;
                EditText.ImeOptions = ImeAction.Done;
                EditText.SetOnEditorActionListener(this);
                EditText.AddTextChangedListener(this);
                EditText.OnFocusChangeListener = this;
                EditText.SetOnTouchListener(this);
                EditText.SetSingleLine(true);
                EditText.Gravity = GravityFlags.Right;
                SetOnClickListener(this);

                EditText.Background.Alpha = 0;  //下線は非表示
                relative.AddView(EditText, lparam);
            }


            using (var lparam = new ARelativeLayout.LayoutParams(-1, -1)) {
                lparam.AddRule(LayoutRules.AlignRight);

                ErrorLabel = new TextView(Context);
                ErrorLabel.SetTextColor(new AGraphics.Color(255, 0, 0, 200));
                ErrorLabel.SetBackgroundColor(new AGraphics.Color(255, 255, 255, 128));
                ErrorLabel.TextSize = 10f;
                ErrorLabel.Gravity = GravityFlags.Right | GravityFlags.Top;

                ErrorLabel.Visibility = ViewStates.Invisible;
                ErrorLabel.SetPadding(8, 0, 8, 0);

                relative.AddView(ErrorLabel, lparam);
            }


            var textParams = new LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent) {
                Width = 0,
                Weight = 1,
                Gravity = GravityFlags.FillHorizontal | GravityFlags.CenterVertical
            };
            using (textParams) {
                AddView(relative, textParams);
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
        void ShowKeyboard(Android.Views.View inputView) {
            using (var inputMethodManager = (InputMethodManager)Forms.Context.GetSystemService(Context.InputMethodService)) {
                
                inputMethodManager.ShowSoftInput(inputView, ShowFlags.Forced);
                inputMethodManager.ToggleSoftInput(ShowFlags.Forced, HideSoftInputFlags.ImplicitOnly);
               
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
            if (focusChanged != null) {
                focusChanged(hasFocus);
            }
            if (hasFocus) {
                //フォーカス時のみ下線表示
                EditText.Background.Alpha = 100;
            }
            else {
                //非フォーカス時は非表示
                EditText.Background.Alpha = 0;
            }
        }

        void OnKeyboardDoneButtonPressed(object sender, EventArgs e) {
            // TODO Clear focus
            Action editingCompleted = EditingCompleted;
            if (editingCompleted != null)
                editingCompleted();
        }

        public void OnClick(Android.Views.View v) {
            //EditTextのタッチイベントを生成
            OnTouch(EditText,
                        MotionEvent.Obtain(DateTime.Now.Millisecond,
                                           DateTime.Now.Millisecond + 100,
                                           MotionEventActions.Down, 0, 0, 0));
            //この順番じゃないとフォーカスが当たらない
            EditText.RequestFocus();
            //自動で出ないので手動でキーボードを出す
            ShowKeyboard(EditText);
        }

        public bool OnTouch(Android.Views.View v, MotionEvent e) {
            //EditTextがフォーカスを受けられるようにする
            var p = Parent as LinearLayout;
            p.DescendantFocusability = DescendantFocusability.AfterDescendants;

            return false;
        }


    }
}
