namespace TomsToolbox.Essentials;

using System;
using System.Collections.Generic;
using System.Reflection;

/// <inheritdoc />
/// <summary>
/// <see cref="T:System.Collections.Generic.IComparer`1" /> implementation using a delegate function to compare the values.
/// </summary>
public class DelegateComparer<T> : IComparer<T>
{
    private readonly Func<T?, T?, int> _comparer;

    /// <summary>
    /// Initializes a new instance of the <see cref="DelegateComparer{T}"/> class.
    /// </summary>
    /// <param name="comparer">The comparer.</param>
    public DelegateComparer(Func<T?, T?, int> comparer)
    {
        _comparer = comparer;
    }

    /// <inheritdoc />
    public int Compare(T? x, T? y)
    {
        if (!typeof(T).GetTypeInfo().IsValueType)
        {
            if (x is null)
                return y is null ? 0 : -1;

            if (y is null)
                return 1;
        }

        return _comparer(x, y);
    }
}