namespace TomsToolbox.Wpf;

using System;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;

/// <summary>
/// Extension methods to ease usage of dispatcher calls.
/// </summary>
public static class DispatcherExtensions
{
    /// <summary>Gets the <see cref="Dispatcher" /> for the thread currently executing and creates a new <see cref="Dispatcher" /> if one is not already associated with the thread.</summary>
    /// <returns>The dispatcher associated with the current thread.</returns>
    public static Dispatcher CurrentDispatcher => Dispatcher.CurrentDispatcher;

    /// <summary>Gets the <see cref="Dispatcher" /> for the application's main thread.</summary>
    /// <returns>The dispatcher associated with applications main thread</returns>
    /// <throws cref="InvalidOperationException">The application is not running.</throws>
    public static Dispatcher UIThreadDispatcher => Application.Current?.Dispatcher ?? throw new InvalidOperationException("The application is not running.");

    /// <summary>
    /// Invokes the specified method in the dispatcher thread.
    /// </summary>
    /// <typeparam name="T">The return type of the method.</typeparam>
    /// <param name="dispatcher">The dispatcher.</param>
    /// <param name="method">The method.</param>
    /// <returns>The result of the method.</returns>
    /// <remarks>Exceptions thrown by <paramref name="method"/> are passed back to the caller and are not wrapped into a <see cref="TargetInvocationException"/>.</remarks>
    public static T? Invoke<T>(this Dispatcher? dispatcher, Func<T> method)
    {
        return InternalInvoke(dispatcher, method);
    }

    /// <summary>
    /// Invokes the specified method in the dispatcher thread.
    /// </summary>
    /// <param name="dispatcher">The dispatcher.</param>
    /// <param name="method">The method.</param>
    /// <remarks>Exceptions thrown by <paramref name="method"/> are passed back to the caller and are not wrapped into a <see cref="TargetInvocationException"/>.</remarks>
    public static void Invoke(this Dispatcher? dispatcher, Action method)
    {
        InternalInvoke(dispatcher, method);
    }

    private static T? InternalInvoke<T>(Dispatcher? dispatcher, Func<T> method)
    {
        var result = InternalInvoke(dispatcher, (Delegate)method);
        if (result == null)
            return default;

        return (T)result;
    }

    private static object? InternalInvoke(Dispatcher? dispatcher, Delegate method)
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

        Exception? innerException = null;

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

    private static Exception UnwrapTargetInvocation(Exception ex)
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
    public static DispatcherOperation BeginInvoke(this Dispatcher dispatcher, Action method)
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
    public static DispatcherOperation BeginInvoke(this Dispatcher dispatcher, DispatcherPriority priority, Action method)
    {
        return dispatcher.BeginInvoke(method, priority, null);
    }

    /// <summary>
    /// Restarts the specified timer.
    /// </summary>
    /// <param name="timer">The timer.</param>
    public static void Restart(this DispatcherTimer timer)
    {
        timer.Stop();
        timer.Start();
    }
}
