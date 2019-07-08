namespace TomsToolbox.Essentials
{
    using System.Collections.Generic;

    using JetBrains.Annotations;

    /// <summary>
    /// Encapsulation of a lazy object with metadata.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <typeparam name="TMetadata">The type of the metadata.</typeparam>
    public interface ILazy<out T, out TMetadata>
    {
        /// <summary>
        /// Gets the value.
        /// </summary>
        [CanBeNull]
        T Value { get; }

        /// <summary>
        /// Gets the metadata.
        /// </summary>
        [CanBeNull]
        TMetadata Metadata { get; }
    }

    /// <summary>
    /// Encapsulation of a lazy object with generic metadata.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    public interface ILazy<out T> : ILazy<T, IDictionary<string, object>>
    {
    }
}