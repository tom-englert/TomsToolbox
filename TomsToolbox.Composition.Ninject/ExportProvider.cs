namespace TomsToolbox.Composition.Ninject
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using global::Ninject;
    using global::Ninject.Planning.Bindings;

    using JetBrains.Annotations;

    /// <summary>
    /// Implements the <see cref="IExportProvider"/> interface for the Ninject DI container
    /// </summary>
    /// <seealso cref="IExportProvider" />
    public class ExportProvider : IExportProvider
    {
        /// <summary>
        /// The key under which the export metadata is stored.
        /// </summary>
        public const string ExportMetadataKey = "ExportMetadata";

        private readonly IKernel _kernel;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportProvider"/> class.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        public ExportProvider(IKernel kernel)
        {
            _kernel = kernel;
        }

        /// <inheritdoc />
        public event EventHandler<EventArgs>? ExportsChanged;

        T IExportProvider.GetExportedValue<T>([CanBeNull] string? contractName) where T : class
        {
            return (contractName != null ? _kernel.Get<T>(contractName) : _kernel.Get<T>());
        }

        [CanBeNull]
        T? IExportProvider.GetExportedValueOrDefault<T>([CanBeNull] string? contractName) where T : class
        {
            return GetExportedValues<T>(contractName).SingleOrDefault();
        }

        bool IExportProvider.TryGetExportedValue<T>([CanBeNull] string? contractName, [CanBeNull, System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out T? value) where T : class
        {
            var values = GetExportedValues<T>(contractName).ToList();

            if (values.Count != 1)
            {
                value = default;
                return false;
            }

            value = values[0];
            return true;
        }

        IEnumerable<T> IExportProvider.GetExportedValues<T>([CanBeNull] string? contractName) where T : class
        {
            return GetExportedValues<T>(contractName);
        }

        IEnumerable<IExport<object>> IExportProvider.GetExports(Type contractType, [CanBeNull] string? contractName)
        {
            var bindings = _kernel.GetBindings(contractType)
                .Where(binding => binding.Metadata.Name == contractName);

            var result = bindings.Select(binding => new ExportAdapter<object>(() => GetExportedValue(binding), binding.Metadata.Get<IMetadata>(ExportMetadataKey)));

            return result.ToList();
        }

        private IEnumerable<T> GetExportedValues<T>([CanBeNull] string? contractName)
        {
            return (contractName != null ? _kernel.GetAll<T>(contractName) : _kernel.GetAll<T>());
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