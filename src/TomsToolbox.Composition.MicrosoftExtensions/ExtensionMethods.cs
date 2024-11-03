namespace TomsToolbox.Composition.MicrosoftExtensions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;
using TomsToolbox.Essentials;

/// <summary>
/// Extension methods for Microsoft.Extensions.DependencyInjection
/// </summary>
public static class ExtensionMethods
{
    private static readonly MethodInfo _addMetaDataExportMethod = typeof(ExtensionMethods).GetMethod(nameof(AddMetadataExportT), BindingFlags.NonPublic | BindingFlags.Static)!;
    private static readonly MethodInfo _getMetaDataMethod = typeof(ExtensionMethods).GetMethod(nameof(GetMetadataT), BindingFlags.NonPublic | BindingFlags.Static)!;

    private static readonly Type _exportType = typeof(IExport<>);

    /// <summary>
    /// Binds the exports of the specified assemblies to the service collection.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <param name="assemblies">The assemblies.</param>
    /// <returns>The <paramref name="serviceCollection"/></returns>
    public static IServiceCollection BindExports(this IServiceCollection serviceCollection, params Assembly[] assemblies)
    {
        return BindExports(serviceCollection, (IEnumerable<Assembly>)assemblies);
    }

    /// <summary>
    /// Binds the exports of the specified assemblies to the service collection.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <param name="assemblies">The assemblies.</param>
    /// <returns>The <paramref name="serviceCollection"/></returns>
    public static IServiceCollection BindExports(this IServiceCollection serviceCollection, IEnumerable<Assembly> assemblies)
    {
        return BindExports(serviceCollection, assemblies.SelectMany(MetadataReader.Read));
    }

    /// <summary>
    /// Binds the specified exports to the service collection.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <param name="exportInfos">The exports.</param>
    /// <returns>The <paramref name="serviceCollection"/></returns>
    public static IServiceCollection BindExports(this IServiceCollection serviceCollection, IEnumerable<ExportInfo> exportInfos)
    {
        foreach (var exportInfo in exportInfos)
        {
            var type = exportInfo.Type;
            if (type == null)
                continue;
            var exportMetadata = exportInfo.Metadata;
            if (exportMetadata == null)
                continue;

            var exports = exportMetadata
                .Select(item => new { Type = type, ContractType = item.GetContractTypeFor(type), ContractName = item.GetContractName(), Metadata = item })
                .Distinct()
                .ToList();

            if (exportInfo.IsShared)
            {
                switch (exportInfo.SharingBoundary)
                {
                    case SharingBoundary.Global:
                        serviceCollection.AddSingleton(type);
                        break;
                    case SharingBoundary.Scoped:
                        serviceCollection.AddScoped(type);
                        break;
                    default:
                        throw new InvalidOperationException($"Unknown sharing boundary: {exportInfo.SharingBoundary}; supported are null, SharingBoundary.Global or SharingBoundary.Scoped");
                }
            }
            else
            {
                serviceCollection.AddTransient(type);
            }

            foreach (var export in exports)
            {
                var contractType = export.ContractType;

                if (contractType != null)
                {
                    if (!contractType.IsAssignableFrom(type))
                        throw new InvalidOperationException($"The contract type '{contractType.FullName}' is not assignable from the implementation type '{type.FullName}'.");

                    if (string.IsNullOrEmpty(export.ContractName))
                    {
                        if (exportInfo.IsShared)
                        {
                            switch (exportInfo.SharingBoundary)
                            {
                                case SharingBoundary.Global:
                                    serviceCollection.AddSingleton(contractType, sp => sp.GetService(type)!);
                                    break;
                                case SharingBoundary.Scoped:
                                    serviceCollection.AddScoped(contractType, sp => sp.GetService(type)!);
                                    break;
                                default:
                                    throw new InvalidOperationException($"Unknown sharing boundary: {exportInfo.SharingBoundary}; supported are null, SharingBoundary.Global or SharingBoundary.Scoped");
                            }
                        }
                        else
                        {
                            serviceCollection.AddTransient(contractType, sp => sp.GetService(type)!);
                        }
                    }
                }

                serviceCollection.AddMetadataExport(contractType ?? type, type, new MetadataAdapter(export.Metadata));
            }
        }

        return serviceCollection;
    }

