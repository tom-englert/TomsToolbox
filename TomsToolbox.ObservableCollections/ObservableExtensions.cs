namespace TomsToolbox.ObservableCollections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;

    using JetBrains.Annotations;

    using TomsToolbox.Core;

    /// <summary>
    /// Extension methods for some observable patterns.
    /// </summary>
    public static class ObservableExtensions
    {
        /// <summary>
        /// Projects each element of a sequence to an <see cref="T:System.Collections.Generic.IList`1"/>,
        /// flattens the resulting sequences into one sequence, and invokes a result selector function on each element therein.
        /// If the source is an observable collection, the resulting sequence will track the changes.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.Generic.IList`1"/> whose elements are the result of invoking the one-to-many transform function defined by <paramref name="itemGeneratorExpression"/>
        /// on each element of <paramref name="source"/> and then mapping each of those sequence elements and their corresponding source element to a result element.
        /// </returns>
        /// <param name="source">A sequence of values to project.</param>
        /// <param name="itemGeneratorExpression">A property expression of a transform function to apply to each element of the intermediate sequence.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <typeparam name="TTarget">The type of the elements of the resulting sequence.</typeparam>
        /// <remarks>
        /// The selector must aways return the same object for the source, else removing elements will fail!
        /// </remarks>
        [NotNull, ItemCanBeNull]
        public static IObservableCollection<TTarget> ObservableSelectMany<TSource, TTarget>([NotNull, ItemCanBeNull] this IList<TSource> source, [NotNull] Expression<Func<TSource, IList<TTarget>>> itemGeneratorExpression)
        {

            return new ObservableSelectManyImpl<TSource, TTarget>(source, itemGeneratorExpression);
        }

        /// <summary>
        /// Projects each element of a sequence into a new form.
        /// </summary>
        /// <returns>
        /// An observable <see cref="T:System.Collections.Generic.IList`1"/> whose elements are the result of invoking the transform function on each element of <paramref name="source"/>.
        /// </returns>
        /// <param name="source">A sequence of values to invoke a transform function on.</param>
        /// <param name="itemGeneratorExpression">An expression that describes the transform function to apply to each element.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <typeparam name="TTarget">The type of the value returned by the transform function compiled from <paramref name="itemGeneratorExpression"/>.</typeparam>
        /// <remarks>
        /// If source is observable, i.e. implements <see cref="INotifyCollectionChanged"/>, the returned collection is observable, too.
        /// <para/>
        /// If the <paramref name="itemGeneratorExpression"/> is a property expression like "item => item.SomeProperty" and <typeparamref name="TSource"/>
        /// implements <see cref="INotifyPropertyChanged"/>, the returned collection will be updated when any items property changes.
        /// </remarks>
        [NotNull, ItemCanBeNull]
        public static IObservableCollection<TTarget> ObservableSelect<TSource, TTarget>([NotNull, ItemCanBeNull] this IList<TSource> source, [NotNull] Expression<Func<TSource, TTarget>> itemGeneratorExpression)
        {

            return new ObservableSelectImpl<TSource, TTarget>(source, itemGeneratorExpression);
        }

        /// <summary>
        /// Returns an observable collection of objects of type <typeparamref name="TTarget"/> that mirrors the source collection.
        /// </summary>
        /// <typeparam name="TTarget">The type of the target elements.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <returns>The observable collection of objects of type <typeparamref name="TTarget"/> that mirrors the source collection.</returns>
        /// <remarks>
        /// See <see cref="ObservableWrappedCollection{T1,T2}"/> for usage details.
        /// </remarks>
        [NotNull, ItemCanBeNull]
        public static IObservableCollection<TTarget> ObservableCast<TTarget>([NotNull, ItemCanBeNull] this IEnumerable source)
        {

            return new ObservableWrappedCollection<object, TTarget>(source, item => (TTarget)item);
        }

        /// <summary>
        /// Returns an observable collection of objects of type <typeparamref name="T" /> that contains all items of the source collection that pass the filter.
        /// See <see cref="ObservableFilteredCollection{T}"/> for details.
        /// </summary>
        /// <typeparam name="T">The type of the target elements.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="predicate">The predicate used to filter.</param>
        /// <param name="liveTrackingProperties">The live tracking properties. Whenever one of these properties in any item changes, the filter is reevaluated for the item.</param>
        /// <returns>
        /// The observable collection of objects of type <typeparamref name="T" /> that contains the filtered items of the source collection.
        /// </returns>
        /// <remarks>
        /// See <see cref="ObservableFilteredCollection{T}"/> for usage details.
        /// </remarks>
        [NotNull, ItemCanBeNull]
        public static IObservableCollection<T> ObservableWhere<T>([NotNull, ItemCanBeNull] this IList<T> source, [NotNull] Func<T, bool> predicate, [NotNull, ItemNotNull] params string[] liveTrackingProperties)
        {

            return new ObservableFilteredCollection<T>(source, predicate, liveTrackingProperties);
        }

        private class ObservableSelectImpl<TSource, TTarget> : ObservableWrappedCollection<TSource, TTarget>
        {
            [NotNull]
            private readonly TomsToolbox.Core.WeakReference<IList<TSource>> _sourceReference;
            [CanBeNull]
            private readonly string _propertyName;
            [CanBeNull, ItemNotNull]
            private List<WeakEventListener<ObservableSelectImpl<TSource, TTarget>, INotifyPropertyChanged, PropertyChangedEventArgs>> _propertyChangeEventListeners;

            /// <summary>
            /// Initializes a new instance of the <see cref="T:TomsToolbox.ObservableCollections.ObservableExtensions.ObservableSelectImpl`2" /> class.
            /// </summary>
            /// <param name="sourceCollection">The source collection to wrap.</param>
            /// <param name="itemGeneratorExpression">The item generator to generate the wrapper for each item.</param>
            public ObservableSelectImpl([NotNull, ItemCanBeNull] IList<TSource> sourceCollection, [NotNull] Expression<Func<TSource, TTarget>> itemGeneratorExpression)
                : base(sourceCollection, itemGeneratorExpression.Compile())
            {

                _propertyName = PropertySupport.TryExtractPropertyName(itemGeneratorExpression);
                _sourceReference = new TomsToolbox.Core.WeakReference<IList<TSource>>(sourceCollection);

                if (_propertyName == null)
                    return;

                _propertyChangeEventListeners = GeneratePropertyChangeEventListeners(sourceCollection);
            }

            [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Caused by code contracts.")]
            [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
            protected override void OnSourceCollectionChanged(IEnumerable sourceCollection, NotifyCollectionChangedEventArgs e)
            {
                // ReSharper disable PossibleMultipleEnumeration
                base.OnSourceCollectionChanged(sourceCollection, e);

                if (_propertyChangeEventListeners == null) // no property change events watching....
                    return;

                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        var insertionIndex = e.NewStartingIndex;
                        foreach (var item in e.NewItems)
                        {
                            AttachEvent(insertionIndex++, item as INotifyPropertyChanged);
                        }
                        break;

                    case NotifyCollectionChangedAction.Move:
                        var weakEvent = _propertyChangeEventListeners[e.OldStartingIndex];
                        _propertyChangeEventListeners.RemoveAt(e.OldStartingIndex);
                        _propertyChangeEventListeners.Insert(e.NewStartingIndex, weakEvent);
                        break;


                    case NotifyCollectionChangedAction.Remove:
                        var removeIndex = e.OldStartingIndex;
                        for (var k = 0; k < e.OldItems?.Count; k++)
                        {
                            DetachEvent(removeIndex);
                        }
                        break;


                    case NotifyCollectionChangedAction.Replace:
                        var replaceIndex = e.OldStartingIndex;
                        foreach (var item in e.NewItems)
                        {
                            DetachEvent(replaceIndex);
                            AttachEvent(replaceIndex++, item as INotifyPropertyChanged);
                        }
                        break;

                    case NotifyCollectionChangedAction.Reset:
                        _propertyChangeEventListeners.ForEach(item => item?.Detach());
                        _propertyChangeEventListeners = GeneratePropertyChangeEventListeners(sourceCollection);
                        break;
                }
                // ReSharper enable PossibleMultipleEnumeration
            }

            private void SourceItem_PropertyChanged([NotNull] object sender, [NotNull] PropertyChangedEventArgs e)
            {

                if (e.PropertyName != _propertyName)
                    return;

                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (!_sourceReference.TryGetTarget(out var sourceCollection))
                    // ReSharper disable once HeuristicUnreachableCode
                    return;

                // ReSharper disable once PossibleNullReferenceException
                var index = sourceCollection.IndexOf((TSource)sender);

                if (index == -1)
                    return;

                Items[index] = ItemGenerator((TSource)sender);
            }

            private void DetachEvent(int itemIndex)
            {

                var propertyChangeEventListeners = _propertyChangeEventListeners;

                if (propertyChangeEventListeners == null)
                    return;

                propertyChangeEventListeners[itemIndex].Detach();
                propertyChangeEventListeners.RemoveAt(itemIndex);
            }

            private void AttachEvent(int itemIndex, [CanBeNull] INotifyPropertyChanged item)
            {

                var weakEventListener = GeneratePropertyChangedEventListener(item);

                if (itemIndex == -1)
                {
                    _propertyChangeEventListeners.Add(weakEventListener);
                }
                else
                {
                    _propertyChangeEventListeners.Insert(itemIndex, weakEventListener);
                }
            }

            [CanBeNull]
            private WeakEventListener<ObservableSelectImpl<TSource, TTarget>, INotifyPropertyChanged, PropertyChangedEventArgs> GeneratePropertyChangedEventListener([CanBeNull] INotifyPropertyChanged item)
            {
                return (item == null) ? null :
                    new WeakEventListener<ObservableSelectImpl<TSource, TTarget>, INotifyPropertyChanged, PropertyChangedEventArgs>(this,item, OnCollectionChanged, Attach, Detach);
            }

            private static void OnCollectionChanged([NotNull, ItemCanBeNull] ObservableSelectImpl<TSource, TTarget> self, [NotNull] object sender, [NotNull] PropertyChangedEventArgs e)
            {

                self.SourceItem_PropertyChanged(sender, e);
            }

            private static void Attach([NotNull] WeakEventListener<ObservableSelectImpl<TSource, TTarget>, INotifyPropertyChanged, PropertyChangedEventArgs> weakEvent, [NotNull] INotifyPropertyChanged sender)
            {

                sender.PropertyChanged += weakEvent.OnEvent;
            }

            private static void Detach([NotNull] WeakEventListener<ObservableSelectImpl<TSource, TTarget>, INotifyPropertyChanged, PropertyChangedEventArgs> weakEvent, [NotNull] INotifyPropertyChanged sender)
            {

                sender.PropertyChanged -= weakEvent.OnEvent;
            }

            [NotNull, ItemCanBeNull]
            private List<WeakEventListener<ObservableSelectImpl<TSource, TTarget>, INotifyPropertyChanged, PropertyChangedEventArgs>> GeneratePropertyChangeEventListeners([NotNull, ItemCanBeNull] IEnumerable sourceList)
            {

                return sourceList.Cast<object>()
                    .Select(item => GeneratePropertyChangedEventListener(item as INotifyPropertyChanged))
                    .ToList();
            }
        }

        private class ObservableSelectManyImpl<TSource, TTarget> : ReadOnlyObservableCollectionAdapter<TTarget, ObservableCompositeCollection<TTarget>>
        {
            [NotNull, ItemCanBeNull]
            private readonly IObservableCollection<IList<TTarget>> _source;

            public ObservableSelectManyImpl([NotNull, ItemCanBeNull] IList<TSource> items, [NotNull] Expression<Func<TSource, IList<TTarget>>> itemGeneratorExpression)
                : base(new ObservableCompositeCollection<TTarget>())
            {

                _source = items.ObservableSelect(itemGeneratorExpression);
                _source.CollectionChanged += Source_CollectionChanged;

                Items.Content.AddRange(_source);
            }
            [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
            private void Source_CollectionChanged([CanBeNull] object sender, [NotNull] NotifyCollectionChangedEventArgs e)
            {

                var newStartingIndex = e.NewStartingIndex;
                var oldStartingIndex = e.OldStartingIndex;
                var content = Items.Content;

                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        foreach (IList<TTarget> item in e.NewItems)
                        {
                            content.Insert(newStartingIndex++, item);
                        }
                        break;

                    case NotifyCollectionChangedAction.Remove:
                        for (var i = 0; i < e.OldItems.Count; i++)
                        {
                            content.RemoveAt(oldStartingIndex);
                        }
                        break;

                    case NotifyCollectionChangedAction.Reset:
                        content.Clear();
                        content.AddRange(_source);
                        break;

                    case NotifyCollectionChangedAction.Replace:
                        foreach (IList<TTarget> item in e.NewItems)
                        {
                            content[newStartingIndex++] = item;
                        }
                        break;

                    case NotifyCollectionChangedAction.Move:
                        content.RemoveAt(oldStartingIndex);
                        content.Insert(newStartingIndex, e.NewItems.Cast<IList<TTarget>>().Single());
                        break;

                    default:
                        throw new InvalidOperationException("Invalid NotifyCollectionChangedEventArgs.Action");
                }
            }
        }
    }
}
