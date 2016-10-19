using System;
using NGraphics;

namespace MyFormsLibrary.DependencyServices
{
	public interface ISvgService
	{
		IImage GetCanvas(Graphic g,double width,double height);
		IImage GetCanvas(Graphic g, double width, double height,Xamarin.Forms.Color color);
	}
}

