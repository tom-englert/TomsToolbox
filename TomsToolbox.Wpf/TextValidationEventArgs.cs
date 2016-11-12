namespace TomsToolbox.Wpf
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    using JetBrains.Annotations;

    /// <summary>
    /// Event argument for text validation events.
    /// </summary>
    public class TextValidationEventArgs : EventArgs
    {
        [NotNull]
        private readonly string _text;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextValidationEventArgs"/> class.
        /// </summary>
        /// <param name="text">The text to validate.</param>
        public TextValidationEventArgs(string text)
        {
            _text = text ?? string.Empty;
        }

        /// <summary>
        /// Gets the text to validate.
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

        /// <summary>
        /// Gets or sets the action to take.
        /// </summary>
        public TextValidationAction Action
        {
            get;
            set;
        }

        [ContractInvariantMethod]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        [Conditional("CONTRACTS_FULL")]
        private void ObjectInvariant()
        {
            Contract.Invariant(_text != null);
        }
    }

    /// <summary>
    /// The action to be taken after text validation.
    /// </summary>
    public enum TextValidationAction
    {
        /// <summary>
        /// The text is OK, nothing to do.
        /// </summary>
        None,
        /// <summary>
        /// The text contains errors and should be highlighted.
        /// </summary>
        Error,
        /// <summary>
        /// The text contains errors, the last change should be undone.
        /// </summary>
        Undo
    }
}