namespace TomsToolbox.Composition.Ninject
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using global::Ninject;

    using TomsToolbox.Essentials;

    /// <summary>
    /// Extension methods for Ninject DI
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Binds the exports of the specified assemblies to the kernel.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        /// <param name="assemblies">The assemblies.</param>
        public static void BindExports(this IKernel kernel, params Assembly[] assemblies)
        {
            BindExports(kernel, (IEnumerable<Assembly>)assemblies);
        }

        /// <summary>
        /// Binds the exports of the specified assemblies to the kernel.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        /// <param name="assemblies">The assemblies.</param>
        public static void BindExports(this IKernel kernel, IEnumerable<Assembly> assemblies)
        {
            BindExports(kernel, assemblies.SelectMany(MetadataReader.Read));
        }

        /// <summary>
        /// Binds the specified exports to the kernel.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        /// <param name="exportInfos">The exports.</param>
        public static void BindExports(this IKernel kernel, IEnumerable<ExportInfo> exportInfos)
        {
            foreach (var exportInfo in exportInfos)
            {
                var type = exportInfo.Type;
                if (type == null)
                    continue;
                var exportMetadata = exportInfo.Metadata;
                if (exportMetadata == null)
                    continue;

                if ((exportMetadata.Length == 1) || !exportInfo.IsShared)
                {
                    foreach (var metadata in exportMetadata)
                    {
                        var explicitContractType = metadata.GetContractTypeFor(type);
                        var contractName = metadata.GetContractName();

                        var binding = explicitContractType == null
                            ? kernel.Bind(type).ToSelf()
                            : kernel.Bind(explicitContractType).To(type);

                        if (contractName != null)
                        {
                            binding.Named(contractName);
                        }

                        binding.WithMetadata(ExportProvider.ExportMetadataKey, new MetadataAdapter(metadata));

                        if (exportInfo.IsShared)
                        {
                            binding.InSingletonScope();
                        }
                    }
                }
                else
                {
                    var masterBindingName = ExportProvider.DefaultMasterBindingName;

                    var exports = exportMetadata
                        .Select(item => (Type: type, ContractType: item.GetContractTypeFor(type), ContractName: item.GetContractName(), Metadata: item))
                        .Distinct()
                        .ToList();

                    var nativeNamedExports = exports
                        .Where(export => export.ContractType == null && export.ContractName != null)
                        .ToList();

                    if (nativeNamedExports.Any())
                    {
                        masterBindingName = nativeNamedExports[0].ContractName;
                    }

                    kernel.Bind(type).ToSelf().InSingletonScope().Named(masterBindingName);

                    foreach (var export in exports)
                    {
                        var explicitContractType = export.ContractType;
                        var contractName = export.ContractName;

                        if (explicitContractType == null && (contractName == null || contractName == masterBindingName))
                            continue;

                        var binding = kernel.Bind(explicitContractType ?? type).ToMethod(context => context.Kernel.Get(type, masterBindingName));

                        if (contractName != null)
                        {
                            binding.Named(contractName);
                        }

                        binding.WithMetadata(ExportProvider.ExportMetadataKey, new MetadataAdapter(export.Metadata));
                    }
                }
            }
        }

        private static Type? GetContractTypeFor(this IDictionary<string, object> metadata, Type elementType)
        {
            var type = metadata.GetValueOrDefault("ContractType") as Type;
            return type == elementType ? null : type;
        }

        private static string? GetContractName(this IDictionary<string, object> metadata)
        {
            return metadata.GetValueOrDefault("ContractName") as string;
        }
    }
}
