namespace TomsToolbox.Wpf
{
    using System.Windows;
    using System.Windows.Controls;

    using JetBrains.Annotations;

    using TomsToolbox.Essentials;

    /// <summary>
    /// Anchors for a canvas to specify two coordinates in one single operation specifying a <see cref="Point"/>.
    /// </summary>
    public static class CanvasAnchor
    {
        private static readonly Point _noPoint = new Point(double.NaN, double.NaN);

        /// <summary>
        /// Gets the elements <see cref="P:TomsToolbox.Wpf.CanvasAnchor.TopLeft"/> point in the canvas.
        /// </summary>
        /// <param name="obj">The object on which this property was set.</param>
        /// <returns>The point.</returns>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static Point GetTopLeft([NotNull] UIElement obj)
        {
            return obj.GetValue<Point>(TopLeftProperty);
        }
        /// <summary>
        /// Sets the elements <see cref="P:TomsToolbox.Wpf.CanvasAnchor.TopLeft"/> point in the canvas.
        /// </summary>
        /// <param name="obj">The object on which to set <see cref="P:System.Windows.Controls.Canvas.Top"/> and <see cref="P:System.Windows.Controls.Canvas.Left"/>.</param>
        /// <param name="value">The point.</param>
        public static void SetTopLeft([NotNull] UIElement obj, Point value)
        {
            obj.SetValue(TopLeftProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="P:TomsToolbox.Wpf.CanvasAnchor.TopLeft"/> attached property
        /// </summary>
        /// <AttachedPropertyComments>
        /// <summary>
        /// A helper property to assign the coordinates of a <see cref="Point"/> to the 
        /// <see cref="P:System.Windows.Controls.Canvas.Top"/> and <see cref="P:System.Windows.Controls.Canvas.Left"/> property in one single operation.
        /// </summary>
        /// <remarks>
        /// This property is only "one way", i.e. changing <see cref="P:System.Windows.Controls.Canvas.Top"/> or <see cref="P:System.Windows.Controls.Canvas.Left"/> 
        /// will not affect this attached property.
        /// </remarks>
        /// </AttachedPropertyComments>
        [NotNull] public static readonly DependencyProperty TopLeftProperty =
            DependencyProperty.RegisterAttached("TopLeft", typeof(Point), typeof(CanvasAnchor), new FrameworkPropertyMetadata(_noPoint, TopLeft_Changed));

        private static void TopLeft_Changed([NotNull] DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var point = e.NewValue.SafeCast<Point>();

            if (!(d is UIElement uiElement))
                return;

            Canvas.SetTop(uiElement, point.Y);
            Canvas.SetLeft(uiElement, point.X);
        }



        /// <summary>
        /// Gets the elements <see cref="P:TomsToolbox.Wpf.CanvasAnchor.TopRight"/> point in the canvas.
        /// </summary>
        /// <param name="obj">The object on which this property was set.</param>
        /// <returns>The point.</returns>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static Point GetTopRight([NotNull] UIElement obj)
        {
            return obj.GetValue<Point>(TopRightProperty);
        }

        /// <summary>
        /// Sets the elements <see cref="P:TomsToolbox.Wpf.CanvasAnchor.TopRight"/> point in the canvas.
        /// </summary>
        /// <param name="obj">The object on which to set <see cref="P:System.Windows.Controls.Canvas.Top"/> and <see cref="P:System.Windows.Controls.Canvas.Right"/>.</param>
        /// <param name="value">The point.</param>
        public static void SetTopRight([NotNull] UIElement obj, Point value)
        {
            obj.SetValue(TopRightProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="P:TomsToolbox.Wpf.CanvasAnchor.TopRight"/> attached property
        /// </summary>
        /// <AttachedPropertyComments>
        /// <summary>
        /// A helper property to assign the coordinates of a <see cref="Point"/> to the 
        /// <see cref="P:System.Windows.Controls.Canvas.Top"/> and <see cref="P:System.Windows.Controls.Canvas.Right"/> property in one single operation.
        /// </summary>
        /// <remarks>
        /// This property is only "one way", i.e. changing <see cref="P:System.Windows.Controls.Canvas.Top"/> or <see cref="P:System.Windows.Controls.Canvas.Right"/> 
        /// will not affect this attached property.
        /// </remarks>
        /// </AttachedPropertyComments>
        [NotNull] public static readonly DependencyProperty TopRightProperty =
            DependencyProperty.RegisterAttached("TopRight", typeof(Point), typeof(CanvasAnchor), new FrameworkPropertyMetadata(_noPoint, TopRight_Changed));

        private static void TopRight_Changed([NotNull] DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var point = e.NewValue.SafeCast<Point>();

            if (!(d is UIElement uiElement))
                return;

            Canvas.SetTop(uiElement, point.Y);
            Canvas.SetRight(uiElement, point.X);
        }


        /// <summary>
        /// Gets the elements <see cref="P:TomsToolbox.Wpf.CanvasAnchor.BottomLeft"/> point in the canvas.
        /// </summary>
        /// <param name="obj">The object on which this property was set.</param>
        /// <returns>The point.</returns>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static Point GetBottomLeft([NotNull] UIElement obj)
        {
            return obj.GetValue<Point>(BottomLeftProperty);
        }

        /// <summary>
        /// Sets the elements <see cref="P:TomsToolbox.Wpf.CanvasAnchor.BottomLeft"/> point in the canvas.
        /// </summary>
        /// <param name="obj">The object on which to set <see cref="P:System.Windows.Controls.Canvas.Bottom"/> and <see cref="P:System.Windows.Controls.Canvas.Left"/>.</param>
        /// <param name="value">The point.</param>
        public static void SetBottomLeft([NotNull] UIElement obj, Point value)
        {
            obj.SetValue(BottomLeftProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="P:TomsToolbox.Wpf.CanvasAnchor.BottomLeft"/> attached property
        /// </summary>
        /// <AttachedPropertyComments>
        /// <summary>
        /// A helper property to assign the coordinates of a <see cref="Point"/> to the 
        /// <see cref="P:System.Windows.Controls.Canvas.Bottom"/> and <see cref="P:System.Windows.Controls.Canvas.Left"/> property in one single operation.
        /// </summary>
        /// <remarks>
        /// This property is only "one way", i.e. changing <see cref="P:System.Windows.Controls.Canvas.Bottom"/> or <see cref="P:System.Windows.Controls.Canvas.Left"/> 
        /// will not affect this attached property.
        /// </remarks>
        /// </AttachedPropertyComments>
        [NotNull] public static readonly DependencyProperty BottomLeftProperty =
            DependencyProperty.RegisterAttached("BottomLeft", typeof(Point), typeof(CanvasAnchor), new FrameworkPropertyMetadata(_noPoint, BottomLeft_Changed));

        private static void BottomLeft_Changed([NotNull] DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var point = e.NewValue.SafeCast<Point>();

            if (!(d is UIElement uiElement))
                return;

            Canvas.SetBottom(uiElement, point.Y);
            Canvas.SetLeft(uiElement, point.X);
        }



        /// <summary>
        /// Gets the elements <see cref="P:TomsToolbox.Wpf.CanvasAnchor.BottomRight"/> point in the canvas.
        /// </summary>
        /// <param name="obj">The object on which this property was set.</param>
        /// <returns>The point.</returns>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static Point GetBottomRight([NotNull] UIElement obj)
        {
            return obj.GetValue<Point>(BottomRightProperty);
        }

        /// <summary>
        /// Sets the elements <see cref="P:TomsToolbox.Wpf.CanvasAnchor.BottomRight"/> point in the canvas.
        /// </summary>
        /// <param name="obj">The object on which to set <see cref="P:System.Windows.Controls.Canvas.Bottom"/> and <see cref="P:System.Windows.Controls.Canvas.Right"/>.</param>
        /// <param name="value">The point.</param>
        public static void SetBottomRight([NotNull] UIElement obj, Point value)
        {
            obj.SetValue(BottomRightProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="P:TomsToolbox.Wpf.CanvasAnchor.BottomRight"/> attached property
        /// </summary>
        /// <AttachedPropertyComments>
        /// <summary>
        /// A helper property to assign the coordinates of a <see cref="Point"/> to the 
        /// <see cref="P:System.Windows.Controls.Canvas.Bottom"/> and <see cref="P:System.Windows.Controls.Canvas.Right"/> property in one single operation.
        /// </summary>
        /// <remarks>
        /// This property is only "one way", i.e. changing <see cref="P:System.Windows.Controls.Canvas.Bottom"/> or <see cref="P:System.Windows.Controls.Canvas.Right"/> 
        /// will not affect this attached property.
        /// </remarks>
        /// </AttachedPropertyComments>
        [NotNull] public static readonly DependencyProperty BottomRightProperty =
            DependencyProperty.RegisterAttached("BottomRight", typeof(Point), typeof(CanvasAnchor), new FrameworkPropertyMetadata(_noPoint, BottomRight_Changed));

        private static void BottomRight_Changed([NotNull] DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var point = e.NewValue.SafeCast<Point>();

            if (!(d is UIElement uiElement))
                return;

            Canvas.SetBottom(uiElement, point.Y);
            Canvas.SetRight(uiElement, point.X);
        }
    }
}
