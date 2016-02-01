namespace TomsToolbox.Wpf
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Threading;
    using TomsToolbox.Core;
    using TomsToolbox.Desktop;

    /// <summary>
    /// Common extension methods for presentation framework objects.
    /// </summary>
    public static class PresentationFrameworkExtensions
    {
        /// <summary>
        /// Waits until all pending messages up to the <see cref="DispatcherPriority.Background"/> priority are processed.
        /// </summary>
        /// <param name="visual">The dispatcher object to wait on.</param>
        public static void ProcessMessages(this Visual visual)
        {
            Contract.Requires(visual != null);

            ProcessMessages(visual, DispatcherPriority.Background);
        }

        /// <summary>
        /// Waits until all pending messages up to the specified priority are processed.
        /// </summary>
        /// <param name="visual">The dispatcher object to wait on.</param>
        /// <param name="priority">The priority up to which all messages should be processed.</param>
        public static void ProcessMessages(this Visual visual, DispatcherPriority priority)
        {
            Contract.Requires(visual != null);

            var dispatcher = visual.Dispatcher;
            Contract.Assume(dispatcher != null);

            ProcessMessages(dispatcher, priority);
        }

        /// <summary>
        /// Waits until all pending messages up to the <see cref="DispatcherPriority.Background"/> priority are processed.
        /// </summary>
        /// <param name="dispatcher">The dispatcher to wait on.</param>
        public static void ProcessMessages(this Dispatcher dispatcher)
        {
            Contract.Requires(dispatcher != null);

            ProcessMessages(dispatcher, DispatcherPriority.Background);
        }

        /// <summary>
        /// Waits until all pending messages up to the specified priority are processed.
        /// </summary>
        /// <param name="dispatcher">The dispatcher to wait on.</param>
        /// <param name="priority">The priority up to which all messages should be processed.</param>
        public static void ProcessMessages(this Dispatcher dispatcher, DispatcherPriority priority)
        {
            Contract.Requires(dispatcher != null);

            var frame = new DispatcherFrame();
            dispatcher.BeginInvoke(priority, () => frame.Continue = false);
            Dispatcher.PushFrame(frame);
        }

        /// <summary>
        /// Gets the center point of the specified rectangle.
        /// </summary>
        /// <param name="rect">The rectangle.</param>
        /// <returns>The center point.</returns>
        public static Point GetCenter(this Rect rect)
        {
            return new Point(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);
        }

        /// <summary>
        /// Translates the coordinates of the specified rectangle from the first visual to the second visual.
        /// </summary>
        /// <param name="rect">The rectangle.</param>
        /// <param name="from">The visual for which the rectangle coordinates are specified.</param>
        /// <param name="to">The visual to which the rectangle coordinates are translated.</param>
        /// <returns>The translated rectangle</returns>
        public static Rect Translate(this Rect rect, Visual from, Visual to)
        {
            Contract.Requires(from != null);
            Contract.Requires(to != null);
            Contract.Ensures(!Contract.Result<Rect>().IsEmpty);

            var transform = from.TransformToVisual(to);

            var translated = new Rect(transform.Transform(rect.TopLeft), transform.Transform(rect.BottomRight));
            Contract.Assume(!translated.IsEmpty);
            return translated;
        }

        /// <summary>
        /// Translates the coordinates of the specified point from the first visual to the second visual.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="from">The visual for which the point coordinates are specified.</param>
        /// <param name="to">The visual to which the point coordinates are translated.</param>
        /// <returns>The translated point</returns>
        public static Point Translate(this Point point, UIElement from, UIElement to)
        {
            Contract.Requires(from != null);
            Contract.Requires(to != null);

            return from.TranslatePoint(point, to);
        }

        /// <summary>
        /// Gets the client rectangle of the framework element.
        /// </summary>
        /// <param name="self">The framework element for which to retrieve the client rectangle.</param>
        /// <returns>The client rectangle</returns>
        public static Rect GetClientRect(this FrameworkElement self)
        {
            Contract.Requires(self != null);

            return new Rect(0, 0, self.ActualWidth, self.ActualHeight);
        }

        /// <summary>
        /// Gets the client rectangle of the framework element translated to another visual.
        /// </summary>
        /// <param name="self">The framework element for which to retrieve the client rectangle.</param>
        /// <param name="relativeTo">The visual to which the rectangle coordinates are translated.</param>
        /// <returns>
        /// The client rectangle relative to the visual.
        /// </returns>
        public static Rect GetClientRect(this FrameworkElement self, Visual relativeTo)
        {
            Contract.Requires(self != null);
            Contract.Requires(relativeTo != null);

            return self.GetClientRect().Translate(self, relativeTo);
        }

        /// <summary>
        /// Gets the extent of the framework element.
        /// </summary>
        /// <param name="self">The framework element for which to retrieve the extent.</param>
        /// <returns>The extent.</returns>
        public static Size GetExtent(this FrameworkElement self)
        {
            Contract.Requires(self != null);

            return new Size(self.ActualWidth, self.ActualHeight);
        }

        /// <summary>
        /// Gets the extent of the framework element translated to another visual.
        /// </summary>
        /// <param name="self">The framework element for which to retrieve the extent.</param>
        /// <param name="relativeTo">The visual to which the extent is translated.</param>
        /// <returns>
        /// The extent relative to the visual.
        /// </returns>
        public static Size GetExtent(this FrameworkElement self, FrameworkElement relativeTo)
        {
            Contract.Requires(self != null);
            Contract.Requires(relativeTo != null);

            return (Size)self.TranslatePoint(new Point(self.ActualWidth, self.ActualHeight), relativeTo);
        }

        /// <summary>
        /// Gets the physical size of one pixel in design units.
        /// </summary>
        /// <param name="self">The framework element used to get the presentation source.</param>
        /// <returns>The physical size of one pixel in design units.</returns>
        /// <exception cref="System.ArgumentException">The framework element is not loaded in the visual tree.</exception>
        public static Size GetPhysicalPixelSize(this FrameworkElement self)
        {
            Contract.Requires(self != null);

            var source = PresentationSource.FromVisual(self);

            if (source == null)
                throw new ArgumentException("The framework element is not loaded in the visual tree.");

            var compositionTarget = source.CompositionTarget;
            if (compositionTarget == null)
                throw new ArgumentException("The framework element is not loaded in the visual tree.");

            var transformFromDevice = compositionTarget.TransformFromDevice;

            var width = transformFromDevice.M11;
            var height = transformFromDevice.M22;
            Contract.Assume(width >= 0.0);
            Contract.Assume(height >= 0.0);

            return new Size(width, height);
        }

        /// <summary>
        /// Gets the physical size of one design unit in pixels.
        /// </summary>
        /// <param name="self">The framework element used to get the presentation source.</param>
        /// <returns>The physical size of one design unit in pixels.</returns>
        /// <exception cref="System.ArgumentException">The framework element is not loaded in the visual tree.</exception>
        public static Size GetDesignUnitSize(this FrameworkElement self)
        {
            Contract.Requires(self != null);

            var source = PresentationSource.FromVisual(self);

            if (source == null)
                throw new ArgumentException("The framework element is not loaded in the visual tree.");

            var compositionTarget = source.CompositionTarget;
            if (compositionTarget == null)
                throw new ArgumentException("The framework element is not loaded in the visual tree.");

            var transformFromDevice = compositionTarget.TransformToDevice;

            var width = transformFromDevice.M11;
            var height = transformFromDevice.M22;
            Contract.Assume(width >= 0.0);
            Contract.Assume(height >= 0.0);

            return new Size(width, height);
        }

        /// <summary>
        /// Merges the first transformation with the second.
        /// </summary>
        /// <param name="first">The base transformation.</param>
        /// <param name="others">The transformations to merge.</param>
        /// <returns>The merged transformation.</returns>
        public static GeneralTransform MergeWith(this GeneralTransform first, params GeneralTransform[] others)
        {
            Contract.Requires(first != null);
            Contract.Requires(others != null);
            Contract.Ensures(Contract.Result<GeneralTransform>() != null);

            var transformGroup = new GeneralTransformGroup();
            var children = transformGroup.Children;
            Contract.Assume(children != null);

            children.Add(first);
            children.AddRange(others);

            return transformGroup;
        }
    }
}
