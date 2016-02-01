namespace TomsToolbox.Core
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Helper methods to get the default value for a type when the type is only available at runtime.
    /// </summary>
    public static class DefaultValue
    {
        /// <summary>
        /// Creates the default value (C#: default(T)) for the specified type, where the type is only known at runtime.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The default value.</returns>
        public static object CreateDefault(Type type)
        {
            Contract.Requires(type != null);

            // every value type has a default constructor, default for reference types is always null
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        /// <summary>
        /// Create an empty value that is not null for value types or strings. 
        /// <list type="bullet">
        /// <item>Value type: The empty value is the same as the default value (usually 0).</item>
        /// <item>String: The empty value is an empty string.</item>
        /// <item>All other reference types: <c>null</c>.</item>
        /// </list>
        /// </summary>
        /// <remarks>
        /// Useful to initialize boxed nullable fields in data base tables with a not null value.
        /// </remarks>
        /// <param name="type">The type.</param>
        /// <returns>The empty value.</returns>
        public static object CreateEmpty(Type type)
        {
            Contract.Requires(type != null);

            return type == typeof(string) ? string.Empty : CreateDefault(type);
        }
    }
}
