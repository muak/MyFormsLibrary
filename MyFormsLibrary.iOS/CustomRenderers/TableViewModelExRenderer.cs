using System;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;
using UIKit;
using CoreGraphics;
using MyFormsLibrary.CustomRenderers;

namespace MyFormsLibrary.iOS.CustomRenderers
{
    public class TableViewModelExRenderer : UnEvenTableViewModelRenderer
    {
        TableViewEx Model;

        public TableViewModelExRenderer(TableView model) : base(model) {
            Model = model as TableViewEx;
        }

        public override UITableViewCell GetCell(UITableView tableView, Foundation.NSIndexPath indexPath) {
            var cell =  base.GetCell(tableView, indexPath);

            if (!(Model.BackgroundColor == Color.Default && Model.CellBackgroundColor == Color.Default)) {
                cell.BackgroundColor = Model.CellBackgroundColor.ToUIColor();
            }
            return cell;
        }


        public override nfloat GetHeightForRow(UITableView tableView, Foundation.NSIndexPath indexPath) {
            if (this is UnEvenTableViewModelExRenderer) {
                return base.GetHeightForRow(tableView, indexPath);
            }
            return UITableView.AutomaticDimension;
        }

        public override UIKit.UIView GetViewForHeader(UIKit.UITableView tableView, nint section) {
            
            var title = TitleForHeader(tableView, section);
            var label = new IndentLabel(Model.HeaderHeight, (float)Model.HeaderFontSize, Model.HeaderTextVerticalAlign);

            label.Text = title;
            label.TextColor = Model.HeaderTextColor == Color.Default ?
                             UIColor.Gray : Model.HeaderTextColor.ToUIColor();
            label.Font = UIFont.SystemFontOfSize((nfloat)Model.HeaderFontSize);
            label.BackgroundColor = Model.HeaderBackgroundColor.ToUIColor();

            label.Lines = 0;

            return label;
        }

        public override nfloat GetHeightForHeader(UITableView tableView, nint section) {
            return Model.HeaderHeight;
        }

        public override void RowSelected(UITableView tableView, Foundation.NSIndexPath indexPath) {
            base.RowSelected(tableView, indexPath);
                       
            var cell = tableView.CellAt(indexPath);
            if (cell is CommandCellView) {
                (cell as CommandCellView)?.Execute?.Invoke();
            }
        }


        class IndentLabel : UILabel
        {
            VerticalAlign vAlign;

            float Height;
            float FontHeight;
            float Padding = 8f;
            float PaddingLeft = 14f;

            public IndentLabel(float header, float text, LayoutAlignment formsAlign) : base() {
                Height = header;
                FontHeight = text;
                if (formsAlign == LayoutAlignment.Fill) {
                    vAlign = VerticalAlign.End;
                }
                else {
                    vAlign = (VerticalAlign)formsAlign;
                }
            }

            public override void DrawText(CGRect rect) {
                
                rect.X = PaddingLeft;
                switch (vAlign) {
                    case VerticalAlign.Start :
                        rect.Y = Padding;
                        break;
                    case VerticalAlign.Center:
                        rect.Y = (Height / 2f) - (FontHeight / 2f);
                        break;
                    case VerticalAlign.End:                 
                        rect.Y = rect.Height - FontHeight - Padding;
                        break;
                }
                rect.Height = FontHeight;
                base.DrawText(rect);

            }

            enum VerticalAlign
            {
                Start=0,
                Center=1,
                End=2
            }
        }
    }
    public class UnEvenTableViewModelExRenderer : TableViewModelExRenderer
    {
        public UnEvenTableViewModelExRenderer(TableView model) : base(model) {
        }

       
    }
}
