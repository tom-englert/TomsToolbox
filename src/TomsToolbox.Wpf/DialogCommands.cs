namespace TomsToolbox.Wpf;

using System.Windows.Input;

/// <summary>
/// Routed commands for dialog management.
/// </summary>
public static class DialogCommands
{
    /// <summary>
    /// Gets the OK Dialog command.
    /// </summary>
    public static RoutedUICommand Ok { get; } = new("OK", "OK", typeof(DialogCommands));

    /// <summary>
    /// Gets the Cancel Dialog command.
    /// </summary>
    public static RoutedUICommand Cancel { get; } = new("Cancel", "Cancel", typeof(DialogCommands));

    /// <summary>
    /// Gets the Yes Dialog command.
    /// </summary>
    public static RoutedUICommand Yes { get; } = new("Yes", "Yes", typeof(DialogCommands));

    /// <summary>
    /// Gets the No Dialog command.
    /// </summary>
    public static RoutedUICommand No { get; } = new("No", "No", typeof(DialogCommands));

    /// <summary>
    /// Gets the Ignore Dialog command.
    /// </summary>
    public static RoutedUICommand Ignore { get; } = new("Ignore", "Ignore", typeof(DialogCommands));

    /// <summary>
    /// Gets the Retry Dialog command.
    /// </summary>
    public static RoutedUICommand Retry { get; } = new("Retry", "Retry", typeof(DialogCommands));

    /// <summary>
    /// Gets the Abort Dialog command.
    /// </summary>
    public static RoutedUICommand Abort { get; } = new("Abort", "Abort", typeof(DialogCommands));
}