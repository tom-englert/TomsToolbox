namespace TomsToolbox.Wpf
{
    using System.Diagnostics.Contracts;
    using System.Windows.Input;

    /// <summary>
    /// Routed commands for dialog management.
    /// </summary>
    public static class DialogCommands
    {
        private static readonly RoutedUICommand _ok = new RoutedUICommand("OK", "OK", typeof(DialogCommands));
        private static readonly RoutedUICommand _cancel = new RoutedUICommand("Cancel", "Cancel", typeof(DialogCommands));
        private static readonly RoutedUICommand _yes = new RoutedUICommand("Yes", "Yes", typeof(DialogCommands));
        private static readonly RoutedUICommand _no = new RoutedUICommand("No", "No", typeof(DialogCommands));
        private static readonly RoutedUICommand _ignore = new RoutedUICommand("Ignore", "Ignore", typeof(DialogCommands));
        private static readonly RoutedUICommand _retry = new RoutedUICommand("Retry", "Retry", typeof(DialogCommands));
        private static readonly RoutedUICommand _abort = new RoutedUICommand("Abort", "Abort", typeof(DialogCommands));

        /// <summary>
        /// Gets the OK Dialog command.
        /// </summary>
        public static RoutedUICommand OK
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
