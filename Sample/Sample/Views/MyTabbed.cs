using System;
using XF = Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using MyFormsLibrary.CustomRenderers;
using System.Collections.Generic;

namespace Sample.Views
{
    public class MyTabbed:TabbedPageEx
    {
        public MyTabbed() 
        {
            this.On<Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);

            UnSelectedColor = XF.Color.Orange.MultiplyAlpha(0.5);
            SelectedColor = XF.Color.Orange;
            UnSelectedTextColor = XF.Color.LightGray;
            SelectedTextColor = XF.Color.Gray;

            TabAttributes = new List<TabAttribute> {
                new TabAttribute{
                    Title = "Main",
                    Resource = "calendar.svg",
                },
                new TabAttribute{
                    Title = "Sub",
                    Resource = "colours.svg",
                },
                new TabAttribute{
                    Title = "Next",
                    Resource = "colours.svg",
                },
                new TabAttribute{
                    Title = "Other",
                    Resource = "colours.svg",
                },
            };
        }
    }
}
