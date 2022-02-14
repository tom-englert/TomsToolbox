namespace TomsToolbox.Composition.MicrosoftExtensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using global::Microsoft.Extensions.DependencyInjection;
    using global::Microsoft.Extensions.Options;

    /// <summary>
    /// An <see cref="IExportProvider"/> adapter for the Microsoft.Extensions.DependencyInjection <see cref="ServiceCollection"/>
    /// </summary>
    /// <seealso cref="IExportProvider" />
    public class ExportProviderAdapter : IExportProvider
    {
        private readonly ServiceProvider _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportProviderAdapter"/> class.
        /// </summary>
        /// <param name="context">The context providing the exports.</param>
        public ExportProviderAdapter(ServiceProvider context)
        {
            _context = context;
        }

#pragma warning disable CS0067
        /// <inheritdoc />
        public event EventHandler<EventArgs>? ExportsChanged;

        T IExportProvider.GetExportedValue<T>(string? contractName) where T : class
        {
            if (string.IsNullOrEmpty(contractName))
            {
                return _context.GetRequiredService<T>();
            }

            return _context.GetRequiredService<IOptionsSnapshot<T>>().Get(contractName);
        }

        T? IExportProvider.GetExportedValueOrDefault<T>(string? contractName) where T : class
        {
            if (string.IsNullOrEmpty(contractName))
            {
                return _context.GetService<T>();
            }

            return _context.GetService<IOptionsSnapshot<T>>()?.Get(contractName);
        }

        bool IExportProvider.TryGetExportedValue<T>(string? contractName, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out T? value) where T : class
        {
            return (value = ((IExportProvider)this).GetExportedValueOrDefault<T>()) != null;
        }

        IEnumerable<T> IExportProvider.GetExportedValues<T>(string? contractName) where T : class
        {
            if (string.IsNullOrEmpty(contractName))
            {
                return _context.GetServices<T>();
            }

            return _context
                .GetServices<IOptionsSnapshot<T>>()
                .Select(snapshot => snapshot.Get(contractName));
        }

        IEnumerable<IExport<object>> IExportProvider.GetExports(Type contractType, string? contractName)
        {
            var exportMethod = GetType().GetMethod(nameof(GetExports))?.MakeGenericMethod(contractType);
            if (exportMethod == null)
                throw new InvalidOperationException("Method not found: " + nameof(GetExports));
            return (IEnumerable<IExport<object>>)exportMethod.Invoke(this, new object?[] { contractName });
        }

        /// <summary>
        /// Gets the exports.
        /// </summary>
        /// <typeparam name="T">The type of the object</typeparam>
        /// <param name="contractName">Name of the contract.</param>
        /// <returns>The exported object.</returns>
        public IEnumerable<IExport<object>> GetExports<T>(string? contractName)
        {
            return Enumerable.Empty<IExport<object>>();

            //return _context
            //    .GetExports<ExportFactory<T, IDictionary<string, object>>>(contractName)
            //    .Select(item => new ExportAdapter<object>(() => item.CreateExport().Value, new MetadataAdapter(item.Metadata)));
        }
    }
}
