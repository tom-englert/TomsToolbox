﻿namespace TomsToolbox.Wpf.Converters
{
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Windows.Data;

    /// <summary>
    /// Extension methods for value converters.
    /// </summary>
    public static class ValueConverterExtensions
    {
        /// <summary>
        /// The error number shown in the output.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible")]
        public static int ConverterErrorNumber = 9000;

        /// <summary>
        /// Traces an error for the specified converter.
        /// </summary>
        /// <param name="converter">The converter.</param>
        /// <param name="message">The message.</param>
        /// <param name="methodName">Name of the calling method.</param>
        public static void TraceError(this IValueConverter converter, string message, string methodName)
        {
            Contract.Requires(converter != null);
            Contract.Requires(message != null);
            Contract.Requires(methodName != null);

            InternalTraceError(converter, message, methodName);
        }

        /// <summary>
        /// Traces an error for the specified converter.
        /// </summary>
        /// <param name="converter">The converter.</param>
        /// <param name="message">The message.</param>
        /// <param name="methodName">Name of the calling method.</param>
        public static void TraceError(this IMultiValueConverter converter, string message, string methodName)
        {
            Contract.Requires(converter != null);
            Contract.Requires(message != null);
            Contract.Requires(methodName != null);

            InternalTraceError(converter, message, methodName);
        }

        private static void InternalTraceError(object converter, string message, string methodName)
        {
            Contract.Requires(converter != null);
            Contract.Requires(message != null);
            Contract.Requires(methodName != null);

            var traceSource = PresentationTraceSources.DataBindingSource;
            if (traceSource == null)
                return;

            traceSource.TraceEvent(TraceEventType.Error, ConverterErrorNumber, "{0}.{1} failed: {2}", converter.GetType().Name, methodName, message);
        }
    }
}
