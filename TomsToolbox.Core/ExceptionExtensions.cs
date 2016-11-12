namespace TomsToolbox.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

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
        [NotNull]
        public static IEnumerable<Exception> ExceptionChain(this Exception ex)
        {
            Contract.Ensures(Contract.Result<IEnumerable<Exception>>() != null);

            while (ex != null)
            {
                yield return ex;
                ex = ex.InnerException;
            }
        }
    }
}
