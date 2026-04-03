namespace TomsToolbox.Wpf;

/// <summary>
/// Like <see cref="ObservableObjectBase" />, with an additional dispatcher field to track the owning thread.
/// This version is not serializable, since <see cref="Dispatcher"/> is not.
/// </summary>
/// <seealso cref="ObservableObjectBase" />
public abstract class ObservableObject : ObservableObjectBase
{
    /// <summary>
    /// Gets the dispatcher of the thread where this object was created.
    /// </summary>
    public Dispatcher Dispatcher { get; } = DispatcherExtensions.CurrentDispatcher;
}
