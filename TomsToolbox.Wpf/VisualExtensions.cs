namespace TomsToolbox.Wpf
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Threading;

    using JetBrains.Annotations;

    /// <summary>
    /// Extension methods to ease usage of <see cref="Visual"/> object.
    /// </summary>
    public static class VisualExtensions
    {
        /// <summary>
        /// Invokes the specified method asynchronously in the dispatcher thread of the visual.
        /// </summary>
        /// <param name="visual">The visual.</param>
        /// <param name="method">The method.</param>
        /// <returns>The dispatcher operation to track the outcome of the call.</returns>
        /// <exception cref="System.InvalidOperationException">The dispatcher has already shut down.</exception>
        /// <remarks>
        /// <see cref="DispatcherObject"/> may have an empty Dispatcher, e.g. for <see cref="Freezable"/> objects, 
        /// thus calling DispatcherObject.Dispatcher.BeginInvoke is considered unsafe. However <see cref="Visual"/> objects always 
        /// have a valid dispatcher, so calling Visual.BeginInvoke via this extension can be considered safe.
        /// </remarks>
        [NotNull]
        public static DispatcherOperation BeginInvoke([NotNull] this Visual visual, [NotNull] Action method)
        {
            Contract.Requires(visual != null);
            Contract.Requires(method != null);
            Contract.Ensures(Contract.Result<DispatcherOperation>() != null);

            var dispatcher = visual.Dispatcher;
            Contract.Assume(dispatcher != null); // visuals always have a dispatcher.

            return dispatcher.BeginInvoke(DispatcherPriority.Normal, method);
        }

        /// <summary>
        /// Invokes the specified method asynchronously in the dispatcher thread of the visual.
        /// </summary>
        /// <param name="visual">The visual.</param>
        /// <param name="priority">The priority to use.</param>
        /// <param name="method">The method.</param>
        /// <returns>The dispatcher operation to track the outcome of the call.</returns>
        /// <exception cref="System.InvalidOperationException">The dispatcher has already shut down.</exception>
        /// <remarks>
        /// <see cref="DispatcherObject"/> may have an empty Dispatcher, e.g. for <see cref="Freezable"/> objects, 
        /// thus calling DispatcherObject.Dispatcher.BeginInvoke is considered unsafe. However <see cref="Visual"/> objects always 
        /// have a valid dispatcher, so calling Visual.BeginInvoke via this extension can be considered safe.
        /// </remarks>
        [NotNull]
        public static DispatcherOperation BeginInvoke([NotNull] this Visual visual, DispatcherPriority priority, [NotNull] Action method)
        {
            Contract.Requires(visual != null);
            Contract.Requires(method != null);
            Contract.Ensures(Contract.Result<DispatcherOperation>() != null);

            var dispatcher = visual.Dispatcher;
            Contract.Assume(dispatcher != null); // visuals always have a dispatcher.

            return dispatcher.BeginInvoke(method, priority, null);
        }

    }
}
