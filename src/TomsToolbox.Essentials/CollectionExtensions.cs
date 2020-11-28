namespace TomsToolbox.Essentials
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using JetBrains.Annotations;
    using NotNullAttribute = JetBrains.Annotations.NotNullAttribute;

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
        public static void SynchronizeWith<T>([NotNull, ItemCanBeNull] this ICollection<T> target, [NotNull, ItemCanBeNull] ICollection<T> source)
        {
            SynchronizeWith(target, source, null);
        }

        /// <summary>
        /// Synchronizes the items of the source list with the items of the target list. The order of the items is ignored.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="target">The list to synchronize.</param>
        /// <param name="source">The items that should be in the target list.</param>
        /// <param name="comparer">The comparer used to compare the items. If comparer is <c>null</c>, the default equality comparer is used to compare values.</param>
        public static void SynchronizeWith<T>([NotNull, ItemCanBeNull] this ICollection<T> target, [NotNull, ItemCanBeNull] ICollection<T> source, [CanBeNull] IEqualityComparer<T>? comparer)
        {
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
        public static void AddRange([NotNull, ItemCanBeNull] this IList target, [NotNull, ItemCanBeNull] IEnumerable items)
        {
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
        public static void AddRange<T>([NotNull, ItemCanBeNull] this ICollection<T> target, [CanBeNull] T firstItem, [CanBeNull] T secondItem, [NotNull, ItemCanBeNull] params T[] moreItems)
        {
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
        public static void AddRange<T>([NotNull, ItemCanBeNull] this ICollection<T> target, [NotNull, ItemCanBeNull] IEnumerable<T> items)
        {
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
        public static void TryAddRange<T>([NotNull, ItemCanBeNull] this ICollection<T> target, [NotNull, ItemCanBeNull] IEnumerable<T> items)
        {
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
        public static void RemoveRange([NotNull, ItemCanBeNull] this IList target, [NotNull, ItemCanBeNull] IEnumerable items)
        {
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
        public static void RemoveRange<T>([NotNull, ItemCanBeNull] this ICollection<T> target, [NotNull, ItemCanBeNull] IEnumerable<T> items)
        {
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
        public static void RemoveWhere<T>([NotNull, ItemCanBeNull] this ICollection<T> target, [NotNull] Func<T, bool> condition)
        {
            target.RemoveRange(target.Where(condition).ToList());
        }

        /// <summary>
        /// Retrieves the specified number of items from the source. If source contains less items than specified, all available items are returned.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="source">The source enumerator to retrieve the items from. The enumerator position will be incremented by the number of items returned.</param>
        /// <param name="numberOfItems">The number of items to retrieve.</param>
        /// <returns>A list that contains up to n items from the source.</returns>
        [NotNull, ItemCanBeNull]
        public static IList<T> Take<T>([NotNull] this IEnumerator<T> source, int numberOfItems)
        {
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
        public static bool ContainsAny([NotNull] this string self, [NotNull] params char[] characters)
        {
            return self.IndexOfAny(characters) >= 0;
        }

        /// <summary>
        /// Shortcut to test if any of the given items are contained in the specified object.
        /// </summary>
        /// <typeparam name="T">The type of objects.</typeparam>
        /// <param name="self">The object to analyze.</param>
        /// <param name="items">The items to test for.</param>
        /// <returns><c>true</c> if any of the items is contained in the specified object; otherwise <c>false</c>.</returns>
        public static bool ContainsAny<T>([NotNull, ItemCanBeNull] this IEnumerable<T> self, [NotNull, ItemCanBeNull] params T[] items)
        {
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
        public static bool ContainsAny<T>([NotNull, ItemCanBeNull] this IEnumerable<T> self, [CanBeNull] IEqualityComparer<T>? comparer, [NotNull, ItemCanBeNull] params T[] items)
        {
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
        public static int IndexOf<T>([NotNull, ItemCanBeNull] this IEnumerable<T> collection, [CanBeNull] T item)
        {
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
        public static int IndexOf<T>([NotNull, ItemCanBeNull] this IEnumerable<T> collection, [CanBeNull] T item, [CanBeNull] IEqualityComparer<T>? comparer)
        {
            var comp = comparer ?? EqualityComparer<T>.Default;

            var index = 0;

            foreach (var element in collection)
            {
                if (comp.Equals(element, item))
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
        public static void ForEach<T>([NotNull, ItemCanBeNull] this IEnumerable<T> collection, [NotNull] Action<T> action)
        {
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
        public static void ForEach<T>([NotNull, ItemCanBeNull] this IEnumerable<T> collection, [NotNull] Action<T, int> action)
        {
            foreach (var i in collection.Select((item, index) => new { item, index }))
            {
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
        [NotNull]
        public static IEnumerable<KeyValuePair<TValue, TKey>> Transpose<TKey, TValue>([NotNull] this IEnumerable<KeyValuePair<TKey, TValue>> items)
        {
            return items.Select(item => new KeyValuePair<TValue, TKey>(item.Value, item.Key));
        }

        /// <summary>
        /// Creates a Dictionary{TKey, TValue} from an IEnumerable{KeyValuePair{TKey, TValue}}.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="items">The items containing the key-value pairs.</param>
        /// <returns>The dictionary</returns>
        /// <exception cref="ArgumentNullException">Any of the keys is null.</exception>
        /// <exception cref="ArgumentException">Any of the keys is duplicate.</exception>
        [NotNull]
        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>([NotNull] this IEnumerable<KeyValuePair<TKey, TValue>> items)
        {
            return items.ToDictionary(item => item.Key, item => item.Value);
        }

        /// <summary>
        /// Creates a Dictionary{TKey, TValue} from an IEnumerable{KeyValuePair{TKey, TValue}}.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="items">The items containing the key-value pairs.</param>
        /// <param name="comparer">The comparer.</param>
        /// <returns>The dictionary</returns>
        /// <exception cref="ArgumentNullException">Any of the keys is null.</exception>
        /// <exception cref="ArgumentException">Any of the keys is duplicate.</exception>
        [NotNull]
        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>([NotNull] this IEnumerable<KeyValuePair<TKey, TValue>> items, [CanBeNull] IEqualityComparer<TKey>? comparer)
        {
            return items.ToDictionary(item => item.Key, item => item.Value, comparer);
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
        [CanBeNull][return:MaybeNull]
        public static TValue GetValueOrDefault<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> dictionary, [NotNull] TKey key, [CanBeNull, AllowNull] TValue defaultValue)
        {
            return dictionary.TryGetValue(key, out var value) ? value : defaultValue;
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
        [CanBeNull][return:MaybeNull]
        public static TValue GetValueOrDefault<TKey, TValue>([NotNull] this Dictionary<TKey, TValue> dictionary, [NotNull] TKey key, [CanBeNull, AllowNull] TValue defaultValue)
        {
            return dictionary.TryGetValue(key, out var value) ? value : defaultValue;
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
        [CanBeNull][return:MaybeNull]
        public static TValue GetValueOrDefault<TKey, TValue>([NotNull] this IReadOnlyDictionary<TKey, TValue> dictionary, [NotNull] TKey key, [CanBeNull, AllowNull] TValue defaultValue)
        {
            return dictionary.TryGetValue(key, out var value) ? value : defaultValue;
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
        [CanBeNull][return:MaybeNull]
        public static TValue GetValueOrDefault<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> dictionary, [NotNull] TKey key)
        {
            return dictionary.GetValueOrDefault(key, default);
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
        [CanBeNull][return:MaybeNull]
        public static TValue GetValueOrDefault<TKey, TValue>([NotNull] this Dictionary<TKey, TValue> dictionary, [NotNull] TKey key)
        {
            return dictionary.GetValueOrDefault(key, default);
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
        [CanBeNull][return:MaybeNull]
        public static TValue GetValueOrDefault<TKey, TValue>([NotNull] this IReadOnlyDictionary<TKey, TValue> dictionary, [NotNull] TKey key)
        {
            return dictionary.GetValueOrDefault(key, default);
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
        [CanBeNull][return:MaybeNull]
        public static TValue ForceValue<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> dictionary, [NotNull] TKey key, [NotNull] Func<TKey, TValue> generator)
        {
            if (dictionary.TryGetValue(key, out var result))
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
        /// <returns>The element with the specified key.</returns>
        [CanBeNull][return:MaybeNull]
        public static TValue ForceValue<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> dictionary, [NotNull] TKey key, [CanBeNull, AllowNull] TValue defaultValue)
        {
            return ForceValue(dictionary, key, _ => defaultValue!);
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
        [NotNull, ItemCanBeNull]
        public static TTarget[] ToArray<TSource, TTarget>([NotNull, ItemCanBeNull] this ICollection<TSource> items, [NotNull] Func<TSource, TTarget> selector)
        {
            var count = items.Count;

            var result = new TTarget[count];

            var i = 0;

            foreach (var item in items)
            {
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
        [ItemCanBeNull]
        [NotNull]
        public static IEnumerable<T> Repeat<T>([NotNull, ItemCanBeNull] ICollection<T> source, int count)
        {
            for (var i = 0; i < count; i++)
            {
                foreach (var item in source)
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate,
        /// and returns the zero-based index of the first occurrence within the entire <see cref="T:System.Collections.Generic.IEnumerable`1" />.
        /// </summary>
        /// <typeparam name="T">The type of the items in the source collection.</typeparam>
        /// <param name="source">The collection containing the items to be searched.</param>
        /// <param name="match">The <see cref="T:System.Predicate`1" /> delegate that defines the conditions of the element to search for.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of an element that matches the conditions
        /// defined by <paramref name="match" />, if found; otherwise, –1.
        /// </returns>
        public static int FindIndex<T>([NotNull] this IEnumerable<T> source, [NotNull] Predicate<T> match)
        {
            return source
                .Select((item, index) => new { item, index })
                .FirstOrDefault(item => match(item.item))?.index ?? -1;

        }
    }
}
