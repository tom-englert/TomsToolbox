namespace TomsToolbox.Wpf.XamlExtensions
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    /// <summary>
    /// Extensions to properly display a <see cref="System.Windows.Shapes.Polyline"/> in the coordinates of the containing canvas.
    /// It normalizes the points and aligns the polyline so the coordinates of the points match the coordinates of the canvas.
    /// </summary>
    public static class Polyline
    {
        /// <summary>
        /// Gets the data points in the canvas coordinates.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The data points</returns>
        [AttachedPropertyBrowsableForType(typeof(System.Windows.Shapes.Polyline))]
        public static ICollection<Point>? GetDataPoints(DependencyObject element)
        {
            return (ICollection<Point>?)element.GetValue(DataPointsProperty);
        }
        /// <summary>
        /// Sets the data points in the canvas coordinates.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="value">The value.</param>
        public static void SetDataPoints(DependencyObject element, ICollection<Point> value)
        {
            element.SetValue(DataPointsProperty, value);
        }
        /// <summary>
        /// Identifies the <see cref="P:TomsToolbox.Wpf.XamlExtensions.Polyline.DataPoints"/> attached property
        /// </summary>
        /// <AttachedPropertyComments>
        /// <summary>
        /// A helper property to normalize the data points and align the <see cref="System.Windows.Shapes.Polyline"/> in the containing canvas, so the data
        /// points match the coordinates of the canvas.
        /// </summary>
        /// </AttachedPropertyComments>
        public static readonly DependencyProperty DataPointsProperty = DependencyProperty.RegisterAttached(
            "DataPoints", typeof(ICollection<Point>), typeof(Polyline), 
            new FrameworkPropertyMetadata(default(ICollection<Point>), Points_Changed));


        private static void Points_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (!(sender is System.Windows.Shapes.Polyline target))
                return;

            if (!(args.NewValue is ICollection<Point> points))
            {
                target.Points = null;
                return;
            }

            var bounds = points.GetBoundingRect();
            var offset = (Vector)bounds.TopLeft;

            target.Points = new PointCollection(points.Select(item => item - offset));
            target.Stretch = Stretch.Fill;

            Canvas.SetTop(target, bounds.Top);
            Canvas.SetLeft(target, bounds.Left);
            Canvas.SetRight(target, bounds.Right);
            Canvas.SetBottom(target, bounds.Bottom);
        }

        /// <summary>
        /// Gets the bounding rectangle of all points.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <returns>The bounding rectangle.</returns>
        public static Rect GetBoundingRect(this ICollection<Point> points)
        {
            if (!points.Any())
                return new Rect();

            var xMin = points.Min(p => p.X);
            var xMax = points.Max(p => p.X);
            var yMin = points.Min(p => p.Y);
            var yMax = points.Max(p => p.Y);

            return new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
        }
    }
}
