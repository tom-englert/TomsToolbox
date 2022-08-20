namespace TomsToolbox.ObservableCollections;

using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

/// <summary>
/// Combines <see cref="IList{T}" />, <see cref="INotifyCollectionChanged" /> and <see cref="INotifyPropertyChanged" /> into a single interface.
/// </summary>
/// <typeparam name="T">The type of elements in the list.</typeparam>
public interface IObservableCollection<T> : IList<T>, INotifyCollectionChanged, INotifyPropertyChanged
{
}