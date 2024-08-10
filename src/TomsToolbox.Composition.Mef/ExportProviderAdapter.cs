namespace TomsToolbox.Composition.Mef;

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;

/// <summary>
/// An <see cref="IExportProvider"/> adapter for the MEF 1 <see cref="ExportProvider"/>
/// </summary>
/// <seealso cref="IExportProvider" />
public class ExportProviderAdapter : IExportProvider
{
    private readonly ExportProvider _exportProvider;

    private event EventHandler<EventArgs>? ExportsChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExportProviderAdapter"/> class.
    /// </summary>
    /// <param name="exportProvider">The export provider.</param>
    public ExportProviderAdapter(ExportProvider exportProvider)
    {
        _exportProvider = exportProvider;
        exportProvider.ExportsChanged += ExportProvider_ExportsChanged;
    }

    private void ExportProvider_ExportsChanged(object? sender, ExportsChangeEventArgs e)
    {
        ExportsChanged?.Invoke(sender, e);
    }

    event EventHandler<EventArgs>? IExportProvider.ExportsChanged
    {
        add => ExportsChanged += value;
        remove => ExportsChanged -= value;
    }

    T IExportProvider.GetExportedValue<T>(string? contractName) where T : class
    {
        return _exportProvider.GetExportedValue<T>(contractName ?? string.Empty) ?? throw new InvalidOperationException($"No export found for type {typeof(T).FullName} with contract '{contractName}'");
    }

    T? IExportProvider.GetExportedValueOrDefault<T>(string? contractName) where T : class
    {
        return _exportProvider.GetExportedValueOrDefault<T>(contractName ?? string.Empty);
    }

    bool IExportProvider.TryGetExportedValue<T>(string? contractName, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out T? value) where T : class
    {
        value = _exportProvider.GetExportedValueOrDefault<T>();

        return !Equals(value, default(T));
    }

    IEnumerable<T> IExportProvider.GetExportedValues<T>(string? contractName) where T : class
    {
        return _exportProvider.GetExportedValues<T>(contractName ?? string.Empty);
    }

    IEnumerable<object> IExportProvider.GetExportedValues(Type contractType, string? contractName)
    {
        return _exportProvider
            .GetExports(contractType, null, contractName ?? string.Empty)
            .Select(item => item.Value);
    }

    IEnumerable<IExport<object>> IExportProvider.GetExports(Type contractType, string? contractName)
    {
        return _exportProvider
            .GetExports(contractType, null, contractName ?? string.Empty)
            .Select(item => new ExportAdapter<object>(() => item.Value, new MetadataAdapter((IDictionary<string, object?>)item.Metadata)));
    }

    IEnumerable<IExport<T>> IExportProvider.GetExports<T>(string? contractName) where T: class
    {
        return _exportProvider
            .GetExports(typeof(T), null, contractName ?? string.Empty)
            .Select(item => new ExportAdapter<T>(() => (T)item.Value, new MetadataAdapter((IDictionary<string, object?>)item.Metadata)));
    }
}
