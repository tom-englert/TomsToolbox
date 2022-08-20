namespace TomsToolbox.Wpf;

using System.Windows.Input;

/// <summary>
/// Routed commands for window management.
/// </summary>
public static class WindowCommands
{
    /// <summary>
    /// Gets the minimize window command.
    /// </summary>
    public static RoutedUICommand Minimize { get; } = new("Minimize", "Minimize", typeof (WindowCommands));

    /// <summary>
    /// Gets the maximize window command.
    /// </summary>
    public static RoutedUICommand Maximize { get; } = new("Maximize", "Maximize", typeof(WindowCommands));

    /// <summary>
    /// Gets the close window command.
    /// </summary>
    public static RoutedUICommand Close { get; } = new("Close", "Close", typeof(WindowCommands));

    /// <summary>
    /// Gets the restore window command.
    /// </summary>
    public static RoutedUICommand Restore { get; } = new("Restore", "Restore", typeof(WindowCommands));
}