using System;
using System.Linq;
using Xamarin.Forms;
using System.Diagnostics;

namespace MyFormsLibrary.CustomViews
{
	/// <summary>
	/// Simple Layout panel which performs wrapping on the boundaries.
	/// https://forums.xamarin.com/discussion/17961/stacklayout-with-horizontal-orientation-how-to-wrap-vertically 
	/// </summary>
	public class WrapLayout : StackLayout
	{
		/// <summary>
		///  number for uniform child width 
		/// </summary>
		public static readonly BindableProperty UniformColumnsProperty =
			BindableProperty.Create(
				propertyName: "UniformColumns",
				returnType: typeof(int),
				declaringType: typeof(WrapLayout),
				defaultValue: default(int),
				propertyChanged: (bindable, oldvalue, newvalue) => ((WrapLayout)bindable).OnSizeChanged()
		);
		public int UniformColumns {
			get { return (int)GetValue(UniformColumnsProperty); }
			set { SetValue(UniformColumnsProperty, value); }
		}

		/// <summary>
		///  make item height equal to item width when UniformColums > 0
		/// </summary>
		public static readonly BindableProperty IsSquareProperty =
			BindableProperty.Create(
				propertyName: "IsSquare",
				returnType: typeof(bool),
				declaringType: typeof(WrapLayout),
				defaultValue: false
		);
		public bool IsSquare {
			get { return (bool)GetValue(IsSquareProperty); }
			set { SetValue(IsSquareProperty, value); }
		}



		/// <summary>
		/// This is called when the spacing or orientation properties are changed - it forces
		/// the control to go back through a layout pass.
		/// </summary>
		private void OnSizeChanged() {
			this.ForceLayout();
		}



		/// <summary>
		/// Positions and sizes the children of a Layout.
		/// </summary>
		/// <param name="x">A value representing the x coordinate of the child region bounding box.</param>
		/// <param name="y">A value representing the y coordinate of the child region bounding box.</param>
		/// <param name="width">A value representing the width of the child region bounding box.</param>
		/// <param name="height">A value representing the height of the child region bounding box.</param>
		protected override void LayoutChildren(double x, double y, double width, double height) {
			if (Orientation == StackOrientation.Vertical) {
				double colWidth = 0;
				double yPos = y, xPos = x;

				foreach (var child in Children.Where(c => c.IsVisible)) {
					var request = child.Measure(width, height);

					double childWidth = request.Request.Width;
					double childHeight = request.Request.Height;
					colWidth = Math.Max(colWidth, childWidth);

					if (yPos + childHeight > height) {
						yPos = y;
						xPos += colWidth + Spacing;
						colWidth = 0;
					}

					var region = new Rectangle(xPos, yPos, childWidth, childHeight);
					LayoutChildIntoBoundingRegion(child, region);
					yPos += region.Height + Spacing;
				}
			}
			else {
				double rowHeight = 0;
				double yPos = y, xPos = x;

				foreach (var child in Children.Where(c => c.IsVisible)) {
					
					var request = child.Measure(width, height);

					double childWidth = request.Request.Width;
					double childHeight = request.Request.Height;

					var divOver = 0;
					if (UniformColumns != default(int)) {
						var exceptWidth = (int)width - (UniformColumns - 1) * Spacing;
						divOver = (int)exceptWidth  % UniformColumns;
						var columsSize = (int)exceptWidth / UniformColumns;
						if (columsSize < 1) {
							columsSize = 1;
						}

						childWidth = columsSize;
						childHeight = columsSize;
					}

					rowHeight = Math.Max(rowHeight, childHeight);

					if (Math.Abs(xPos - x) <= double.Epsilon) {
						childWidth += divOver;
					}

					var region = new Rectangle(xPos, yPos, childWidth, childHeight);
					Debug.WriteLine($"x:{xPos}/{childWidth}/{child.Height}");
					LayoutChildIntoBoundingRegion(child, region);

					xPos += region.Width + Spacing;

					if (xPos > width) {
						xPos = x;
						yPos += rowHeight + Spacing;
						rowHeight = 0;
					}

				}

			}
		}
	}
}
