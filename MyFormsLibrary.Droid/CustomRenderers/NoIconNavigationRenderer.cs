using Android.App;  
using Android.Graphics.Drawables;  
using Xamarin.Forms;  
using Xamarin.Forms.Platform.Android;  
using MyFormsLibrary.CustomRenderers;  
using MyFormsLibrary.Droid.CustomRenderers;   

[assembly: ExportRenderer(typeof(NoIconNavigationPage), typeof(NoIconNavigationRenderer))] 
namespace MyFormsLibrary.Droid.CustomRenderers
{
	public class NoIconNavigationRenderer: NavigationRenderer 
	{
		protected override void OnElementChanged(ElementChangedEventArgs < NavigationPage > e) {  
			base.OnElementChanged(e);  

			RemoveAppIconFromActionBar();  
		}  
		void RemoveAppIconFromActionBar() {  
			var actionBar = ((Activity) Context).ActionBar;  
			actionBar.SetIcon(new ColorDrawable(Color.Transparent.ToAndroid()));  
		}   
	}
}

