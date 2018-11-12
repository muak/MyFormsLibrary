using System;
using Xamarin.Forms;
namespace MyFormsLibrary.CustomRenderers
{
    public class NestedListView:ListView
    {
        public NestedListView():base(ListViewCachingStrategy.RecycleElement)
        {
        }
    }
}
