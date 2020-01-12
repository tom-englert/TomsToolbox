namespace TomsToolbox.Composition
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
        event EventHandler<EventArgs>? ExportsChanged;

        /// <summary>
        /// Gets the exported object with the specified contract name.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="contractName">Name of the contract.</param>
        /// <returns>
        /// The object
        /// </returns>
        [NotNull]
        T GetExportedValue<T>([CanBeNull] string? contractName = null) where T : class;

        /// <summary>
        /// Gets the exported object with the specified contract name.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="contractName">Name of the contract.</param>
        /// <returns>
        /// The object, or null if no such export exists.
        /// </returns>
        [CanBeNull]
        T? GetExportedValueOrDefault<T>([CanBeNull] string? contractName = null) where T : class;
        /// <summary>
        /// Tries to the get exported value.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="contractName">Name of the contract.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        /// true if the value exists.
        /// </returns>
        [ContractAnnotation("=> false,value:null;=>true,value:notnull")]
        bool TryGetExportedValue<T>([CanBeNull] string? contractName, [CanBeNull, System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out T? value) where T : class;

        /// <summary>
        /// Gets all the exported objects with the specified contract name.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="contractName">Name of the contract.</param>
        /// <returns>
        /// The object
        /// </returns>
        [NotNull, ItemNotNull]
        IEnumerable<T> GetExportedValues<T>([CanBeNull] string? contractName = null) where T : class;

        /// <summary>
        /// Gets the exports for the specified parameters.
        /// </summary>
        /// <param name="contractType">The type of the requested object.</param>
        /// <param name="contractName">Name of the contract.</param>
        /// <returns>
        /// The exports.
        /// </returns>
        [NotNull, ItemNotNull]
        IEnumerable<IExport<object>> GetExports([NotNull] Type contractType, [CanBeNull] string? contractName = null);
    }

    /// <summary>
    /// Extension methods for the <see cref="IExportProvider"/> interface.
    /// </summary>
    public static partial class ExtensionMethods
    {
        /// <summary>
        /// Tries to the get exported value.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="exportProvider">The export provider.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        /// true if the value exists.
        /// </returns>
        [ContractAnnotation("=> false,value:null;=>true,value:notnull")]
        public static bool TryGetExportedValue<T>([NotNull] this IExportProvider exportProvider, [CanBeNull, System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out T? value) 
            where T : class
        {
            return exportProvider.TryGetExportedValue(null, out value);
        }

        /// <summary>
        /// Gets the exports for the specified parameters.
        /// </summary>
        /// <typeparam name="TMetadata">The type of the metadata.</typeparam>
        /// <param name="exportProvider">The export provider.</param>
        /// <param name="type">The type of the requested object.</param>
        /// <param name="metadataFactory">The factory method to create the metadata object from the metadata dictionary.</param>
        /// <returns>
        /// The exports.
        /// </returns>
        [NotNull, ItemNotNull]
        public static IEnumerable<IExport<object, TMetadata>> GetExports<TMetadata>(this IExportProvider exportProvider, [NotNull] Type type, [NotNull] Func<IMetadata?, TMetadata?> metadataFactory)
            where TMetadata: class
        {
            return GetExports(exportProvider, type, null, metadataFactory);
        }

        /// <summary>
        /// Gets the exports for the specified parameters.
        /// </summary>
        /// <typeparam name="TMetadata">The type of the metadata.</typeparam>
        /// <param name="exportProvider">The export provider.</param>
        /// <param name="type">The type of the requested object.</param>
        /// <param name="contractName">Name of the contract.</param>
        /// <param name="metadataFactory">The factory method to create the metadata object from the metadata dictionary.</param>
        /// <returns>
        /// The exports.
        /// </returns>
        [NotNull, ItemNotNull]
        public static IEnumerable<IExport<object, TMetadata>> GetExports<TMetadata>(this IExportProvider exportProvider, [NotNull] Type type, [CanBeNull] string? contractName, [NotNull] Func<IMetadata?, TMetadata?> metadataFactory)
            where TMetadata: class
        {
            return exportProvider
                .GetExports(type, contractName)
                .Select(item => new ExportAdapter<object, TMetadata>(item, metadataFactory));

        }

        private class ExportAdapter<TObject, TMetadata> : IExport<TObject, TMetadata>
            where TObject : class
            where TMetadata : class
        {
            private readonly IExport<TObject> _source;

            public ExportAdapter(IExport<TObject> source, Func<IMetadata?, TMetadata?> metadataFactory)
            {
                _source = source;
                Metadata = metadataFactory(source.Metadata);
            }

            [CanBeNull]
            public TObject? Value => _source.Value;

            [CanBeNull]
            public TMetadata? Metadata { get; }
        }
    }
}
