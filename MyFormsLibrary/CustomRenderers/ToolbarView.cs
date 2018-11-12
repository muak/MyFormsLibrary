using System;

using Xamarin.Forms;

namespace MyFormsLibrary.CustomRenderers
{
    public class ToolbarView : ContentView
    {
        public ToolbarView() {
            Content = new Label { Text = "Hello ContentView" };
        }
    }
}

