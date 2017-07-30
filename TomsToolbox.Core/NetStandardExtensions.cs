using System.Diagnostics.Contracts;

namespace TomsToolbox.Core
{
#if !NETSTANDARD1_0
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;

    using JetBrains.Annotations;

    /// <summary>
    /// Some emulations of .NetStandard methods
    /// </summary>
    public static class NetStandardExtensions
    {
        /// <summary>
        /// Gets the method information from a delegate.
        /// </summary>
        /// <param name="delegate">The delegate.</param>
        /// <returns>The <see cref="MethodInfo"/></returns>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "CA seems to be wrong about this!")]
        [NotNull]
        [System.Diagnostics.Contracts.Pure]
        public static MethodInfo GetMethodInfo([NotNull] this Delegate @delegate)
        {
            Contract.Requires(@delegate != null);
            Contract.Ensures(Contract.Result<MethodInfo>() != null);

            // ReSharper disable once AssignNullToNotNullAttribute
            return @delegate.Method;
        }

        /// <summary>
        /// Gets the type information for a type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The specified type.</returns>
        [NotNull]
        [System.Diagnostics.Contracts.Pure]
        public static Type GetTypeInfo([NotNull] this Type type)
        {
            Contract.Requires(type != null);
            Contract.Ensures(Contract.Result<Type>() != null);

            return type;
        }
    }
#endif
}