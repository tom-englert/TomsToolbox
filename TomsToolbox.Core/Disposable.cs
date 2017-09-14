namespace TomsToolbox.Core
{
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Linq;

    using JetBrains.Annotations;

    /// <summary>
    /// Helper methods to ease robust implementation of the IDisposable pattern.
    /// </summary>
    public static class Disposable
    {
        /// <summary>
        /// Occurs when <see cref="ReportNotDisposedObject"/> has been called, i.e. a not disposed object had been detected. 
        /// Use this event to do some custom error handling, e.g. logging or throwing an exception.
        /// The <see cref="TextEventArgs.Text"/> will contain the name of the class of the object that was not disposed.
        /// </summary>
        public static event EventHandler<TextEventArgs> NotDisposedObject;

        /// <summary>
        /// Handle reporting of a not disposed object.<para/>
        /// Using this pattern is a good practice to avoid code where disposable objects get never disposed.<para/>
        /// Calling this method will raise the <see cref="NotDisposedObject"/> event if any event handler is attached; 
        /// otherwise it will throw an <see cref="InvalidOperationException"/> if a debugger is attached. 
        /// If the application does not run in a debugger and no event handler is attached, calling this method does nothing.
        /// </summary>
        /// <param name="obj">The object for which to report the missing dispose call.</param>
        /// <example>
        /// Implement <see cref="IDisposable"/> like this:<para/>
        /// <code language="C#"><![CDATA[
        /// void Dispose()
        /// {
        ///     Dispose(true);
        ///     GC.SuppressFinalize(this);
        /// }
        /// 
        /// ~MyClass()
        /// {
        ///     this.ReportNotDisposedObject();
        /// }
        /// ]]></code></example>
        [ContractVerification(false)]
        public static void ReportNotDisposedObject([NotNull] this IDisposable obj)
        {
            Contract.Requires(obj != null);

            var objectType = obj.GetType();
            // ReSharper disable once PossibleNullReferenceException
            var message = "Object not disposed: " + objectType.Name;

            var eventHandler = NotDisposedObject;
            if (eventHandler != null)
            {
                eventHandler(obj, new TextEventArgs(message));
            }
            else if (Debugger.IsAttached)
            {
                throw new InvalidOperationException(message);
            }
        }

        /// <summary>
        /// If the specified object implements <see cref="IDisposable"/>, it will be disposed, else nothing is done.
        /// </summary>
        /// <param name="item">The object to dispose.</param>
        /// <returns><c>true</c> if the object has been disposed.</returns>
        public static bool Dispose([CanBeNull] object item)
        {
            if (!(item is IDisposable disposable))
                return false;

            disposable.Dispose();
            return true;
        }

        /// <summary>
        /// Calls <see cref="Dispose(object)" /> for all objects in the list.
        /// </summary>
        /// <param name="items">The objects to dispose.</param>
        /// <returns><c>true</c> if any object has been disposed.</returns>
        public static bool DisposeAll([NotNull, ItemCanBeNull] IEnumerable items)
        {
            Contract.Requires(items != null);

            return items.Cast<object>().Count(Dispose) > 0;
        }
    }
}