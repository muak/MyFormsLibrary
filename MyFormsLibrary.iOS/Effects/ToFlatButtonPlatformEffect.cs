using System;
using MyFormsLibrary.Effects;
using MyFormsLibrary.iOS.Effects;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportEffect(typeof(ToFlatButtonPlatformEffect), nameof(ToFlatButton))]
namespace MyFormsLibrary.iOS.Effects
{
    public class ToFlatButtonPlatformEffect:PlatformEffect
    {
        public ToFlatButtonPlatformEffect()
        {
        }

        protected override void OnAttached()
        {
            //throw new NotImplementedException();
        }

        protected override void OnDetached()
        {
            //throw new NotImplementedException();
        }
    }
}
