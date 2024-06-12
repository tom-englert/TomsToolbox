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
                serviceCollection.AddSingleton(type);
            else
                serviceCollection.AddTransient(type);

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
                            serviceCollection.AddSingleton(contractType, sp => sp.GetService(type)!);
                        else
                            serviceCollection.AddTransient(contractType, sp => sp.GetService(type)!);
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
}
