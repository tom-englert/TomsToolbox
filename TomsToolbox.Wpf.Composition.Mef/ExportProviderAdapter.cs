namespace TomsToolbox.Wpf.Composition.Mef
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition.Hosting;
    using System.Linq;

    using JetBrains.Annotations;

    /// <summary>
    /// An <see cref="IExportProvider"/> adapter for the MEF 1 <see cref="ExportProvider"/>
    /// </summary>
    /// <seealso cref="TomsToolbox.Wpf.IExportProvider" />
    public class ExportProviderAdapter : IExportProvider
    {
        [NotNull]
        private readonly ExportProvider _exportProvider;

        private event EventHandler<EventArgs> ExportsChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportProviderAdapter"/> class.
        /// </summary>
        /// <param name="exportProvider">The export provider.</param>
        public ExportProviderAdapter([NotNull] ExportProvider exportProvider)
        {
            _exportProvider = exportProvider;
            exportProvider.ExportsChanged += ExportProvider_ExportsChanged;
        }

        private void ExportProvider_ExportsChanged(object sender, ExportsChangeEventArgs e)
        {
            ExportsChanged?.Invoke(sender, e);
        }

        event EventHandler<EventArgs> IExportProvider.ExportsChanged
        {
            add => ExportsChanged += value;
            remove => ExportsChanged -= value;
        }

        IEnumerable<T> IExportProvider.GetExportedValues<T>([CanBeNull] string contractName)
        {
            return _exportProvider.GetExportedValues<T>(contractName ?? string.Empty);
        }

        IEnumerable<ILazy<object>> IExportProvider.GetExports([NotNull] Type type, [CanBeNull] string contractName)
        {
            return _exportProvider
                .GetExports(type, null, contractName ?? string.Empty)
                .Select(item => new LazyAdapter<object>(item, item.Metadata as IDictionary<string, object>));
        }
    }
}
