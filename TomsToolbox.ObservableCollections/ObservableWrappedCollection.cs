namespace TomsToolbox.ObservableCollections
{
    using System;
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;

    using TomsToolbox.Core;

    /// <summary>
    /// A read only observable collection of type TTarget where TTarget is a wrapper for type TSource.
    /// </summary>
    /// <typeparam name="TSource">The type of elements in the source collection.</typeparam>
    /// <typeparam name="TTarget">The type of elements in the wrapped collection.</typeparam>
    /// <remarks>
    /// This collection does <c>not</c> hold a reference to the source collection.
    /// To keep the source collection alive, the object generating the <see cref="ObservableWrappedCollection{TTarget, TSource}" /> must hold a reference to the source collection.
    /// </remarks>
    public class ObservableWrappedCollection<TSource, TTarget> : ReadOnlyObservableCollectionAdapter<TTarget, ObservableCollection<TTarget>>
    {
        private readonly Func<TSource, TTarget> _itemGenerator;
        private readonly IWeakEventListener _collectionChangedWeakEvent;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableWrappedCollection{TTarget, TSource}" /> class.
        /// </summary>
        /// <param name="sourceCollection">The source collection to wrap. This instance will not hold a reference to the source collection.</param>
        /// <param name="itemGenerator">The item generator to generate the wrapper for each item.</param>
        public ObservableWrappedCollection(IEnumerable sourceCollection, Func<TSource, TTarget> itemGenerator)
            : base(new ObservableCollection<TTarget>(sourceCollection.Cast<TSource>().Select(itemGenerator)))
        {
            Contract.Requires(sourceCollection != null);
            Contract.Requires(itemGenerator != null);

            _itemGenerator = itemGenerator;

            var eventSource = sourceCollection as INotifyCollectionChanged;
            if (eventSource != null)
            {
                _collectionChangedWeakEvent = CreateEvent(eventSource);
            }
        }

        [ContractVerification(false)]
        private WeakEventListener<ObservableWrappedCollection<TSource, TTarget>, INotifyCollectionChanged, NotifyCollectionChangedEventArgs> CreateEvent(INotifyCollectionChanged eventSource)
        {
            Contract.Ensures(Contract.Result<WeakEventListener<ObservableWrappedCollection<TSource, TTarget>, INotifyCollectionChanged, NotifyCollectionChangedEventArgs>>() != null);

            return new WeakEventListener<ObservableWrappedCollection<TSource, TTarget>, INotifyCollectionChanged, NotifyCollectionChangedEventArgs>(
                this, eventSource,
                (self, sender, e) => self.sourceItems_CollectionChanged(sender, e),
                (weakEvent, sender) => sender.CollectionChanged += weakEvent.OnEvent,
                (weakEvent, sender) => sender.CollectionChanged -= weakEvent.OnEvent);
        }

        /// <summary>
        /// Gets the item generator used to generate the wrapper for each item.
        /// </summary>
        protected Func<TSource, TTarget> ItemGenerator
        {
            get
            {
                Contract.Ensures(Contract.Result<Func<TSource, TTarget>>() != null);
                return _itemGenerator;
            }
        }

        [ContractVerification(false)] // Just mirrors the original collection
        private void sourceItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnSourceCollectionChanged((IEnumerable)sender, e);
        }

        /// <summary>
        /// Called when the source collection has changed.
        /// </summary>
        /// <param name="sourceCollection">The source collection.</param>
        /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        /// <exception cref="System.ArgumentException">Event source must provide index!</exception>
        /// <exception cref="System.NotImplementedException">Moving more than one item is not supported</exception>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Caused by code contracts.")]
        [ContractVerification(false)]
        protected virtual void OnSourceCollectionChanged(IEnumerable sourceCollection, NotifyCollectionChangedEventArgs e)
        {
            Contract.Requires(sourceCollection != null);
            Contract.Requires(e != null);
            Contract.Requires((e.Action != NotifyCollectionChangedAction.Add) || ((e.NewStartingIndex >= 0) && (e.NewStartingIndex <= Items.Count) && (e.NewItems != null)));
            Contract.Requires((e.Action != NotifyCollectionChangedAction.Move) || ((e.OldItems != null) && (e.OldItems.Count == 1) && (e.OldStartingIndex >= 0) && (e.OldStartingIndex < Items.Count) && (e.NewStartingIndex >= 0) && (e.NewStartingIndex < Items.Count)));
            Contract.Requires((e.Action != NotifyCollectionChangedAction.Remove) || ((e.OldStartingIndex >= 0) && (e.OldStartingIndex < Items.Count) && (e.OldItems != null)));
            Contract.Requires((e.Action != NotifyCollectionChangedAction.Replace) || ((e.NewStartingIndex >= 0) && (e.NewStartingIndex < Items.Count) && (e.NewItems != null)));

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    var insertionIndex = e.NewStartingIndex;
                    foreach (var item in e.NewItems.Cast<TSource>())
                    {
                        Items.Insert(insertionIndex++, ItemGenerator(item));
                    }
                    break;


                case NotifyCollectionChangedAction.Move:
                    // Exactly one item....
                    Items.Move(e.OldStartingIndex, e.NewStartingIndex);
                    break;


                case NotifyCollectionChangedAction.Remove:
                    var removeIndex = e.OldStartingIndex;
                    for (var k = 0; k < e.OldItems.Count; k++)
                    {
                        Contract.Assume(removeIndex < Items.Count);
                        Items.RemoveAt(removeIndex);
                    }
                    break;


                case NotifyCollectionChangedAction.Replace:
                    var replaceIndex = e.NewStartingIndex;
                    foreach (var item in e.NewItems)
                    {
                        Items[replaceIndex++] = ItemGenerator((TSource)item);
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    Items.Clear();
                    Items.AddRange(sourceCollection.Cast<TSource>().Select(ItemGenerator));
                    break;
            }
        }

        /// <summary>
        /// Occurs when the collection has changed.
        /// </summary>
        public new event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add
            {
                base.CollectionChanged += value;
            }
            remove
            {
                base.CollectionChanged -= value;
            }
        }

        /// <summary>
        /// Occurs when a property has changed.
        /// </summary>
        public new event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                base.PropertyChanged += value;
            }
            remove
            {
                base.PropertyChanged -= value;
            }
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="ObservableWrappedCollection{TSource, TTarget}"/> class.
        /// </summary>
        ~ObservableWrappedCollection()
        {
            if (_collectionChangedWeakEvent != null)
            {
                _collectionChangedWeakEvent.Detach();
            }
        }

        [ContractInvariantMethod]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant()
        {
            Contract.Invariant(_itemGenerator != null);
        }
    }
}