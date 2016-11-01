using System;
using System.ComponentModel;
using MyFormsLibrary.CustomRenderers;
using MyFormsLibrary.iOS.CustomRenderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CommandCell), typeof(CommandCellRenderer))]
namespace MyFormsLibrary.iOS.CustomRenderers
{
    public class CommandCellRenderer:CellBaseRenderer
    {
        CommandCell CommandCell;
        CommandCellView TableViewCell;

        public override UIKit.UITableViewCell GetCell(Xamarin.Forms.Cell item, UIKit.UITableViewCell reusableCell, UIKit.UITableView tv) {
            base.GetCell(item,reusableCell,tv);

            CommandCell = (CommandCell)item;

            TableViewCell = reusableCell as CommandCellView;
            if (TableViewCell == null) {
                TableViewCell = new CommandCellView(item.GetType().FullName);
            }
            else {
                TableViewCell.Execute = null;
                ParentElement.PropertyChanged -= ParentElement_PropertyChanged;
                item.PropertyChanged -= Item_PropertyChanged;          
            }

            Cell = TableViewCell;
            TableViewCell.Cell = item;
            item.PropertyChanged += Item_PropertyChanged;
            ParentElement.PropertyChanged += ParentElement_PropertyChanged;
            TableViewCell.Execute = () => CommandCell.Command?.Execute(CommandCell.CommandParameter);

            WireUpForceUpdateSizeRequested(item, TableViewCell, tv);

            UpdateBackground(TableViewCell, CommandCell);
            UpdateBase();

            UpdateValueText();
            UpdateValueTextFontSize();
            UpdateValueTextColor();

            return TableViewCell;
        }

        void ParentElement_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == TableViewEx.CellValueTextColorProperty.PropertyName) {
                UpdateValueTextColor();
            }
            else if (e.PropertyName == TableViewEx.CellValueTextFontSizeProperty.PropertyName) {
                UpdateValueTextFontSize();
            }

        }

        void Item_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == CommandCell.ValueTextProperty.PropertyName) {
                UpdateValueText();
            }
            else if (e.PropertyName == CommandCell.ValueTextFontSizeProperty.PropertyName) {
                UpdateValueTextFontSize();
            }
            else if (e.PropertyName == CommandCell.ValueTextColorProperty.PropertyName) {
                UpdateValueTextColor();
            }
        }

        void UpdateValueText() {
            TableViewCell.DetailTextLabel.Text = CommandCell.ValueText;
        }
        void UpdateValueTextFontSize() {
            if (CommandCell.ValueTextFontSize > 0) {
                TableViewCell.DetailTextLabel.Font = TableViewCell.DetailTextLabel.Font.WithSize((nfloat)CommandCell.ValueTextFontSize);
            }
            else {
                TableViewCell.DetailTextLabel.Font = TableViewCell.DetailTextLabel.Font.WithSize((nfloat)ParentElement.CellValueTextFontSize);
            }
            Cell.SetNeedsLayout();

        }
        void UpdateValueTextColor() {
            if (CommandCell.ValueTextColor != Xamarin.Forms.Color.Default) {
                TableViewCell.DetailTextLabel.TextColor = CommandCell.ValueTextColor.ToUIColor();
            }
            else if (ParentElement.CellValueTextColor != Xamarin.Forms.Color.Default) {
                TableViewCell.DetailTextLabel.TextColor = ParentElement.CellValueTextColor.ToUIColor();
            }
        }




    }

    public class CommandCellView : CellTableViewCell
    {
        public Action Execute { get; set; }

        public CommandCellView(string cellName) : base(UITableViewCellStyle.Value1,cellName) {
            
            this.Accessory = UITableViewCellAccessory.DisclosureIndicator;
        }

    }
}
