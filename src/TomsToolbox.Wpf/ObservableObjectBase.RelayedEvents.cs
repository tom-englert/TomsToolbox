namespace TomsToolbox.Wpf;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using TomsToolbox.Essentials;

partial class ObservableObjectBase
{
    private static readonly AutoWeakIndexer<Type, IDictionary<Type, IDictionary<string, string>>> _relayMappingCache = new(RelayedEventAttribute.CreateRelayMapping!);
    [NonSerialized]
    private IDictionary<Type, IDictionary<string, string>>? _relayMapping;

    [NonSerialized]
    private Dictionary<Type, WeakReference<INotifyPropertyChanged>>? _eventSources;

    /// <summary>
    /// Relays the property changed events of the source object (if not null) and detaches the old source (if not null).
    /// </summary>
    /// <param name="oldSource"></param>
    /// <param name="newSource"></param>
    protected void RelayEventsOf(INotifyPropertyChanged? oldSource, INotifyPropertyChanged? newSource)
    {
        if (ReferenceEquals(oldSource, newSource))
            return;

        if (newSource != null)
        {
            RelayEventsOf(newSource);
        }
        else
        {
            DetachEventSource(oldSource!);
        }
    }

    /// <summary>
    /// Relays the property changed events of the source object.
    /// The properties to relay must be declared with the <see cref="RelayedEventAttribute"/>.
    /// </summary>
    /// <param name="source">The source.</param>
    protected void RelayEventsOf(INotifyPropertyChanged source)
    {
        var sourceType = source.GetType();
        if (RelayMapping.Keys.All(key => key?.IsAssignableFrom(sourceType) != true))
            throw new InvalidOperationException(@"This class has no property with a RelayedEventAttribute for the type " + sourceType);

        if (EventSources.TryGetValue(sourceType, out var oldListener) && oldListener.TryGetTarget(out var oldTarget))
        {
            oldTarget.PropertyChanged -= RelaySource_PropertyChanged;
        }

        source.PropertyChanged += RelaySource_PropertyChanged;

        EventSources[sourceType] = new WeakReference<INotifyPropertyChanged>(source);
    }

    /// <summary>
    /// Detaches all event sources.
    /// </summary>
    protected void DetachEventSources()
    {
        foreach (var item in EventSources.Values.Select(item => item.GetTargetOrDefault()).ExceptNullItems())
        {
            item.PropertyChanged -= RelaySource_PropertyChanged;
        }

        EventSources.Clear();
    }

    /// <summary>
    /// Detaches the event source.
    /// </summary>
    /// <param name="item">The item to detach.</param>
    protected void DetachEventSource(INotifyPropertyChanged item)
    {
        var sourceType = item.GetType();

        if (EventSources.TryGetValue(sourceType, out var oldListener) && oldListener.TryGetTarget(out var target))
        {
            target.PropertyChanged -= RelaySource_PropertyChanged;
            EventSources.Remove(sourceType);
        }
    }

    private Dictionary<Type, WeakReference<INotifyPropertyChanged>> EventSources => _eventSources ??= new Dictionary<Type, WeakReference<INotifyPropertyChanged>>();

    private IDictionary<Type, IDictionary<string, string>> RelayMapping => _relayMapping ??= _relayMappingCache[GetType()];

    [WeakEventHandler.MakeWeak]
    private void RelaySource_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender == null || e.PropertyName == null)
            return;

        var sourceType = sender.GetType();
        foreach (var mapping in RelayMapping.Where(item => item.Key.IsAssignableFrom(sourceType)).Select(item => item.Value))
        {
            if (mapping.TryGetValue(e.PropertyName, out var targetPropertyName) && !string.IsNullOrEmpty(targetPropertyName))
            {
                OnPropertyChanged(targetPropertyName);
            }
        }
    }

    /// <inheritdoc />
    ~ObservableObjectBase()
    {
        DetachEventSources();
    }
}
