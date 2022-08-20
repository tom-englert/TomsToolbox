namespace TomsToolbox.Wpf;

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

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
    public static DispatcherOperation BeginInvoke(this Visual visual, Action method)
    {
        var dispatcher = visual.Dispatcher;

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
    public static DispatcherOperation BeginInvoke(this Visual visual, DispatcherPriority priority, Action method)
    {
        var dispatcher = visual.Dispatcher;

        return dispatcher.BeginInvoke(method, priority, null);
    }

}