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
    /// </summary>
    /// <typeparam name="T">Type of the items in the collection.</typeparam>
    /// <remarks>
    /// Changes in the source collection will be tracked always, changes in the individual objects that would affect the filter will be tracked when any of the live tracking properties changes.<para/>
    /// The order of the elements may be different than the order in the source collection; also changes that affect the items order in the source collection (see <see cref="NotifyCollectionChangedAction.Move"/>, <see cref="IList.Insert"/>) will be ignored.<para/>
    /// This collection does <c>not</c> hold a reference to the source collection. To keep the source collection alive, the object generating the <see cref="ObservableFilteredCollection{T}" /> must hold a reference to the source collection.<para/>
    /// When live tracking is active, Reset of the source collection is not supported.
    /// </remarks>
    public class ObservableFilteredCollection<T> : ReadOnlyObservableCollectionAdapter<T, ObservableCollection<T>>, IObservableCollection<T>
    {
        private readonly IWeakEventListener _collectionChangedWeakEvent;
        private readonly Func<T, bool> _filter;
        private readonly string[] _liveTrackingProperties;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableFilteredCollection{T}" /> class.
        /// </summary>
        /// <param name="sourceCollection">The source collection. This instance will not hold a reference to the source collection.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="liveTrackingProperties">The live tracking properties. Whenever one of these properties in any item changes, the filter is reevaluated for the item.</param>
        public ObservableFilteredCollection(IEnumerable sourceCollection, Func<T, bool> filter, params string[] liveTrackingProperties)
            : base(new ObservableCollection<T>(sourceCollection.Cast<T>().Where(filter)))
        {
            Contract.Requires(sourceCollection != null);
            Contract.Requires(filter != null);
            Contract.Requires(liveTrackingProperties != null);

            _filter = filter;
            _liveTrackingProperties = liveTrackingProperties;

            if (liveTrackingProperties.Any())
            {
                sourceCollection.Cast<T>().ForEach(AttachItemEvents);
            }

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
                this, eventSource, OnCollectionChanged, Attach, Detach);
        }

        private static void OnCollectionChanged(ObservableFilteredCollection<T> self, object sender, NotifyCollectionChangedEventArgs e)
        {
            Contract.Requires(self != null);

            self.SourceCollection_CollectionChanged(sender, e);
        }

        private static void Attach(WeakEventListener<ObservableFilteredCollection<T>, INotifyCollectionChanged, NotifyCollectionChangedEventArgs> weakEvent, INotifyCollectionChanged sender)
        {
            Contract.Requires(weakEvent != null);
            Contract.Requires(sender != null);

            sender.CollectionChanged += weakEvent.OnEvent;
        }

        private static void Detach(WeakEventListener<ObservableFilteredCollection<T>, INotifyCollectionChanged, NotifyCollectionChangedEventArgs> weakEvent, INotifyCollectionChanged sender)
        {
            Contract.Requires(weakEvent != null);
            Contract.Requires(sender != null);

            sender.CollectionChanged -= weakEvent.OnEvent;
        }

        private void SourceCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Contract.Requires(sender != null);

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    AddItems(e.NewItems?.Cast<T>());
                    break;

                case NotifyCollectionChangedAction.Remove:
                    RemoveItems(e.OldItems?.Cast<T>());
                    break;

                case NotifyCollectionChangedAction.Reset:
                    if (_liveTrackingProperties.Any())
                        throw new NotSupportedException("NotifyCollectionChangedAction.Reset is not supported when the ObservableFilteredCollection is in live tracking mode.");

                    Items.Clear();
                    AddItems(((IEnumerable)sender).Cast<T>());
                    break;

                case NotifyCollectionChangedAction.Replace:
                    RemoveItems(e.OldItems?.Cast<T>());
                    AddItems(e.NewItems?.Cast<T>());
                    break;

                case NotifyCollectionChangedAction.Move:
                    break;

                default:
                    throw new NotImplementedException("Source action not supported");
            }
        }

        private void AddItems(IEnumerable<T> newItems)
        {
            if (newItems == null)
                return;

            foreach (var newItem in newItems)
            {
                if (_liveTrackingProperties.Any())
                    AttachItemEvents(newItem);

                if (_filter(newItem))
                    Items.Add(newItem);
            }
        }

        private void AttachItemEvents(T newItem)
        {
            var eventSource = newItem as INotifyPropertyChanged;
            if (eventSource != null)
                eventSource.PropertyChanged += Item_PropertyChanged;
        }

        private void RemoveItems(IEnumerable<T> oldItems)
        {
            if (oldItems == null)
                return;

            foreach (var oldItem in oldItems)
            {
                if (_liveTrackingProperties.Any())
                    DetachItemEvents(oldItem);

                Items.Remove(oldItem);
            }
        }

        private void DetachItemEvents(T oldItem)
        {
            var eventSource = oldItem as INotifyPropertyChanged;
            if (eventSource != null)
                eventSource.PropertyChanged -= Item_PropertyChanged;
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var item = (T)sender;

            if (!_liveTrackingProperties.Contains(e.PropertyName))
                return;

            if (_filter(item))
            {
                if (!Items.Contains(item))
                    Items.Add(item);
            }
            else
            {
                Items.Remove(item);
            }
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="ObservableFilteredCollection{T}"/> class.
        /// </summary>
        ~ObservableFilteredCollection()
        {
            _collectionChangedWeakEvent?.Detach();
        }

        [ContractInvariantMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant()
        {
            Contract.Invariant(_filter != null);
        }
    }
}
