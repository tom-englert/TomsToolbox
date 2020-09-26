namespace TomsToolbox.Wpf
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text.RegularExpressions;

    using JetBrains.Annotations;

    /// <summary>
    /// Helper class to detect binding errors during debugging.<para/> 
    /// All functionality is only active if a debugger is attached.<para/> 
    /// </summary>
    public static class BindingErrorTracer
    {
        /// <summary>
        /// The errors to be ignored during error handling.
        /// </summary>
        [NotNull, ItemNotNull]
        public static readonly IList<Regex> IgnoredErrors = new List<Regex>
        {
            // There are no more validation errors, but Validation.HasError is still true!
            new Regex(@"Cannot get 'Item\[\]' value \(type 'ValidationError'\) from '\(Validation.Errors\)' \(type 'ReadOnlyObservableCollection`1'\)."),

            // Validation via Exception
            new Regex(@"System\.Windows\.Data Error: 8 : Cannot save value from target back to source\."),

            // Frequently happens when overriding the ItemContainerStyle; uncritical, but often impossible to avoid.
            new Regex(@"System\.Windows\.Data Error: 4 : Cannot find source for binding with reference 'RelativeSource FindAncestor, AncestorType='System.Windows.Controls.ItemsControl', AncestorLevel='1''. BindingExpression:Path=(Horizontal|Vertical)ContentAlignment; DataItem=null;"),

            // Bindings on behaviors...
            new Regex(@"System\.Windows\.Data Error: 2 : Cannot find governing FrameworkElement or FrameworkContentElement for target element\. BindingExpression:Path=\w+; DataItem='\w+' \(HashCode=.*\); target element is '\w+Behavior'")
        };

        /// <summary>
        /// Hooks into <see cref="PresentationTraceSources" /> to listen for binding errors with <see cref="SourceLevels.Warning"/>.
        /// </summary>
        /// <param name="errorCallback">The callback that is called in case of binding errors, to show an error message or throw an exception</param>
        public static void Start([NotNull] Action<string> errorCallback)
        {
            Start(errorCallback, SourceLevels.Warning);
        }

        /// <summary>
        /// Hooks into <see cref="PresentationTraceSources" /> to listen for binding errors.
        /// </summary>
        /// <param name="errorCallback">The callback that is called in case of binding errors, to show an error message or throw an exception</param>
        /// <param name="sourceLevels">The source levels that trigger a warning.</param>
        public static void Start([NotNull] Action<string> errorCallback, SourceLevels sourceLevels)
        {
            if (!Debugger.IsAttached)
                return;

            PresentationTraceSources.Refresh();

            var dataBindingSource = PresentationTraceSources.DataBindingSource;

            var listeners = dataBindingSource?.Listeners;
            listeners?.Add(new Listener(errorCallback));

            var sourceSwitch = dataBindingSource?.Switch;
            if (sourceSwitch != null)
                sourceSwitch.Level = sourceLevels;
        }

        class Listener : TraceListener
        {
            [NotNull]
            private readonly Action<string> _errorCallback;
            [CanBeNull]
            private string? _buffer;

            public Listener([NotNull] Action<string> errorCallback)
            {
                _errorCallback = errorCallback;
            }

            /// <summary>
            /// When overridden in a derived class, writes the specified message to the listener you create in the derived class.
            /// </summary>
            /// <param name="message">A message to write. </param>
            public override void Write([CanBeNull] string? message)
            {
                _buffer += message;
            }

            /// <summary>
            /// When overridden in a derived class, writes a message to the listener you create in the derived class, followed by a line terminator.
            /// </summary>
            /// <param name="message">A message to write. </param>
            public override void WriteLine([CanBeNull] string? message)
            {
                _buffer += message;

                try
                {
                    if (IgnoredErrors.Any(err => err.Match(_buffer).Success))
                        return;

                    _errorCallback(_buffer);
                }
                finally 
                {
                    _buffer = null;
                }
            }
        }
    }
}
