namespace TomsToolbox.Composition.MicrosoftExtensions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

using TomsToolbox.Essentials;

/// <summary>
/// An <see cref="IExportProvider"/> adapter for the Microsoft.Extensions.DependencyInjection <see cref="ServiceCollection"/>
/// </summary>
/// <seealso cref="IExportProvider" />
public sealed class ExportProviderAdapter : IExportProvider
{
    private static readonly MethodInfo _getExportsAsObjectMethod = typeof(ExportProviderAdapter).GetMethod(nameof(GetExportsAsObject), BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new InvalidOperationException("Method not found: " + nameof(GetExportsAsObject));

    /// <summary>
    /// Initializes a new instance of the <see cref="ExportProviderAdapter" /> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    public ExportProviderAdapter(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }

#pragma warning disable CS0067
    /// <inheritdoc />
    public event EventHandler<EventArgs>? ExportsChanged;

    /// <summary>
    /// Gets the adapted service provider.
    /// </summary>
    public IServiceProvider ServiceProvider { get; }

    T IExportProvider.GetExportedValue<T>(string? contractName) where T : class
    {
        if (string.IsNullOrEmpty(contractName))
        {
            return ServiceProvider.GetRequiredService<T>();
        }

        var exports = GetServices<T>(contractName)
            .Select(item => item.Value)
            .LastOrDefault();

        return exports ?? throw new InvalidOperationException($"No service is registered for type {typeof(T).FullName}.");
    }

    T? IExportProvider.GetExportedValueOrDefault<T>(string? contractName) where T : class
    {
        if (string.IsNullOrEmpty(contractName))
        {
            return ServiceProvider.GetService<T>();
        }

        var export = GetServices<T>(contractName)
            .Select(item => item.Value)
            .LastOrDefault();

        return export;
    }

    bool IExportProvider.TryGetExportedValue<T>(string? contractName, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out T? value) where T : class
    {
        return (value = ((IExportProvider)this).GetExportedValueOrDefault<T>()) != null;
    }

    IEnumerable<T> IExportProvider.GetExportedValues<T>(string? contractName) where T : class
    {
        if (string.IsNullOrEmpty(contractName))
        {
            return ServiceProvider.GetServices<T>().ExceptNullItems();
        }

        var exports = GetServices<T>(contractName)
            .Select(item => item.Value);

        return exports.ExceptNullItems();
    }

    IEnumerable<object> IExportProvider.GetExportedValues(Type contractType, string? contractName)
    {
        if (string.IsNullOrEmpty(contractName))
        {
            return ServiceProvider.GetServices(contractType).ExceptNullItems();
        }

        var exports = GetExports(contractType, contractName)
            .Select(item => item.Value);

        return exports.ExceptNullItems();
    }

    IEnumerable<IExport<object>> IExportProvider.GetExports(Type contractType, string? contractName)
    {
        return GetExports(contractType, contractName);
    }

    IEnumerable<IExport<T>> IExportProvider.GetExports<T>(string? contractName)
        where T : class
    {
        var exports = GetServices<T>(contractName)
            .Select(item => new ExportAdapter<T>(() => item.Value, item.Metadata));

        return exports;
    }

    private IEnumerable<IExport<object>> GetExports(Type contractType, string? contractName)
    {
        var exportMethod = _getExportsAsObjectMethod.MakeGenericMethod(contractType);

        return (IEnumerable<IExport<object>>)exportMethod.Invoke(this, new object?[] { contractName });
    }

    private IEnumerable<IExport<object>> GetExportsAsObject<T>(string? contractName)
        where T : class
    {
        var exports = GetServices<T>(contractName)
            .Select(item => new ExportAdapter<object>(() => item.Value, item.Metadata));

        return exports;
    }

    private IEnumerable<IExport<T>> GetServices<T>(string? contractName) where T : class
    {
        return ServiceProvider.GetServices<IExport<T>>()
            .Where(item => item.Metadata.ContractNameMatches(contractName));
    }
}
