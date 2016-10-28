using Android.Content;
using Android.Widget;
using MyFormsLibrary.CustomRenderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using AListView = Android.Widget.ListView;

namespace MyFormsLibrary.Droid.CustomRenderers
{
    public class TableViewModelExRenderer:TableViewModelRenderer
    {
        readonly TableView _view;
        ITableViewController Controller => _view;

        public TableViewModelExRenderer(Context context, AListView listview, TableView view)
            : base(context, listview, view) {
            _view = view;
        }

        public override Android.Views.View GetView(int position, Android.Views.View convertView, Android.Views.ViewGroup parent) {
           
            var layout = base.GetView(position, convertView, parent);

            if (IsHeader(position)) {
               
                var tmp = layout as LinearLayout;
                var textView = tmp.GetChildAt(0) as BaseCellView;
                if (textView != null) {
                    var color = (_view as TableViewEx).SectionTitleColor;
                    if (color != Color.Default) {
                        textView.SetMainTextColor(color);
                    }
                }
                var border = tmp.GetChildAt(1);
                if (border != null) {
                    var color = (_view as TableViewEx).SeparatorColor;
                    if (color != Color.Default) {
                        border.SetBackgroundColor(color.ToAndroid());
                    }
                }

            }
           
            return layout;
        }

        bool IsHeader(int position) {
          
            ITableModel model = Controller.Model;
            int sectionCount = model.GetSectionCount();

            for (var sectionIndex = 0; sectionIndex < sectionCount; sectionIndex++) {
                int size = model.GetRowCount(sectionIndex) + 1;

                if (position == 0) {
                    return true;
                }
                 position -= size;
            }

            return false;
        }
    }
}