    /// <summary>
    /// Binds the default metadata to the service.
    /// </summary>
    /// <param name="serviceCollection">The service collection</param>
    /// <param name="serviceType">The service type that the service was registered with.</param>
    /// <returns>The <paramref name="serviceCollection"/></returns>
    public static IServiceCollection BindMetadata(this IServiceCollection serviceCollection, Type serviceType)
    {
        return BindMetadata(serviceCollection, serviceType, serviceType);
    }

    /// <summary>
    /// Binds the metadata to the service.
    /// </summary>
    /// <param name="serviceCollection">The service collection</param>
    /// <param name="serviceType">The service type that the service was registered with.</param>
    /// <param name="metadata">The metadata for the service</param>
    /// <returns>The <paramref name="serviceCollection"/></returns>
    public static IServiceCollection BindMetadata(this IServiceCollection serviceCollection, Type serviceType, IDictionary<string, object?> metadata)
    {
        return BindMetadata(serviceCollection, serviceType, serviceType, metadata);
    }

    /// <summary>
    /// Binds the default metadata to the service.
    /// </summary>
    /// <param name="serviceCollection">The service collection</param>
    /// <param name="serviceType">The service type that the service was registered with.</param>
    /// <param name="implementationType">The implementation type that the service was registered with.</param>
    /// <returns>The <paramref name="serviceCollection"/></returns>
    public static IServiceCollection BindMetadata(this IServiceCollection serviceCollection, Type serviceType, Type implementationType)
    {
        return serviceCollection.BindMetadata(serviceType, implementationType, implementationType.GetDefaultMetadata(serviceType));
    }

    /// <summary>
    /// Binds the metadata to the service.
    /// </summary>
    /// <param name="serviceCollection">The service collection</param>
    /// <param name="serviceType">The service type that the service was registered with.</param>
    /// <param name="implementationType">The implementation type that the service was registered with.</param>
    /// <param name="metadata">The metadata for the service</param>
    /// <returns>The <paramref name="serviceCollection"/></returns>
    public static IServiceCollection BindMetadata(this IServiceCollection serviceCollection, Type serviceType, Type implementationType, IDictionary<string, object?> metadata)
    {
        return serviceCollection.AddMetadataExport(serviceType, implementationType, new MetadataAdapter(metadata));
    }

    private static void AddMetadataExportT<T>(IServiceCollection serviceCollection, Type implementationType, IMetadata metadata) where T : class
    {
        serviceCollection.AddTransient<IExport<T>>(sp => new ExportAdapter<T>(() => (T?)sp.GetService(implementationType), metadata));
    }

    private static IServiceCollection AddMetadataExport(this IServiceCollection serviceCollection, Type serviceType, Type implementationType, IMetadata metadata)
    {
        var method = _addMetaDataExportMethod.MakeGenericMethod(serviceType);

        method.Invoke(null, new object?[] { serviceCollection, implementationType, metadata });

        return serviceCollection;
    }

    /// <summary>
    /// Gets the service descriptor and the associated metadata of the metadata entries matching the type.
    /// </summary>
    /// <typeparam name="T">The service type</typeparam>
    public static IList<ServiceInfo> GetServiceInfo<T>(this IServiceCollection serviceCollection) where T : class
    {
        var existing = serviceCollection
            .Where(item => typeof(IExport<T>) == item.ServiceType)
            .Select(item => new ServiceInfo(item, GetMetadataT<T>(item)))
            .ToArray();

        return existing;
    }

    /// <summary>
    /// Gets the service descriptor and the associated metadata of all metadata entries.
    /// </summary>
    public static IList<ServiceInfo> GetServiceInfo(this IServiceCollection serviceCollection)
    {
        var existing = serviceCollection
            .Where(IsExportType)
            .Select(item => new ServiceInfo(item, GetMetadata(item)))
            .ToArray();

        return existing;
    }

