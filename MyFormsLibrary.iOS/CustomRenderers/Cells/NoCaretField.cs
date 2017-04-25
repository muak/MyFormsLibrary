using System;
using CoreGraphics;
using UIKit;

namespace MyFormsLibrary.iOS.CustomRenderers
{
    internal class NoCaretField: UITextField
    {
        public NoCaretField() : base(new CGRect())
        {
        }

        public override CoreGraphics.CGRect GetCaretRectForPosition(UITextPosition position)
        {
            return new CGRect();
        }

    }
}
