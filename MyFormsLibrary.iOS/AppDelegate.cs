using System;
using Foundation;
using Xamarin.Forms.Svg.iOS;

namespace MyFormsLibrary.iOS
{
    [Foundation.Preserve(AllMembers = true)]
    public static class Forms
    {
        public static void Init() {
            SvgImage.Init();
        }
    }
}

