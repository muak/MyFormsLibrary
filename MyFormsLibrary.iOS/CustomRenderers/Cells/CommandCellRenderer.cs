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
    public class CommandCellRenderer:LabelCellRenderer
    {
        CommandCell CommandCell;
        CommandCellView TableViewCell;

        public override UIKit.UITableViewCell GetCell(Xamarin.Forms.Cell item, UIKit.UITableViewCell reusableCell, UIKit.UITableView tv) {
            TableViewCell = base.GetCell(item,reusableCell,tv) as CommandCellView;

            CommandCell = (CommandCell)item;

            if (reusableCell == null) {
                
            }
            else {
                TableViewCell.Execute = null;
            }
                      
            TableViewCell.Execute = () => CommandCell.Command?.Execute(CommandCell.CommandParameter);

            return TableViewCell;
        }

    }

}
