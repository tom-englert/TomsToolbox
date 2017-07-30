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
        /// <param name="d">The delegate.</param>
        /// <returns>The <see cref="MethodInfo"/></returns>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "CA seems to be wrong about this!")]
        [NotNull] 
        public static MethodInfo GetMethodInfo([NotNull] this Delegate d)
        {
            Contract.Requires(d != null);
            Contract.Ensures(Contract.Result<MethodInfo>() != null);

            // ReSharper disable once AssignNullToNotNullAttribute
            return d.Method;
        }

        /// <summary>
        /// Gets the type information for a type.
        /// </summary>
        /// <param name="t">The type.</param>
        /// <returns>The specified type.</returns>
        [NotNull] 
        public static Type GetTypeInfo([NotNull] this Type t)
        {
            Contract.Requires(t != null);
            Contract.Ensures(Contract.Result<Type>() != null);

            return t;
        }
    }
#endif
}