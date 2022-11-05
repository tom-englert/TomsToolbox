namespace TomsToolbox.Composition.Mef2;

using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Reflection;

/// <summary>
/// An <see cref="IExportProvider"/> adapter for the MEF 2 <see cref="CompositionContext"/>
/// </summary>
/// <seealso cref="IExportProvider" />
public class ExportProviderAdapter : IExportProvider
{
    private static readonly MethodInfo _getExportsAsObjectMethod = typeof(ExportProviderAdapter).GetMethod(nameof(GetExportsAsObject), BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new InvalidOperationException("Method not found: " + nameof(GetExportsAsObject));
    private readonly CompositionContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExportProviderAdapter"/> class.
    /// </summary>
    /// <param name="context">The context providing the exports.</param>
    public ExportProviderAdapter(CompositionContext context)
    {
        _context = context;
    }

#pragma warning disable CS0067
    /// <inheritdoc />
    public event EventHandler<EventArgs>? ExportsChanged;

    T IExportProvider.GetExportedValue<T>(string? contractName) where T : class
    {
        return _context.GetExport<T>(contractName);
    }

    T? IExportProvider.GetExportedValueOrDefault<T>(string? contractName) where T : class
    {
        return _context.TryGetExport<T>(contractName, out var value) ? value : default;
    }

    bool IExportProvider.TryGetExportedValue<T>(string? contractName, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out T? value) where T : class
    {
        return _context.TryGetExport(contractName, out value) && value != null;
    }

    IEnumerable<T> IExportProvider.GetExportedValues<T>(string? contractName) where T : class
    {
        return _context.GetExports<T>(contractName);
    }

    IEnumerable<IExport<object>> IExportProvider.GetExports(Type contractType, string? contractName)
    {
        var exportMethod = _getExportsAsObjectMethod.MakeGenericMethod(contractType);

        return (IEnumerable<IExport<object>>)exportMethod.Invoke(this, new object?[] { contractName })!;
    }

    IEnumerable<object> IExportProvider.GetExportedValues(Type contractType, string? contractName)
    {
        return _context.GetExports(contractType, contractName);
    }

    IEnumerable<IExport<T>> IExportProvider.GetExports<T>(string? contractName) where T : class
    {
        return _context
            .GetExports<ExportFactory<T, IDictionary<string, object?>>>(contractName)
            .Select(item => new ExportAdapter<T>(() => item.CreateExport().Value ?? throw new InvalidOperationException("Export did not return a value."), new MetadataAdapter(item.Metadata)));
    }

    private IEnumerable<IExport<object>> GetExportsAsObject<T>(string? contractName)
    {
        return _context
            .GetExports<ExportFactory<T, IDictionary<string, object?>>>(contractName)
            .Select(item => new ExportAdapter<object>(() => item.CreateExport().Value ?? throw new InvalidOperationException("Export did not return a value."), new MetadataAdapter(item.Metadata)));
    }
}
