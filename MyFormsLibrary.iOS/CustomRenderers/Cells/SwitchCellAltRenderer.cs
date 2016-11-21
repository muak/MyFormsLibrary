using System;
using MyFormsLibrary.CustomRenderers;
using Xamarin.Forms.Platform.iOS;
using UIKit;
using Xamarin.Forms;
using MyFormsLibrary.iOS.CustomRenderers;

[assembly: ExportRenderer(typeof(SwitchCellAlt), typeof(SwitchCellAltRenderer))]
namespace MyFormsLibrary.iOS.CustomRenderers
{
    public class SwitchCellAltRenderer:CellBaseRenderer
    {
        SwitchCellAlt SwitchCell;
        SwitchCellAltView TableViewCell;


        public override UIKit.UITableViewCell GetCell(Xamarin.Forms.Cell item, UIKit.UITableViewCell reusableCell, UIKit.UITableView tv) {
            base.GetCell(item, reusableCell, tv);

            SwitchCell = (SwitchCellAlt)item;

            TableViewCell = reusableCell as SwitchCellAltView;
            if (TableViewCell == null) {
                TableViewCell = new SwitchCellAltView(SwitchCell, item.GetType().FullName);

            }
            else {
                item.PropertyChanged -= Item_PropertyChanged;
                TableViewCell.Switch.ValueChanged -= Switch_ValueChanged;
            }

            Cell = TableViewCell;
            TableViewCell.Cell = item;

            item.PropertyChanged += Item_PropertyChanged;;
            TableViewCell.Switch.ValueChanged += Switch_ValueChanged;

            WireUpForceUpdateSizeRequested(item, TableViewCell, tv);

            UpdateBackground(TableViewCell, SwitchCell);
            UpdateBase();
            UpdateOn();
          
            return TableViewCell;
        }

        void Item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == SwitchCellAlt.OnProperty.PropertyName) {
                UpdateOn();
            }
        }

        void UpdateOn() {
            TableViewCell.Switch.On = SwitchCell.On;
        }

        void Switch_ValueChanged(object sender, EventArgs e) {
            SwitchCell.On = TableViewCell.Switch.On;
        }
    }

    public class SwitchCellAltView : CellTableViewCell
    {
        public UISwitch Switch { get; set; }
       
        public SwitchCellAltView(SwitchCellAlt cell,string cellName) : base(UIKit.UITableViewCellStyle.Value1, cellName) {
            var switchCell = cell;

            Switch = new UISwitch();

            if (switchCell.AccentColor != Xamarin.Forms.Color.Default) {
                Switch.OnTintColor = switchCell.AccentColor.ToUIColor();
            }
           
            this.AccessoryView = Switch;
        }

    }
}
