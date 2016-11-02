using System;
using MyFormsLibrary.CustomRenderers;
using MyFormsLibrary.Droid.CustomRenderers;
using Xamarin.Forms;
using Android.Content;
using Android.Widget;
using Android.Text;
using Android.Util;
using Android.Views;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CommandCell), typeof(CommandCellRenderer))]
namespace MyFormsLibrary.Droid.CustomRenderers
{
    public class CommandCellRenderer:LabelCellRenderer
    {
        CommandCell CommandCell;
        CommandCellView CellView;

        protected override Android.Views.View GetCellCore(Cell item, Android.Views.View convertView, Android.Views.ViewGroup parent, Android.Content.Context context) {
            CellView =  base.GetCellCore(item, convertView, parent, context) as CommandCellView;

            CommandCell = item as CommandCell;

            if (convertView == null) {
                
            }
            else {
                CellView.Execute = null;
            }

            CellView.Execute = () => CommandCell.Command?.Execute(CommandCell.CommandParameter);

            return CellView;
        }


    }

}
