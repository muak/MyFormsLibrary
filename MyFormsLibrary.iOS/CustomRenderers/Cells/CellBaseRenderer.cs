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
        protected TableViewEx ParentElement;
        CellBase Item;


        public override UIKit.UITableViewCell GetCell(Xamarin.Forms.Cell item, UIKit.UITableViewCell reusableCell, UIKit.UITableView tv) {
            Item = item as CellBase;

            ParentElement = item.Parent as TableViewEx;

            var tvc = reusableCell;

            if (tvc == null) {
                item.PropertyChanged += Item_PropertyChanged;
                ParentElement.PropertyChanged += ParentElement_PropertyChanged;
            }
            else {
                item.PropertyChanged -= Item_PropertyChanged;
                ParentElement.PropertyChanged -= ParentElement_PropertyChanged;
            }

            return tvc;
        }

        void ParentElement_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == TableViewEx.CellLabelColorProperty.PropertyName) {
                UpdateLabelColor();
            }
            else if (e.PropertyName == TableViewEx.CellLabelFontSizeProperty.PropertyName) {
                UpdateLabelFontSize();
            }

        }

        void UpdateLabelText() {
            Cell.TextLabel.Text = Item.LabelText;
        }
        void UpdateLabelColor() {
            if (Item.LabelColor != Xamarin.Forms.Color.Default) {
                Cell.TextLabel.TextColor = Item.LabelColor.ToUIColor();
            }
            else if(ParentElement.CellLabelColor != Xamarin.Forms.Color.Default) {
                
                Cell.TextLabel.TextColor = ParentElement.CellLabelColor.ToUIColor();
            }
        }
        void UpdateLabelFontSize() {
            if (Item.LabelFontSize > 0) {
               Cell.TextLabel.Font = Cell.TextLabel.Font.WithSize((nfloat)Item.LabelFontSize);
            }
            else {
                Cell.TextLabel.Font = Cell.TextLabel.Font.WithSize((nfloat)ParentElement.CellLabelFontSize);
            }
            Cell.SetNeedsLayout();
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