    private static bool IsExportType(ServiceDescriptor serviceDescriptor)
    {
        var serviceType = serviceDescriptor.ServiceType;

        return serviceType.Name == _exportType.Name && serviceType.Namespace == _exportType.Namespace;
    }

    private static IMetadata? GetMetadataT<T>(ServiceDescriptor serviceDescriptor) where T : class
    {
        try
        {
            // ! We can call ImplementationFactory with null here, because we only get the export, but don't instantiate it's value
            var export = serviceDescriptor.ImplementationFactory?.Invoke(null!) as IExport<T>;

            return export?.Metadata;
        }
        catch
        {
            return null;
        }
    }

    private static IMetadata? GetMetadata(ServiceDescriptor serviceDescriptor)
    {
        try
        {
            var itemType = serviceDescriptor.ServiceType.GenericTypeArguments.Single();

            var instance = _getMetaDataMethod.MakeGenericMethod(itemType);

            var metadata = (IMetadata?)instance.Invoke(null, new object[] { serviceDescriptor });

            return metadata;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Gets the exported value of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the exported value.</typeparam>
    /// <param name="serviceProvider">The service provider.</param>
    /// <returns>The exported value of type <typeparamref name="T"/>.</returns>
    public static T GetExportedValue<T>(this IServiceProvider serviceProvider) where T : class
    {
        return serviceProvider.GetRequiredService<T>();
    }

    /// <summary>
    /// Gets the exported value of the specified type and contract name.
    /// </summary>
    /// <typeparam name="T">The type of the exported value.</typeparam>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="contractName">The contract name.</param>
    /// <returns>The exported value of type <typeparamref name="T"/>.</returns>
    public static T GetExportedValue<T>(this IServiceProvider serviceProvider, string? contractName) where T : class
    {
        return serviceProvider.GetExportProvider().GetExportedValue<T>(contractName);
    }

    /// <summary>
    /// Gets the exported value of the specified type, or default if not found.
    /// </summary>
    /// <typeparam name="T">The type of the exported value.</typeparam>
    /// <param name="serviceProvider">The service provider.</param>
    /// <returns>The exported value of type <typeparamref name="T"/> or default if not found.</returns>
    public static T? GetExportedValueOrDefault<T>(this IServiceProvider serviceProvider) where T : class
    {
        return serviceProvider.GetService<T>();
    }

    /// <summary>
    /// Gets the exported value of the specified type and contract name, or default if not found.
    /// </summary>
    /// <typeparam name="T">The type of the exported value.</typeparam>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="contractName">The contract name.</param>
    /// <returns>The exported value of type <typeparamref name="T"/> or default if not found.</returns>
    public static T? GetExportedValueOrDefault<T>(this IServiceProvider serviceProvider, string? contractName) where T : class
    {
        return serviceProvider.GetExportProvider().GetExportedValueOrDefault<T>(contractName);
    }

    /// <summary>
    /// Gets the exported values of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the exported values.</typeparam>
    /// <param name="serviceProvider">The service provider.</param>
    /// <returns>An enumerable of exported values of type <typeparamref name="T"/>.</returns>
    public static IEnumerable<T> GetExportedValues<T>(this IServiceProvider serviceProvider) where T : class
    {
        return serviceProvider.GetServices<T>().ExceptNullItems();
    }

    /// <summary>
    /// Gets the exported values of the specified type and contract name.
    /// </summary>
    /// <typeparam name="T">The type of the exported values.</typeparam>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="contractName">The contract name.</param>
    /// <returns>An enumerable of exported values of type <typeparamref name="T"/>.</returns>
    public static IEnumerable<T> GetExportedValues<T>(this IServiceProvider serviceProvider, string? contractName) where T : class
    {
        return serviceProvider.GetExportProvider().GetExportedValues<T>(contractName);
    }

#pragma warning disable CA1859 // Use concrete types when possible for improved performance => This method is required to explicitly cast the IServiceProvider to an IExportProvider
    private static IExportProvider GetExportProvider(this IServiceProvider serviceProvider)
#pragma warning restore CA1859 // Use concrete types when possible for improved performance
    {
        return new ExportProviderAdapter(serviceProvider);
    }
}
