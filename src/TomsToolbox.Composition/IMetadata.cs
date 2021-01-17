namespace TomsToolbox.Composition
{
    using System.Diagnostics.CodeAnalysis;

    using JetBrains.Annotations;
    using NotNullAttribute = JetBrains.Annotations.NotNullAttribute;

    /// <summary>
    /// A collection of named metadata objects.
    /// </summary>
    public interface IMetadata
    {
        /// <summary>
        /// Gets the metadata with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The metadata value.</returns>
        [NotNull]
        object GetValue(string name);

        /// <summary>
        /// Gets the metadata with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        /// <c>true</c> if the metadata value exists.
        /// </returns>
        bool TryGetValue(string name, [CanBeNull, MaybeNull, NotNullWhen(true)] out object? value);
    }

    public static partial class ExtensionMethods
    {
        /// <summary>
        /// Gets the metadata with the specified name.
        /// </summary>
        /// <typeparam name="T">The type of the metadata object.</typeparam>
        /// <param name="metadata">The metadata.</param>
        /// <param name="name">The name.</param>
        /// <returns>The metadata value.</returns>
        public static T GetValue<T>(this IMetadata metadata, string name)
        {
            return (T)metadata.GetValue(name);
        }

        /// <summary>
        /// Gets the metadata with the specified name.
        /// </summary>
        /// <typeparam name="T">The type of the metadata object.</typeparam>
        /// <param name="metadata"></param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        /// <c>true</c> if the metadata value exists.
        /// </returns>
        public static bool TryGetValue<T>([NotNull] this IMetadata metadata, string name, [CanBeNull, MaybeNull, NotNullWhen(true)] out T value)
        {
            if (metadata.TryGetValue(name, out var v) && (v is T t))
            {
                value = t;
                return true;
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Gets the value, or default if no item with this name exists.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        /// <param name="name">The name.</param>
        /// <returns>The value, or default if no item with this name exists.</returns>
        [CanBeNull]
        public static object? GetValueOrDefault([NotNull] this IMetadata metadata, string name)
        {
            return metadata.TryGetValue(name, out var value) ? value : default;
        }

        /// <summary>
        /// Gets the value, or default if no item with this name exists.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="metadata">The metadata.</param>
        /// <param name="name">The name.</param>
        /// <returns>
        /// The value, or default if no item with this name exists.
        /// </returns>
        [CanBeNull]
        [return:MaybeNull]
        public static T GetValueOrDefault<T>([NotNull] this IMetadata metadata, string name)
        {
            return metadata.TryGetValue<T>(name, out var value) ? value : default;
        }
    }
}
