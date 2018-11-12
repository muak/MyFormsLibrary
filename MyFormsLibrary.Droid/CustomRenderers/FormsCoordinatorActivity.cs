using System;
using Android.OS;
using Xamarin.Forms.Platform.Android;
using Android.Widget;
using Android.Views;
using System.Reflection;

namespace MyFormsLibrary.Droid.CustomRenderers
{
    public class FormsCoordinatorActivity:FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var fieldInfo = typeof(FormsAppCompatActivity).GetField("_layout", BindingFlags.Instance | BindingFlags.NonPublic);
            var root = (RelativeLayout)fieldInfo.GetValue(this);
           
            root.RemoveFromParent();

            var newRoot = (ViewGroup)LayoutInflater.Inflate(Resource.Layout.CoodinatorPageLayout, null, false);

            var container = newRoot.FindViewById<Android.Widget.FrameLayout>(Resource.Id.coordinatorContent);
        

            using (var param = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent))
            {
                container.AddView(root, 0, param);
            }

            SetContentView(newRoot);
        }
    }
}
