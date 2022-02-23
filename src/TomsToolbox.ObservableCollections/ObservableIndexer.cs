namespace TomsToolbox.ObservableCollections
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    /// <summary>
    /// A Dictionary like implementation that populates it's content on demand, i.e. calling indexer[key] will never return null.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public sealed class ObservableIndexer<TKey, TValue> : ReadOnlyObservableCollectionAdapter<KeyValuePair<TKey, TValue>, ObservableCollection<KeyValuePair<TKey, TValue>>>
    {
        private readonly Func<TKey, TValue> _generator;
        private Dictionary<TKey, int> _index;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableIndexer{TKey, TValue}"/> class.
        /// </summary>
        /// <param name="generator">The generator.</param>
        public ObservableIndexer(Func<TKey, TValue> generator)
            : this(generator, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableIndexer{TKey, TValue}"/> class.
        /// </summary>
        /// <param name="generator">The generator.</param>
        /// <param name="comparer">The comparer.</param>
        public ObservableIndexer(Func<TKey, TValue> generator, IEqualityComparer<TKey>? comparer)
            : base(new ObservableCollection<KeyValuePair<TKey, TValue>>())
        {
            _generator = generator;
            _index = new Dictionary<TKey, int>(comparer);
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
        /// <exception cref="System.ArgumentNullException"><paramref name="key" /> is null.</exception>
        public TValue this[TKey key]
        {
            get
            {
                TValue value;

                if (_index.TryGetValue(key, out var index))
                {
                    value = Items[index].Value;
                }
                else
                {
                    value = _generator(key);
                    if (ReferenceEquals(value, null))
                        throw new InvalidOperationException("The generator did not generate a valid item.");

                    index = Items.Count;
                    _index.Add(key, index);
                    Items.Add(new KeyValuePair<TKey, TValue>(key, value));
                }

                return value;
            }
            set
            {
                if (_index.TryGetValue(key, out var index))
                {
                    Items[index] = new KeyValuePair<TKey, TValue>(key, value);
                }
                else
                {
                    index = Items.Count;
                    _index.Add(key, index);
                    Items.Add(new KeyValuePair<TKey, TValue>(key, value));
                }
            }
        }

        /// <summary>
        /// Gets the <see cref="T:System.Collections.Generic.IEqualityComparer`1"/> that is used to determine equality of keys for the dictionary.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Collections.Generic.IEqualityComparer`1"/> generic interface implementation that is used to determine equality of keys for the current <see cref="T:System.Collections.Generic.Dictionary`2"/> and to provide hash values for the keys.
        /// </returns>
        public IEqualityComparer<TKey> Comparer => _index.Comparer;

        /// <summary>
        /// Removes the value with the specified key from the <see cref="T:System.Collections.Generic.Dictionary`2"/>.
        /// </summary>
        /// <returns>
        /// true if the element is successfully found and removed; otherwise, false.  This method returns false if <paramref name="key"/> is not found in the <see cref="T:System.Collections.Generic.Dictionary`2"/>.
        /// </returns>
        /// <param name="key">The key of the element to remove.</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception>
        public bool Remove(TKey key)
        {
            if (!_index.TryGetValue(key, out var index))
                return false;

            // Remove will fire an event, index should be updated first to ensure code is re-entrant.
            _index = Items
                .Where(item => !Equals(key, item.Key))
                .Select((item, i) => new { item.Key, i })
                .ToDictionary(x => x.Key, x => x.i, _index.Comparer);


            Items.RemoveAt(index);

            return true;
        }

        /// <summary>
        /// Removes all keys and values from the <see cref="T:System.Collections.Generic.Dictionary`2"/>.
        /// </summary>
        public void Clear()
        {
            _index = new Dictionary<TKey, int>(_index.Comparer);
            Items.Clear();
        }
    }
}
