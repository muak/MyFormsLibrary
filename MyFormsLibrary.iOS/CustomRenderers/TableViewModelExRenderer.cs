using System;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;
using UIKit;
using CoreGraphics;
using MyFormsLibrary.CustomRenderers;

namespace MyFormsLibrary.iOS.CustomRenderers
{
    public class TableViewModelExRenderer:TableViewModelRenderer
    {
        TableViewEx Model;

        public TableViewModelExRenderer(TableView model):base(model) {
            Model = model as TableViewEx;        
        }

        public override UITableViewCell GetCell(UITableView tableView, Foundation.NSIndexPath indexPath) {
            return base.GetCell(tableView, indexPath);
        }

        public override UIKit.UIView GetViewForHeader(UIKit.UITableView tableView, nint section) {
            
            var title = TitleForHeader(tableView, section);
            var label = new IndentLabel(section == 0);
            label.Text = title;
            label.TextColor = Model.SectionTitleColor == Color.Default ? 
                             UIColor.Gray : Model.SectionTitleColor.ToUIColor();
            label.Font = UIFont.SystemFontOfSize(14f);

            return label;
        }

        public override nfloat GetHeightForHeader(UITableView tableView, nint section) {
            if (section == 0) {
                return 50.0f;
            }
            return 30.0f;
        }

        public override void RowSelected(UITableView tableView, Foundation.NSIndexPath indexPath) {
            base.RowSelected(tableView, indexPath);

            var cell = GetCell(tableView,indexPath);
            if (cell is CommandCellView) {
                (cell as CommandCellView)?.Execute();
            }
        }

       
        class IndentLabel : UILabel{
            bool _isFirst;

            public IndentLabel(bool isFirst = false) :base() {
                _isFirst = isFirst;
            }

            public override void DrawText(CGRect rect) {
                var top = _isFirst ? 28 : 8;
                var insets = new UIEdgeInsets(top, 14, 8, 6);
                base.DrawText(insets.InsetRect(rect));
            }
        }
    }
}
