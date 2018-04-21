namespace TomsToolbox.Wpf
{
    using System.Windows.Input;

    using JetBrains.Annotations;

    /// <summary>
    /// Routed commands for dialog management.
    /// </summary>
    public static class DialogCommands
    {
        [NotNull] private static readonly RoutedUICommand _ok = new RoutedUICommand("OK", "OK", typeof(DialogCommands));
        [NotNull] private static readonly RoutedUICommand _cancel = new RoutedUICommand("Cancel", "Cancel", typeof(DialogCommands));
        [NotNull] private static readonly RoutedUICommand _yes = new RoutedUICommand("Yes", "Yes", typeof(DialogCommands));
        [NotNull] private static readonly RoutedUICommand _no = new RoutedUICommand("No", "No", typeof(DialogCommands));
        [NotNull] private static readonly RoutedUICommand _ignore = new RoutedUICommand("Ignore", "Ignore", typeof(DialogCommands));
        [NotNull] private static readonly RoutedUICommand _retry = new RoutedUICommand("Retry", "Retry", typeof(DialogCommands));
        [NotNull] private static readonly RoutedUICommand _abort = new RoutedUICommand("Abort", "Abort", typeof(DialogCommands));

        /// <summary>
        /// Gets the OK Dialog command.
        /// </summary>
        [NotNull]
        public static RoutedUICommand Ok
        {
            get
            {
                return _ok;
            }
        }

        /// <summary>
        /// Gets the Cancel Dialog command.
        /// </summary>
        [NotNull]
        public static RoutedUICommand Cancel
        {
            get
            {
                return _cancel;
            }
        }

        /// <summary>
        /// Gets the Yes Dialog command.
        /// </summary>
        [NotNull]
        public static RoutedUICommand Yes
        {
            get
            {
                return _yes;
            }
        }

        /// <summary>
        /// Gets the No Dialog command.
        /// </summary>
        [NotNull]
        public static RoutedUICommand No
        {
            get
            {
                return _no;
            }
        }

        /// <summary>
        /// Gets the Ignore Dialog command.
        /// </summary>
        [NotNull]
        public static RoutedUICommand Ignore
        {
            get
            {
                return _ignore;
            }
        }

        /// <summary>
        /// Gets the Retry Dialog command.
        /// </summary>
        [NotNull]
        public static RoutedUICommand Retry
        {
            get
            {
                return _retry;
            }
        }

        /// <summary>
        /// Gets the Abort Dialog command.
        /// </summary>
        [NotNull]
        public static RoutedUICommand Abort
        {
            get
            {
                return _abort;
            }
        }
    }
}
