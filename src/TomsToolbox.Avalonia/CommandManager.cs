using TomsToolbox.Essentials;

namespace TomsToolbox.Wpf;

/// <summary>
/// Provides a substitute for the WPF <c>System.Windows.Input.CommandManager</c> for use in Avalonia applications.
/// Unlike the WPF <c>CommandManager</c>, which automatically raises <see cref="RequerySuggested"/> in response
/// to various input events, this implementation requires explicit calls to <see cref="InvalidateRequerySuggested"/>
/// to notify commands that their <c>CanExecute</c> state may have changed.
/// </summary>
/// <remarks>
/// Commands that rely on <see cref="RequerySuggested"/> to re-evaluate their <c>CanExecute</c> logic should
/// subscribe to this event. Subscribers are held via weak references, so commands do not need to unsubscribe
/// to avoid memory leaks.
/// </remarks>
public static class CommandManager
{
    private static readonly WeakEventSource _source = new();

    /// <summary>
    /// Occurs when the <see cref="CommandManager"/> detects conditions that might change
    /// the ability of a command to execute.
    /// </summary>
    /// <remarks>
    /// Subscribers are held via weak references. Call <see cref="InvalidateRequerySuggested"/>
    /// to raise this event and trigger re-evaluation of command states.
    /// </remarks>
    public static event EventHandler RequerySuggested
    {
        add => _source.Subscribe(value);
        remove => _source.Unsubscribe(value);
    }

    /// <summary>
    /// Raises the <see cref="RequerySuggested"/> event, signalling to all subscribed commands
    /// that they should re-evaluate their <c>CanExecute</c> state.
    /// </summary>
    /// <remarks>
    /// Call this method after any state change that may affect whether one or more commands
    /// can execute — for example, after modifying a collection, completing an async operation,
    /// or changing a property that a command's <c>CanExecute</c> depends on.
    /// </remarks>
    public static void InvalidateRequerySuggested()
    {
        DispatcherExtensions.CurrentDispatcher.BeginInvoke(() => _source.Raise(null, EventArgs.Empty), DispatcherPriority.Background);
    }
}
