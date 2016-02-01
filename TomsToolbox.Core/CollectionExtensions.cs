namespace TomsToolbox.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    /// <summary>
    /// Extensions methods to ease dealing with collections.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Synchronizes the items of the source list with the items of the target list. The order of the items is ignored.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="target">The list to synchronize.</param>
        /// <param name="source">The items that should be in the target list.</param>
        public static void SynchronizeWith<T>(this ICollection<T> target, ICollection<T> source)
        {
            Contract.Requires(target != null);
            Contract.Requires(source != null);

            SynchronizeWith(target, source, null);
        }

        /// <summary>
        /// Synchronizes the items of the source list with the items of the target list. The order of the items is ignored.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="target">The list to synchronize.</param>
        /// <param name="source">The items that should be in the target list.</param>
        /// <param name="comparer">The comparer used to compare the items. If comparer is <c>null</c>, the default equality comparer is used to compare values.</param>
        public static void SynchronizeWith<T>(this ICollection<T> target, ICollection<T> source, IEqualityComparer<T> comparer)
        {
            Contract.Requires(target != null);
            Contract.Requires(source != null);

            var removedItems = target.Except(source, comparer).ToArray();
            var addedItems = source.Except(target, comparer).ToArray();

            target.RemoveRange(removedItems);
            target.AddRange(addedItems);
        }

        /// <summary>
        /// Adds the elements of the specified collection to the end of the list.
        /// </summary>
        /// <param name="target">The target list.</param>
        /// <param name="items">The collection whose elements should be added to the end of the list. The collection itself cannot be null, but it can contain elements that are null, if type T is a reference type.</param>
        public static void AddRange(this IList target, IEnumerable items)
        {
            Contract.Requires(target != null);
            Contract.Requires(items != null);

            foreach (var i in items)
            {
                target.Add(i);
            }
        }

        /// <summary>
        /// Adds the elements of the specified collection to the end of the list.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="target">The target list.</param>
        /// <param name="firstItem">The first item to add.</param>
        /// <param name="secondItem">The second item to add.</param>
        /// <param name="moreItems">Any more items to add.</param>
        public static void AddRange<T>(this ICollection<T> target, T firstItem, T secondItem, params T[] moreItems)
        {
            Contract.Requires(target != null);
            Contract.Requires(moreItems != null);

            target.Add(firstItem);
            target.Add(secondItem);
            AddRange(target, moreItems);
        }

        /// <summary>
        /// Adds the elements of the specified collection to the end of the list.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="target">The target list.</param>
        /// <param name="items">The collection whose elements should be added to the end of the list. The collection itself cannot be null, but it can contain elements that are null, if type T is a reference type.</param>
        public static void AddRange<T>(this ICollection<T> target, IEnumerable<T> items)
        {
            Contract.Requires(target != null);
            Contract.Requires(items != null);

            foreach (var i in items)
            {
                target.Add(i);
            }
        }

        /// <summary>
        /// Adds the elements of the specified collection to the end of the list, but ignores all <see cref="ArgumentException"/>, e.g. when trying to add duplicate keys to a dictionary.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="target">The target list.</param>
        /// <param name="items">The collection whose elements should be added to the end of the list. The collection itself cannot be null, but it can contain elements that are null, if type T is a reference type.</param>
        public static void TryAddRange<T>(this ICollection<T> target, IEnumerable<T> items)
        {
            Contract.Requires(target != null);
            Contract.Requires(items != null);

            foreach (var i in items)
            {
                try
                {
                    target.Add(i);
                }
                catch (ArgumentException)
                {
                }
            }
        }

        /// <summary>
        /// Removes a range of elements from the list.
        /// </summary>
        /// <param name="target">The target list.</param>
        /// <param name="items">The items to remove.</param>
        public static void RemoveRange(this IList target, IEnumerable items)
        {
            Contract.Requires(target != null);
            Contract.Requires(items != null);

            foreach (var i in items)
            {
                target.Remove(i);
            }
        }

        /// <summary>
        /// Removes a range of elements from the list.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="target">The target.</param>
        /// <param name="items">The items to remove.</param>
        public static void RemoveRange<T>(this ICollection<T> target, IEnumerable<T> items)
        {
            Contract.Requires(target != null);
            Contract.Requires(items != null);

            foreach (var i in items)
            {
                target.Remove(i);
            }
        }

        /// <summary>
        /// Removes the range of elements from the list that fulfill the condition.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="target">The target.</param>
        /// <param name="condition">The condition; all items that fulfill this condition will be removed.</param>
        public static void RemoveRange<T>(this ICollection<T> target, Func<T, bool> condition)
        {
            Contract.Requires(target != null);
            Contract.Requires(condition != null);

            target.RemoveRange(target.Where(condition).ToArray());
        }

        /// <summary>
        /// Retrieves the specified number of items from the source. If source contains less items than specified, all available items are returned.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="source">The source enumerator to retrieve the items from. The enumerator position will be incremented by the number of items returned.</param>
        /// <param name="numberOfItems">The number of items to retrieve.</param>
        /// <returns>A list that contains up to n items from the source.</returns>
        public static IList<T> Take<T>(this IEnumerator<T> source, int numberOfItems)
        {
            Contract.Requires(source != null);
            Contract.Requires(numberOfItems >= 0);
            Contract.Ensures(Contract.Result<IList<T>>() != null);

            var result = new List<T>(numberOfItems);

            while ((numberOfItems > 0) && (source.MoveNext()))
            {
                result.Add(source.Current);
                numberOfItems -= 1;
            }

            return result;
        }

        /// <summary>
        /// Shortcut to test if any of the given characters is contained in the specified string.
        /// </summary>
        /// <param name="self">The string to analyze self.</param>
        /// <param name="characters">The characters to test for.</param>
        /// <returns><c>true</c> if any of the characters is contained in the specified string; otherwise <c>false</c>.</returns>
        public static bool ContainsAny(this string self, params char[] characters)
        {
            Contract.Requires(self != null);
            Contract.Requires(characters != null);

            return self.IndexOfAny(characters) >= 0;
        }

        /// <summary>
        /// Shortcut to test if any of the given items are contained in the specified object.
        /// </summary>
        /// <typeparam name="T">The type of objects.</typeparam>
        /// <param name="self">The object to analyze.</param>
        /// <param name="items">The items to test for.</param>
        /// <returns><c>true</c> if any of the items is contained in the specified object; otherwise <c>false</c>.</returns>
        public static bool ContainsAny<T>(this IEnumerable<T> self, params T[] items)
        {
            Contract.Requires(self != null);
            Contract.Requires(items != null);

            return items.Any(self.Contains);
        }

        /// <summary>
        /// Shortcut to test if any of the given items are contained in the specified object.
        /// </summary>
        /// <typeparam name="T">The type of objects.</typeparam>
        /// <param name="self">The object to analyze.</param>
        /// <param name="items">The items to test for.</param>
        /// <param name="comparer">The comparer to compare the individual items.</param>
        /// <returns><c>true</c> if any of the items is contained in the specified object; otherwise <c>false</c>.</returns>
        public static bool ContainsAny<T>(this IEnumerable<T> self, IEqualityComparer<T> comparer, params T[] items)
        {
            Contract.Requires(self != null);
            Contract.Requires(items != null);

            return items.Any(item => self.Contains(item, comparer));
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first occurrence within all items.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="collection">The collection to search.</param>
        /// <param name="item">The object to locate. The value can be null for reference types.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of <paramref name="item" />, if found; otherwise, –1.
        /// </returns>
        public static int IndexOf<T>(this IEnumerable<T> collection, T item)
        {
            Contract.Requires(collection != null);

            var index = 0;

            foreach (var element in collection)
            {
                if (Equals(element, item))
                    return index;

                index += 1;
            }

            return -1;
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first occurrence within all items.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="collection">The collection to search.</param>
        /// <param name="item">The object to locate. The value can be null for reference types.</param>
        /// <param name="comparer">The comparer.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of <paramref name="item" />, if found; otherwise, –1.
        /// </returns>
        public static int IndexOf<T>(this IEnumerable<T> collection, T item, IEqualityComparer<T> comparer)
        {
            Contract.Requires(collection != null);
            Contract.Requires(comparer != null);

            var index = 0;

            foreach (var element in collection)
            {
                if (comparer.Equals(element, item))
                    return index;

                index += 1;
            }

            return -1;
        }

        /// <summary>
        /// Performs the specified action on each element of the collection.
        /// </summary>
        /// <typeparam name="T">The type of elements in the collection.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="action">The action.</param>
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            Contract.Requires(collection != null);
            Contract.Requires(action != null);

            foreach (var item in collection)
            {
                action(item);
            }
        }

        /// <summary>
        /// Performs the specified action on each element of the collection, providing also the index of the item.
        /// </summary>
        /// <typeparam name="T">The type of elements in the collection.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="action">The action.</param>
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T, int> action)
        {
            Contract.Requires(collection != null);
            Contract.Requires(action != null);

            foreach (var i in collection.Select((item, index) => new { item, index }))
            {
                Contract.Assume(i != null);

                action(i.item, i.index);
            }
        }

        /// <summary>
        /// Transposes the specified items, i.e. exchanges key and value.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="items">The items.</param>
        /// <returns>The transposed items.</returns>
        public static IEnumerable<KeyValuePair<TValue, TKey>> Transpose<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> items)
        {
            Contract.Requires(items != null);
            Contract.Ensures(Contract.Result<IEnumerable<KeyValuePair<TValue, TKey>>>() != null);

            return items.Select(item => new KeyValuePair<TValue, TKey>(item.Value, item.Key));
        }

        /// <summary>
        /// Gets the value from the dictionary, or the default value if no item with the specified key exists.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key to lookup.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>
        /// The value from the dictionary, or the default value if no item with the specified key exists.
        /// </returns>
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
        {
            Contract.Requires(dictionary != null);
            Contract.Requires(!ReferenceEquals(key, null));

            TValue value;

            return dictionary.TryGetValue(key, out value) ? value : defaultValue;
        }

        /// <summary>
        /// Gets the value from the dictionary, or the default value of <typeparamref name="TValue"/> if no item with the specified key exists.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key to lookup.</param>
        /// <returns>
        /// The value from the dictionary, or the default value of <typeparamref name="TValue"/> if no item with the specified key exists.
        /// </returns>
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            Contract.Requires(dictionary != null);
            Contract.Requires(!ReferenceEquals(key, null));

            return dictionary.GetValueOrDefault(key, default(TValue));
        }

        /// <summary>
        /// Gets the value associated with the specified key from the <paramref name="dictionary"/>, or creates a new entry if the dictionary does not contain a value associated with the key.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <param name="generator">The generator function called when a new value needs to be created.</param>
        /// <returns>The element with the specified key.</returns>
        public static TValue ForceValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> generator)
        {
            Contract.Requires(dictionary != null);
            Contract.Requires(!ReferenceEquals(key, null));
            Contract.Requires(generator != null);

            TValue result;

            if (dictionary.TryGetValue(key, out result))
                return result;

            result = generator(key);

            dictionary.Add(key, result);

            return result;
        }

        /// <summary>
        /// Gets the value associated with the specified key from the <paramref name="dictionary" />, or creates a new entry if the dictionary does not contain a value associated with the key.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The value that will be added to the dictionary if the dictionary does not contain a value associated with the key.</param>
        /// <returns> The element with the specified key.</returns>
        public static TValue ForceValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
        {
            Contract.Requires(dictionary != null);
            Contract.Requires(!ReferenceEquals(key, null));

            return ForceValue(dictionary, key, _ => defaultValue);
        }

        /// <summary>
        /// Creates an array from a <see cref=" System.Collections.Generic.ICollection{TSource}"/>.
        /// This method is faster than using Enumerable.Select(selector).ToArray() because the size of the target array is known in advance.
        /// </summary>
        /// <typeparam name="TSource">The type of the items in the source collection.</typeparam>
        /// <typeparam name="TTarget">The type of the items in the result collection.</typeparam>
        /// <param name="items"> A System.Collections.Generic.ICollection{TSource} to create an array from.</param>
        /// <param name="selector">The selector to select the elements in the array.</param>
        /// <returns>An array that contains the selected elements from the input sequence.</returns>
        public static TTarget[] ToArray<TSource, TTarget>(this ICollection<TSource> items, Func<TSource, TTarget> selector)
        {
            Contract.Requires(items != null);
            Contract.Requires(selector != null);
            Contract.Ensures(Contract.Result<TTarget[]>() != null);

            var count = items.Count;

            var result = new TTarget[count];

            var i = 0;

            foreach (var item in items)
            {
                Contract.Assume(i < result.Length); // because result.Length == items.Count
                result[i++] = selector(item);
            }

            return result;
        }


        /// <summary>
        /// Repeats the specified source multiple times.
        /// </summary>
        /// <typeparam name="T">The type of the items in the source collection.</typeparam>
        /// <param name="source">The collection to be repeated.</param>
        /// <param name="count">The number of times to repeat the source sequence.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> that contains the repeated source sequence.</returns>
        public static IEnumerable<T> Repeat<T>(ICollection<T> source, int count)
        {
            Contract.Requires(source != null);

            for (var i = 0; i < count; i++)
            {
                foreach (var item in source)
                {
                    yield return item;
                }
            }
        }
    }
}
