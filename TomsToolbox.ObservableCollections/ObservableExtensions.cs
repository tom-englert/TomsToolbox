namespace TomsToolbox.ObservableCollections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Linq.Expressions;

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
        public static IObservableCollection<TTarget> ObservableSelectMany<TSource, TTarget>(this IList<TSource> source, Expression<Func<TSource, IList<TTarget>>> itemGeneratorExpression)
        {
            Contract.Requires(source != null);
            Contract.Requires(itemGeneratorExpression != null);
            Contract.Ensures(Contract.Result<IObservableCollection<TTarget>>() != null);

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
        public static IObservableCollection<TTarget> ObservableSelect<TSource, TTarget>(this IList<TSource> source, Expression<Func<TSource, TTarget>> itemGeneratorExpression)
        {
            Contract.Requires(source != null);
            Contract.Requires(itemGeneratorExpression != null);
            Contract.Ensures(Contract.Result<IObservableCollection<TTarget>>() != null);

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
        public static IObservableCollection<TTarget> ObservableCast<TTarget>(this IEnumerable source)
        {
            Contract.Requires(source != null);
            Contract.Ensures(Contract.Result<IObservableCollection<TTarget>>() != null);

            return new ObservableWrappedCollection<object, TTarget>(source, item => (TTarget)item);
        }

        /// <summary>
        /// Returns an observable collection of objects of type <typeparamref name="T" /> that contains all items of the source collection that pass the filter.
        /// See <see cref="ObservableFilteredCollection{T}"/> for details.
        /// </summary>
        /// <typeparam name="T">The type of the target elements.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="predicate">The predicate used to filter.</param>
        /// <returns>
        /// The observable collection of objects of type <typeparamref name="T" /> that contains the filtered items of the source collection.
        /// </returns>
        /// <remarks>
        /// See <see cref="ObservableFilteredCollection{T}"/> for usage details.
        /// </remarks>
        public static IObservableCollection<T> ObservableWhere<T>(this IList<T> source, Func<T, bool> predicate)
        {
            Contract.Requires(source != null);
            Contract.Requires(predicate != null);
            Contract.Ensures(Contract.Result<IObservableCollection<T>>() != null);

            return new ObservableFilteredCollection<T>(source, predicate);
        }

        private class ObservableSelectImpl<TSource, TTarget> : ObservableWrappedCollection<TSource, TTarget>
        {
            private readonly WeakReference<IList<TSource>> _sourceReference;
            private readonly string _propertyName;
            private List<WeakEventListener<ObservableSelectImpl<TSource, TTarget>, INotifyPropertyChanged, PropertyChangedEventArgs>> _propertyChangeEventListeners;

            /// <summary>
            /// Initializes a new instance of the <see cref="ObservableWrappedCollection&lt;TTarget, TSource&gt;" /> class.
            /// </summary>
            /// <param name="sourceCollection">The source collection to wrap.</param>
            /// <param name="itemGeneratorExpression">The item generator to generate the wrapper for each item.</param>
            public ObservableSelectImpl(IList<TSource> sourceCollection, Expression<Func<TSource, TTarget>> itemGeneratorExpression)
                : base(sourceCollection, itemGeneratorExpression.Compile())
            {
                Contract.Requires(sourceCollection != null);
                Contract.Requires(itemGeneratorExpression != null);

                _propertyName = PropertySupport.TryExtractPropertyName(itemGeneratorExpression);
                _sourceReference = new WeakReference<IList<TSource>>(sourceCollection);

                if (_propertyName == null)
                    return;

                _propertyChangeEventListeners = GeneratePropertyChangeEventListeners(sourceCollection);
            }

            [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Caused by code contracts.")]
            [ContractVerification(false)]
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
                        for (var k = 0; k < e.OldItems.Count; k++)
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
                        _propertyChangeEventListeners.ForEach(item => item.SafeDetach());
                        _propertyChangeEventListeners = GeneratePropertyChangeEventListeners(sourceCollection);
                        break;
                }
                // ReSharper enable PossibleMultipleEnumeration
            }

            private void SourceItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                Contract.Requires(sender != null);

                if (e.PropertyName != _propertyName)
                    return;

                IList<TSource> sourceCollection;
                if (!_sourceReference.TryGetTarget(out sourceCollection))
                    return;

                var index = sourceCollection.IndexOf((TSource)sender);

                if (index == -1)
                    return;

                Contract.Assume(index < Items.Count);

                Items[index] = ItemGenerator((TSource)sender);
            }

            private void DetachEvent(int itemIndex)
            {
                Contract.Requires(_propertyChangeEventListeners != null);
                Contract.Requires((itemIndex >= 0) && (itemIndex < _propertyChangeEventListeners.Count));
                Contract.Ensures(_propertyChangeEventListeners.Count == Contract.OldValue(_propertyChangeEventListeners.Count) - 1);

                _propertyChangeEventListeners[itemIndex].SafeDetach();
                _propertyChangeEventListeners.RemoveAt(itemIndex);
            }

            private void AttachEvent(int itemIndex, INotifyPropertyChanged item)
            {
                Contract.Requires(_propertyChangeEventListeners != null);
                Contract.Requires(itemIndex >= -1);
                Contract.Requires(itemIndex <= _propertyChangeEventListeners.Count);

                var weakEventListener = GeneratePropertyChangedEventListener(item);

                Contract.Assume(_propertyChangeEventListeners != null); // else we didn't register for events.

                if (itemIndex == -1)
                {
                    _propertyChangeEventListeners.Add(weakEventListener);
                }
                else
                {
                    Contract.Assume(itemIndex <= _propertyChangeEventListeners.Count);
                    _propertyChangeEventListeners.Insert(itemIndex, weakEventListener);
                }
            }

            [ContractVerification(false)]
            private WeakEventListener<ObservableSelectImpl<TSource, TTarget>, INotifyPropertyChanged, PropertyChangedEventArgs> GeneratePropertyChangedEventListener(INotifyPropertyChanged item)
            {
                return (item == null) ? null :
                    new WeakEventListener<ObservableSelectImpl<TSource, TTarget>, INotifyPropertyChanged, PropertyChangedEventArgs>(
                        this,
                        item,
                        (self, sender, e) => self.SourceItem_PropertyChanged(sender, e),
                        (weakEvent, sender) => sender.PropertyChanged += weakEvent.OnEvent,
                        (weakEvent, sender) => sender.PropertyChanged -= weakEvent.OnEvent);
            }

            private List<WeakEventListener<ObservableSelectImpl<TSource, TTarget>, INotifyPropertyChanged, PropertyChangedEventArgs>> GeneratePropertyChangeEventListeners(IEnumerable sourceList)
            {
                Contract.Requires(sourceList != null);
                Contract.Ensures(Contract.Result<List<WeakEventListener<ObservableSelectImpl<TSource, TTarget>, INotifyPropertyChanged, PropertyChangedEventArgs>>>() != null);

                return sourceList.Cast<object>()
                    .Select(item => GeneratePropertyChangedEventListener(item as INotifyPropertyChanged))
                    .ToList();
            }

            [ContractInvariantMethod]
            [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
            private void ObjectInvariant()
            {
                Contract.Invariant(_sourceReference != null);
            }
        }

        private static void SafeDetach<TSource, TTarget>(this WeakEventListener<ObservableSelectImpl<TSource, TTarget>, INotifyPropertyChanged, PropertyChangedEventArgs> eventListener)
        {
            if (eventListener != null)
                eventListener.Detach();
        }

        private class ObservableSelectManyImpl<TSource, TTarget> : ReadOnlyObservableCollectionAdapter<TTarget, ObservableCompositeCollection<TTarget>>
        {
            private readonly IObservableCollection<IList<TTarget>> _source;

            public ObservableSelectManyImpl(IList<TSource> items, Expression<Func<TSource, IList<TTarget>>> itemGeneratorExpression)
                : base(new ObservableCompositeCollection<TTarget>())
            {
                Contract.Requires(items != null);
                Contract.Requires(itemGeneratorExpression != null);

                _source = items.ObservableSelect(itemGeneratorExpression);
                _source.CollectionChanged += Source_CollectionChanged;

                Items.Content.AddRange(_source);
            }

            [ContractVerification(false)]
            private void Source_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                Contract.Requires(e != null);

                var newStartingIndex = e.NewStartingIndex;
                var oldStartingIndex = e.OldStartingIndex;
                var content = Items.Content;

                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        Contract.Assume(e.NewItems != null);
                        foreach (IList<TTarget> item in e.NewItems)
                        {
                            content.Insert(newStartingIndex++, item);
                        }
                        break;

                    case NotifyCollectionChangedAction.Remove:
                        Contract.Assume(e.OldItems != null);
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
                        Contract.Assume(e.NewItems != null);
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

            [ContractInvariantMethod]
            [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
            private void ObjectInvariant()
            {
                Contract.Invariant(_source != null);
            }
        }
    }
}
