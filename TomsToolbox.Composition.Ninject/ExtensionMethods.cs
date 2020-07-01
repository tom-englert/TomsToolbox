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
        /// <param name="exports">The exports.</param>
        public static void BindExports(this IKernel kernel, IEnumerable<ExportInfo> exports)
        {
            foreach (var export in exports)
            {
                var type = export.Type;
                if (type == null)
                    continue;
                var exportMetadata = export.Metadata;
                if (exportMetadata == null)
                    continue;

                if ((exportMetadata.Length == 1) || !export.IsShared)
                {
                    foreach (var metadata in exportMetadata)
                    {
                        var explicitContractType = metadata.GetValueOrDefault("ContractType") as Type;
                        var contractName = metadata.GetValueOrDefault("ContractName") as string;

                        var binding = explicitContractType == null
                            ? kernel.Bind(type).ToSelf()
                            : kernel.Bind(explicitContractType).To(type);

                        if (contractName != null)
                        {
                            binding.Named(contractName);
                        }

                        binding.WithMetadata(ExportProvider.ExportMetadataKey, new MetadataAdapter(metadata));

                        if (export.IsShared)
                        {
                            binding.InSingletonScope();
                        }
                    }
                }
                else
                {
                    const string masterBindingName = "71751FFE-46C5-465A-9F50-6AEFD1C14232";

                    var masterBinding = kernel.Bind(type).ToSelf().InSingletonScope();

                    var hasNativeExport = exportMetadata.Any(metadata => metadata.GetContractType() == null && metadata.GetContractName() == null);

                    if (!hasNativeExport)
                    {
                        // make binding to self hidden, object has only specific exports.
                        masterBinding.Named(masterBindingName);
                    }

                    foreach (var metadata in exportMetadata)
                    {
                        var explicitContractType = metadata.GetContractType();
                        var contractName = metadata.GetContractName();

                        if (explicitContractType == null && contractName == null)
                            continue;

                        var binding = hasNativeExport 
                            ? kernel.Bind(explicitContractType ?? type).ToMethod(_ => kernel.Get(type))
                            : kernel.Bind(explicitContractType ?? type).ToMethod(_ => kernel.Get(type, masterBindingName));

                        if (contractName != null)
                        {
                            binding.Named(contractName);
                        }

                        binding.WithMetadata(ExportProvider.ExportMetadataKey, new MetadataAdapter(metadata));
                    }
                }
            }
        }

        private static Type? GetContractType(this IDictionary<string, object> metadata)
        {
            return metadata.GetValueOrDefault("ContractType") as Type;
        }

        private static string? GetContractName(this IDictionary<string, object> metadata)
        {
            return metadata.GetValueOrDefault("ContractName") as string;
        }
    }
}
