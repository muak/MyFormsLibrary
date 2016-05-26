using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microsoft.Practices.Unity;

namespace MyFormsLibrary.CustomRenderers
{
	public class CameraPreview:View
	{
		public CameraPreview(){
			
		}

		public static readonly BindableProperty CameraProperty = BindableProperty.Create (
			propertyName: "Camera",
			returnType: typeof(CameraOptions),
			declaringType: typeof(CameraPreview),
			defaultValue: CameraOptions.Rear);

		public CameraOptions Camera {
			get { return (CameraOptions)GetValue (CameraProperty); }
			set { SetValue (CameraProperty, value); }
		}

		public void OnColorChanged(Color color){
			if(ColorChanged != null){
				ColorChanged (this,new ColorChangedEventArgs(color));
			}

		}

		public event EventHandler<ColorChangedEventArgs> ColorChanged;
	}

	public enum CameraOptions
	{
		Rear,
		Front
	}

	public sealed class ColorChangedEventArgs:EventArgs{
		public Color Color {get;set;}

		public ColorChangedEventArgs(Color color){
			Color = color;
		}
	}
}

