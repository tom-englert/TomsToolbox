namespace TomsToolbox.Wpf.Composition.Mef
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition.Hosting;
    using System.Linq;

    using JetBrains.Annotations;

    using TomsToolbox.Composition;

    /// <summary>
    /// An <see cref="IExportProvider"/> adapter for the MEF 1 <see cref="ExportProvider"/>
    /// </summary>
    /// <seealso cref="IExportProvider" />
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

        [NotNull]
        T IExportProvider.GetExportedValue<T>([CanBeNull] string contractName)
        {
            return _exportProvider.GetExportedValue<T>(contractName ?? string.Empty);
        }

        [CanBeNull]
        T IExportProvider.GetExportedValueOrDefault<T>([CanBeNull] string contractName)
        {
            return _exportProvider.GetExportedValueOrDefault<T>(contractName ?? string.Empty);
        }

        bool IExportProvider.TryGetExportedValue<T>([CanBeNull] string contractName, [CanBeNull] out T value)
        {
            value = _exportProvider.GetExportedValueOrDefault<T>();

            return !Equals(value, default(T));
        }

        IEnumerable<T> IExportProvider.GetExportedValues<T>([CanBeNull] string contractName)
        {
            return _exportProvider.GetExportedValues<T>(contractName ?? string.Empty);
        }

        IEnumerable<IExport<object>> IExportProvider.GetExports([NotNull] Type contractType, [CanBeNull] string contractName)
        {
            return _exportProvider
                .GetExports(contractType, null, contractName ?? string.Empty)
                .Select(item => new ExportAdapter<object>(() => item.Value, new MetadataAdapter((IDictionary<string, object>)item.Metadata)));
        }
    }
}
