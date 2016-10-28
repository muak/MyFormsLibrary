using System;
using System.Reflection;
using Xamarin.Forms.Platform.iOS;
using ObjCRuntime;
using UIKit;
using Xamarin.Forms;
using MyFormsLibrary.iOS.CustomRenderers;
using MyFormsLibrary.CustomRenderers;

[assembly: ExportRenderer(typeof(EntryCellEx), typeof(EntryCellExRenderer))]
namespace MyFormsLibrary.iOS.CustomRenderers
{
    public class EntryCellExRenderer : EntryCellRenderer
    {
        static PropertyInfo TextFieldPropInfo;
        static EntryCellExRenderer() {
            var asm = typeof(EntryCellRenderer).Assembly;
            var childtype = asm.GetType("Xamarin.Forms.Platform.iOS.EntryCellRenderer+EntryCellTableViewCell");

            TextFieldPropInfo = childtype.GetProperty("TextField", BindingFlags.Instance | BindingFlags.Public);
        }

        UITextField TextField;
        Cell Item;

        public override UIKit.UITableViewCell GetCell(Xamarin.Forms.Cell item, UIKit.UITableViewCell reusableCell, UIKit.UITableView tv) {
            var cell = base.GetCell(item, reusableCell, tv);


            TextField = (UITextField)TextFieldPropInfo.GetValue(cell);
            if (reusableCell != null) {
                item.PropertyChanged -= Item_PropertyChanged;
            }
            item.PropertyChanged += Item_PropertyChanged;

            Item = item;

            UpdateTextColor();

            return cell;
        }


        void Item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == EntryCellEx.TextColorProperty.PropertyName) {
                UpdateTextColor();
            }
        }

        void UpdateTextColor() {
            var color = (Item as EntryCellEx).TextColor;
            if (color != Color.Default) {
                TextField.TextColor = color.ToUIColor();
            }
        }
    }
}
