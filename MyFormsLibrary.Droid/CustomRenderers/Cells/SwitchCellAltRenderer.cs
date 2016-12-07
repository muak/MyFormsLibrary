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
        public Android.Widget.Switch Switch { get; set; }
        private SwitchCellAlt SwitchCell;

        public SwitchCellAltView(Context context, Cell cell) : base(context, cell) {
            SwitchCell = cell as SwitchCellAlt;

            Switch = new Android.Widget.Switch(context);

            Switch.SetOnCheckedChangeListener(this);
            Switch.Gravity = Android.Views.GravityFlags.Right;


            if (SwitchCell.AccentColor != Xamarin.Forms.Color.Default) {
                
                var thumb = Switch.ThumbDrawable.GetConstantState().NewDrawable() as AnimatedStateListDrawable;
                

                var thumbState = thumb.GetConstantState() as DrawableContainer.DrawableContainerState;
                var thumbChildren = thumbState.GetChildren();

                //thumbChildren[0].SetColorFilter(Android.Graphics.Color.Black, Android.Graphics.PorterDuff.Mode.SrcIn);
                //thumbChildren[1].SetColorFilter(SwitchCell.AccentColor.ToAndroid(), Android.Graphics.PorterDuff.Mode.SrcIn); //ON初期値
                thumbChildren[1].SetTint(SwitchCell.AccentColor.ToAndroid());
                //thumbChildren[2].SetColorFilter(Android.Graphics.Color.Argb(255, 224, 224, 224), Android.Graphics.PorterDuff.Mode.SrcIn); //OFF初期値？
                thumbChildren[2].SetTint(Android.Graphics.Color.Argb(255, 244, 244, 244));
                //thumbChildren[3].SetColorFilter(SwitchCell.AccentColor.ToAndroid(), Android.Graphics.PorterDuff.Mode.SrcIn); //ON
                thumbChildren[3].SetTint(SwitchCell.AccentColor.ToAndroid());
                //thumbChildren[4].SetColorFilter(Android.Graphics.Color.Black, Android.Graphics.PorterDuff.Mode.SrcIn);
                //thumbChildren[5].SetColorFilter(Android.Graphics.Color.Argb(255,224,224,224), Android.Graphics.PorterDuff.Mode.SrcIn);//OFF
                thumbChildren[5].SetTint(Android.Graphics.Color.Argb(255, 244, 244, 244));  //OFF
                //thumbChildren[6].SetColorFilter(SwitchCell.AccentColor.ToAndroid(), Android.Graphics.PorterDuff.Mode.SrcIn);

                Switch.ThumbDrawable = thumb;

                var track = Switch.TrackDrawable.GetConstantState().NewDrawable() as StateListDrawable;
                var trackState = track.GetConstantState() as DrawableContainer.DrawableContainerState;
                var trackChildren = trackState.GetChildren();

                //trackChildren[1].SetColorFilter(SwitchCell.AccentColor.ToAndroid(), Android.Graphics.PorterDuff.Mode.SrcIn); //ON
                trackChildren[1].SetTint(SwitchCell.AccentColor.ToAndroid());
                //trackChildren[2].SetColorFilter(Android.Graphics.Color.Argb(255,117,117,117), Android.Graphics.PorterDuff.Mode.SrcIn); //初期値 OFF
                trackChildren[2].SetTint(Android.Graphics.Color.Argb(255, 117, 117, 117));

                Switch.TrackDrawable = track;
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
