namespace TomsToolbox.Wpf
{
    using System.Diagnostics.Contracts;
    using System.Windows.Input;

    /// <summary>
    /// Routed commands for window management.
    /// </summary>
    public static class WindowCommands
    {
        private static readonly RoutedUICommand _minimize = new RoutedUICommand("Minimize", "Minimize", typeof (WindowCommands));
        private static readonly RoutedUICommand _maximize = new RoutedUICommand("Maximize", "Maximize", typeof(WindowCommands));
        private static readonly RoutedUICommand _restore = new RoutedUICommand("Restore", "Restore", typeof(WindowCommands));
        private static readonly RoutedUICommand _close = new RoutedUICommand("Close", "Close", typeof(WindowCommands));

        /// <summary>
        /// Gets the minimize window command.
        /// </summary>
        public static RoutedUICommand Minimize
        {
            get
            {
                Contract.Ensures(Contract.Result<RoutedUICommand>() != null);
                return _minimize;
            }
        }

        /// <summary>
        /// Gets the maximize window command.
        /// </summary>
        public static RoutedUICommand Maximize
        {
            get
            {
                Contract.Ensures(Contract.Result<RoutedUICommand>() != null);
                return _maximize;
            }
        }

        /// <summary>
        /// Gets the close window command.
        /// </summary>
        public static RoutedUICommand Close
        {
            get
            {
                Contract.Ensures(Contract.Result<RoutedUICommand>() != null);
                return _close;
            }
        }

        /// <summary>
        /// Gets the restore window command.
        /// </summary>
        public static RoutedUICommand Restore
        {
            get
            {
                Contract.Ensures(Contract.Result<RoutedUICommand>() != null);
                return _restore;
            }
        }
    }
}
