namespace TomsToolbox.Core
{
    using System;
    using System.Collections.Generic;

    using JetBrains.Annotations;

    /// <summary>
    /// Extension methods to ease dealing with exceptions.
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Returns an enumeration of exceptions that contains this exception and all inner exceptions.
        /// </summary>
        /// <param name="ex">The exception to start with.</param>
        /// <returns>The exception and all inner exceptions.</returns>
        [NotNull, ItemNotNull]
        public static IEnumerable<Exception> ExceptionChain([CanBeNull] this Exception ex)
        {
            while (ex != null)
            {
                yield return ex;
                ex = ex.InnerException;
            }
        }
    }
}
