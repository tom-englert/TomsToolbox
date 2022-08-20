namespace TomsToolbox.Composition.MicrosoftExtensions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for Microsoft.Extensions.DependencyInjection
/// </summary>
public static class ExtensionMethods
{
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
                    serviceCollection.AddTransient(contractType, sp => sp.GetRequiredService(type));
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
    public static IServiceCollection BindMetadata(this IServiceCollection serviceCollection, Type serviceType, Dictionary<string, object?> metadata)
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
        serviceCollection.AddTransient<IExport<T>>(sp => new ExportAdapter<T>(() => (T)sp.GetRequiredService(implementationType), metadata));
    }

    private static IServiceCollection AddMetadataExport(this IServiceCollection serviceCollection, Type serviceType, Type implementationType, IMetadata metadata)
    {
        var method = typeof(ExtensionMethods).GetMethod(nameof(AddMetadataExportT), BindingFlags.NonPublic | BindingFlags.Static)!.MakeGenericMethod(serviceType);
        method.Invoke(null, new object?[] { serviceCollection, implementationType, metadata });
        return serviceCollection;
    }
}