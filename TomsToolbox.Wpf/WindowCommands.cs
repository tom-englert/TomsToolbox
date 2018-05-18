namespace TomsToolbox.Wpf
{
    using System.Windows.Input;

    using JetBrains.Annotations;

    /// <summary>
    /// Routed commands for window management.
    /// </summary>
    public static class WindowCommands
    {
        /// <summary>
        /// Gets the minimize window command.
        /// </summary>
        [NotNull]
        public static RoutedUICommand Minimize { get; } = new RoutedUICommand("Minimize", "Minimize", typeof (WindowCommands));

        /// <summary>
        /// Gets the maximize window command.
        /// </summary>
        [NotNull]
        public static RoutedUICommand Maximize { get; } = new RoutedUICommand("Maximize", "Maximize", typeof(WindowCommands));

        /// <summary>
        /// Gets the close window command.
        /// </summary>
        [NotNull]
        public static RoutedUICommand Close { get; } = new RoutedUICommand("Close", "Close", typeof(WindowCommands));

        /// <summary>
        /// Gets the restore window command.
        /// </summary>
        [NotNull]
        public static RoutedUICommand Restore { get; } = new RoutedUICommand("Restore", "Restore", typeof(WindowCommands));
    }
}
