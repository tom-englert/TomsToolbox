namespace TomsToolbox.Desktop
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Reflection;

    using JetBrains.Annotations;

    /// <summary>
    /// A helper to ease dealing with <see cref="AppDomain"/> specific tasks.
    /// </summary>
    public static class AppDomainHelper 
    {
        /// <summary>
        /// Invokes the specified function in a temporary separate domain.
        /// </summary>
        /// <typeparam name="T">Return type of the function.</typeparam>
        /// <param name="func">The function.</param>
        /// <returns>The result of the function.</returns>
        public static T InvokeInSeparateDomain<T>([NotNull] this Func<T> func)
        {
            Contract.Requires(func != null);

            return InternalInvokeInSeparateDomain<T>(func);
        }

        /// <summary>
        /// Invokes the specified function in a temporary separate domain.
        /// </summary>
        /// <typeparam name="TA1">The type of the a1.</typeparam>
        /// <typeparam name="T">Return type of the function.</typeparam>
        /// <param name="func">The function.</param>
        /// <param name="arg1">The argument of the function.</param>
        /// <returns>The result of the function.</returns>
        public static T InvokeInSeparateDomain<TA1, T>([NotNull] this Func<TA1, T> func, [CanBeNull] TA1 arg1)
        {
            Contract.Requires(func != null);

            return InternalInvokeInSeparateDomain<T>(func, arg1);
        }

        /// <summary>
        /// Invokes the specified function in a temporary separate domain.
        /// </summary>
        /// <typeparam name="TA1">The type of the arguments.</typeparam>
        /// <typeparam name="TA2">The type of the arguments.</typeparam>
        /// <typeparam name="T">Return type of the function.</typeparam>
        /// <param name="func">The function.</param>
        /// <param name="arg1">The arguments of the function.</param>
        /// <param name="arg2">The arguments of the function.</param>
        /// <returns>The result of the function.</returns>
        public static T InvokeInSeparateDomain<TA1, TA2, T>([NotNull] this Func<TA1, TA2, T> func, [CanBeNull] TA1 arg1, [CanBeNull] TA2 arg2)
        {
            Contract.Requires(func != null);

            return InternalInvokeInSeparateDomain<T>(func, arg1, arg2);
        }

        /// <summary>
        /// Invokes the specified function in a temporary separate domain.
        /// </summary>
        /// <typeparam name="T">Return type of the function.</typeparam>
        /// <typeparam name="TA1">The type of the arguments.</typeparam>
        /// <typeparam name="TA2">The type of the arguments.</typeparam>
        /// <typeparam name="TA3">The type of the arguments.</typeparam>
        /// <param name="func">The function.</param>
        /// <param name="arg1">The arguments of the function.</param>
        /// <param name="arg2">The arguments of the function.</param>
        /// <param name="arg3">The arguments of the function.</param>
        /// <returns>The result of the function.</returns>
        public static T InvokeInSeparateDomain<TA1, TA2, TA3, T>([NotNull] this Func<TA1, TA2, TA3, T> func, [CanBeNull] TA1 arg1, [CanBeNull] TA2 arg2, [CanBeNull] TA3 arg3)
        {
            Contract.Requires(func != null);

            return InternalInvokeInSeparateDomain<T>(func, arg1, arg2, arg3);
        }

        /// <summary>
        /// Invokes the specified function in a temporary separate domain.
        /// </summary>
        /// <typeparam name="T">Return type of the function.</typeparam>
        /// <typeparam name="TA1">The type of the arguments.</typeparam>
        /// <typeparam name="TA2">The type of the arguments.</typeparam>
        /// <typeparam name="TA3">The type of the arguments.</typeparam>
        /// <typeparam name="TA4">The type of the arguments.</typeparam>
        /// <param name="func">The function.</param>
        /// <param name="arg1">The arguments of the function.</param>
        /// <param name="arg2">The arguments of the function.</param>
        /// <param name="arg3">The arguments of the function.</param>
        /// <param name="arg4">The arguments of the function.</param>
        /// <returns>The result of the function.</returns>
        public static T InvokeInSeparateDomain<TA1, TA2, TA3, TA4, T>([NotNull] this Func<TA1, TA2, TA3, TA4, T> func, [CanBeNull] TA1 arg1, [CanBeNull] TA2 arg2, [CanBeNull] TA3 arg3, [CanBeNull] TA4 arg4)
        {
            Contract.Requires(func != null);

            return InternalInvokeInSeparateDomain<T>(func, arg1, arg2, arg3, arg4);
        }

        /// <summary>
        /// A wrapper for <see cref="AppDomain.CreateInstanceAndUnwrap(string, string)"/>
        /// </summary>
        /// <typeparam name="T">The type to create.</typeparam>
        /// <param name="appDomain">The application domain.</param>
        /// <returns>The proxy of the unwrapped type.</returns>
        [NotNull]
        public static T CreateInstanceAndUnwrap<T>([NotNull] this AppDomain appDomain) where T : class
        {
            Contract.Requires(appDomain != null);
            Contract.Ensures(Contract.Result<T>() != null);

            // ReSharper disable once AssignNullToNotNullAttribute (every type has a full name)
            return (T)appDomain.CreateInstanceAndUnwrap(typeof(T).Assembly.FullName, typeof(T).FullName);
        }

        private static T InternalInvokeInSeparateDomain<T>([NotNull] Delegate func, [NotNull] params object[] args)
        {
            Contract.Requires(func != null);
            Contract.Requires(args != null);

            var friendlyName = "Temporary domain for " + func.Method.Name;
            var currentDomain = AppDomain.CurrentDomain;
            var appDomain = AppDomain.CreateDomain(friendlyName, currentDomain.Evidence, currentDomain.BaseDirectory, currentDomain.RelativeSearchPath, false);
            Contract.Assume(appDomain != null);

            try
            {
                var helper = appDomain.CreateInstanceAndUnwrap<DomainHelper>();
                var result = helper.Invoke(func.Method, func.Target, args);

                if (result == null) // avoid unboxing null values.
                    return default(T);

                return (T)result;
            }
            finally
            {
                AppDomain.Unload(appDomain);
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Created in another AppDomain.")]
        // ReSharper disable once ClassNeverInstantiated.Local
        private class DomainHelper : MarshalByRefObject
        {
            [CanBeNull]
            [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
            public object Invoke([NotNull] MethodInfo method, [CanBeNull] object target, [CanBeNull] object[] args)
            {
                Contract.Requires(method != null);
                return method.Invoke(target, args);
            }
        }
    }
}
