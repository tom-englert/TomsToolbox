namespace TomsToolbox.Essentials;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// A thread safe, <see cref="Dictionary{TKey,TValue}"/> like implementation that populates it's content on demand, i.e. calling indexer[key] will never return null.
/// The cache has only weak references to the values, so the values may come and go.
/// </summary>
/// <typeparam name="TKey">The type of the key.</typeparam>
/// <typeparam name="TValue">The type of the value.</typeparam>
/// <remarks>
/// This implementation is thread safe; the draw back is that generating new items is slow, so this type is not suitable for caching a large amount of items.
/// </remarks>
public sealed class AutoWeakIndexer<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    where TKey : notnull
    where TValue : class
{
    private readonly object _sync = new();
    private readonly Func<TKey, TValue> _generator;
    private Dictionary<TKey, WeakReference<TValue>> _items;

    /// <summary>
    /// Initializes a new instance of the <see cref="AutoWeakIndexer{TKey, TValue}"/> class.
    /// </summary>
    /// <param name="generator">The generator.</param>
    /// <param name="comparer">The comparer.</param>
    public AutoWeakIndexer(Func<TKey, TValue> generator, IEqualityComparer<TKey>?  comparer = null)
    {
        _generator = generator;
        _items = new Dictionary<TKey, WeakReference<TValue>>(comparer);
    }

    /// <summary>
    /// Gets the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the value to get or set.</param>
    /// <returns>
    /// The value associated with the specified key. If the specified key is not found, or the value at the key is null,
    /// the item generator is called to create a new element with the specified key.
    /// </returns>
    /// <exception cref="System.InvalidOperationException">The generator did not generate a valid item.</exception>
    public TValue this[TKey key]
    {
        get
        {
            var items1 = _items;
            if (items1.TryGetValue(key, out var value) && value.TryGetTarget(out var target) && (target != null))
                return target;

            lock (_sync)
            {
                var items2 = _items;

                if (!ReferenceEquals(items2, items1) && items2.TryGetValue(key, out value) && value.TryGetTarget(out target))
                    return target;

                target = _generator(key);
                if (target == null)
                    throw new InvalidOperationException("The generator did not generate a valid item.");

                var newItems = new Dictionary<TKey, WeakReference<TValue>>(items2.Comparer);
                newItems.AddRange(items2.Where(item => item.Value.TryGetTarget(out _)));
                newItems[key] = new WeakReference<TValue>(target);

                _items = newItems;

                return target;
            }
        }
    }

    /// <summary>
    /// Gets a collection containing the values in the <see cref="AutoWeakIndexer{TKey, TValue}"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="ICollection{TValue}"/> containing the values in the <see cref="AutoWeakIndexer{TKey, TValue}"/>.
    /// </returns>
    public ICollection<TValue> Values
    {
        get
        {
            static TValue? GetTargetOrDefault(WeakReference<TValue>? item)
            {
                if (item == null)
                    return default;

                return item.TryGetTarget(out var value) ? value : null;
            }

            return _items.Values.Select(GetTargetOrDefault).ExceptNullItems().ToArray();
        }
    }

    /// <summary>
    /// Gets a collection containing the keys in the <see cref="AutoWeakIndexer{TKey, TValue}"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="ICollection{TKey}"/> containing the keys in the <see cref="AutoWeakIndexer{TKey, TValue}"/>.
    /// </returns>
    public ICollection<TKey> Keys
    {
        get
        {
            return _items
                .Where(item => item.Value.TryGetTarget(out _))
                .Select(item => item.Key)
                .ToList()
                .AsReadOnly();
        }
    }

    /// <summary>
    /// Gets the <see cref="T:System.Collections.Generic.IEqualityComparer`1"/> that is used to determine equality of keys for the dictionary.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.Collections.Generic.IEqualityComparer`1"/> generic interface implementation that is used to determine equality of keys for the current <see cref="AutoWeakIndexer{TKey, TValue}"/> and to provide hash values for the keys.
    /// </returns>
    public IEqualityComparer<TKey> Comparer => _items.Comparer;

    /// <summary>
    /// Gets the value associated with the specified key.
    /// </summary>
    /// <returns>
    /// true if the <see cref="AutoWeakIndexer{TKey, TValue}"/> contains an element with the specified key; otherwise, false.
    /// </returns>
    /// <param name="key">The key of the value to get.</param>
    /// <param name="value">When this method returns, contains the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value"/> parameter. This parameter is passed uninitialized.</param>
    public bool TryGetValue(TKey key, out TValue? value)
    {
        value = default;

        return _items.TryGetValue(key, out var reference) && (reference != null) && reference.TryGetTarget(out value);
    }

    /// <summary>
    /// Returns an enumerator that iterates through the items of the <see cref="AutoWeakIndexer{TKey, TValue}"/>.
    /// </summary>
    /// <returns>
    /// An enumerator for the <see cref="AutoWeakIndexer{TKey, TValue}"/>.
    /// </returns>
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        var inner = _items;

        foreach (var item in inner)
        {
            var value = default(TValue);

            if (item.Value?.TryGetTarget(out value) == true && value != null)
            {
                yield return new KeyValuePair<TKey, TValue>(item.Key, value);
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <summary>
    /// Determines whether the <see cref="AutoWeakIndexer{TKey, TValue}"/> contains the specified key.
    /// </summary>
    /// <returns>
    /// true if the <see cref="AutoWeakIndexer{TKey, TValue}"/> contains an element with the specified key; otherwise, false.
    /// </returns>
    /// <param name="key">The key to locate in the <see cref="AutoWeakIndexer{TKey, TValue}"/>.</param>
    public bool ContainsKey(TKey key)
    {
        return _items.TryGetValue(key, out var reference) && (reference != null) && reference.TryGetTarget(out _);
    }

    /// <summary>
    /// Removes all keys and values from the <see cref="AutoWeakIndexer{TKey, TValue}"/>.
    /// </summary>
    public void Clear()
    {
        _items = new Dictionary<TKey, WeakReference<TValue>>(_items.Comparer);
    }
}
