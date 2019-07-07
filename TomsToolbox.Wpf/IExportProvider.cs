namespace TomsToolbox.Wpf
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using JetBrains.Annotations;

    /// <summary>
    /// Retrieves exports which match a specified ImportDefinition object.
    /// </summary>
    public interface IExportProvider
    {
        /// <summary>
        /// Occurs when the exports in the IExportProvider change.
        /// </summary>
        event EventHandler<EventArgs> ExportsChanged;

        /// <summary>
        /// Gets all the exported objects with the specified contract name.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <returns>The object</returns>
        [NotNull, ItemNotNull]
        IEnumerable<T> GetExportedValues<T>([CanBeNull] string contractName = null);

        /// <summary>
        /// Gets the exports.
        /// </summary>
        /// <param name="type">The type of the requested object.</param>
        /// <param name="contractName">Name of the contract.</param>
        /// <returns></returns>
        [NotNull, ItemNotNull]
        IEnumerable<ILazy<object>> GetExports([NotNull] Type type, [CanBeNull] string contractName = null);
    }

    /// <summary>
    /// Extension methods for the <see cref="IExportProvider"/> interface.
    /// </summary>
    public static class ExportProviderExtensions
    {
        /// <summary>
        /// Gets the exports.
        /// </summary>
        /// <typeparam name="TMetadata">The type of the metadata.</typeparam>
        /// <param name="exportProvider">The export provider.</param>
        /// <param name="type">The type of the requested object.</param>
        /// <param name="metadataFactory">The factory method to create the metadata object from the metadata dictionary.</param>
        /// <returns></returns>
        [NotNull, ItemNotNull]
        public static IEnumerable<ILazy<object, TMetadata>> GetExports<TMetadata>(this IExportProvider exportProvider, [NotNull] Type type, [NotNull] Func<IDictionary<string, object>, TMetadata> metadataFactory)
        {
            return GetExports(exportProvider, type, null, metadataFactory);
        }

        /// <summary>
        /// Gets the exports.
        /// </summary>
        /// <typeparam name="TMetadata">The type of the metadata.</typeparam>
        /// <param name="exportProvider">The export provider.</param>
        /// <param name="type">The type of the requested object.</param>
        /// <param name="contractName">Name of the contract.</param>
        /// <param name="metadataFactory">The factory method to create the metadata object from the metadata dictionary.</param>
        /// <returns></returns>
        [NotNull, ItemNotNull]
        public static IEnumerable<ILazy<object, TMetadata>> GetExports<TMetadata>(this IExportProvider exportProvider, [NotNull] Type type, [CanBeNull] string contractName, [NotNull] Func<IDictionary<string, object>, TMetadata> metadataFactory)
        {
            return exportProvider
                .GetExports(type, contractName)
                .Select(item => new LazyAdapter<object, TMetadata>(item, metadataFactory));

        }

        private class LazyAdapter<TObject, TMetadata> : ILazy<TObject, TMetadata>
        {
            private readonly ILazy<TObject> _source;

            public LazyAdapter(ILazy<TObject> source, Func<IDictionary<string, object>, TMetadata> metadataFactory)
            {
                _source = source;
                Metadata = metadataFactory(source.Metadata);
            }

            [CanBeNull]
            public TObject Value => _source.Value;

            [CanBeNull]
            public TMetadata Metadata { get; }
        }
    }
}
