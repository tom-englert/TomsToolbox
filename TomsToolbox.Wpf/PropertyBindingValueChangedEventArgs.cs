namespace TomsToolbox.Wpf
{
    using System;

    using JetBrains.Annotations;

    /// <summary>
    /// Event arguments for the <see cref="PropertyBinding{T}.ValueChanged"/> event.
    /// </summary>
    /// <typeparam name="T">The type of the variable.</typeparam>
    public class PropertyBindingValueChangedEventArgs<T> : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyBindingValueChangedEventArgs{T}"/> class.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        public PropertyBindingValueChangedEventArgs([CanBeNull] T oldValue, [CanBeNull] T newValue)
        {
            NewValue = newValue;
            OldValue = oldValue;
        }

        /// <summary>
        /// Gets the old value.
        /// </summary>
        [CanBeNull]
        public T OldValue { get; }

        /// <summary>
        /// Gets the new value.
        /// </summary>
        [CanBeNull]
        public T NewValue { get; }
    }
}