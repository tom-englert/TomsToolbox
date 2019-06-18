namespace TomsToolbox.ObservableCollections
{
    using System;
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using JetBrains.Annotations;

    using TomsToolbox.Core;

    /// <summary>
    /// A read only observable collection of type TTarget where TTarget is a wrapper for type TSource.
    /// </summary>
    /// <typeparam name="TSource">The type of elements in the source collection.</typeparam>
    /// <typeparam name="TTarget">The type of elements in the wrapped collection.</typeparam>
    /// <remarks>
    /// This collection does <c>not</c> hold a reference to the source collection.
    /// To keep the source collection alive, the object generating the <see cref="T:TomsToolbox.ObservableCollections.ObservableWrappedCollection`2" /> must hold a reference to the source collection.
    /// </remarks>
    /// <inheritdoc />
    public class ObservableWrappedCollection<TSource, TTarget> : ReadOnlyObservableCollectionAdapter<TTarget, ObservableCollection<TTarget>>
    {
        [NotNull]
        private readonly Func<TSource, TTarget> _itemGenerator;
        [CanBeNull]
        private readonly IWeakEventListener _collectionChangedWeakEvent;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:TomsToolbox.ObservableCollections.ObservableWrappedCollection`2" /> class.
        /// </summary>
        /// <param name="sourceCollection">The source collection to wrap. This instance will not hold a reference to the source collection.</param>
        /// <param name="itemGenerator">The item generator to generate the wrapper for each item.</param>
        public ObservableWrappedCollection([NotNull, ItemNotNull] IEnumerable sourceCollection, [NotNull] Func<TSource, TTarget> itemGenerator)
            : base(new ObservableCollection<TTarget>(sourceCollection.Cast<TSource>().Select(itemGenerator)))
        {
            _itemGenerator = itemGenerator;

            if (sourceCollection is INotifyCollectionChanged eventSource)
            {
                _collectionChangedWeakEvent = CreateEvent(eventSource);
            }
        }
        [NotNull]
        private WeakEventListener<ObservableWrappedCollection<TSource, TTarget>, INotifyCollectionChanged, NotifyCollectionChangedEventArgs> CreateEvent([NotNull] INotifyCollectionChanged eventSource)
        {
            return new WeakEventListener<ObservableWrappedCollection<TSource, TTarget>, INotifyCollectionChanged, NotifyCollectionChangedEventArgs>(
                this, eventSource, OnCollectionChanged, Attach, Detach);
        }

        /// <summary>
        /// Gets the item generator used to generate the wrapper for each item.
        /// </summary>
        [NotNull]
        protected Func<TSource, TTarget> ItemGenerator
        {
            get
            {
                return _itemGenerator;
            }
        }

        private static void OnCollectionChanged([NotNull, ItemCanBeNull] ObservableWrappedCollection<TSource, TTarget> self, [NotNull] object sender, [NotNull] NotifyCollectionChangedEventArgs e)
        {
            self.SourceItems_CollectionChanged(sender, e);
        }

        private static void Attach([NotNull] WeakEventListener<ObservableWrappedCollection<TSource, TTarget>, INotifyCollectionChanged, NotifyCollectionChangedEventArgs> weakEvent, [NotNull] INotifyCollectionChanged sender)
        {
            sender.CollectionChanged += weakEvent.OnEvent;
        }

        private static void Detach([NotNull] WeakEventListener<ObservableWrappedCollection<TSource, TTarget>, INotifyCollectionChanged, NotifyCollectionChangedEventArgs> weakEvent, [NotNull] INotifyCollectionChanged sender)
        {
            sender.CollectionChanged -= weakEvent.OnEvent;
        }
        private void SourceItems_CollectionChanged([NotNull] object sender, [NotNull] NotifyCollectionChangedEventArgs e)
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
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        protected virtual void OnSourceCollectionChanged([NotNull, ItemCanBeNull] IEnumerable sourceCollection, [NotNull] NotifyCollectionChangedEventArgs e)
        {
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
            add => base.CollectionChanged += value;
            remove => base.CollectionChanged -= value;
        }

        /// <summary>
        /// Occurs when a property has changed.
        /// </summary>
        public new event PropertyChangedEventHandler PropertyChanged
        {
            add => base.PropertyChanged += value;
            remove => base.PropertyChanged -= value;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="ObservableWrappedCollection{TSource, TTarget}"/> class.
        /// </summary>
        ~ObservableWrappedCollection()
        {
            _collectionChangedWeakEvent?.Detach();
        }
    }
}