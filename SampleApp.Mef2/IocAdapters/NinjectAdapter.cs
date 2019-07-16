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

                var selfBinding = kernel.Bind(type).ToSelf();

                foreach (var metadata in exportMetadata)
                {
                    var explicitContractType = metadata.GetValueOrDefault("ContractType") as Type;
                    var contractName = metadata.GetValueOrDefault("ContractName") as string;
                    IBindingWhenInNamedWithOrOnSyntax<object> binding;

                    if ((explicitContractType == null) && (contractName == null))
                    {
                        binding = selfBinding;
                    }
                    else
                    {
                        binding = kernel.Bind(explicitContractType).ToMethod(_ => kernel.Get(type));

                        if (contractName != null)
                        {
                            binding.Named(contractName);
                        }
                    }

                    binding.WithMetadata("ExportMetadata", metadata);

                    if (export.IsShared)
                    {
                        binding.InSingletonScope();
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

                var result = bindings.Select(binding => new ExportAdapter<object>(() => GetExportedValue(binding), binding.Metadata.Get<IDictionary<string, object>>("ExportMetadata")));

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

        internal class ExportAdapter<T> : IExport<T>
        {
            [NotNull]
            private readonly Func<T> _valueFactoryCallback;
            [CanBeNull]
            private readonly IDictionary<string, object> _metadata;

            public ExportAdapter([NotNull] Func<T> valueFactoryCallback, [CanBeNull] IDictionary<string, object> metadata)
            {
                _valueFactoryCallback = valueFactoryCallback;
                _metadata = metadata;
            }

            [CanBeNull]
            T IExport<T, IDictionary<string, object>>.Value => _valueFactoryCallback();

            [CanBeNull]
            IDictionary<string, object> IExport<T, IDictionary<string, object>>.Metadata => _metadata;
        }
    }
}
