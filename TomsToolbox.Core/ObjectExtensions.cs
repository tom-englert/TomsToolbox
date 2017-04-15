namespace TomsToolbox.Core
{
    using JetBrains.Annotations;

    /// <summary>
    /// Extensions for any objects.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Performs a cast from object to <typeparamref name="T"/>, avoiding possible null violations if <typeparamref name="T"/> is a value type.
        /// </summary>
        /// <typeparam name="T">The target type</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>The value casted to <typeparamref name="T"/>, or <c>default(T)</c> if value is <c>null</c>.</returns>
        [CanBeNull]
        public static T SafeCast<T>([CanBeNull] this object value)
        {
            return (value == null) ? default(T) : (T)value;
        }
    }
}
