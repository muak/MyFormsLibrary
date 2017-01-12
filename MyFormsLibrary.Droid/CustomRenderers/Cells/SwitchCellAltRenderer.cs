using System;
using MyFormsLibrary.CustomRenderers;
using Android.Widget;
using Android.Content;
using Xamarin.Forms;
using Android.Views;
using MyFormsLibrary.Droid.CustomRenderers;
using System.Diagnostics;
using Android.Graphics.Drawables;
using System.Linq;
using Xamarin.Forms.Platform.Android;
using ARelativeLayout = Android.Widget.RelativeLayout;
using Android.Util;
using Android.Content.Res;
using Android.Support.V7.Widget;

[assembly: ExportRenderer(typeof(SwitchCellAlt), typeof(SwitchCellAltRenderer))]
namespace MyFormsLibrary.Droid.CustomRenderers
{
    public class SwitchCellAltRenderer:CellBaseRenderer
    {
        SwitchCellAlt SwitchCell;
        SwitchCellAltView CellView;

        protected override Android.Views.View GetCellCore(Xamarin.Forms.Cell item, Android.Views.View convertView, Android.Views.ViewGroup parent, Android.Content.Context context) {
            base.GetCellCore(item, convertView, parent, context);

            SwitchCell = item as SwitchCellAlt;

            CellView = convertView as SwitchCellAltView;
            if (CellView == null) {
                CellView = new SwitchCellAltView(context, item);

            }
            else {
                
            }

            BaseView = CellView;
            UpdateBase();
            UpdateOn();
           
            return CellView;
        }

        protected override void OnCellPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            base.OnCellPropertyChanged(sender, e);

            if (e.PropertyName == SwitchCellAlt.OnProperty.PropertyName) {
                UpdateOn();
            }
        }

        void UpdateOn() {
            CellView.Switch.Checked = SwitchCell.On;
        }

    }

    public class SwitchCellAltView : CellBaseView, CompoundButton.IOnCheckedChangeListener
    {
        public SwitchCompat Switch { get; set; }
        private SwitchCellAlt SwitchCell;

        public SwitchCellAltView(Context context, Cell cell) : base(context, cell) {
            SwitchCell = cell as SwitchCellAlt;

            Switch = new SwitchCompat(context);

            Switch.SetOnCheckedChangeListener(this);
            Switch.Gravity = Android.Views.GravityFlags.Right | GravityFlags.CenterVertical;


            if (SwitchCell.AccentColor != Xamarin.Forms.Color.Default) {
                
                var color = SwitchCell.AccentColor.ToAndroid();

                var trackColors = new ColorStateList(new int[][]
                     {
                                new int[]{global::Android.Resource.Attribute.StateChecked},
                                new int[]{-global::Android.Resource.Attribute.StateChecked},
                     },
                    new int[] {
                                Android.Graphics.Color.Argb(76,color.R,color.G,color.B),
                                Android.Graphics.Color.Argb(76, 117, 117, 117)
                     });


                Switch.TrackDrawable.SetTintList(trackColors);

                var thumbColors = new ColorStateList(new int[][]
                     {
                                new int[]{global::Android.Resource.Attribute.StateChecked},
                                new int[]{-global::Android.Resource.Attribute.StateChecked},
                     },
                    new int[] {
                                color,
                                Android.Graphics.Color.Argb(255, 244, 244, 244)
                     });

                Switch.ThumbDrawable.SetTintList(thumbColors);

                var ripple = Switch.Background as RippleDrawable;
                ripple.SetColor(trackColors);


            }
           
            using (var lparam = new ARelativeLayout.LayoutParams(-2, -1)) {
                lparam.AddRule(LayoutRules.AlignParentRight);
                ContentView.AddView(Switch, lparam);
            }

            ErrorLabel.BringToFront();

        }

        public void OnCheckedChanged(CompoundButton buttonView, bool isChecked) {
            SwitchCell.On = isChecked;
        }
    }
}
