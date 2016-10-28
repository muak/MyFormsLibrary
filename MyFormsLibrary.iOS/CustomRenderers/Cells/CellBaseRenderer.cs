using System;
using Xamarin.Forms.Platform.iOS;
using MyFormsLibrary.CustomRenderers;
using Xamarin.Forms;
using UIKit;
using NGraphics;
namespace MyFormsLibrary.iOS.CustomRenderers
{
    public class CellBaseRenderer:CellRenderer
    {
        protected CellTableViewCell Cell { get; set; }
        CellBase Item;

        public override UIKit.UITableViewCell GetCell(Xamarin.Forms.Cell item, UIKit.UITableViewCell reusableCell, UIKit.UITableView tv) {
            Item = item as CellBase;

            var tvc = reusableCell;
           
            if (tvc == null)
                item.PropertyChanged += Item_PropertyChanged;
            else {
                item.PropertyChanged -= Item_PropertyChanged;
            }

            return tvc;
        }


        void UpdateLabelText() {
            Cell.TextLabel.Text = Item.LabelText;
        }
        void UpdateLabelColor() {
            if (Item.LabelColor != Xamarin.Forms.Color.Default) {
                Cell.TextLabel.TextColor = Item.LabelColor.ToUIColor();
            }
        }
        void UpdateLabelFontSize() {
            Cell.TextLabel.Font = Cell.TextLabel.Font.WithSize((nfloat)Item.LabelFontSize);
        }
        void UpdateIcon() {
            Cell.ImageView.Image = Item.Image?.GetUIImage();
        }


        protected void UpdateBase() {
            UpdateLabelText();
            UpdateLabelColor();
            UpdateLabelFontSize();
            UpdateIcon();
        }

        void Item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == CellBase.LabelTextProperty.PropertyName) {
                UpdateLabelText();
            }
            else if (e.PropertyName == CellBase.LabelColorProperty.PropertyName) {
                UpdateLabelColor();
            }
            else if (e.PropertyName == CellBase.LabelFontSizeProperty.PropertyName) {
                UpdateLabelFontSize();
            }
        }
    }
}
