using System;
using Xamarin.Forms.Platform.iOS;
using MyFormsLibrary.CustomRenderers;
using System.ComponentModel;
using UIKit;
using MyFormsLibrary.iOS.CustomRenderers;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(LabelCell), typeof(LabelCellRenderer))]
namespace MyFormsLibrary.iOS.CustomRenderers
{
    public class LabelCellRenderer:CellBaseRenderer
    {
        LabelCell labelCell;
        CommandCellView TableViewCell;

       public override UIKit.UITableViewCell GetCell(Xamarin.Forms.Cell item, UIKit.UITableViewCell reusableCell, UIKit.UITableView tv) {
            base.GetCell(item, reusableCell, tv);

            labelCell = (LabelCell)item;

            TableViewCell = reusableCell as CommandCellView;
            if (TableViewCell == null) {
                TableViewCell = new CommandCellView(item.GetType().FullName,item is CommandCell);
            }
            else {
                //ParentElement.PropertyChanged -= ParentElement_PropertyChanged;
                item.PropertyChanged -= Item_PropertyChanged;
            }

            Cell = TableViewCell;
            TableViewCell.Cell = item;
            item.PropertyChanged += Item_PropertyChanged;
            //ParentElement.PropertyChanged += ParentElement_PropertyChanged;


            WireUpForceUpdateSizeRequested(item, TableViewCell, tv);

            UpdateBackground(TableViewCell, labelCell);
            UpdateBase();

            UpdateValueText();
            //UpdateValueTextFontSize();
            //UpdateValueTextColor();

            return TableViewCell;
        }

        //void ParentElement_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
        //    if (e.PropertyName == TableViewEx.CellValueTextColorProperty.PropertyName) {
        //        UpdateValueTextColor();
        //    }
        //    else if (e.PropertyName == TableViewEx.CellValueTextFontSizeProperty.PropertyName) {
        //        UpdateValueTextFontSize();
        //    }

        //}

        void Item_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == LabelCell.ValueTextProperty.PropertyName) {
                UpdateValueText();
            }
            //else if (e.PropertyName == LabelCell.ValueTextFontSizeProperty.PropertyName) {
            //    UpdateValueTextFontSize();
            //}
            //else if (e.PropertyName == LabelCell.ValueTextColorProperty.PropertyName) {
            //    UpdateValueTextColor();
            //}
        }

        void UpdateValueText() {
            TableViewCell.DetailTextLabel.Text = labelCell.ValueText;
        }
        //void UpdateValueTextFontSize() {
        //    if (labelCell.ValueTextFontSize > 0) {
        //        TableViewCell.DetailTextLabel.Font = TableViewCell.DetailTextLabel.Font.WithSize((nfloat)labelCell.ValueTextFontSize);
        //    }
        //    else {
        //        TableViewCell.DetailTextLabel.Font = TableViewCell.DetailTextLabel.Font.WithSize((nfloat)ParentElement.CellValueTextFontSize);
        //    }
        //    Cell.SetNeedsLayout();

        //}
        //void UpdateValueTextColor() {
        //    if (labelCell.ValueTextColor != Xamarin.Forms.Color.Default) {
        //        TableViewCell.DetailTextLabel.TextColor = labelCell.ValueTextColor.ToUIColor();
        //    }
        //    else if (ParentElement.CellValueTextColor != Xamarin.Forms.Color.Default) {
        //        TableViewCell.DetailTextLabel.TextColor = ParentElement.CellValueTextColor.ToUIColor();
        //    }
        //}

    }

    public class CommandCellView : CellBaseView
    {
        public Action Execute { get; set; }

        public CommandCellView(string cellName,bool useIndicator=false) : base(cellName) {
            if (useIndicator) {
                this.Accessory = UITableViewCellAccessory.DisclosureIndicator;
            }
        }

    }
}
