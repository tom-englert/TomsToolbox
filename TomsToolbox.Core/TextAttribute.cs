namespace TomsToolbox.Core
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;

    using JetBrains.Annotations;

    /// <summary>
    /// Specifies a general usable attribute to associate text with an object, 
    /// similar to  <see cref="DisplayNameAttribute"/> or <see cref="DescriptionAttribute"/>, but without a predefined usage scope.
    /// </summary>
    [SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes", Justification = "May need to derive a 'LocalizedTextAttribute'")]
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class TextAttribute : Attribute
    {
        [CanBeNull]
        private readonly object _key;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextAttribute"/> class.
        /// </summary>
        /// <param name="key">A user defined key to classify the usage of this text.</param>
        public TextAttribute([CanBeNull] object key)
        {
            _key = key;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextAttribute"/> class.
        /// </summary>
        /// <param name="key">A user defined key to classify the usage of this text.</param>
        /// <param name="text">The text.</param>
        public TextAttribute([CanBeNull] object key, [CanBeNull] string text)
            : this(key)
        {
            TextValue = text;
        }

        /// <summary>
        /// Gets the key that classifies the usage of this text.
        /// </summary>
        [CanBeNull]
        public object Key => _key;

        /// <summary>
        /// Gets the text associated with this attribute.
        /// </summary>
        [CanBeNull]
        public virtual string Text => TextValue;

        /// <summary>
        /// Gets or sets the text to be returned by the Text property.
        /// </summary>
        [CanBeNull]
        protected string TextValue
        {
            get;
            set;
        }
    }
}
