namespace TomsToolbox.ObservableCollections
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;

    using JetBrains.Annotations;

    /// <summary>
    /// Similar to the <see cref="ReadOnlyObservableCollection{T}" />, except it does not require the items
    /// collection to be an <see cref="ObservableCollection{T}" /> but only an <see cref="IList{T}" /> that implements also INotifyCollectionChanged.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <typeparam name="TList">The type of the list to wrap.</typeparam>
    public abstract class ReadOnlyObservableCollectionAdapter<T, TList> : ReadOnlyCollection<T>, IObservableCollection<T> 
        where TList : class, IList<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyObservableCollectionAdapter{T, TList}"/> class.
        /// </summary>
        /// <param name="items">The items.</param>
        protected ReadOnlyObservableCollectionAdapter([NotNull] TList items)
            : base(items)
        {

            items.CollectionChanged += Items_CollectionChanged;
            items.PropertyChanged += Items_PropertyChanged;
        }

        /// <summary>
        /// Returns the collection that the <see cref="ReadOnlyObservableCollectionAdapter{T, TList}"/> wraps.
        /// </summary>
        [NotNull]
        protected new TList Items
        {
            get
            {

                return (TList)base.Items;
            }
        }

        /// <summary>
        /// Occurs when the collection changes.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the <see cref="INotifyPropertyChanged.PropertyChanged" /> event.
        /// </summary>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnPropertyChanged([CanBeNull] PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the <see cref="INotifyCollectionChanged.CollectionChanged" /> event.
        /// </summary>
        /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnCollectionChanged([CanBeNull] NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(this, e);
        }

        private void Items_PropertyChanged([CanBeNull] object sender, [CanBeNull] PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e);
        }

        private void Items_CollectionChanged([CanBeNull] object sender, [CanBeNull] NotifyCollectionChangedEventArgs e)
        {
            OnCollectionChanged(e);
        }
    }
}
