namespace TomsToolbox.Core
{
    using System;

    /// <summary>
    /// General usable sequence attribute to assign sequences to any object that may need to be e.g. sorted by some means.
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
#if !PORTABLE && !NETSTANDARD1_0
    [Serializable]
#endif
    public sealed class SequenceAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SequenceAttribute"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public SequenceAttribute(double value)
        {
            Value = value;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public double Value
        {
            get;
            private set;
        }
    }
}
