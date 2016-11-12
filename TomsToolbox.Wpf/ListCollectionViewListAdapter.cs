namespace TomsToolbox.Wpf
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Windows.Data;

    using JetBrains.Annotations;

    using TomsToolbox.ObservableCollections;

    /// <summary>
    /// Adapter for a <see cref="ListCollectionView" /> that exposes the content as a read-only collection with an IList interface.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    [SuppressMessage("Microsoft.Design", "CA1035:ICollectionImplementationsHaveStronglyTypedMembers", Justification = "ListCollectionView is not strongly typed.")]
    [SuppressMessage("Microsoft.Design", "CA1039:ListsAreStronglyTyped", Justification = "ListCollectionView is not strongly typed.")]
    [ContractVerification(false)] // Just wrapping inner object
    public sealed class ListCollectionViewListAdapter<T> : IObservableCollection<T>, IList
    {
        [NotNull]
        private readonly ListCollectionView _collectionView;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListCollectionViewListAdapter{T}"/> class.
        /// </summary>
        /// <param name="collectionView">The collection view.</param>
        public ListCollectionViewListAdapter([NotNull] ListCollectionView collectionView)
        {
            Contract.Requires(collectionView != null);
            _collectionView = collectionView;
            ((INotifyCollectionChanged)collectionView).CollectionChanged += CollectionView_CollectionChanged;
            ((INotifyPropertyChanged)collectionView).PropertyChanged += CollectionView_PropertyChanged;
        }

        /// <summary>
        /// Gets the underlying collection view.
        /// </summary>
        [NotNull]
        public ListCollectionView CollectionView
        {
            get
            {
                Contract.Ensures(Contract.Result<ICollectionView>() != null);
                return _collectionView;
            }
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        public int Count
        {
            get
            {
                var count = _collectionView.Count;
                Contract.Assume(count >= 0);
                return count;
            }
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
        /// </summary>
        /// <returns>
        /// true if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
        /// </returns>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        public bool Contains(T item)
        {
            return _collectionView.Contains(item);
        }

        /// <summary>
        /// Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1" />.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1" />.</param>
        /// <returns>
        /// The index of <paramref name="item" /> if found in the list; otherwise, -1.
        /// </returns>
        public int IndexOf(T item)
        {
            return _collectionView.IndexOf(item);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return _collectionView.Cast<T>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_collectionView).GetEnumerator();
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public T this[int index]
        {
            get
            {
                var value = _collectionView.GetItemAt(index);
                return value == null ? default(T) : (T)value;
            }
            set
            {
                ReadOnlyNotSupported();
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            if (index < 0)
                throw new ArgumentOutOfRangeException("index");
            if (index + array.Length < Count)
                throw new ArgumentException("array is too small");
            if (array.Rank != 1)
                throw new ArgumentException("array is not one-dimensional");

            foreach (var item in _collectionView)
            {
                Contract.Assume(index < array.Length);
                array.SetValue(item, index++);
            }
        }

        bool ICollection<T>.Remove(T item)
        {
            ReadOnlyNotSupported();
            return false;
        }

        object ICollection.SyncRoot
        {
            get
            {
                return _collectionView;
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                return false;
            }
        }

        int IList.Add(object value)
        {
            ReadOnlyNotSupported();
            return 0;
        }

        bool IList.Contains(object value)
        {
            return _collectionView.Contains(value);
        }

        void ICollection<T>.Add(T item)
        {
            ReadOnlyNotSupported();
        }

        void ICollection<T>.Clear()
        {
            ReadOnlyNotSupported();
        }

        void ICollection<T>.CopyTo(T[] array, int index)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            if (index < 0)
                throw new ArgumentOutOfRangeException("index");
            if (index + array.Length < Count)
                throw new ArgumentException("array is too small");
            if (array.Rank != 1)
                throw new ArgumentException("array is not one-dimensional");

            foreach (var item in _collectionView)
            {
                Contract.Assume(index < array.Length);
                array.SetValue(item, index++);
            }
        }

        void IList.Clear()
        {
            ReadOnlyNotSupported();
        }

        int IList.IndexOf(object value)
        {
            return _collectionView.IndexOf(value);
        }

        void IList.Insert(int index, object value)
        {
            ReadOnlyNotSupported();
        }

        void IList.Remove(object value)
        {
            ReadOnlyNotSupported();
        }

        void IList<T>.Insert(int index, T item)
        {
            ReadOnlyNotSupported();
        }

        void IList<T>.RemoveAt(int index)
        {
            ReadOnlyNotSupported();
        }

        void IList.RemoveAt(int index)
        {
            ReadOnlyNotSupported();
        }

        object IList.this[int index]
        {
            get
            {
                return _collectionView.GetItemAt(index);
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                ReadOnlyNotSupported();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.IList" /> has a fixed size.
        /// </summary>
        public bool IsFixedSize
        {
            get
            {
                return false;
            }
        }

        private static void ReadOnlyNotSupported()
        {
            throw new NotSupportedException(@"Collection is read-only.");
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Occurs when the collection changes.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            var handler = CollectionChanged;
            if (handler != null)
                handler(this, e);
        }

        private void CollectionView_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e);
        }

        private void CollectionView_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnCollectionChanged(e);
        }

        [ContractInvariantMethod]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        [Conditional("CONTRACTS_FULL")]
        private void ObjectInvariant()
        {
            Contract.Invariant(_collectionView != null);
        }
    }
}
