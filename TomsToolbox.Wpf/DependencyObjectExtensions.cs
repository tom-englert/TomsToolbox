namespace TomsToolbox.Wpf
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Interop;
    using System.Windows.Media;

    using JetBrains.Annotations;

    /// <summary>
    /// Extensions methods to ease dealing with dependency objects.
    /// </summary>
    public static class DependencyObjectExtensions
    {
        /// <summary>
        /// Gets the window handle of the HwndSource hosting this item.
        /// </summary>
        /// <param name="self">The item.</param>
        /// <returns>The window handle, if the item is part of a valid visual tree, otherwise IntPtr.Zero.</returns>
        public static IntPtr GetWindowHandle([NotNull] this DependencyObject self)
        {
            var hwndSource = PresentationSource.FromDependencyObject(self) as HwndSource;

            return hwndSource?.Handle ?? IntPtr.Zero;
        }

        /// <summary>
        /// Gets the root visual for the item.
        /// </summary>
        /// <param name="item">The item to find the root visual for.</param>
        /// <returns>The root visual.</returns>
        /// <exception cref="ArgumentException">The item is not part of a valid visual tree.</exception>
        /// <remarks>
        /// If the item is inside a control that's embedded in a native or WindowsForms window, the root visual
        /// is <c>not</c> a <see cref="System.Windows.Window"/>.
        /// </remarks>
        [NotNull]
        public static FrameworkElement GetRootVisual([NotNull] this DependencyObject item)
        {
            var hwndSource = (HwndSource)PresentationSource.FromDependencyObject(item);
            if (hwndSource == null)
            {
                throw new ArgumentException(@"Item not part of a valid visual tree.", nameof(item));
            }
            var compositionTarget = hwndSource.CompositionTarget;
            if (compositionTarget == null)
            {
                throw new ArgumentException(@"Item not part of a valid visual tree.", nameof(item));
            }

            var rootVisual = (FrameworkElement)compositionTarget.RootVisual;
            if (rootVisual == null)
            {
                throw new ArgumentException(@"Item not part of a valid visual tree.", nameof(item));
            }

            return rootVisual;
        }

        /// <summary>
        /// Gets the root visual for the item.
        /// </summary>
        /// <param name="item">The item to find the root visual for.</param>
        /// <returns>The root visual if the item is part of a valid visual tree; otherwise <c>null</c>.
        /// </returns>
        /// /// <remarks>
        /// If the item is inside a control that's embedded in a native or WindowsForms window, the root visual
        /// is <c>not</c> a <see cref="System.Windows.Window"/>.
        /// </remarks>
        [CanBeNull]
        public static FrameworkElement TryGetRootVisual([NotNull] this DependencyObject item)
        {
            var hwndSource = (HwndSource)PresentationSource.FromDependencyObject(item);
            var compositionTarget = hwndSource?.CompositionTarget;

            var rootVisual = (FrameworkElement)compositionTarget?.RootVisual;

            return rootVisual;
        }

        /// <summary>
        /// Returns an enumeration of elements that contains this element, and the ancestors in the visual tree of this element.
        /// </summary>
        /// <param name="self">The starting element.</param>
        /// <returns>The ancestor list.</returns>
        [NotNull, ItemNotNull]
        public static IEnumerable<DependencyObject> VisualAncestorsAndSelf([NotNull] this DependencyObject self)
        {
            while (self != null)
            {
                yield return self;
                self = VisualTreeHelper.GetParent(self);
            }
        }

        /// <summary>
        /// Returns an enumeration of the ancestor elements in the visual tree of this element.
        /// </summary>
        /// <param name="self">The starting element.</param>
        /// <returns>The ancestor list.</returns>
        [NotNull, ItemNotNull]
        public static IEnumerable<DependencyObject> VisualAncestors([NotNull] this DependencyObject self)
        {
            return self.VisualAncestorsAndSelf().Skip(1);
        }

        /// <summary>
        /// Returns an enumeration of elements that contain this element, and the ancestors in the logical tree of this element.
        /// </summary>
        /// <param name="self">The starting element.</param>
        /// <returns>The ancestor list.</returns>
        /// <remarks>If the start element is not in the logical tree, this method return elements from the visual tree until the first element from the logical tree is found.</remarks>
        [NotNull, ItemNotNull]
        public static IEnumerable<DependencyObject> AncestorsAndSelf([NotNull] this DependencyObject self)
        {
            while (self != null)
            {
                yield return self;
                self = LogicalTreeHelper.GetParent(self) ?? VisualTreeHelper.GetParent(self);
            }
        }

        /// <summary>
        /// Returns an enumeration of the ancestor elements in the logical tree of this element.
        /// </summary>
        /// <param name="self">The starting element.</param>
        /// <returns>The ancestor list.</returns>
        /// <remarks>If the start element is not in the logical tree, this method return elements from the visual tree until the first element from the logical tree is found.</remarks>
        [NotNull, ItemNotNull]
        public static IEnumerable<DependencyObject> Ancestors([NotNull] this DependencyObject self)
        {
            return self.AncestorsAndSelf().Skip(1);
        }

        /// <summary>
        /// Returns the first element in the ancestor list that implements the type of the type parameter.
        /// </summary>
        /// <typeparam name="T">The type of element to return.</typeparam>
        /// <param name="self">The starting element.</param>
        /// <returns>The first element matching the criteria, or null if no element was found.</returns>
        [CanBeNull]
        public static T TryFindAncestorOrSelf<T>([NotNull] this DependencyObject self)
        {
            return self.AncestorsAndSelf().OfType<T>().FirstOrDefault();
        }

        /// <summary>
        /// Returns the first element in the ancestor list that implements the type of the type parameter.
        /// </summary>
        /// <typeparam name="T">The type of element to return.</typeparam>
        /// <param name="self">The starting element.</param>
        /// <param name="match">The predicate to match.</param>
        /// <returns>The first element matching the criteria, or null if no element was found.</returns>
        [CanBeNull]
        public static T TryFindAncestorOrSelf<T>([NotNull] this DependencyObject self, [NotNull] Func<T, bool> match)
        {
            return self.AncestorsAndSelf().OfType<T>().FirstOrDefault(match);
        }

        /// <summary>
        /// Returns the first element in the ancestor list that implements the type of the type parameter.
        /// </summary>
        /// <typeparam name="T">The type of element to return.</typeparam>
        /// <param name="self">The starting element.</param>
        /// <returns>The first element matching the criteria, or null if no element was found.</returns>
        [CanBeNull]
        public static T TryFindAncestor<T>([NotNull] this DependencyObject self)
        {
            return self.Ancestors().OfType<T>().FirstOrDefault();
        }

        /// <summary>
        /// Returns the first element in the ancestor list that implements the type of the type parameter.
        /// </summary>
        /// <typeparam name="T">The type of element to return.</typeparam>
        /// <param name="self">The starting element.</param>
        /// <param name="match">The predicate to match.</param>
        /// <returns>The first element matching the criteria, or null if no element was found.</returns>
        [CanBeNull]
        public static T TryFindAncestor<T>([NotNull] this DependencyObject self, [NotNull] Func<T, bool> match)
        {
            return self.Ancestors().OfType<T>().FirstOrDefault(match);
        }

        /// <summary>
        /// Enumerates the immediate children of the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The immediate children of the specified item.</returns>
        /// <remarks>
        /// Uses <see cref="VisualTreeHelper.GetChildrenCount"/> and <see cref="VisualTreeHelper.GetChild"/>.
        /// </remarks>
        [NotNull, ItemNotNull]
        public static IEnumerable<DependencyObject> VisualChildren([NotNull] this DependencyObject item)
        {
            var numberOfChildren = VisualTreeHelper.GetChildrenCount(item);
            for (var i = 0; i < numberOfChildren; i++)
            {
                var child = VisualTreeHelper.GetChild(item, i);
                yield return child;
            }
        }

        /// <summary>
        /// Enumerates the specified item and it's immediate children.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The specified item and it's immediate.</returns>
        /// <remarks>
        /// Uses <see cref="VisualTreeHelper.GetChildrenCount"/> and <see cref="VisualTreeHelper.GetChild"/>.
        /// </remarks>
        [NotNull, ItemNotNull]
        public static IEnumerable<DependencyObject> VisualChildrenAndSelf([NotNull] this DependencyObject item)
        {
            yield return item;

            foreach (var x in item.VisualChildren())
            {
                yield return x;
            }
        }

        /// <summary>
        /// Enumerates all visuals descendants of the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The descendants of the item.</returns>
        /// <remarks>
        /// Uses <see cref="VisualTreeHelper.GetChildrenCount"/> and <see cref="VisualTreeHelper.GetChild"/>.
        /// </remarks>
        [NotNull, ItemNotNull]
        public static IEnumerable<DependencyObject> VisualDescendants([NotNull] this DependencyObject item)
        {
            foreach (var child in item.VisualChildren())
            {
                yield return child;

                foreach (var x in child.VisualDescendants())
                {
                    yield return x;
                }
            }
        }

        /// <summary>
        /// Enumerates the specified item and all it's visual descendants.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The specified item and all it's visual descendants.</returns>
        /// <remarks>
        /// Uses <see cref="VisualTreeHelper.GetChildrenCount"/> and <see cref="VisualTreeHelper.GetChild"/>.
        /// </remarks>
        [NotNull, ItemNotNull]
        public static IEnumerable<DependencyObject> VisualDescendantsAndSelf([NotNull] this DependencyObject item)
        {
            yield return item;

            foreach (var x in item.VisualDescendants())
            {
                yield return x;
            }
        }

        /// <summary>
        /// Gets the extent of the thickness when applied to an empty rectangle.
        /// </summary>
        /// <param name="value">The thickness.</param>
        /// <returns>The extent of the thickness.</returns>
        /// <remarks>
        /// Returns a <see cref="Vector"/> because <see cref="Thickness"/> allows negative values.
        /// </remarks>
        public static Vector GetExtent(this Thickness value)
        {
            return new Vector(value.Left + value.Right, value.Top + value.Bottom);
        }
    }
}