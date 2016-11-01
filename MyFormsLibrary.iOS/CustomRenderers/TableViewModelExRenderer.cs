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
            var label = new IndentLabel();
            label.Text = title;
            label.TextColor = Model.SectionTitleColor == Color.Default ? 
                             UIColor.Gray : Model.SectionTitleColor.ToUIColor();
            label.Font = UIFont.SystemFontOfSize(14f);

            return label;
        }

        public override nfloat GetHeightForHeader(UITableView tableView, nint section) {
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

            public IndentLabel() :base() {
               
            }

            public override void DrawText(CGRect rect) {
                var insets = new UIEdgeInsets(8, 14, 8, 6);
                base.DrawText(insets.InsetRect(rect));
            }
        }
    }
}
