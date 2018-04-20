namespace TomsToolbox.ObservableCollections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;

    using JetBrains.Annotations;

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
    public class ObservableFilteredCollection<T> : ReadOnlyObservableCollectionAdapter<T, ObservableCollection<T>>
    {
        [CanBeNull]
        private readonly IWeakEventListener _collectionChangedWeakEvent;
        [NotNull]
        private readonly Func<T, bool> _filter;
        [NotNull, ItemNotNull]
        private readonly string[] _liveTrackingProperties;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableFilteredCollection{T}" /> class.
        /// </summary>
        /// <param name="sourceCollection">The source collection. This instance will not hold a reference to the source collection.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="liveTrackingProperties">The live tracking properties. Whenever one of these properties in any item changes, the filter is reevaluated for the item.</param>
        public ObservableFilteredCollection([NotNull, ItemCanBeNull] IEnumerable sourceCollection, [NotNull] Func<T, bool> filter, [NotNull, ItemNotNull] params string[] liveTrackingProperties)
        // ReSharper disable PossibleMultipleEnumeration
            : base(new ObservableCollection<T>(sourceCollection.Cast<T>().Where(filter)))
        {

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
            // ReSharper restore PossibleMultipleEnumeration
        }
        [NotNull]
        private WeakEventListener<ObservableFilteredCollection<T>, INotifyCollectionChanged, NotifyCollectionChangedEventArgs> CreateEvent([NotNull] INotifyCollectionChanged eventSource)
        {

            return new WeakEventListener<ObservableFilteredCollection<T>, INotifyCollectionChanged, NotifyCollectionChangedEventArgs>(
                this, eventSource, OnCollectionChanged, Attach, Detach);
        }
        private static void OnCollectionChanged([NotNull, ItemCanBeNull] ObservableFilteredCollection<T> self, [NotNull] object sender, [NotNull] NotifyCollectionChangedEventArgs e)
        {

            self.SourceCollection_CollectionChanged(sender, e);
        }

        private static void Attach([NotNull] WeakEventListener<ObservableFilteredCollection<T>, INotifyCollectionChanged, NotifyCollectionChangedEventArgs> weakEvent, [NotNull] INotifyCollectionChanged sender)
        {

            sender.CollectionChanged += weakEvent.OnEvent;
        }

        private static void Detach([NotNull] WeakEventListener<ObservableFilteredCollection<T>, INotifyCollectionChanged, NotifyCollectionChangedEventArgs> weakEvent, [NotNull] INotifyCollectionChanged sender)
        {

            sender.CollectionChanged -= weakEvent.OnEvent;
        }
        private void SourceCollection_CollectionChanged([NotNull] object sender, [NotNull] NotifyCollectionChangedEventArgs e)
        {

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

        private void AddItems([CanBeNull, ItemCanBeNull] IEnumerable<T> newItems)
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

        private void AttachItemEvents([CanBeNull] T newItem)
        {
            var eventSource = newItem as INotifyPropertyChanged;
            if (eventSource != null)
                eventSource.PropertyChanged += Item_PropertyChanged;
        }

        private void RemoveItems([CanBeNull, ItemCanBeNull] IEnumerable<T> oldItems)
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

        private void DetachItemEvents([CanBeNull] T oldItem)
        {
            var eventSource = oldItem as INotifyPropertyChanged;
            if (eventSource != null)
                eventSource.PropertyChanged -= Item_PropertyChanged;
        }

        private void Item_PropertyChanged([NotNull] object sender, [NotNull] PropertyChangedEventArgs e)
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
    }
}
