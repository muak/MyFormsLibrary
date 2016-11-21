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

[assembly: ExportRenderer(typeof(SwitchCellAlt), typeof(SwitchCellAltRenderer))]
namespace MyFormsLibrary.Droid.CustomRenderers
{
    public class SwitchCellAltRenderer:CellBaseRenderer
    {
        SwitchCellAlt SwitchCell;
        SwitchCellAltView CellView;

        public SwitchCellAltRenderer() {
        }

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

            var space = new Android.Widget.Space(context);
            space.SetBackgroundColor(Android.Graphics.Color.Red);
            using(var param = new LayoutParams(ViewGroup.LayoutParams.WrapContent,
                  ViewGroup.LayoutParams.WrapContent){Width=0,Weight=1 })
            {
                AddView(space,param);
            }

            Switch = new Android.Widget.Switch(context);
            Switch.SetOnCheckedChangeListener(this);
            Switch.Gravity = Android.Views.GravityFlags.Right;


            if (SwitchCell.AccentColor != Xamarin.Forms.Color.Default) {
                var thumb = Switch.ThumbDrawable.GetConstantState().NewDrawable() as StateListDrawable;
                var thumbState = thumb.GetConstantState() as DrawableContainer.DrawableContainerState;
                var thumbChildren = thumbState.GetChildren();

                thumbChildren[3].SetColorFilter(SwitchCell.AccentColor.ToAndroid(), Android.Graphics.PorterDuff.Mode.SrcIn); //ON
                //thumbChildren[2].SetColorFilter(SwitchCell.AccentColor.ToAndroid(), Android.Graphics.PorterDuff.Mode.SrcIn); //OFF初期値？
                //thumbChildren[5].SetColorFilter(Android.Graphics.Color.Violet, Android.Graphics.PorterDuff.Mode.SrcIn);//OFF
                thumbChildren[1].SetColorFilter(SwitchCell.AccentColor.ToAndroid(), Android.Graphics.PorterDuff.Mode.SrcIn); //ON初期値
                Switch.ThumbDrawable = thumb;

                var track = Switch.TrackDrawable.GetConstantState().NewDrawable() as StateListDrawable;
                var trackState = track.GetConstantState() as DrawableContainer.DrawableContainerState;
                var trackChildren = trackState.GetChildren();

                trackChildren[1].SetColorFilter(SwitchCell.AccentColor.ToAndroid(), Android.Graphics.PorterDuff.Mode.SrcIn); //ON
                //trackChildren[2].SetColorFilter(SwitchCell.TrackColor.ToAndroid(), Android.Graphics.PorterDuff.Mode.SrcIn); //初期値 OFF

                Switch.TrackDrawable = track;
            }
           
            var Params = new LayoutParams(
                ViewGroup.LayoutParams.WrapContent,
                ViewGroup.LayoutParams.WrapContent) {               
                Gravity = GravityFlags.CenterVertical | GravityFlags.Right
            };
            using (Params) {
                AddView(Switch, Params);
            }


        }

        public void OnCheckedChanged(CompoundButton buttonView, bool isChecked) {
            SwitchCell.On = isChecked;
        }
    }
}
