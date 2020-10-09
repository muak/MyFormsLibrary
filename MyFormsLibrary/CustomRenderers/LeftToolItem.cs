using System;
using Xamarin.Forms;

namespace MyFormsLibrary.CustomRenderers
{
    public static class LeftToolItem
    {
        public static readonly BindableProperty ToolbarItemProperty = BindableProperty.CreateAttached(
            "ToolbarItem",
            typeof(ToolbarItem),
            typeof(LeftToolItem),
            default(ToolbarItem),
            propertyChanging: (bindable, oldValue, newValue) => {
                // Pageに直接添付するとBindingContextが継承されないので手動で設定する
                // こんなことは以前無かった気がするのだが…
                if(newValue is ToolbarItem item)
                {
                    item.Parent = bindable as VisualElement;
                    item.BindingContext = bindable.BindingContext;
                }
            }
        );

        public static void SetToolbarItem(BindableObject view, ToolbarItem value)
        {
            view.SetValue(ToolbarItemProperty, value);
        }

        public static ToolbarItem GetToolbarItem(BindableObject view)
        {
            return (ToolbarItem)view.GetValue(ToolbarItemProperty);
        }
    }
}
