namespace TomsToolbox.ObservableCollections;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

using TomsToolbox.Essentials;

using WeakEventHandler;

/// <summary>
/// A simple filtered collection implementation.<para/>
/// This collection contains only the items from the source collection passing the filter.<para/>
/// </summary>
/// <typeparam name="T">Type of the items in the collection.</typeparam>
/// <remarks>
/// Changes in the source collection will be tracked always, changes in the individual objects that would affect the filter will be tracked when any of the live tracking properties changes.<para/>
/// The order of the elements may be different than the order in the source collection; also changes that affect the items order in the source collection (see <see cref="NotifyCollectionChangedAction.Move"/>, <see cref="IList.Insert"/>) will be ignored.<para/>
/// This collection does <c>not</c> hold a reference to the source collection. To keep the source collection alive, the object generating the <see cref="ObservableFilteredCollection{T}" /> must hold a reference to the source collection.<para/>
/// </remarks>
public class ObservableFilteredCollection<T> : ReadOnlyObservableCollectionAdapter<T, ObservableCollection<T>>
{
    private readonly Func<T, bool> _filter;
    private readonly string[] _liveTrackingProperties;

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableFilteredCollection{T}" /> class.
    /// </summary>
    /// <param name="sourceCollection">The source collection. This instance will not hold a reference to the source collection.</param>
    /// <param name="filter">The filter.</param>
    /// <param name="liveTrackingProperties">The live tracking properties. Whenever one of these properties in any item changes, the filter is reevaluated for the item.</param>
    public ObservableFilteredCollection(IEnumerable sourceCollection, Func<T, bool> filter, params string[] liveTrackingProperties)
        // ReSharper disable PossibleMultipleEnumeration
        : base(new ObservableCollection<T>(sourceCollection.Cast<T>().Where(filter)))
    {
        _filter = filter;
        _liveTrackingProperties = liveTrackingProperties;

        if (liveTrackingProperties.Any())
        {
            sourceCollection.Cast<T>().ForEach(AttachItemEvents);
        }

        AttachCollectionEvents(sourceCollection as INotifyCollectionChanged);
        // ReSharper restore PossibleMultipleEnumeration
    }

    private void AttachCollectionEvents(INotifyCollectionChanged? sender)
    {
        if (sender == null)
            return;

        sender.CollectionChanged += SourceCollection_CollectionChanged;
    }

    [MakeWeak]
    private void SourceCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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
                WeakEvents.Unsubscribe(this);
                Items.Clear();
                AttachCollectionEvents((INotifyCollectionChanged)sender);
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

    private void AddItems(IEnumerable<T>? newItems)
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
        if (newItem is INotifyPropertyChanged eventSource)
        {
            eventSource.PropertyChanged += Item_PropertyChanged;
        }
    }

    private void RemoveItems(IEnumerable<T>? oldItems)
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
        if (oldItem is INotifyPropertyChanged eventSource)
        {
            eventSource.PropertyChanged -= Item_PropertyChanged;
        }
    }

    [MakeWeak]
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

/*
        // ReSharper disable once EmptyDestructor
        /// <inheritdoc />
        ~ObservableFilteredCollection()
        {

        }
*/
}