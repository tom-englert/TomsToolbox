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
        public TextValidationEventArgs([CanBeNull] string text)
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