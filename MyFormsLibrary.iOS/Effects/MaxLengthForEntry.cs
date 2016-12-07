using System;
using MyFormsLibrary.Effects;
using UIKit;
using Xamarin.Forms;
namespace MyFormsLibrary.iOS.Effects
{
    public class MaxLengthForEntry:IMaxLength {
        UITextField _control;
        UIView _container;
        Entry _element;

        public MaxLengthForEntry(UIView control,UIView container, Xamarin.Forms.Element element)
        {
            _control = control as UITextField;
            _container = container;
            _element = element as Entry;

            _element.TextChanged += _element_TextChanged;
        }

        void _element_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue.Length > 10) {
                _element.Text = e.OldTextValue;
            }
        }

        void _control_ValueChanged(object sender, EventArgs e)
        {
            
        }

        public void OnDetached()
        {
            
        }

        public void Update()
        {
            
        }


    }
}
