namespace TomsToolbox.Essentials
{
    using System;
    using System.Diagnostics.CodeAnalysis;

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
        [CanBeNull][return: MaybeNull]
        public static T SafeCast<T>([CanBeNull] this object? value)
        {
            return (value == null) ? default : (T)value;
        }

        /// <summary>
        /// Intercepts the specified value. Can be used to e.g. log LINQ expressions.
        /// </summary>
        /// <typeparam name="T">The target type</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="interceptor">The interceptor.</param>
        /// <returns>The <paramref name="value"/></returns>
        [CanBeNull, ContractAnnotation("value:notnull=>notnull")]
        [return:NotNullIfNotNull("value")]
        public static T Intercept<T>([CanBeNull] this T value, [JetBrains.Annotations.NotNull] Action<T> interceptor)
        {
            interceptor(value);

            return value;
        }
    }
}
