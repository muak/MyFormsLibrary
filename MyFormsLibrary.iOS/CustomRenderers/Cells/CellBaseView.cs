﻿using System;
using UIKit;
using Xamarin.Forms.Platform.iOS;
using System.Linq;

namespace MyFormsLibrary.iOS.CustomRenderers
{
    public class CellBaseView:CellTableViewCell
    {
        public UILabel ErrorLabel { get; private set; }
        private NSLayoutConstraint[] _constraint;

        public CellBaseView(string cellName):base(UIKit.UITableViewCellStyle.Value1,cellName)
        {
            SetErrorMessageLabel();
        }

        protected override void Dispose(bool disposing)
        {
            ContentView.RemoveConstraints(_constraint);
            ErrorLabel.RemoveFromSuperview();
            ErrorLabel.Dispose();
            base.Dispose(disposing);
        }

        private void SetErrorMessageLabel()
        {
            ErrorLabel = new UILabel();
            ErrorLabel.LineBreakMode = UILineBreakMode.Clip;
            ErrorLabel.Lines = 1;
            ErrorLabel.BackgroundColor = UIColor.FromWhiteAlpha(1, .5f);
            ErrorLabel.TextColor = UIColor.Red.ColorWithAlpha(0.8f);
            ErrorLabel.TintAdjustmentMode = UIViewTintAdjustmentMode.Automatic;
            ErrorLabel.AdjustsFontSizeToFitWidth = true;
            ErrorLabel.BaselineAdjustment = UIBaselineAdjustment.AlignCenters;
            ErrorLabel.TextAlignment = UITextAlignment.Right;
            ErrorLabel.AdjustsLetterSpacingToFitWidth = true;
            ErrorLabel.Font = ErrorLabel.Font.WithSize(10);

            ContentView.AddSubview(ErrorLabel);

            ErrorLabel.TranslatesAutoresizingMaskIntoConstraints = false;
            _constraint = new NSLayoutConstraint[]{
                NSLayoutConstraint.Create(
                    ErrorLabel,
                    NSLayoutAttribute.Top,
                    NSLayoutRelation.Equal,
                    ContentView,
                    NSLayoutAttribute.Top,
                    1,
                    2
                ),
                NSLayoutConstraint.Create(
                    ErrorLabel,
                    NSLayoutAttribute.Right,
                    NSLayoutRelation.Equal,
                    ContentView,
                    NSLayoutAttribute.Right,
                    1,
                    -10
                ),
                NSLayoutConstraint.Create(
                    ErrorLabel,
                    NSLayoutAttribute.Height,
                    NSLayoutRelation.Equal,
                    null,
                    NSLayoutAttribute.Height,
                    1,
                    14
                ),
                NSLayoutConstraint.Create(
                    ErrorLabel,
                    NSLayoutAttribute.Width,
                    NSLayoutRelation.Equal,
                    ContentView,
                    NSLayoutAttribute.Width,
                    1,
                    0
                )
            };

            ContentView.AddConstraints(_constraint);
            ErrorLabel.SizeToFit();
        }
    }
}
