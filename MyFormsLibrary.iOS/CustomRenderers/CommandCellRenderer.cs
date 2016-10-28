using System;
using System.ComponentModel;
using Xamarin.Forms.Platform.iOS;
using UIKit;
using MyFormsLibrary.CustomRenderers;
using Xamarin.Forms;
using MyFormsLibrary.iOS.CustomRenderers;

[assembly: ExportRenderer(typeof(CommandCell), typeof(CommandCellRenderer))]
namespace MyFormsLibrary.iOS.CustomRenderers
{
    public class CommandCellRenderer:CellRenderer
    {
        public override UIKit.UITableViewCell GetCell(Xamarin.Forms.Cell item, UIKit.UITableViewCell reusableCell, UIKit.UITableView tv) {
            var commandCell = (CommandCell)item;

            var tvc = reusableCell as CommandCellView;
            if (tvc == null)
                tvc = new CommandCellView(item.GetType().FullName);
            else {
                
            }


            //tvc.Cell = item;
           
            WireUpForceUpdateSizeRequested(item, tvc, tv);

            UpdateBackground(tvc, commandCell);
          
            return tvc;
        }



        static void OnCellPropertyChanged(object sender, PropertyChangedEventArgs e) {

        }

        class CommandCellView : UITableViewCell
        {
            UIStackView stack;

            public CommandCellView(string cellName) : base(UIKit.UITableViewCellStyle.Default, cellName) {
                stack = new UIStackView();
                stack.Axis = UILayoutConstraintAxis.Horizontal;
                stack.Alignment = UIStackViewAlignment.Center;
                stack.Distribution = UIStackViewDistribution.Fill;
                stack.Spacing = 0;

                var label = new UILabel();
                label.Text = "abc";
                label.TextColor = UIColor.Black;
                label.Font = UIFont.SystemFontOfSize(12f);
                label.SetContentCompressionResistancePriority(10f, UILayoutConstraintAxis.Horizontal);

                var space = new UIView();
                space.SetContentCompressionResistancePriority(0f, UILayoutConstraintAxis.Horizontal);

                var valueLabel = new UILabel();
                valueLabel.Text = "def";
                valueLabel.TextColor = UIColor.LightGray;
                valueLabel.Font = UIFont.SystemFontOfSize(12f);
                valueLabel.SetContentCompressionResistancePriority(5f, UILayoutConstraintAxis.Horizontal);

                stack.AddArrangedSubview(label);
                stack.AddArrangedSubview(space);
                stack.AddArrangedSubview(valueLabel);

                ContentView.AddSubview(stack);

                //TextLabel.Text = "XYZ";
                this.Accessory = UITableViewCellAccessory.DisclosureIndicator;
            }

            public override void LayoutSubviews() {
                base.LayoutSubviews();

                stack.Frame = new CoreGraphics.CGRect(0, 0, ContentView.Bounds.Width, ContentView.Bounds.Height);
            }

        }
    }
}
