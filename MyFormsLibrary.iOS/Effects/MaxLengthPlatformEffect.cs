using System;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;
using MyFormsLibrary.iOS.Effects;
using MyFormsLibrary.Effects;


namespace MyFormsLibrary.iOS.Effects
{
    public class MaxLengthPlatformEffect:PlatformEffect
    {
        IMaxLength _effect;

        protected override void OnAttached()
        {
            if (Element is Entry) {
                _effect = new MaxLengthForEntry(Control, Container,Element);
            }

            _effect.Update();
        }

        protected override void OnDetached()
        {
            
        }
    }
}
