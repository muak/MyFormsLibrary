using MyFormsLibrary.CustomRenderers;
using MyFormsLibrary.iOS.CustomRenderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(TableViewEx), typeof(TableViewExRenderer))]
namespace MyFormsLibrary.iOS.CustomRenderers
{
    public class TableViewExRenderer:TableViewRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.TableView> e) {
            base.OnElementChanged(e);

            if (e.NewElement != null) {
                UpdateSeparator();

                SetSource();
            }
        }


        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == TableViewEx.SeparatorColorProperty.PropertyName) {
                UpdateSeparator();
            }
            else if (e.PropertyName == TableViewEx.SectionTitleColorProperty.PropertyName) {
                Control.ReloadData();
            }
        }

        void UpdateSeparator() {
            var color = (Element as TableViewEx).SeparatorColor;
            Control.SeparatorColor = color == Color.Default ? UIColor.FromRGB(199,199,204) :color.ToUIColor();
        }

    

        void SetSource() {
            var modeledView = Element;
            Control.Source = modeledView.HasUnevenRows ? new UnEvenTableViewModelRenderer(modeledView) : (TableViewModelRenderer)new TableViewModelExRenderer(modeledView);
        }

    }
}
