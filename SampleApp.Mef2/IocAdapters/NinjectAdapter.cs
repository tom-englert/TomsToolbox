namespace SampleApp.Mef2.IocAdapters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Data;

    using JetBrains.Annotations;

    using Ninject;
    using Ninject.Extensions.Conventions;
    using Ninject.Planning.Bindings;
    using Ninject.Syntax;

    using TomsToolbox.Composition;
    using TomsToolbox.Essentials;
    using TomsToolbox.Wpf.Converters;

    internal class NinjectAdapter : IIocAdapter
    {
        private const string ExportMetadataKey = "ExportMetadata";

        [CanBeNull]
        private IKernel _kernel;

        public IExportProvider Initialize()
        {
            var kernel = new StandardKernel();
            var exports = MetadataReader.Read(GetType().Assembly);

            foreach (var export in exports)
            {
                var type = export.Type;
                if (type == null)
                    continue;
                var exportMetadata = export.Metadata;
                if (exportMetadata == null)
                    continue;

                if (exportMetadata.Length == 1)
                {
                    var metadata = exportMetadata.Single();

                    var explicitContractType = metadata.GetValueOrDefault("ContractType") as Type;
                    var contractName = metadata.GetValueOrDefault("ContractName") as string;

                    IBindingWhenInNamedWithOrOnSyntax<object> binding;
                    if (explicitContractType == null)
                    {
                        binding = kernel.Bind(type).ToSelf();
                    }
                    else
                    {
                        binding = kernel.Bind(explicitContractType).To(type);
                    }

                    if (contractName != null)
                    {
                        binding.Named(contractName);
                    }

                    binding.WithMetadata(ExportMetadataKey, metadata);

                    if (export.IsShared)
                    {
                        binding.InSingletonScope();
                    }
                }
                else if (!export.IsShared)
                {
                    foreach (var metadata in exportMetadata)
                    {
                        var explicitContractType = metadata.GetValueOrDefault("ContractType") as Type;
                        var contractName = metadata.GetValueOrDefault("ContractName") as string;
                        IBindingWhenInNamedWithOrOnSyntax<object> binding;

                        if (explicitContractType == null)
                        {
                            binding = kernel.Bind(type).ToSelf();
                        }
                        else
                        {
                            binding = kernel.Bind(explicitContractType).To(type);
                        }

                        if (contractName != null)
                        {
                            binding.Named(contractName);
                        }

                        binding.WithMetadata(ExportMetadataKey, metadata);

                        if (export.IsShared)
                        {
                            binding.InSingletonScope();
                        }
                    }
                }
                else
                {
                    var masterBindingName = "71751FFE-46C5-465A-9F50-6AEFD1C14232";
                    
                    kernel.Bind(type).ToSelf().InSingletonScope().Named(masterBindingName);

                    foreach (var metadata in exportMetadata)
                    {
                        var explicitContractType = metadata.GetValueOrDefault("ContractType") as Type;
                        var contractName = metadata.GetValueOrDefault("ContractName") as string;
                        var binding = kernel.Bind(explicitContractType ?? type).ToMethod(_ => kernel.Get(type, masterBindingName));

                        if (contractName != null)
                        {
                            binding.Named(contractName);
                        }

                        binding.WithMetadata(ExportMetadataKey, metadata);
                    }
                }
            }

            var exportProvider = new ExportProvider(kernel);

            kernel.Bind<IExportProvider>().ToConstant(exportProvider);
            kernel.Bind(syntax => syntax
                .From(typeof(CoordinatesToPointConverter).Assembly)
                .Select(type => typeof(IValueConverter).IsAssignableFrom(type))
                .BindToSelf());

            _kernel = kernel;
            return exportProvider;
        }

        public void Dispose()
        {
            _kernel?.Dispose();
        }

        private class ExportProvider : IExportProvider
        {
            private readonly IKernel _kernel;

            public ExportProvider(IKernel kernel)
            {
                _kernel = kernel;
            }

            public event EventHandler<EventArgs> ExportsChanged;

            public T GetExportedValue<T>(string contractName = null)
            {
                return (contractName != null ? _kernel.Get<T>(contractName) : _kernel.Get<T>());
            }

            [CanBeNull]
            public T GetExportedValueOrDefault<T>(string contractName = null)
            {
                return GetExportedValues<T>().SingleOrDefault();
            }

            public IEnumerable<T> GetExportedValues<T>(string contractName = null)
            {
                return (contractName != null ? _kernel.GetAll<T>(contractName) : _kernel.GetAll<T>());
            }

            public IEnumerable<IExport<object>> GetExports(Type contractType, string contractName = null)
            {
                var bindings = _kernel.GetBindings(contractType)
                    .Where(binding => binding.Metadata.Name == contractName);

                var result = bindings.Select(binding => new ExportAdapter<object>(() => GetExportedValue(binding), binding.Metadata.Get<IDictionary<string, object>>(ExportMetadataKey)));

                return result.ToList();
            }

            private object GetExportedValue(IBinding binding)
            {
                bool Constraint(IBindingMetadata bindingMetadata)
                {
                    return bindingMetadata == binding.Metadata;
                }

                var request = _kernel.CreateRequest(binding.Service, Constraint, binding.Parameters, false, true);

                var result = _kernel.Resolve(request).Single();

                return result;
            }
        }
    }
}
