namespace TomsToolbox.ObservableCollections
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.Linq;

    using JetBrains.Annotations;

    /// <summary>
    /// Helper class to create typed change trackers from arbitrary lists.
    /// </summary>
    public static class ObservablePropertyChangeTracker
    {
        /// <summary>
        /// Creates a new <see cref="ObservablePropertyChangeTracker{T}"/>
        /// </summary>
        /// <typeparam name="TList">The type of the collection.</typeparam>
        /// <typeparam name="T">The type of the items in the collection.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <returns>The <see cref="ObservablePropertyChangeTracker{T}"></see></returns>
        [NotNull]
        public static ObservablePropertyChangeTracker<T> Create<TList, T>([NotNull] TList collection)
            where TList : IList<T>, INotifyCollectionChanged
            where T : INotifyPropertyChanged
        {
            Contract.Requires(collection != null);
            Contract.Ensures(Contract.Result<ObservablePropertyChangeTracker<T>>() != null);

            return new ObservablePropertyChangeTracker<T>(collection, collection);
        }
    }

    /// <summary>
    /// Tracks <see cref="INotifyPropertyChanged.PropertyChanged"/> events of all items in an observable collection.
    /// </summary>
    /// <typeparam name="T">The type of the items in the collection.</typeparam>
    public class ObservablePropertyChangeTracker<T>
        where T : INotifyPropertyChanged
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObservablePropertyChangeTracker{T}"/> class.
        /// </summary>
        /// <param name="collection">The collection.</param>
        public ObservablePropertyChangeTracker([NotNull] IObservableCollection<T> collection)
            : this(collection, collection)
        {
            Contract.Requires(collection != null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservablePropertyChangeTracker{T}"/> class.
        /// </summary>
        /// <param name="collection">The collection.</param>
        public ObservablePropertyChangeTracker([NotNull] ObservableCollection<T> collection)
            : this(collection, collection)
        {
            Contract.Requires(collection != null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservablePropertyChangeTracker{T}"/> class.
        /// </summary>
        /// <param name="collection">The collection.</param>
        public ObservablePropertyChangeTracker([NotNull] ReadOnlyObservableCollection<T> collection)
            : this(collection, collection)
        {
            Contract.Requires(collection != null);
        }

        /// <summary>
        /// Occurs when the property of any item has changed. The sender in the event is the item that has changed, not this instance.
        /// </summary>
        public event EventHandler<PropertyChangedEventArgs> ItemPropertyChanged;

        internal ObservablePropertyChangeTracker([NotNull] IList<T> items, [NotNull] INotifyCollectionChanged eventSource)
        {
            Contract.Requires(items != null);
            Contract.Requires(eventSource != null);

            eventSource.CollectionChanged += Items_CollectionChanged;

            foreach (var item in items)
            {
                Contract.Assume(item != null);
                item.PropertyChanged += Item_PropertyChanged;
            }
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ItemPropertyChanged?.Invoke(sender, e);
        }

        [ContractVerification(false)] // Too complex, checker is confused.
        private void Items_CollectionChanged(object sender, [NotNull] NotifyCollectionChangedEventArgs e)
        {
            Contract.Requires((e.Action != NotifyCollectionChangedAction.Add) || (e.NewItems != null));
            Contract.Requires((e.Action != NotifyCollectionChangedAction.Remove) || (e.OldItems != null));
            Contract.Requires((e.Action != NotifyCollectionChangedAction.Replace) || ((e.OldItems != null) && (e.NewItems != null)));

            // ReSharper disable PossibleNullReferenceException
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems.OfType<T>())
                    {
                        item.PropertyChanged += Item_PropertyChanged;
                    }
                    break;


                case NotifyCollectionChangedAction.Move:
                    break;


                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems.OfType<T>())
                    {
                        item.PropertyChanged -= Item_PropertyChanged;
                    }
                    break;


                case NotifyCollectionChangedAction.Replace:
                    foreach (var item in e.OldItems.OfType<T>())
                    {
                        item.PropertyChanged -= Item_PropertyChanged;
                    }
                    foreach (var item in e.NewItems.OfType<T>())
                    {
                        item.PropertyChanged += Item_PropertyChanged;
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    throw new NotSupportedException("Reset collection is not supported when the ObservablePropertyChangeTracker is active.");
                
                // ReSharper restore PossibleNullReferenceException
            }
        }
    }
}
