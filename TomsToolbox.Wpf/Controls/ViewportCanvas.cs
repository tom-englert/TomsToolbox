namespace TomsToolbox.Wpf.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Controls;

    using JetBrains.Annotations;

    /// <summary>
    /// A canvas that transforms the coordinates of it's children to the own visual before arranging them. 
    /// It is used to have an overlay of non-scaled object on top of a scaled object, where the coordinates of the overlay objects are expressed in the coordinate system 
    /// of the scaled object (<see cref="World"/>). One major use case are e.g. pin points on a map, where the map scales, while the pin points only move but keep their size.
    /// While the <see cref="Canvas"/> only accepts one of <see cref="P:System.Windows.Controls.Canvas.Left"/>/<see cref="P:System.Windows.Controls.Canvas.Right"/> 
    /// or <see cref="P:System.Windows.Controls.Canvas.Top"/>/<see cref="P:System.Windows.Controls.Canvas.Bottom"/>, 
    /// with the ViewportCanvas you can specify both to stretch the object accordingly to the transformation.
    /// </summary>
    public class ViewportCanvas : Canvas, ILayer
    {
        /// <summary>
        /// Gets or sets the physical layer. The children's coordinates are assumed to be relative to the physical layer.
        /// </summary>
        [CanBeNull]
        public FrameworkElement World
        {
            get { return (FrameworkElement)GetValue(WorldProperty); }
            set { SetValue(WorldProperty, value); }
        }
        /// <summary>
        /// Identifies the <see cref="World"/> dependency property.
        /// </summary>
        [NotNull] public static readonly DependencyProperty WorldProperty =
            DependencyProperty.Register("World", typeof(FrameworkElement), typeof(ViewportCanvas));

        /// <summary>
        /// When overridden in a derived class, measures the size in layout required for child elements and determines a size for the <see cref="T:System.Windows.FrameworkElement" />-derived class.
        /// </summary>
        /// <param name="availableSize">The available size that this element can give to child elements. Infinity can be specified as a value to indicate that the element will size to whatever content is available.</param>
        /// <returns>
        /// The size that this element determines it needs during layout, based on its calculations of child element sizes.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            var worldLayer = World;
            var viewportLayer = this;

            if (worldLayer == null)
                return new Size();

            // ReSharper disable once PossibleNullReferenceException
            foreach (UIElement child in InternalChildren)
            {
                if (child == null)
                    continue;

                var left = GetLeft(child);
                var top = GetTop(child);
                var right = GetRight(child);
                var bottom = GetBottom(child);

                var rect = new Rect(0, 0, Math.Abs(right - left), Math.Abs(bottom - top));
                var constraint = rect.Translate(worldLayer, viewportLayer).Size;

                if (double.IsNaN(constraint.Width))
                    constraint.Width = double.PositiveInfinity;
                if (double.IsNaN(constraint.Height))
                    constraint.Height = double.PositiveInfinity;

                child.Measure(constraint);
            }

            return new Size();
        }

        /// <summary>
        /// When overridden in a derived class, positions child elements and determines a size for a <see cref="T:System.Windows.FrameworkElement" /> derived class.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use to arrange itself and its children.</param>
        /// <returns>
        /// The actual size used.
        /// </returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            var worldLayer = World;
            var viewportLayer = this;

            if (worldLayer == null)
                return finalSize;

            // ReSharper disable once PossibleNullReferenceException
            foreach (UIElement child in Children)
            {
                if (child == null)
                    continue;

                var left = GetLeft(child);
                var top = GetTop(child);
                var right = GetRight(child);
                var bottom = GetBottom(child);

                var targetRect = GetTargetRect(left, right, top, bottom);

                targetRect =  targetRect.Translate(worldLayer, viewportLayer);

                if (Math.Abs(targetRect.Width) < double.Epsilon)
                    targetRect.Width = child.DesiredSize.Width;
                if (Math.Abs(targetRect.Height) < double.Epsilon)
                    targetRect.Height = child.DesiredSize.Height;

                if (double.IsNaN(left)) // only right specified
                    targetRect.X -= targetRect.Width;
                if (double.IsNaN(top)) // only bottom specified
                    targetRect.Y -= targetRect.Height;

                if (double.IsNaN(targetRect.X) || double.IsNaN(targetRect.Y))
                    continue;

                child.Arrange(targetRect);
            }

            return finalSize;
        }

        private static Rect GetTargetRect(double left, double right, double top, double bottom)
        {
            var targetRect = new Rect(0,0,0,0);

            if (!double.IsNaN(left))
            {
                if (!double.IsNaN(right))
                {
                    targetRect.X = Math.Min(left, right);
                    targetRect.Width = Math.Abs(right - left);
                }
                else
                {
                    targetRect.X = left;
                }
            }
            else
            {
                targetRect.X = right;
            }

            if (!double.IsNaN(top))
            {
                if (!double.IsNaN(bottom))
                {
                    targetRect.Y = Math.Min(top, bottom);
                    targetRect.Height = Math.Abs(bottom - top);
                }
                else
                {
                    targetRect.Y = top;
                }
            }
            else
            {
                targetRect.Y = bottom;
            }

            return targetRect;
        }

        /// <summary>
        /// Invalidates the layout of this instance.
        /// </summary>
        public void Invalidate()
        {
            InvalidateMeasure();
        }
    }
}
