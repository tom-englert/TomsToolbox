namespace TomsToolbox.Essentials
{
    using System;
    using System.Collections.Generic;

    using JetBrains.Annotations;

    /// <summary>
    /// Extension methods for <see cref="Type"/>
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Gets the type and all it's base types.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The type and all it's base types.</returns>
        [NotNull, ItemNotNull]
        public static IEnumerable<Type> GetSelfAndBaseTypes([NotNull] this Type type)
        {
            while (type != null)
            {
                yield return type;
                type = type.BaseType;
            }
        }
    }
}
