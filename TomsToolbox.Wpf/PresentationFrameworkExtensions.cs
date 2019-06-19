namespace TomsToolbox.Wpf
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Threading;

    using JetBrains.Annotations;

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
        public static void ProcessMessages([NotNull] this Visual visual)
        {
            ProcessMessages(visual, DispatcherPriority.Background);
        }

        /// <summary>
        /// Waits until all pending messages up to the specified priority are processed.
        /// </summary>
        /// <param name="visual">The dispatcher object to wait on.</param>
        /// <param name="priority">The priority up to which all messages should be processed.</param>
        public static void ProcessMessages([NotNull] this Visual visual, DispatcherPriority priority)
        {
            var dispatcher = visual.Dispatcher;

            ProcessMessages(dispatcher, priority);
        }

        /// <summary>
        /// Waits until all pending messages up to the <see cref="DispatcherPriority.Background"/> priority are processed.
        /// </summary>
        /// <param name="dispatcher">The dispatcher to wait on.</param>
        public static void ProcessMessages([NotNull] this Dispatcher dispatcher)
        {
            ProcessMessages(dispatcher, DispatcherPriority.Background);
        }

        /// <summary>
        /// Waits until all pending messages up to the specified priority are processed.
        /// </summary>
        /// <param name="dispatcher">The dispatcher to wait on.</param>
        /// <param name="priority">The priority up to which all messages should be processed.</param>
        public static void ProcessMessages([NotNull] this Dispatcher dispatcher, DispatcherPriority priority)
        {
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
        public static Rect Translate(this Rect rect, [NotNull] Visual from, [NotNull] Visual to)
        {
            var transform = from.TransformToVisual(to);

            // ReSharper disable once PossibleNullReferenceException
            var translated = new Rect(transform.Transform(rect.TopLeft), transform.Transform(rect.BottomRight));
            return translated;
        }

        /// <summary>
        /// Translates the coordinates of the specified point from the first visual to the second visual.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="from">The visual for which the point coordinates are specified.</param>
        /// <param name="to">The visual to which the point coordinates are translated.</param>
        /// <returns>The translated point</returns>
        public static Point Translate(this Point point, [NotNull] UIElement from, [NotNull] UIElement to)
        {
            return from.TranslatePoint(point, to);
        }

        /// <summary>
        /// Gets the client rectangle of the framework element.
        /// </summary>
        /// <param name="self">The framework element for which to retrieve the client rectangle.</param>
        /// <returns>The client rectangle</returns>
        public static Rect GetClientRect([NotNull] this FrameworkElement self)
        {
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
        public static Rect GetClientRect([NotNull] this FrameworkElement self, [NotNull] Visual relativeTo)
        {
            return self.GetClientRect().Translate(self, relativeTo);
        }

        /// <summary>
        /// Gets the extent of the framework element.
        /// </summary>
        /// <param name="self">The framework element for which to retrieve the extent.</param>
        /// <returns>The extent.</returns>
        public static Size GetExtent([NotNull] this FrameworkElement self)
        {
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
        public static Size GetExtent([NotNull] this FrameworkElement self, [NotNull] FrameworkElement relativeTo)
        {
            return (Size)self.TranslatePoint(new Point(self.ActualWidth, self.ActualHeight), relativeTo);
        }

        /// <summary>
        /// Gets the physical size of one pixel in design units.
        /// </summary>
        /// <param name="self">The framework element used to get the presentation source.</param>
        /// <returns>The physical size of one pixel in design units.</returns>
        /// <exception cref="System.ArgumentException">The framework element is not loaded in the visual tree.</exception>
        public static Size GetPhysicalPixelSize([NotNull] this FrameworkElement self)
        {
            var source = PresentationSource.FromVisual(self);

            if (source == null)
                throw new ArgumentException("The framework element is not loaded in the visual tree.");

            var compositionTarget = source.CompositionTarget;
            if (compositionTarget == null)
                throw new ArgumentException("The framework element is not loaded in the visual tree.");

            var transformFromDevice = compositionTarget.TransformFromDevice;

            var width = transformFromDevice.M11;
            var height = transformFromDevice.M22;

            return new Size(width, height);
        }

        /// <summary>
        /// Gets the physical size of one design unit in pixels.
        /// </summary>
        /// <param name="self">The framework element used to get the presentation source.</param>
        /// <returns>The physical size of one design unit in pixels.</returns>
        /// <exception cref="System.ArgumentException">The framework element is not loaded in the visual tree.</exception>
        public static Size GetDesignUnitSize([NotNull] this FrameworkElement self)
        {
            var source = PresentationSource.FromVisual(self);

            if (source == null)
                throw new ArgumentException("The framework element is not loaded in the visual tree.");

            var compositionTarget = source.CompositionTarget;
            if (compositionTarget == null)
                throw new ArgumentException("The framework element is not loaded in the visual tree.");

            var transformFromDevice = compositionTarget.TransformToDevice;

            var width = transformFromDevice.M11;
            var height = transformFromDevice.M22;

            return new Size(width, height);
        }

        /// <summary>
        /// Merges the first transformation with the second.
        /// </summary>
        /// <param name="first">The base transformation.</param>
        /// <param name="others">The transformations to merge.</param>
        /// <returns>The merged transformation.</returns>
        [NotNull]
        public static GeneralTransform MergeWith([NotNull] this GeneralTransform first, [NotNull, ItemNotNull] params GeneralTransform[] others)
        {
            var transformGroup = new GeneralTransformGroup();
            var children = transformGroup.Children;

            children.Add(first);
            children.AddRange(others);

            return transformGroup;
        }

        /// <summary>
        /// Tracks the changes of the specified property. 
        /// Unlike the <see cref="Desktop.DependencyObjectExtensions.Track{T}"/>, it tracks events only while the <paramref name="frameworkElement"/> is loaded, 
        /// to avoid memory leaks because the event handlers are referenced by the global <see cref="DependencyPropertyDescriptor"/>.
        /// </summary>
        /// <typeparam name="T">The type of the framework element to track.</typeparam>
        /// <param name="frameworkElement">The framework element.</param>
        /// <param name="property">The property to track.</param>
        /// <returns>The object providing the changed event.</returns>
        [NotNull]
        public static INotifyChanged ChangeTracker<T>([NotNull] this T frameworkElement, [NotNull] DependencyProperty property)
            where T : FrameworkElement
        {
            return new DependencyPropertyEventWrapper<T>(frameworkElement, property);
        }

        private class DependencyPropertyEventWrapper<T> : INotifyChanged
            where T : FrameworkElement
        {
            [NotNull]
            private readonly T _frameworkElement;
            [NotNull]
            private readonly DependencyPropertyDescriptor _dependencyPropertyDescriptor;
            [NotNull, ItemNotNull]
            private readonly HashSet<EventHandler> _eventHandlers = new HashSet<EventHandler>();

            public DependencyPropertyEventWrapper([NotNull] T frameworkElement, [NotNull] DependencyProperty property)
            {
                _frameworkElement = frameworkElement;
                // ReSharper disable once AssignNullToNotNullAttribute
                _dependencyPropertyDescriptor = DependencyPropertyDescriptor.FromProperty(property, typeof(T));

                _frameworkElement.Loaded += FrameworkElement_Loaded;
                _frameworkElement.Unloaded += FrameworkElement_Unloaded;
            }

            private void FrameworkElement_Loaded(object sender, RoutedEventArgs e)
            {
                foreach (var eventHandler in _eventHandlers)
                {
                    _dependencyPropertyDescriptor.RemoveValueChanged(_frameworkElement, eventHandler);
                    _dependencyPropertyDescriptor.AddValueChanged(_frameworkElement, eventHandler);
                }
            }

            private void FrameworkElement_Unloaded(object sender, RoutedEventArgs routedEventArgs)
            {
                foreach (var eventHandler in _eventHandlers)
                {
                    _dependencyPropertyDescriptor.RemoveValueChanged(_frameworkElement, eventHandler);
                }
            }

            public event EventHandler Changed
            {
                add => Subscribe(value);
                remove => Unsubscribe(value);
            }

            private void Subscribe(EventHandler value)
            {
                if (value == null)
                    return;

                _frameworkElement.VerifyAccess();

                if (!_eventHandlers.Add(value))
                    return;

                if (!_frameworkElement.IsLoaded) 
                    return;

                _dependencyPropertyDescriptor.AddValueChanged(_frameworkElement, value);
            }

            private void Unsubscribe(EventHandler value)
            {
                if (value == null)
                    return;

                _frameworkElement.VerifyAccess();

                _eventHandlers.Remove(value);
                _dependencyPropertyDescriptor.RemoveValueChanged(_frameworkElement, value);
            }
        }
    }
}
