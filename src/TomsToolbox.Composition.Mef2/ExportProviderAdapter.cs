namespace TomsToolbox.Composition.Mef2
{
    using System;
    using System.Collections.Generic;
    using System.Composition;
    using System.Linq;

    /// <summary>
    /// An <see cref="IExportProvider"/> adapter for the MEF 2 <see cref="CompositionContext"/>
    /// </summary>
    /// <seealso cref="IExportProvider" />
    public class ExportProviderAdapter : IExportProvider
    {
        private readonly CompositionContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportProviderAdapter"/> class.
        /// </summary>
        /// <param name="context">The context providing the exports.</param>
        public ExportProviderAdapter(CompositionContext context)
        {
            _context = context;
        }

#pragma warning disable CS0067
        /// <inheritdoc />
        public event EventHandler<EventArgs>? ExportsChanged;

        T IExportProvider.GetExportedValue<T>(string? contractName) where T : class
        {
            return _context.GetExport<T>(contractName);
        }

        T? IExportProvider.GetExportedValueOrDefault<T>(string? contractName) where T : class
        {
            return _context.TryGetExport<T>(contractName, out var value) ? value : default;
        }

        bool IExportProvider.TryGetExportedValue<T>(string? contractName, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out T? value) where T : class
        {
            return _context.TryGetExport(contractName, out value) && value != null;
        }

        IEnumerable<T> IExportProvider.GetExportedValues<T>(string? contractName) where T : class
        {
            return _context.GetExports<T>(contractName);
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
            return _context
                .GetExports<ExportFactory<T, IDictionary<string, object>>>(contractName)
                .Select(item => new ExportAdapter<object>(() => item.CreateExport().Value, new MetadataAdapter(item.Metadata)));
        }
    }
}
