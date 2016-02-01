namespace TomsToolbox.ObservableCollections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.Linq;

    using TomsToolbox.Core;

    /// <summary>
    /// A simple filtered collection implementation.<para/>
    /// This collection contains only the items from the source collection passing the filter.<para/>
    /// Changes in the source collection will be tracked, however changes in the individual objects that would affect the filter will not be tracked.<para/>
    /// Changes that affect the items order in the source collection (see <see cref="NotifyCollectionChangedAction.Move"/>, <see cref="IList.Insert"/>) will be ignored.<para/>
    /// </summary>
    /// <typeparam name="T">Type of the items in the collection.</typeparam>
    /// <remarks>
    /// This collection does <c>not</c> hold a reference to the source collection.
    /// To keep the source collection alive, the object generating the <see cref="ObservableFilteredCollection{T}" /> must hold a reference to the source collection.
    /// </remarks>
    public class ObservableFilteredCollection<T> : ReadOnlyObservableCollectionAdapter<T, ObservableCollection<T>>, IObservableCollection<T>
    {
        private static readonly IEnumerable _emptyList = new T[0];

        private readonly IWeakEventListener _collectionChangedWeakEvent;
        private readonly Func<T, bool> _filter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableFilteredCollection{T}"/> class.
        /// </summary>
        /// <param name="sourceCollection">The source collection. This instance will not hold a reference to the source collection.</param>
        /// <param name="filter">The filter.</param>
        public ObservableFilteredCollection(IEnumerable sourceCollection, Func<T, bool> filter)
            : base(new ObservableCollection<T>(sourceCollection.Cast<T>().Where(filter)))
        {
            Contract.Requires(sourceCollection != null);
            Contract.Requires(filter != null);

            _filter = filter;

            var eventSource = sourceCollection as INotifyCollectionChanged;
            if (eventSource != null)
            {
                _collectionChangedWeakEvent = CreateEvent(eventSource);
            }
        }

        [ContractVerification(false)]
        private WeakEventListener<ObservableFilteredCollection<T>, INotifyCollectionChanged, NotifyCollectionChangedEventArgs> CreateEvent(INotifyCollectionChanged eventSource)
        {
            Contract.Ensures(Contract.Result<WeakEventListener<ObservableFilteredCollection<T>, INotifyCollectionChanged, NotifyCollectionChangedEventArgs>>() != null);

            return new WeakEventListener<ObservableFilteredCollection<T>, INotifyCollectionChanged, NotifyCollectionChangedEventArgs>(
                this, eventSource,
                (self, sender, e) => self.SourceCollection_CollectionChanged(sender, e),
                (weakEvent, sender) => sender.CollectionChanged += weakEvent.OnEvent,
                (weakEvent, sender) => sender.CollectionChanged -= weakEvent.OnEvent);
        }

        private void SourceCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Contract.Requires(sender != null);

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    var newItems = e.NewItems ?? _emptyList;
                    foreach (var newItem in newItems.Cast<T>().Where(_filter))
                    {
                        Items.Add(newItem);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    var oldItems = e.OldItems ?? _emptyList;
                    foreach (var oldItem in oldItems.Cast<T>())
                    {
                        Items.Remove(oldItem);
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    Items.Clear();
                    foreach (var newItem in ((IEnumerable)sender).Cast<T>().Where(_filter))
                    {
                        Items.Add(newItem);
                    }
                    break;

                case NotifyCollectionChangedAction.Move:
                    break;

                default:
                    throw new NotImplementedException("Source action not supported");
            }
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="ObservableFilteredCollection{T}"/> class.
        /// </summary>
        ~ObservableFilteredCollection()
        {
            if (_collectionChangedWeakEvent != null)
            {
                _collectionChangedWeakEvent.Detach();
            }
        }

        [ContractInvariantMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant()
        {
            Contract.Invariant(_filter != null);
        }
    }
}
