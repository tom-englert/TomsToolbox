namespace TomsToolbox.Composition;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

/// <summary>
/// Retrieves exports which match a specified ImportDefinition object.
/// </summary>
public interface IExportProvider
{
    /// <summary>
    /// Occurs when the exports in the IExportProvider change. Not supported by all implementations.
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
    T GetExportedValue<T>(string? contractName = null) where T : class;

    /// <summary>
    /// Gets the exported object with the specified contract name.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <param name="contractName">Name of the contract.</param>
    /// <returns>
    /// The object, or null if no such export exists.
    /// </returns>
    T? GetExportedValueOrDefault<T>(string? contractName = null) where T : class;
    /// <summary>
    /// Tries to the get exported value.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <param name="contractName">Name of the contract.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    /// true if the value exists.
    /// </returns>
    bool TryGetExportedValue<T>(string? contractName, [NotNullWhen(true)] out T? value) where T : class;

    /// <summary>
    /// Gets all the exported objects with the specified contract name.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <param name="contractName">Name of the contract.</param>
    /// <returns>
    /// The object
    /// </returns>
    IEnumerable<T> GetExportedValues<T>(string? contractName = null) where T : class;

    /// <summary>
    /// Gets all the exported objects with the specified contract name.
    /// </summary>
    /// <param name="contractType">The type of the requested object.</param>
    /// <param name="contractName">Name of the contract.</param>
    /// <returns>
    /// The object
    /// </returns>
    IEnumerable<object> GetExportedValues(Type contractType, string? contractName = null);

    /// <summary>
    /// Gets the exports for the specified parameters.
    /// </summary>
    /// <param name="contractType">The type of the requested object.</param>
    /// <param name="contractName">Name of the contract.</param>
    /// <returns>
    /// The exports.
    /// </returns>
    IEnumerable<IExport<object>> GetExports(Type contractType, string? contractName = null);

    /// <summary>
    /// Gets the exports for the specified parameters.
    /// </summary>
    /// <typeparam name="T">The type of the requested object.</typeparam>
    /// <param name="contractName">Name of the contract.</param>
    /// <returns>
    /// The exports.
    /// </returns>
    IEnumerable<IExport<T>> GetExports<T>(string? contractName = null) where T : class;

    /// <summary>
    /// Gets the exports for the specified parameters.
    /// </summary>
    /// <typeparam name="T">The type of the requested object.</typeparam>
    /// <typeparam name="TMetadataView">The type of the metadata of the requested object</typeparam>
    /// <param name="contractName">Name of the contract.</param>
    /// <returns>
    /// The exports.
    /// </returns>
    IEnumerable<IExport<T, TMetadataView>> GetExports<T, TMetadataView>(string? contractName = null) where T : class where TMetadataView : class;

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
    public static bool TryGetExportedValue<T>(this IExportProvider exportProvider, [NotNullWhen(true)] out T? value)
        where T : class
    {
        return exportProvider.TryGetExportedValue(null, out value);
    }

    /// <summary>
    /// Gets the exports for the specified parameters.
    /// </summary>
    /// <typeparam name="TMetadataView">The type of the metadata.</typeparam>
    /// <param name="exportProvider">The export provider.</param>
    /// <param name="type">The type of the requested object.</param>
    /// <param name="metadataFactory">The factory method to create the metadata object from the metadata dictionary.</param>
    /// <returns>
    /// The exports.
    /// </returns>
    public static IEnumerable<IExport<object, TMetadataView>> GetExports<TMetadataView>(this IExportProvider exportProvider, Type type, Func<IMetadata?, TMetadataView?> metadataFactory)
        where TMetadataView : class
    {
        return GetExports(exportProvider, type, null, metadataFactory);
    }

    /// <summary>
    /// Gets the exports for the specified parameters.
    /// </summary>
    /// <typeparam name="TMetadataView">The type of the metadata.</typeparam>
    /// <param name="exportProvider">The export provider.</param>
    /// <param name="type">The type of the requested object.</param>
    /// <param name="contractName">Name of the contract.</param>
    /// <param name="metadataFactory">The factory method to create the metadata object from the metadata dictionary.</param>
    /// <returns>
    /// The exports.
    /// </returns>
    public static IEnumerable<IExport<object, TMetadataView>> GetExports<TMetadataView>(this IExportProvider exportProvider, Type type, string? contractName, Func<IMetadata?, TMetadataView?> metadataFactory)
        where TMetadataView : class
    {
        return exportProvider
            .GetExports(type, contractName)
            .Select(item => new ExportAdapter<object, TMetadataView>(item, metadataFactory));

    }

    private class ExportAdapter<TObject, TMetadataView>(IExport<TObject> source, Func<IMetadata?, TMetadataView?> metadataFactory) : IExport<TObject, TMetadataView>
        where TObject : class
        where TMetadataView : class
    {
        public TObject? Value => source.Value;

        public TMetadataView? Metadata { get; } = metadataFactory(source.Metadata);
    }
}
