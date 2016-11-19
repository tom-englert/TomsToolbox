namespace TomsToolbox.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;

    using JetBrains.Annotations;

    /// <summary>
    /// A Dictionary like implementation that populates it's content on demand, i.e. calling indexer[key] will never return null.
    /// The indexer has only weak references to the values, so the values may come and go.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public sealed class AutoWeakIndexer<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
        where TValue : class
    {
        [NotNull]
        private readonly object _sync = new object();
        [NotNull]
        private readonly Func<TKey, TValue> _generator;

        [NotNull]
        private Dictionary<TKey, WeakReference<TValue>> _items;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoWeakIndexer{TKey, TValue}"/> class.
        /// </summary>
        /// <param name="generator">The generator.</param>
        public AutoWeakIndexer([NotNull] Func<TKey, TValue> generator)
            : this(generator, null)
        {
            Contract.Requires(generator != null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoWeakIndexer{TKey, TValue}"/> class.
        /// </summary>
        /// <param name="generator">The generator.</param>
        /// <param name="comparer">The comparer.</param>
        public AutoWeakIndexer([NotNull] Func<TKey, TValue> generator, IEqualityComparer<TKey> comparer)
        {
            Contract.Requires(generator != null);

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
        [NotNull]
        public TValue this[[NotNull] TKey key]
        {
            get
            {
                Contract.Requires(!ReferenceEquals(key, null));
                Contract.Ensures(Contract.Result<TValue>() != null);

                TValue target;
                WeakReference<TValue> value;
                var items1 = _items;
                if (items1.TryGetValue(key, out value) && (value != null) && (value.TryGetTarget(out target)))
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
                    newItems.AddRange(items2.Where(item => item.Value.IsAlive));
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
        [ItemNotNull]
        [NotNull]
        public ICollection<TValue> Values
        {
            get
            {
                Contract.Ensures(Contract.Result<ICollection<TValue> >() != null);

                return _items.Values.Select(item => item?.Target).Where(item => item != null).ToArray();
            }
        }

        /// <summary>
        /// Gets a collection containing the keys in the <see cref="AutoWeakIndexer{TKey, TValue}"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="ICollection{TKey}"/> containing the keys in the <see cref="AutoWeakIndexer{TKey, TValue}"/>.
        /// </returns>
        [ItemNotNull]
        [NotNull]
        public ICollection<TKey> Keys
        {
            get
            {
                Contract.Ensures(Contract.Result<ICollection<TKey>>() != null);

                return _items.Where(item => item.Value.IsAlive).Select(item => item.Key).ToArray();
            }
        }

        /// <summary>
        /// Gets the <see cref="T:System.Collections.Generic.IEqualityComparer`1"/> that is used to determine equality of keys for the dictionary.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Collections.Generic.IEqualityComparer`1"/> generic interface implementation that is used to determine equality of keys for the current <see cref="AutoWeakIndexer{TKey, TValue}"/> and to provide hash values for the keys.
        /// </returns>
        [NotNull]
        public IEqualityComparer<TKey> Comparer
        {
            get
            {
                Contract.Ensures(Contract.Result<IEqualityComparer<TKey>>() != null);

                return _items.Comparer;
            }
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <returns>
        /// true if the <see cref="AutoWeakIndexer{TKey, TValue}"/> contains an element with the specified key; otherwise, false.
        /// </returns>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="value">When this method returns, contains the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value"/> parameter. This parameter is passed uninitialized.</param>
        public bool TryGetValue([NotNull] TKey key, out TValue value)
        {
            Contract.Requires(!ReferenceEquals(key, null));

            WeakReference<TValue> reference;
            value = default(TValue);

            return _items.TryGetValue(key, out reference) && (reference != null) && reference.TryGetTarget(out value);
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

                if (item.Value?.TryGetTarget(out value) == true)
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
        public bool ContainsKey([NotNull] TKey key)
        {
            Contract.Requires(!ReferenceEquals(key, null));

            WeakReference<TValue> reference;
            return _items.TryGetValue(key, out reference) && (reference != null) &&  reference.IsAlive;
        }

        /// <summary>
        /// Removes all keys and values from the <see cref="AutoWeakIndexer{TKey, TValue}"/>.
        /// </summary>
        public void Clear()
        {
            _items = new Dictionary<TKey, WeakReference<TValue>>(_items.Comparer);
        }

        [ContractInvariantMethod]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        [Conditional("CONTRACTS_FULL")]
        private void ObjectInvariant()
        {
            Contract.Invariant(_generator != null);
            Contract.Invariant(_items != null);
        }
    }
}
