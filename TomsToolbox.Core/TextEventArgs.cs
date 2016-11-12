namespace TomsToolbox.Core
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    using JetBrains.Annotations;

    /// <summary>
    /// Event arguments for events that deal with text, e.g. text changed or text received.
    /// </summary>
    public class TextEventArgs : EventArgs
    {
        [NotNull]
        private readonly string _text;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextEventArgs"/> class.
        /// </summary>
        /// <param name="text">The text associated with the event.</param>
        public TextEventArgs([NotNull] string text)
        {
            Contract.Requires(text != null);

            _text = text;
        }

        /// <summary>
        /// Gets the text associated with the event.
        /// </summary>
        [NotNull]
        public string Text
        {
            get
            {
                Contract.Ensures(Contract.Result<string>() != null);

                return _text;
            }
        }

#if !PORTABLE
        [ContractInvariantMethod]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        [Conditional("CONTRACTS_FULL")]
        private void ObjectInvariant()
        {
            Contract.Invariant(_text != null);
        }
#endif
    }
}
