namespace TomsToolbox.Wpf.Composition.Mef2
{
    using System;
    using System.Collections.Generic;
    using System.Composition;
    using System.Linq;

    using JetBrains.Annotations;

    /// <summary>
    /// An <see cref="IExportProvider"/> adapter for the MEF 2 <see cref="CompositionContext"/>
    /// </summary>
    /// <seealso cref="TomsToolbox.Wpf.IExportProvider" />
    public class ExportProviderAdapter : IExportProvider
    {
        [NotNull]
        private readonly CompositionContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportProviderAdapter"/> class.
        /// </summary>
        /// <param name="context">The context providing the exports.</param>
        public ExportProviderAdapter([NotNull] CompositionContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public event EventHandler<EventArgs> ExportsChanged;

        IEnumerable<T> IExportProvider.GetExportedValues<T>([CanBeNull] string contractName)
        {
            return _context.GetExports<T>(contractName);
        }

        IEnumerable<ILazy<object>> IExportProvider.GetExports([NotNull] Type type, [CanBeNull] string contractName)
        {
            var exportMethod = GetType().GetMethod(nameof(GetExports))?.MakeGenericMethod(type);
            if (exportMethod == null)
                throw new InvalidOperationException("Method not found: " + nameof(GetExports));
            return (IEnumerable<ILazy<object>>)exportMethod.Invoke(this, new object[] {contractName});
        }

        /// <summary>
        /// Gets the exports.
        /// </summary>
        /// <typeparam name="T">The type of the object</typeparam>
        /// <param name="contractName">Name of the contract.</param>
        /// <returns>The exported object.</returns>
        public IEnumerable<ILazy<object>> GetExports<T>([CanBeNull] string contractName)
        {
            return _context
                .GetExports<ExportFactory<T, IDictionary<string, object>>>(contractName)
                .Select(item => new LazyAdapter<object>(() => item.CreateExport().Value, item.Metadata));
        }
    }
}
