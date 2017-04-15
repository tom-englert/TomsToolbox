namespace TomsToolbox.Wpf
{
    using System.Diagnostics.Contracts;
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
                Contract.Ensures(Contract.Result<RoutedUICommand>() != null);
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
                Contract.Ensures(Contract.Result<RoutedUICommand>() != null);
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
                Contract.Ensures(Contract.Result<RoutedUICommand>() != null);
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
                Contract.Ensures(Contract.Result<RoutedUICommand>() != null);
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
                Contract.Ensures(Contract.Result<RoutedUICommand>() != null);
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
                Contract.Ensures(Contract.Result<RoutedUICommand>() != null);
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
                Contract.Ensures(Contract.Result<RoutedUICommand>() != null);
                return _abort;
            }
        }
    }
}
