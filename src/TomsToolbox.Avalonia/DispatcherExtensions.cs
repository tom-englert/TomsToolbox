namespace TomsToolbox.Wpf;

/// <summary>
/// Extensions for the <see cref="Dispatcher"/> class.
/// </summary>
public static class DispatcherExtensions
{
    /// <summary>Gets the <see cref="Dispatcher" /> for the application's main thread. In Avalonia this is always the UIThread</summary>
    /// <returns>The dispatcher associated with the UI thread.</returns>
    public static Dispatcher CurrentDispatcher => Dispatcher.UIThread;

    /// <summary>Gets the <see cref="Dispatcher" /> for the application's main thread.</summary>
    /// <returns>The dispatcher associated with applications main thread.</returns>
    public static Dispatcher UIThreadDispatcher => Dispatcher.UIThread;

    /// <summary>
    /// Executes the specified method on the dispatcher thread asynchronously, using the WPF-style naming convention.
    /// </summary>
    /// <param name="dispatcher">The dispatcher.</param>
    /// <param name="method">The method.</param>
    /// <param name="priority">The priority</param>
    public static void BeginInvoke(this Dispatcher dispatcher, Action method, DispatcherPriority priority = default)
    {
        dispatcher.Post(method, priority);
    }

    /// <summary>
    /// Executes the specified action on the dispatcher thread asynchronously, using the WPF-style naming convention.
    /// </summary>
    /// <param name="dispatcher">The dispatcher.</param>
    /// <param name="method">The action.</param>
    /// <param name="priority">The priority</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static void BeginInvoke(this Dispatcher dispatcher, DispatcherPriority priority, Action method)
    {
        dispatcher.BeginInvoke(method, priority);
    }
}
