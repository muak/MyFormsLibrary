using Android.Content;
using Android.Graphics.Drawables;
using Android.Widget;
using MyFormsLibrary.CustomRenderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using AListView = Android.Widget.ListView;
using Android.Views;
using Android.Support.V4.View;
using System;
using System.Runtime.InteropServices;
using Android.Text;


namespace MyFormsLibrary.Droid.CustomRenderers
{
    public class TableViewModelExRenderer:TableViewModelRenderer
    {
        readonly TableViewEx _view;
        ITableViewController Controller => _view;

        public TableViewModelExRenderer(Context context, AListView listview, TableView view)
            : base(context, listview, view) {
            _view = view as TableViewEx;
        }

        public override Android.Views.View GetView(int position, Android.Views.View convertView, Android.Views.ViewGroup parent) {
            var layout = base.GetView(position, convertView, parent);

            var linearLayout = layout as LinearLayout;
            var textView = linearLayout.GetChildAt(0) as BaseCellView;
            var border = linearLayout.GetChildAt(1);

            bool isHeader, isNextHeader;

            JudgeSpecialPosition(position,out isHeader,out isNextHeader);

            if (isHeader) {

                if (textView != null) {
                    HeaderConfiguration(textView);
                }

                if (border != null) {
                    if (_view.ShowSectionTopBottomBorder) {
                        border.SetBackgroundColor(_view.SeparatorColor.ToAndroid());
                    }
                    else {
                        border.SetBackgroundColor(Android.Graphics.Color.Transparent);
                    }
                }
            }
            else {
                if (isNextHeader) {
                    if (!_view.ShowSectionTopBottomBorder) {
                        border.SetBackgroundColor(Android.Graphics.Color.Transparent);
                    }
                }
                else {
                    border.SetBackgroundColor(_view.SeparatorColor.ToAndroid());
                }

                layout.SetBackgroundColor(_view.CellBackgroundColor.ToAndroid());
            }

           
            return layout;
        }

        void HeaderConfiguration(BaseCellView textView) {
            
            var color = _view.HeaderTextColor;

            if (color != Color.Default) {
                textView.SetMainTextColor(color);
            }

            var fontsize = _view.HeaderFontSize;
            var textContainer = textView.GetChildAt(1) as LinearLayout;

            if (textContainer != null) {

                var tv = textContainer.GetChildAt(0) as TextView;
                tv?.SetTextSize(Android.Util.ComplexUnitType.Sp, (float)fontsize);

                var vAlign = _view.HeaderTextVerticalAlign;
                if (vAlign != LayoutAlignment.Center) {
                    textContainer.LayoutParameters.Height = ViewGroup.LayoutParams.MatchParent;
                    textContainer.SetGravity(
                        vAlign == LayoutAlignment.Start ? GravityFlags.Top : GravityFlags.Bottom |
                        GravityFlags.ClipVertical
                    );
                }
            }
            var backcolor = _view.HeaderBackgroundColor == Color.Default ? 
                                 _view.BackgroundColor : _view.HeaderBackgroundColor;


            textView.SetBackgroundColor(backcolor.ToAndroid());
            textView.SetRenderHeight(_view.HeaderHeight);
        }

        protected override void HandleItemClick(AdapterView parent, Android.Views.View nview, int position, long id) {
            base.HandleItemClick(parent, nview, position, id);

            var view = (nview as LinearLayout)?.GetChildAt(0);

            if (view is CommandCellView) {
                (view as CommandCellView)?.Execute?.Invoke();
            }
            //else if(view is DatePickerCellView) {
            //    (view as DatePickerCellView)?.OpenDialog?.Invoke();
            //}
        }


        void JudgeSpecialPosition(int position,out bool isHeader,out bool isNextHeader) {
            isHeader = false;
            isNextHeader = false;

            ITableModel model = Controller.Model;
            int sectionCount = model.GetSectionCount();

            for (var sectionIndex = 0; sectionIndex < sectionCount; sectionIndex++) {
                int size = model.GetRowCount(sectionIndex) + 1;

                if (position == 0) {
                    isHeader = true;
                    isNextHeader = size == 0 && sectionIndex < sectionCount - 1;
                    return;
                }
                if (position < size) {
                   isNextHeader = position == size - 1;
                   return;
                }

                position -= size;
            }

        }
    }
}
