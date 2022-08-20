namespace TomsToolbox.Essentials;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

/// <summary>
/// Replacements for some common methods with extra nullable annotations.
/// </summary>
public static class NullableExtensions
{
    /// <summary>Indicates whether the specified string is <see langword="null" /> or an empty string ("").</summary>
    /// <param name="value">The string to test.</param>
    /// <returns>
    /// <see langword="true" /> if the <paramref name="value" /> parameter is <see langword="null" /> or an empty string (""); otherwise, <see langword="false" />.
    /// </returns>
    public static bool IsNullOrEmpty([NotNullWhen(false)] this string? value)
    {
        return string.IsNullOrEmpty(value);
    }

    /// <summary>Indicates whether a specified string is <see langword="null" />, empty, or consists only of white-space characters.</summary>
    /// <param name="value">The string to test.</param>
    /// <returns>
    /// <see langword="true" /> if the <paramref name="value" /> parameter is <see langword="null" /> or <see cref="F:System.String.Empty" />, or if <paramref name="value" /> consists exclusively of white-space characters.
    /// </returns>
    public static bool IsNullOrWhiteSpace([NotNullWhen(false)] this string? value)
    {
        return string.IsNullOrWhiteSpace(value);
    }

    /// <summary>
    /// Filters a sequence of values based on their nullness.
    /// </summary>
    /// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to filter.</param>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
    /// <returns>
    /// An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains all elements from the input sequence that are not null.
    /// </returns>
    public static IEnumerable<TSource> ExceptNullItems<TSource>(this IEnumerable<TSource?> source) where TSource : class
    {
        return source.Where(i => i != null)!;
    }
}