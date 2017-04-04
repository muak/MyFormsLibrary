using System;
using NControl.Abstractions;
using Xamarin.Forms;
using System.Reflection;
using System.Linq;
using NGraphics;
using System.IO;
using XColor = Xamarin.Forms.Color;
using System.Windows.Input;
using MyFormsLibrary.Infrastructure;

namespace MyFormsLibrary.NControls
{
	public class SvgView: NControlView
	{
		public SvgView() {

		}


		public static BindableProperty ResourceProperty =
			BindableProperty.Create(nameof(Resource), typeof(string), typeof(SvgView), null,
				defaultBindingMode: Xamarin.Forms.BindingMode.OneWay
			);


		public string Resource {
			get { return (string)GetValue(ResourceProperty); }
			set {
				if (value == Resource)
					return;

				SetValue(ResourceProperty, value);
				Invalidate();
			}
		}

		public static BindableProperty ColorProperty =
			BindableProperty.Create(
				nameof(Color),
				typeof(XColor),
				typeof(SvgView),
				XColor.Default,
				defaultBindingMode: BindingMode.OneWay
			);

		public XColor Color {
			get { return (XColor)GetValue(ColorProperty); }
			set { SetValue(ColorProperty, value); }
		}

		public static BindableProperty CommandProperty =
			BindableProperty.Create(
				nameof(Command),
				typeof(ICommand),
				typeof(SvgView),
				default(ICommand),
				defaultBindingMode: BindingMode.OneWay
			);

		public ICommand Command {
			get { return (ICommand)GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
		}

		public static BindableProperty CommandParameterProperty =
			BindableProperty.Create(
				nameof(CommandParameter),
				typeof(object),
				typeof(SvgView),
				default(object),
				defaultBindingMode: BindingMode.OneWay
			);

		public object CommandParameter {
			get { return (object)GetValue(CommandParameterProperty); }
			set { SetValue(CommandParameterProperty, value); }
		}

		public override bool TouchesEnded(System.Collections.Generic.IEnumerable<NGraphics.Point> points) {
			base.TouchesEnded(points);

			if (Command != null && !InputTransparent) {
				Command.Execute(CommandParameter ?? this);
				return true;
			}


			return false;
		}
		public override bool TouchesBegan(System.Collections.Generic.IEnumerable<NGraphics.Point> points) {
			base.TouchesBegan(points);
			if (Command != null && !InputTransparent) {
				return true;
			}

			return false;
		}

		public override bool TouchesCancelled(System.Collections.Generic.IEnumerable<NGraphics.Point> points) {
			base.TouchesCancelled(points);

			if (Command != null && !InputTransparent) {
				return true;
			}

			return false;
		}

		protected override void OnPropertyChanging(string propertyName) {
			// [iOS]IsVisible=falseなLayoutにNControlViewを追加すると
			// IsVisible=trueになった時にDrawingFunctionが呼ばれない件への対処

			if (propertyName == "Parent" &&
				Parent != null) {
				Parent.PropertyChanged -= ParentPropertyChanged;
			}
			base.OnPropertyChanged(propertyName);
		}

		protected override void OnPropertyChanged(string propertyName) {
			// [iOS]IsVisible=falseなLayoutにNControlViewを追加すると
			// IsVisible=trueになった時にDrawingFunctionが呼ばれない件への対処

			if (propertyName == "Parent") {
				Parent.PropertyChanged += ParentPropertyChanged;
			}
			else if (propertyName == ColorProperty.PropertyName) {
				Invalidate();
			}
            else if (propertyName == ResourceProperty.PropertyName) {
                Invalidate();
            }
            else if (propertyName == IsVisibleProperty.PropertyName) {
                Invalidate();
            }
			base.OnPropertyChanged(propertyName);
		}

		void ParentPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
			// [iOS]IsVisible=falseなLayoutにNControlViewを追加すると
			// IsVisible=trueになった時にDrawingFunctionが呼ばれない件への対処

			if (e.PropertyName == "IsVisible" &&
				true.Equals((Parent as VisualElement).IsVisible)) {
				Invalidate();
			}

		}


		public override void Draw(NGraphics.ICanvas canvas, NGraphics.Rect rect) {
 			GetResourceAndDraw(canvas,rect,Resource);
		}

		void GetResourceAndDraw(ICanvas canvas,Rect rect,string path) {
			
            var src = SvgLoader.GetResourceAndLoadSvg(path);
            if (src == null) {
                return;
            }

			var transform = Transform.AspectFillRect(src.ViewBox, rect);
			var transformedGraphic = src.TransformGeometry(transform);

			if (Color == XColor.Default) {
				transformedGraphic.Draw(canvas);
			}
			else {
				var nColor = new NGraphics.Color(Color.R, Color.G, Color.B, Color.A);
				foreach (var element in transformedGraphic.Children) {

					ApplyColor(element, nColor);
					element.Draw(canvas);
				}
			}
		}

		private void ApplyColor(NGraphics.Element element, NGraphics.Color color) {
			var children = (element as Group)?.Children;
			if (children != null) {
				foreach (var child in children) {
					ApplyColor(child, color);
				}
			}

			if (element?.Pen != null) {
				element.Pen = new Pen(color, element.Pen.Width);
			}

			if (element?.Brush != null) {
				element.Brush = new SolidBrush(color);
			}
		}

	}
}

