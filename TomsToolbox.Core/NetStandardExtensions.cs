using System.Diagnostics.Contracts;

namespace TomsToolbox.Core
{
#if !NETSTANDARD1_0
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;

    using JetBrains.Annotations;

    internal static class NetStandardExtensions
    {
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "CA seems to be wrong about this!")]
        [NotNull] 
        public static MethodInfo GetMethodInfo([NotNull] this Delegate d)
        {
            Contract.Requires(d != null);
            Contract.Ensures(Contract.Result<MethodInfo>() != null);

            // ReSharper disable once AssignNullToNotNullAttribute
            return d.Method;
        }

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