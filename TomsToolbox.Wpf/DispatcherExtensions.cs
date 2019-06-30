namespace TomsToolbox.Wpf
{
    using System;
    using System.Reflection;
    using System.Windows.Threading;

    using JetBrains.Annotations;

    /// <summary>
    /// Extension methods to ease usage of dispatcher calls.
    /// </summary>
    public static class DispatcherExtensions
    {
        /// <summary>
        /// Invokes the specified method in the dispatcher thread.
        /// </summary>
        /// <typeparam name="T">The return type of the method.</typeparam>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="method">The method.</param>
        /// <returns>The result of the method.</returns>
        /// <remarks>Exceptions thrown by <paramref name="method"/> are passed back to the caller and are not wrapped into a <see cref="TargetInvocationException"/>.</remarks>
        [CanBeNull]
        public static T Invoke<T>([CanBeNull] this Dispatcher dispatcher, [NotNull] Func<T> method)
        {
            return InternalInvoke(dispatcher, method);
        }

        /// <summary>
        /// Invokes the specified method in the dispatcher thread.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="method">The method.</param>
        /// <remarks>Exceptions thrown by <paramref name="method"/> are passed back to the caller and are not wrapped into a <see cref="TargetInvocationException"/>.</remarks>
        public static void Invoke([CanBeNull] this Dispatcher dispatcher, [NotNull] Action method)
        {
            InternalInvoke(dispatcher, method);
        }

        [CanBeNull]
        private static T InternalInvoke<T>([CanBeNull] Dispatcher dispatcher, [NotNull] Func<T> method)
        {
            var result = InternalInvoke(dispatcher, (Delegate)method);
            if (result == null)
                return default;

            return (T)result;
        }

        [CanBeNull]
        private static object InternalInvoke([CanBeNull] Dispatcher dispatcher, [NotNull] Delegate method)
        {
            if ((dispatcher == null) || dispatcher.CheckAccess())
            {
                // No thread affinity, or already in the correct thread: call method directly.
                try
                {
                    return method.DynamicInvoke();
                }
                catch (Exception ex)
                {
                    throw UnwrapTargetInvocation(ex);
                }
            }

            Exception innerException = null;

            var result = dispatcher.Invoke(delegate
            {
                try
                {
                    return method.DynamicInvoke();
                }
                catch (Exception ex)
                {
                    innerException = ex;
                    return null;
                }
            });

            if (innerException != null)
            {
                throw UnwrapTargetInvocation(innerException);
            }

            return result;
        }

        [NotNull]
        private static Exception UnwrapTargetInvocation([NotNull] Exception ex)
        {
            if (ex is TargetInvocationException)
            {
                return ex.InnerException ?? ex;
            }

            return ex;
        }

        /// <summary>
        /// Invokes the specified method asynchronously in the dispatcher thread.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="method">The method.</param>
        /// <returns>The dispatcher operation to track the outcome of the call.</returns>
        /// <exception cref="System.InvalidOperationException">The dispatcher has already shut down.</exception>
        [NotNull]
        public static DispatcherOperation BeginInvoke([NotNull] this Dispatcher dispatcher, [NotNull] Action method)
        {
            return BeginInvoke(dispatcher, DispatcherPriority.Normal, method);
        }

        /// <summary>
        /// Invokes the specified method asynchronously in the dispatcher thread.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="priority">The priority to use.</param>
        /// <param name="method">The method.</param>
        /// <returns>The dispatcher operation to track the outcome of the call.</returns>
        /// <exception cref="System.InvalidOperationException">The dispatcher has already shut down.</exception>
        [NotNull]
        public static DispatcherOperation BeginInvoke([NotNull] this Dispatcher dispatcher, DispatcherPriority priority, [NotNull] Action method)
        {
            return dispatcher.BeginInvoke(method, priority, null);
        }

        /// <summary>
        /// Restarts the specified timer.
        /// </summary>
        /// <param name="timer">The timer.</param>
        public static void Restart([NotNull] this DispatcherTimer timer)
        {
            timer.Stop();
            timer.Start();
        }
    }
}
