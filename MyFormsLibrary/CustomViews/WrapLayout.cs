using System;
using System.Linq;
using Xamarin.Forms;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Collections;

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
		/// This method is called during the measure pass of a layout cycle to get the desired size of an element.
		/// </summary>
		/// <param name="widthConstraint">The available width for the element to use.</param>
		/// <param name="heightConstraint">The available height for the element to use.</param>
		protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint) {
			if (WidthRequest > 0)
				widthConstraint = Math.Min(widthConstraint, WidthRequest);
			if (HeightRequest > 0)
				heightConstraint = Math.Min(heightConstraint, HeightRequest);

			double internalWidth = double.IsPositiveInfinity(widthConstraint) ? double.PositiveInfinity : Math.Max(0, widthConstraint);
			double internalHeight = double.IsPositiveInfinity(heightConstraint) ? double.PositiveInfinity : Math.Max(0, heightConstraint);

			return Orientation == StackOrientation.Vertical
				? DoVerticalMeasure(internalWidth, internalHeight)
					: DoHorizontalMeasure(internalWidth, internalHeight);

		}

		/// <summary>
		/// Does the vertical measure.
		/// </summary>
		/// <returns>The vertical measure.</returns>
		/// <param name="widthConstraint">Width constraint.</param>
		/// <param name="heightConstraint">Height constraint.</param>
		private SizeRequest DoVerticalMeasure(double widthConstraint, double heightConstraint) {
			int columnCount = 1;

			double width = 0;
			double height = 0;
			double minWidth = 0;
			double minHeight = 0;
			double heightUsed = 0;

			foreach (var item in Children) {
				var size = item.Measure(widthConstraint, heightConstraint);
				width = Math.Max(width, size.Request.Width);

				var newHeight = height + size.Request.Height + Spacing;
				if (newHeight > heightConstraint) {
					columnCount++;
					heightUsed = Math.Max(height, heightUsed);
					height = size.Request.Height;
				}
				else
					height = newHeight;

				minHeight = Math.Max(minHeight, size.Minimum.Height);
				minWidth = Math.Max(minWidth, size.Minimum.Width);
			}

			if (columnCount > 1) {
				height = Math.Max(height, heightUsed);
				width *= columnCount;  // take max width
			}

			return new SizeRequest(new Size(width, height), new Size(minWidth, minHeight));
		}

		/// <summary>
		/// Does the horizontal measure.
		/// </summary>
		/// <returns>The horizontal measure.</returns>
		/// <param name="widthConstraint">Width constraint.</param>
		/// <param name="heightConstraint">Height constraint.</param>
		private SizeRequest DoHorizontalMeasure(double widthConstraint, double heightConstraint) {
			int rowCount = 1;

			double width = 0;
			double height = 0;
			double minWidth = 0;
			double minHeight = 0;
			double widthUsed = 0;

			foreach (var item in Children) {
				var size = item.Measure(widthConstraint, heightConstraint);
				height = Math.Max(height, size.Request.Height);

				var newWidth = width + size.Request.Width + Spacing;
				if (newWidth > widthConstraint) {
					rowCount++;
					widthUsed = Math.Max(width, widthUsed);
					width = size.Request.Width;
				}
				else
					width = newWidth;

				minHeight = Math.Max(minHeight, size.Minimum.Height);
				minWidth = Math.Max(minWidth, size.Minimum.Width);
			}

			if (rowCount > 1) {
				width = Math.Max(width, widthUsed);
				height = (height + Spacing) * rowCount - Spacing;  // take max height 
			}

			return new SizeRequest(new Size(width, height), new Size(minWidth, minHeight));
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

						if (IsSquare) {
							childHeight = columsSize;
							child.HeightRequest = columsSize;
						}

						if (Math.Abs(xPos - x) <= double.Epsilon) {
							childWidth += divOver;
						}
						child.WidthRequest = childWidth;
					}

					rowHeight = Math.Max(rowHeight, childHeight);



					var region = new Rectangle(xPos, yPos, childWidth, childHeight);
					//Debug.WriteLine($"x:{xPos}/{childWidth}/{child.Height}/y:{yPos}");
					LayoutChildIntoBoundingRegion(child, region);

					xPos += region.Width + Spacing;

					if (xPos + childWidth > width) {
						xPos = x;
						yPos += rowHeight + Spacing;
						rowHeight = 0;
					}

				}

			}


		}
	}
}
