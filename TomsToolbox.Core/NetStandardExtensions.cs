namespace TomsToolbox.Core
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
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
        [NotNull]
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "CA seems to be wrong about this!")]
        [System.Diagnostics.Contracts.Pure]
#if !NETSTANDARD1_0
        public static MethodInfo GetMethodInfo([NotNull] this Delegate @delegate)
        {
            Contract.Requires(@delegate != null);
            Contract.Ensures(Contract.Result<MethodInfo>() != null);

            // ReSharper disable once AssignNullToNotNullAttribute
            return @delegate.Method;
        }
#else
        public static MethodInfo GetMethodInfo([NotNull] Delegate @delegate)
        {
            throw new NotImplementedException();
        }
#endif

        /// <summary>
        /// Gets the type information for a type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The specified type.</returns>
        [NotNull]
        [System.Diagnostics.Contracts.Pure]
#if !NETSTANDARD1_0
        public static Type GetTypeInfo([NotNull] this Type type)
        {
            Contract.Requires(type != null);
            Contract.Ensures(Contract.Result<Type>() != null);

            return type;
        }
#else
        public static Type GetTypeInfo([NotNull] Type type)
        {
            throw new NotImplementedException();
        }
#endif
    }
}
