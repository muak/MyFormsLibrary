using System;
using Xamarin.Forms;
namespace MyFormsLibrary.CustomRenderers
{
	public class NonSelectionListView : ListView
	{

		public NonSelectionListView():base(ListViewCachingStrategy.RecycleElement) {

		}

        public NonSelectionListView(ListViewCachingStrategy strategy) : base(strategy) {

        }

	}
}

