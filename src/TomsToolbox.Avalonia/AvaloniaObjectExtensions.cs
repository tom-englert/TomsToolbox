namespace TomsToolbox.Wpf;

using System;
using System.Collections.Generic;

using Avalonia;
using Avalonia.VisualTree;

using TomsToolbox.Essentials;

/// <summary>
/// Extensions methods to ease dealing with avalonia objects.
/// </summary>
public static class AvaloniaObjectExtensions
{
    /// <summary>
    /// Gets the value of a dependency property using <see cref="ObjectExtensions.SafeCast{T}(object)" />.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="self">The dependency object from which to get the value.</param>
    /// <param name="property">The property to get.</param>
    /// <returns>The value safely cast to <typeparamref name="T"/></returns>
    public static T? GetValue<T>(this AvaloniaObject self, AvaloniaProperty property)
    {
        return self.GetValue(property).SafeCast<T>();
    }

    /// <summary>
    /// Returns an enumeration of elements that contains this element, and the ancestors in the visual tree of this element.
    /// </summary>
    /// <param name="self">The starting element.</param>
    /// <returns>The ancestor list.</returns>
    public static IEnumerable<AvaloniaObject> VisualAncestorsAndSelf(this AvaloniaObject self)
    {
        AvaloniaObject? target = self;
        while (target != null)
        {
            yield return target;
            target = (target as Visual)?.GetVisualParent();
        }
    }

    /// <summary>
    /// Returns an enumeration of the ancestor elements in the visual tree of this element.
    /// </summary>
    /// <param name="self">The starting element.</param>
    /// <returns>The ancestor list.</returns>
    public static IEnumerable<AvaloniaObject> VisualAncestors(this AvaloniaObject self)
    {
        return self.VisualAncestorsAndSelf().Skip(1);
    }

    /// <summary>
    /// Returns an enumeration of elements that contain this element, and the ancestors in the logical tree of this element.
    /// </summary>
    /// <param name="self">The starting element.</param>
    /// <returns>The ancestor list.</returns>
    /// <remarks>If the start element is not in the logical tree, this method return elements from the visual tree until the first element from the logical tree is found.</remarks>
    public static IEnumerable<AvaloniaObject> AncestorsAndSelf(this AvaloniaObject self)
    {
        AvaloniaObject? target = self;
        while (target != null)
        {
            yield return target;
            target = (target as Visual)?.GetVisualParent();
        }
    }

    /// <summary>
    /// Returns an enumeration of the ancestor elements in the logical tree of this element.
    /// </summary>
    /// <param name="self">The starting element.</param>
    /// <returns>The ancestor list.</returns>
    /// <remarks>If the start element is not in the logical tree, this method return elements from the visual tree until the first element from the logical tree is found.</remarks>
    public static IEnumerable<AvaloniaObject> Ancestors(this AvaloniaObject self)
    {
        return self.AncestorsAndSelf().Skip(1);
    }

    /// <summary>
    /// Returns the first element in the ancestor list that implements the type of the type parameter.
    /// </summary>
    /// <typeparam name="T">The type of element to return.</typeparam>
    /// <param name="self">The starting element.</param>
    /// <returns>The first element matching the criteria, or null if no element was found.</returns>
    public static T? TryFindAncestorOrSelf<T>(this AvaloniaObject self) where T : class
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
    public static T? TryFindAncestorOrSelf<T>(this AvaloniaObject self, Func<T, bool> match) where T : class
    {
        return self.AncestorsAndSelf().OfType<T>().FirstOrDefault(match);
    }

    /// <summary>
    /// Returns the first element in the ancestor list that implements the type of the type parameter.
    /// </summary>
    /// <typeparam name="T">The type of element to return.</typeparam>
    /// <param name="self">The starting element.</param>
    /// <returns>The first element matching the criteria, or null if no element was found.</returns>
    public static T? TryFindAncestor<T>(this AvaloniaObject self) where T : class
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
    public static T? TryFindAncestor<T>(this AvaloniaObject self, Func<T, bool> match) where T : class
    {
        return self.Ancestors().OfType<T>().FirstOrDefault(match);
    }

    /// <summary>
    /// Enumerates the immediate children of the specified item.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>The immediate children of the specified item.</returns>
    public static IEnumerable<AvaloniaObject> VisualChildren(this AvaloniaObject item)
    {
        return (item as Visual)?.GetVisualChildren() ?? [];
    }

    /// <summary>
    /// Enumerates the specified item and it's immediate children.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>The specified item and it's immediate.</returns>
    public static IEnumerable<AvaloniaObject> VisualChildrenAndSelf(this AvaloniaObject item)
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
    public static IEnumerable<AvaloniaObject> VisualDescendants(this AvaloniaObject item)
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
    public static IEnumerable<AvaloniaObject> VisualDescendantsAndSelf(this AvaloniaObject item)
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
