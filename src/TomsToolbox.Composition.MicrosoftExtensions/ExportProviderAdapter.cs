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
public sealed class ExportProviderAdapter : IExportProvider, IDisposable
{
    private static readonly MethodInfo _getExportsAsObjectMethod = typeof(ExportProviderAdapter).GetMethod(nameof(GetExportsAsObject), BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new InvalidOperationException("Method not found: " + nameof(GetExportsAsObject));

    /// <summary>
    /// Initializes a new instance of the <see cref="ExportProviderAdapter" /> class.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <param name="options">The options to build the service provider.</param>
    public ExportProviderAdapter(IServiceCollection serviceCollection, ServiceProviderOptions? options = null)
    {
        serviceCollection.AddSingleton<IExportProvider>(this);

        ServiceProvider = serviceCollection.BuildServiceProvider(options ?? new ServiceProviderOptions());
    }

#pragma warning disable CS0067
    /// <inheritdoc />
    public event EventHandler<EventArgs>? ExportsChanged;

    /// <summary>
    /// Gets the adapted service provider.
    /// </summary>
    public ServiceProvider ServiceProvider { get; }

    T IExportProvider.GetExportedValue<T>(string? contractName) where T : class
    {
        if (string.IsNullOrEmpty(contractName))
        {
            return ServiceProvider.GetRequiredService<T>();
        }

        var exports = GetServices<T>(contractName)
            .Select(item => item.Value)
            .Single();

        return exports;
    }

    T? IExportProvider.GetExportedValueOrDefault<T>(string? contractName) where T : class
    {
        if (string.IsNullOrEmpty(contractName))
        {
            return ServiceProvider.GetService<T>();
        }

        var exports = GetServices<T>(contractName)
            .Select(item => item.Value)
            .SingleOrDefault();

        return exports;
    }

    bool IExportProvider.TryGetExportedValue<T>(string? contractName, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out T? value) where T : class
    {
        return (value = ((IExportProvider)this).GetExportedValueOrDefault<T>()) != null;
    }

    IEnumerable<T> IExportProvider.GetExportedValues<T>(string? contractName) where T : class
    {
        if (string.IsNullOrEmpty(contractName))
        {
            return ServiceProvider.GetServices<T>();
        }

        var exports = GetServices<T>(contractName)
            .Select(item => item.Value);

        return exports;
    }

    IEnumerable<object> IExportProvider.GetExportedValues(Type contractType, string? contractName)
    {
        if (string.IsNullOrEmpty(contractName))
        {
            return ServiceProvider.GetServices(contractType).ExceptNullItems();
        }

        var exports = GetExports(contractType, contractName)
            .Select(item => item.Value);

        return exports;
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

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        ServiceProvider.Dispose();
    }
}
