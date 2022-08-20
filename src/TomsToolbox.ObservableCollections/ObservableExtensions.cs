namespace TomsToolbox.ObservableCollections;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

using TomsToolbox.Essentials;

using WeakEventHandler;

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
    /// The selector must always return the same object for the source, else removing elements will fail!
    /// </remarks>
    public static IObservableCollection<TTarget> ObservableSelectMany<TSource, TTarget>(this IList<TSource> source, Expression<Func<TSource, IList<TTarget>>> itemGeneratorExpression)
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
    public static IObservableCollection<TTarget> ObservableSelect<TSource, TTarget>(this IList<TSource> source, Expression<Func<TSource, TTarget>> itemGeneratorExpression)
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
    public static IObservableCollection<TTarget> ObservableCast<TTarget>(this IEnumerable source)
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
    public static IObservableCollection<T> ObservableWhere<T>(this IList<T> source, Func<T, bool> predicate, params string[] liveTrackingProperties)
    {
        return new ObservableFilteredCollection<T>(source, predicate, liveTrackingProperties);
    }

    private class ObservableSelectImpl<TSource, TTarget> : ObservableWrappedCollection<TSource, TTarget>
    {
        private readonly WeakReference<IList<TSource>> _sourceReference;
        private readonly string? _propertyName;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:TomsToolbox.ObservableCollections.ObservableExtensions.ObservableSelectImpl`2" /> class.
        /// </summary>
        /// <param name="sourceCollection">The source collection to wrap.</param>
        /// <param name="itemGeneratorExpression">The item generator to generate the wrapper for each item.</param>
        public ObservableSelectImpl(IList<TSource> sourceCollection, Expression<Func<TSource, TTarget>> itemGeneratorExpression)
            : base(sourceCollection, itemGeneratorExpression.Compile())
        {
            _propertyName = PropertySupport.TryExtractPropertyName(itemGeneratorExpression);
            _sourceReference = new WeakReference<IList<TSource>>(sourceCollection);

            if (_propertyName == null)
                return;

            sourceCollection.ForEach(item => AttachItemEvents(item as INotifyPropertyChanged));
        }

        protected override void OnSourceCollectionChanged(IEnumerable sourceCollection, NotifyCollectionChangedEventArgs e)
        {
            // ReSharper disable PossibleMultipleEnumeration
            base.OnSourceCollectionChanged(sourceCollection, e);

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    e.NewItems.OfType<INotifyPropertyChanged>().ForEach(AttachItemEvents);
                    break;

                case NotifyCollectionChangedAction.Move:
                    break;

                case NotifyCollectionChangedAction.Remove:
                    e.OldItems.OfType<INotifyPropertyChanged>().ForEach(DetachItemEvents);
                    break;


                case NotifyCollectionChangedAction.Replace:
                    e.OldItems.OfType<INotifyPropertyChanged>().ForEach(DetachItemEvents);
                    e.NewItems.OfType<INotifyPropertyChanged>().ForEach(AttachItemEvents);
                    break;

                case NotifyCollectionChangedAction.Reset:
                    sourceCollection.OfType<INotifyPropertyChanged>().ForEach(AttachItemEvents);
                    break;
            }
            // ReSharper restore PossibleMultipleEnumeration
        }

        [MakeWeak]
        private void SourceItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != _propertyName)
                return;

            if (!_sourceReference.TryGetTarget(out var sourceCollection))
                return;

            var index = sourceCollection.IndexOf((TSource)sender);

            if (index == -1)
                return;

            Items[index] = ItemGenerator((TSource)sender);
        }

        private void AttachItemEvents(INotifyPropertyChanged? sender)
        {
            if (sender == null)
                return;

            sender.PropertyChanged += SourceItem_PropertyChanged;
        }

        private void DetachItemEvents(INotifyPropertyChanged? sender)
        {
            if (sender == null)
                return;

            sender.PropertyChanged -= SourceItem_PropertyChanged;
        }
    }

    private class ObservableSelectManyImpl<TSource, TTarget> : ReadOnlyObservableCollectionAdapter<TTarget, ObservableCompositeCollection<TTarget>>
    {
        private readonly IObservableCollection<IList<TTarget>> _source;

        public ObservableSelectManyImpl(IList<TSource> items, Expression<Func<TSource, IList<TTarget>>> itemGeneratorExpression)
            : base(new ObservableCompositeCollection<TTarget>())
        {
            _source = items.ObservableSelect(itemGeneratorExpression);
            _source.CollectionChanged += Source_CollectionChanged;

            Items.Content.AddRange(_source);
        }
        private void Source_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
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