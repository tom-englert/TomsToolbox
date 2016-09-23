namespace TomsToolbox.Core
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Event arguments for events that deal with text, e.g. text changed or text received.
    /// </summary>
    public class TextEventArgs : EventArgs
    {
        private readonly string _text;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextEventArgs"/> class.
        /// </summary>
        /// <param name="text">The text associated with the event.</param>
        public TextEventArgs(string text)
        {
            Contract.Requires(text != null);

            _text = text;
        }

        /// <summary>
        /// Gets the text associated with the event.
        /// </summary>
        public string Text
        {
            get
            {
                Contract.Ensures(Contract.Result<string>() != null);

                return _text;
            }
        }

        [ContractInvariantMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant()
        {
            Contract.Invariant(_text != null);
        }
    }
}
