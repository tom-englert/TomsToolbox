#if !NETSTANDARD1_0

namespace TomsToolbox.Core
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;

    using JetBrains.Annotations;

    /// <summary>
    /// Some emulations of .NetStandard methods
    /// </summary>
    internal static class NetStandardExtensions
    {
        /// <summary>
        /// Gets the method information from a delegate.
        /// </summary>
        /// <param name="delegate">The delegate.</param>
        /// <returns>The <see cref="MethodInfo"/></returns>
        [NotNull]
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "CA seems to be wrong about this!")]
        public static MethodInfo GetMethodInfo([NotNull] this Delegate @delegate)
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            return @delegate.Method;
        }

        /// <summary>
        /// Gets the type information for a type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The specified type.</returns>
        [NotNull]
        public static Type GetTypeInfo([NotNull] this Type type)
        {
            return type;
        }
    }
}

#endif
