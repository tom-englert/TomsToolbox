namespace TomsToolbox.Wpf.Interactivity;

using System.Windows;
using System.Windows.Input;

using Microsoft.Xaml.Behaviors;

/// <summary>
/// Attaches default handling for the <see cref="WindowCommands"/>
/// </summary>
public class WindowButtonsHandlingBehavior : Behavior<DependencyObject>
{
    private Window? _window;
    /// <summary>
    /// Called after the behavior is attached to an AssociatedObject.
    /// </summary>
    /// <remarks>
    /// Override this to hook up functionality to the AssociatedObject.
    /// </remarks>
    protected override void OnAttached()
    {
        base.OnAttached();

        var window = _window = AssociatedObject as Window ?? Window.GetWindow(AssociatedObject);
        if (window == null)
            return;

        window.CommandBindings.Add(new CommandBinding(WindowCommands.Close, Close));

        if (window.ResizeMode == ResizeMode.NoResize)
            return;

        // to ensure min/max/restore buttons are updated correctly.
        window.StateChanged += (_, _) => CommandManager.InvalidateRequerySuggested();

        window.CommandBindings.Add(new CommandBinding(WindowCommands.Minimize, Minimize));
        window.CommandBindings.Add(new CommandBinding(WindowCommands.Maximize, Maximize, CanMaximize));
        window.CommandBindings.Add(new CommandBinding(WindowCommands.Restore, Restore, CanRestore));
    }

    private void Minimize(object? sender, ExecutedRoutedEventArgs? e)
    {
        var window = _window;
        if (window == null)
            return;

        window.WindowState = WindowState.Minimized;
    }

    private void Close(object? sender, ExecutedRoutedEventArgs? e)
    {
        _window?.Close();
    }

    private void CanRestore(object? sender, CanExecuteRoutedEventArgs e)
    {
        var window = _window;
        if (window == null)
            return;

        e.CanExecute = window.WindowState != WindowState.Normal;
    }

    private void Restore(object? sender, ExecutedRoutedEventArgs? e)
    {
        var window = _window;
        if (window == null)
            return;

        window.WindowState = WindowState.Normal;
    }

    private void CanMaximize(object? sender, CanExecuteRoutedEventArgs e)
    {
        var window = _window;
        if (window == null)
            return;

        e.CanExecute = (window.WindowState == WindowState.Normal) && ((window.ResizeMode == ResizeMode.CanResize) || (window.ResizeMode == ResizeMode.CanResizeWithGrip));
    }

    private void Maximize(object? sender, ExecutedRoutedEventArgs? e)
    {
        var window = _window;
        if (window == null)
            return;

        window.WindowState = WindowState.Maximized;
    }
}