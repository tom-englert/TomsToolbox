namespace TomsToolbox.Wpf
{
    using System.Diagnostics.Contracts;
    using System.Windows.Input;

    using JetBrains.Annotations;

    /// <summary>
    /// Routed commands for window management.
    /// </summary>
    public static class WindowCommands
    {
        [NotNull] private static readonly RoutedUICommand _minimize = new RoutedUICommand("Minimize", "Minimize", typeof (WindowCommands));
        [NotNull] private static readonly RoutedUICommand _maximize = new RoutedUICommand("Maximize", "Maximize", typeof(WindowCommands));
        [NotNull] private static readonly RoutedUICommand _restore = new RoutedUICommand("Restore", "Restore", typeof(WindowCommands));
        [NotNull] private static readonly RoutedUICommand _close = new RoutedUICommand("Close", "Close", typeof(WindowCommands));

        /// <summary>
        /// Gets the minimize window command.
        /// </summary>
        [NotNull]
        public static RoutedUICommand Minimize
        {
            get
            {
                return _minimize;
            }
        }

        /// <summary>
        /// Gets the maximize window command.
        /// </summary>
        [NotNull]
        public static RoutedUICommand Maximize
        {
            get
            {
                return _maximize;
            }
        }

        /// <summary>
        /// Gets the close window command.
        /// </summary>
        [NotNull]
        public static RoutedUICommand Close
        {
            get
            {
                return _close;
            }
        }

        /// <summary>
        /// Gets the restore window command.
        /// </summary>
        [NotNull]
        public static RoutedUICommand Restore
        {
            get
            {
                return _restore;
            }
        }
    }
}
