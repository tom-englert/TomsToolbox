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
        public event EventHandler<EventArgs> ExportsChanged;

        /// <inheritdoc />
        public T GetExportedValue<T>(string contractName = null)
        {
            return (contractName != null ? _kernel.Get<T>(contractName) : _kernel.Get<T>());
        }

        /// <inheritdoc />
        [CanBeNull]
        public T GetExportedValueOrDefault<T>(string contractName = null)
        {
            return GetExportedValues<T>().SingleOrDefault();
        }

        /// <inheritdoc />
        public IEnumerable<T> GetExportedValues<T>(string contractName = null)
        {
            return (contractName != null ? _kernel.GetAll<T>(contractName) : _kernel.GetAll<T>());
        }

        /// <inheritdoc />
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