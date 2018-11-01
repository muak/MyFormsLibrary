using System;
using Xamarin.Forms;
using MyFormsLibrary.CustomRenderers.Cells;

namespace MyFormsLibrary.CustomRenderers
{
    public class ViewCellAlt:ViewCell,IContextMenuCell
    {
        public static BindableProperty ContextMenuTitleProperty =
            BindableProperty.Create(
                nameof(ContextMenuTitle),
                typeof(string),
                typeof(NonSelectionViewCell),
                default(string),
                defaultBindingMode: BindingMode.OneWay
            );

        public string ContextMenuTitle {
            get { return (string)GetValue(ContextMenuTitleProperty); }
            set { SetValue(ContextMenuTitleProperty, value); }
        }
    }
}
